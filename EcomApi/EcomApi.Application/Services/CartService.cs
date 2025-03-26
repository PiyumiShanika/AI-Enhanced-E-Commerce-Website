using EcomApi.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using EcomApi.Application.DTO;
using EcomApi.Application.Interfaces;

namespace EcomApi.Application.Serrvices
{
    public class CartServices : ICartServices
    {

        private readonly ITokenService _tokenService;
        private string _curreLogedUser;
        private readonly ITransaction _transaction;
        private readonly IUpdateTimeStamp _updateTimeStamp;

        public CartServices(ITokenService tokenService, ITransaction transaction, IUpdateTimeStamp updateTimeStamp)
        {

            _tokenService = tokenService;
            _curreLogedUser = _tokenService.GetUserId();
            _transaction = transaction;
            _updateTimeStamp = updateTimeStamp;
        }

        // Method to add a product to the user's cart
        public async Task<UserProduct> AddTocart(CartDTO cartDTOReq)
        {

            try
            {

                // begin transaction
                await _transaction.BeginTransactionAsync();
                // Check if the product exists in the database

                var ProductStock = await _transaction.GetRepository<product>().GetById(cartDTOReq.Product_ID, a => a.Product_Id == cartDTOReq.Product_ID);

                if (ProductStock == null)
                {
                    Log.Information("Product not found. Product ID: {ProductID}", cartDTOReq.Product_ID);
                    throw new Exception($"Product with ID {cartDTOReq.Product_ID} not found.");
                }


                if (cartDTOReq.Quntity > ProductStock.Stock)
                {
                    Log.Information("Insufficient stock. Product ID: {ProductID}, Requested Quantity: {RequestedQuantity}, Available Stock: {AvailableStock}",
                            cartDTOReq.Product_ID, cartDTOReq.Quntity, ProductStock.Stock);
                    throw new Exception($"Insufficient stock. Available stock: {ProductStock.Stock}");
                }



                var existingCartItems = _transaction.GetRepository<UserProduct>().GetAsyncQueryable(_curreLogedUser, up => up.User_ID == _curreLogedUser);


                // Create a new UserProduct entity for the cart

                var myEntity = new UserProduct
                {
                    User_ID = _curreLogedUser,
                    Product_ID = cartDTOReq.Product_ID,
                    Quntity = cartDTOReq.Quntity,
                    CreatedAt = DateTime.Now,
                    CartCreatedAt = existingCartItems.Any() ? existingCartItems.First().CartCreatedAt : DateTime.Now,
                    cartUpdatedAt = DateTime.Now,
                };



                // Add the entity to the context

                await _transaction.GetRepository<UserProduct>().AddAsync(myEntity);
                await _transaction.SaveChangesAsync();
                await _transaction.CommitAsync();


                Log.Information("Item added to cart. User ID: {UserID}, Product ID: {ProductID}, Quantity: {Quantity}",
                           myEntity.User_ID, cartDTOReq.Product_ID, cartDTOReq.Quntity);

                // Update cart timestamp
                await _updateTimeStamp.UpdateCartTimestamp(_curreLogedUser);

                return myEntity;
            }
            catch (Exception ex)
            {
                await _transaction.RollbackAsync();
                throw new ApplicationException("Error creating entity", ex);
            }
        }

        //delete cart 
        public async Task<UserProduct> deleteCart(int product_id)
        {



            try
            {
                //begin transaction
                await _transaction.BeginTransactionAsync();

                var _curreLogedUser = _tokenService.GetUserId();
                var item = await _transaction.GetRepository<UserProduct>().GetById(_curreLogedUser, up => up.User_ID == _curreLogedUser && up.Product_ID == product_id);

                if (item is null)
                {
                    Log.Information("Item not found in cart. User ID: {UserID}, Product ID: {ProductID}", _curreLogedUser, product_id);
                    throw new Exception("Item not fount....");

                }
                Log.Information("Deleting item from cart. User ID: {UserID}, Product ID: {ProductID}, Quantity: {Quantity}, Created At: {CreatedAt}",
                       _curreLogedUser, item.Product_ID, item.Quntity, item.CreatedAt);


                await _transaction.GetRepository<UserProduct>().DeleteAsync(item);
                await _transaction.SaveChangesAsync();
                await _transaction.CommitAsync();

                //update time stamp
                await _updateTimeStamp.UpdateCartTimestamp(_curreLogedUser);

                Log.Information("Item removed from cart. User ID: {UserID}, Product ID: {ProductID}", _curreLogedUser, product_id);

                return item;


            }
            catch (Exception ex)
            {
                await _transaction.RollbackAsync();
                Log.Error(ex, "Error deleting item from cart. User ID: {UserID}, Product ID: {ProductID}", _curreLogedUser, product_id);
                throw ex;
            }

        }

