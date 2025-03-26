using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EcomApi.Application.DTO;
using EcomApi.Application.Interfaces;

namespace EcomApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class cartController : ControllerBase
    {
        private readonly ICartServices _cartServices;


        public cartController(ICartServices cartServices)
        {
            _cartServices = cartServices;

        }

        /// <summary>
        /// Adds items to the user's shopping cart.
        /// </summary>
        /// <param name="cartDTORqe">The data transfer object representing the items to be added to the cart.</param>
        /// <returns>An IActionResult representing the result of the operation.</returns>

        [HttpPost("AddTo-Cart")]
        public async Task<IActionResult> AddToCart([FromBody] CartDTO cartDTORqe)
        {
            try
            {
                var AddtoCart = await _cartServices.AddTocart(cartDTORqe);

                return Ok(AddtoCart);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        /// <summary>
        /// Retrieves the details of the user's shopping cart.
        /// </summary>
        /// <returns>An IActionResult representing the result of the operation.</returns>
        [Authorize]
        [HttpGet("View-Cart")]
        public async Task<IActionResult> viewCart()
        {
            try
            {
                var cart = await _cartServices.View_Cart();

                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        /// <summary>
        /// Deletes an item from the user's shopping cart.
        /// </summary>
        /// <param name="id">The ID of the Product to be deleted from the cart.</param>
        /// <returns>An IActionResult representing the result of the operation.</returns>
        [Authorize]
        [HttpDelete("Delete-cart-Item")]
        public async Task<IActionResult> deleteCart(int productid)
        {
            try
            {
                var delete_Cart = await _cartServices.deleteCart(productid);
                return Ok(delete_Cart);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        /// <summary>
        /// Updates the quantity of a specific item in the user's shopping cart.
        /// </summary>
        /// <param name="productId">The ID of the product whose quantity is to be updated.</param>
        /// <param name="newQuantity">The new quantity of the product in the cart.</param>
        /// <returns>An IActionResult representing the result of the operation.</returns>
        [Authorize]
        [HttpPatch("update-item-quantity")]
        public async Task<IActionResult> UpdateCartItemQuantity(int productId, int newQuantity)
        {
            try
            {
                await _cartServices.UpdateCartItemQuantityAsync(productId, newQuantity);
                return Ok("Cart item quantity updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
