using EcomApi.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using EcomApi.Domain.EmailTemplate;
using EcomApi.Application.DTO;
using EcomApi.Application.Interfaces;

namespace EcomApi.Application.Serrvices
{
    public class orderServices : IOrderServices
    {

        private readonly ITokenService _tokenService;
        private readonly IEmailServices _emailServices;

        //add loggers
        private readonly ILogger<orderServices> _logger;
        private readonly ITransaction _transaction;

        public orderServices(ITokenService tokenService, IEmailServices emailServices, ITransaction transaction)
        {
            _tokenService = tokenService;
            _emailServices = emailServices;
            _transaction = transaction;
        }


        // Method to create a new order
        public async Task<Order> CreateOrder(long price, string Status, string paymentId, string SessionId, string cid)
        {
            try
            {
                await _transaction.BeginTransactionAsync();
                var user = await _transaction.GetRepository<User>().GetById(cid, a => a.Strip_Cust_Id == cid);
                var user_id = user.User_Id;

                // Create a new order
                var newOrder = new Order
                {
                    Order_Date = DateTime.Now,
                    price = (double)price,
                    Order_Status = Status,
                    User_Id = user_id,
                    Payment_ID = paymentId,
                    Strip_session_Id = SessionId,
                };



                var createdOrder = await _transaction.GetRepository<Order>().AddAsync(newOrder);
                await _transaction.SaveChangesAsync();


                Log.Information("Order placement completed successfully. Order ID: {OrderId}, User ID: {UserId}, Total Price: {Price}", newOrder.Order_Id, user_id, newOrder.price);



                // If the order was created successfully, add the order products
                if (createdOrder != null)
                {
                    var userProducts = _transaction.GetRepository<UserProduct>().GetAsyncQueryable(user_id, a => a.User_ID == user_id).Include("product").ToList();
                    var session_Id = await _transaction.GetRepository<Order>().GetById(SessionId, a => a.Strip_session_Id == SessionId);
                    var orderIdFromSession = session_Id.Order_Id;

                    // Iterate through the user's cart items and create order products
                    foreach (var product in userProducts)
                    {
                        OrderProduct orderProduct = new OrderProduct
                        {
                            Product_Id = product.Product_ID,
                            Order_Id = newOrder.Order_Id,
                            Unite_Price = product.product.Price,
                            Order_Qty = product.Quntity

                        };
                        await _transaction.GetRepository<OrderProduct>().AddAsync(orderProduct);
                        await _transaction.SaveChangesAsync();

                        Log.Information("Order product added. Order ID: {OrderId}, Product ID: {ProductId}, Quantity: {Quantity}, Unit Price: {UnitPrice}", newOrder.Order_Id, product.Product_ID, product.Quntity, product.product.Price);


                        var productId = product.Product_ID;
                        // Deduct the quantity from the product's stock
                        var productInStock = await _transaction.GetRepository<product>().GetById(productId, p => p.Product_Id == product.Product_ID);
                        if (productInStock != null && productInStock.Stock >= product.Quntity)
                        {
                            // Deduct the quantity from the product's stock.
                            productInStock.Stock -= product.Quntity;

                            await _transaction.GetRepository<product>().UpdateAsync(productInStock);
                            await _transaction.SaveChangesAsync();

                            Log.Information("Product stock updated. Product ID: {ProductId}, Remaining Stock: {Stock}", product.Product_ID, productInStock.Stock);
                        }



                    }

                    // Clear the user's cart after checkout

                    var usercartproduct = _transaction.GetRepository<UserProduct>().GetAsyncQueryable(user_id, up => up.User_ID == user_id).ToList();
                    if (userProducts == null)
                    {
                        throw new InvalidOperationException("No products found for the given user ID.");
                    }


                    await _transaction.GetRepository<UserProduct>().RemoveRangeAsync(usercartproduct);
                    await _transaction.SaveChangesAsync();
                    Log.Information("User cart cleared. User ID: {UserId}", user_id);

                    // Send order confirmation email
                    var emailOrder = _transaction.GetRepository<Order>().GetAsyncQueryable(newOrder.Order_Id, a => a.Order_Id == newOrder.Order_Id)

                    .Include("User")
                    .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.product);
                    var emailOrderdata = emailOrder.FirstOrDefault();

                    var template = Template.emailTemplate(emailOrderdata);
                    var User_Name = user.first_Name;
                    var User_Email = user.Email;
                    var Subject = "Your order is complete. Your order details are as follows";

                    await _emailServices.SendEmail(User_Name, User_Email, template, Subject);

                    await _transaction.CommitAsync();
                    Log.Information("Email sent to user. User ID: {UserId}, Order ID: {OrderId}", emailOrderdata.User_Id, emailOrderdata.Order_Id);
                }

                return newOrder;
            }
            catch (Exception ex)
            {
                await _transaction.RollbackAsync();
                Log.Error(ex, "Error creating order.");
                throw new Exception("There was an error creating the order. Please try again later.");
            }

        }