        // Method to view the user's cart
        public async Task<object> View_Cart()
        {
            try
            {

                var _curreLogedUser = _tokenService.GetUserId();
                var cart = _transaction.GetRepository<UserProduct>().GetAsyncQueryable(_curreLogedUser, u => u.User_ID == _curreLogedUser)
               .Select(up => new productDTO
               {

                   Name = up.product.Name,
                   Image_Url = up.product.Image_Url,
                   Price = up.product.Price,
                   Discount = up.product.Discount,
                   Description = up.product.Description,
                   Stock = up.product.Stock,
                   Quntity = up.Quntity,
                   Product_Id = up.Product_ID,
                   Category_Id = up.product.Category_Id,
                   CreatedAt = up.CreatedAt,
                   UpdatedAt = up.UpdatedAt,
               }).ToList();


                // Log details of each item in the cart
                foreach (var item in cart)
                {
                    Log.Information("Cart item - Product ID: {ProductID}, Name: {ProductName}, Quantity: {Quantity}, Price: {Price}, Total: {Total}",
                                    item.Product_Id, item.Name, item.Quntity, item.Price, item.Price * item.Quntity);
                }

                DateTime? cartCreatedAt = null;
                DateTime? cartUpdatedAt = null;
                if (cart.Any())
                {

                    var firstCartItem = _transaction.GetRepository<UserProduct>().GetAsyncQueryable
                        (_curreLogedUser, u => u.User_ID == _curreLogedUser)
                        .Select(up => new { up.CartCreatedAt, up.cartUpdatedAt })
                        .FirstOrDefault();

                    if (firstCartItem != null)
                    {
                        cartCreatedAt = firstCartItem.CartCreatedAt;
                        cartUpdatedAt = firstCartItem.cartUpdatedAt;
                    }
                }


                // Log information about the cart
                Log.Information("Viewing cart for User ID: {UserID}. Total items: {ItemCount}, Subtotal: {Subtotal}",
                                _curreLogedUser, cart.Count, cart.Sum(item => item.Price * item.Quntity));

                // Calculate total items in the cart and subtotal
                var count = cart.Count;

                //total of the cart product
                var cartTotal = cart.Sum(item => item.Price * item.Quntity);
                return new
                {
                    cart,
                    totalItems = count,
                    subTotal = cartTotal,
                    cartCreatedAt = cartCreatedAt,
                    cartUpdatedAt = cartUpdatedAt
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error viewing cart for User ID: {UserID}", _curreLogedUser);
                throw new Exception("There was an error viewing the cart. Please try again later.");
            }
        }

        // Method to update the quantity of an item in the cart
        public async Task<UserProduct> UpdateCartItemQuantityAsync(int productId, int newQuantity)
        {

            try
            {
                //begin transaction
                await _transaction.BeginTransactionAsync();
                var _curreLogedUser = _tokenService.GetUserId();
                // Retrieve the cart item for the current user and specified product
                var cartItemData = _transaction.GetRepository<UserProduct>().GetAsyncQueryable
                    (_curreLogedUser, up => up.User_ID == _curreLogedUser && up.Product_ID == productId);

                var cartItem = cartItemData.FirstOrDefault();


                if (cartItem == null)
                {
                    Log.Information("Cart item not found. User ID: {UserID}, Product ID: {ProductID}", _curreLogedUser, productId);
                    throw new Exception($"Cart item with product ID {productId} not found.");
                }


                var product = await _transaction.GetRepository<product>().GetById(productId);


                if (product == null)
                {
                    Log.Information("Product not found. Product ID: {ProductID}", productId);
                    throw new Exception($"Product with ID {productId} not found.");
                }

                if (newQuantity <= 0)
                {
                    Log.Information("Invalid quantity. User ID: {UserID}, Product ID: {ProductID}, Quantity: {Quantity}",
                         _curreLogedUser, productId, newQuantity);
                    throw new Exception("Invalid quantity. Quantity must be greater than zero.");
                }

                if (newQuantity > product.Stock)
                {
                    Log.Information("Insufficient stock. User ID: {UserID}, Product ID: {ProductID}, Available stock: {AvailableStock}",
                            _curreLogedUser, productId, product.Stock);
                    throw new Exception($"Insufficient stock. Available stock: {product.Stock}");
                }

                // Update the cart item with the new quantity and current date
                var update_date = DateTime.Now;
                cartItem.UpdatedAt = update_date;
                cartItem.Quntity = newQuantity;


                await _updateTimeStamp.UpdateCartTimestamp(_curreLogedUser);


                await _transaction.SaveChangesAsync();

                // Log successful update
                Log.Information("Cart item quantity updated successfully. User ID: {UserID}, Product ID: {ProductID}, New Quantity: {NewQuantity}, Update Date: {UpdateDate}",
                                _curreLogedUser, productId, newQuantity, update_date);

                return cartItem;
            }
            catch (Exception ex)
            {
                await _transaction.RollbackAsync();
                Log.Error(ex, "Error updating cart item quantity. User ID: {UserID}, Product ID: {ProductID}, New Quantity: {NewQuantity}",
                 _curreLogedUser, productId, newQuantity);
                throw new Exception("Error updating cart item quantity. try Again");
            }
        }

    }
}