        //view payment history
        public async Task<List<PaymentHistoryDTO>> viewOrderHistory()
        {
            var currentLogedUser = _tokenService.GetUserId();
            try
            {

                var orderHostory = await _transaction.GetRepository<Order>().GetAsyncQueryable(currentLogedUser, u => u.User_Id == currentLogedUser).Select(up => new PaymentHistoryDTO

                {
                    Order_Id = up.Order_Id,
                    Order_Date = up.Order_Date,
                    price = (up.price) / 100,
                    Order_Status = up.Order_Status,
                    Payment_ID = up.Payment_ID,
                }).ToListAsync();

                Log.Information("Retrieved payment details for user ID {UserId}. Number of orders: {OrderCount}", currentLogedUser, orderHostory.Count);

                return orderHostory;
            }
            catch (Exception ex)
            {

                Log.Error(ex, "Error retrieving PAyment history for user ID: {UserId}", currentLogedUser);
                throw new Exception("There was an error retrieving payment history for the user. Please try again later.");
            }
        }

        //payment history items view
        public async Task<List<PaymentHistoryItemDTO>> PaymentHistoryProducts(int orderId)
        {
            try
            {

                var userId = _tokenService.GetUserId();


                var orderQuery = _transaction.GetRepository<Order>().GetAsyncQueryable(orderId, o => o.Order_Id == orderId && o.User.User_Id == userId);

                // New: Including related entities (OrderProducts and product) in the query
                var orderWithDetailsQuery = orderQuery.Include(o => o.OrderProducts).ThenInclude(op => op.product);


                // Retrieve the order with details
                var productList = orderWithDetailsQuery.FirstOrDefault();

                if (productList == null)
                {
                    Log.Warning("No order found with ID: {OrderId}.", orderId);

                    throw new Exception($"No order found with ID: {orderId}");
                }

                // Map the order details to the PaymentHistoryDTO
                List<PaymentHistoryItemDTO> products = productList.OrderProducts.Select(a => new PaymentHistoryItemDTO
                {
                    Image_Url = a.product.Image_Url,
                    Product_Name = a.product.Name,
                    Unite_Price = a.product.Price,
                    Quantity = a.Order_Qty,
                    Sub_Total = a.product.Price * a.Order_Qty
                }).ToList();

                Log.Information("Retrieved payment details for order ID {OrderId}. Number of products: {ProductCount}", orderId, products.Count);
                foreach (var product in products)
                {
                    Log.Information("Product details - Order ID: {OrderId}, Product Name: {ProductName}, Unit Price: {UnitPrice}, Quantity: {Quantity}, Sub Total: {SubTotal}",
                        orderId, product.Product_Name, product.Unite_Price, product.Quantity, product.Sub_Total);
                }
                return products;
            }
            catch (Exception ex)
            {

                Log.Error(ex, "Error retrieving payment history for order ID: {OrderId}", orderId);
                throw new Exception("There was an error retrieving payment history for the specified order. Please try again later.");
            }

        }
    }
}
