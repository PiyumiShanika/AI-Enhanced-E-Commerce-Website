using EcomApi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EcomApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class productController : ControllerBase
    {
        private readonly IProductService _productService;


        public productController(IProductService productService)
        {
            _productService = productService;

        }


        /// <summary>
        /// Creates a new product category.
        /// </summary>
        /// <param name="prodCategoryreq">The data transfer object representing the product category to be created.</param>
        /// <returns>An IActionResult representing the result of the operation.</returns>
        // [HttpPost("add-product-category")]
        //public async Task<IActionResult> createProductCategory([FromBody] ProductCategoryDTO prodCategoryreq )
        //{
        //    try
        //    {
        //        var createProductCategory = await _productService.CreateProductCategory(prodCategoryreq);
        //        return Ok(createProductCategory);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }

        //}


        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="productDTOReq">The data transfer object representing the product to be created.</param>
        /// <returns>An IActionResult representing the result of the operation.</returns>
        //[HttpPost("add-product")]
        //public async Task<IActionResult> createProduct([FromBody] productDTO productDTOReq)
        //{
        //    try
        //    {
        //        var createProduct = await _productService.creteProduct(productDTOReq);
        //        return Ok(createProduct);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }

        //}


        /// <summary>
        /// Retrieves a list of products.
        /// </summary>
        /// <returns>An IActionResult representing the result of the operation.</returns>
        [HttpGet("Products")]
        public async Task<IActionResult> viewProduct()
        {
            try
            {
                var product = await _productService.GetProducts();

                if (product == null)
                {
                    return BadRequest();
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        //all category
        /// <summary>
        /// Retrieves all categoryes
        /// </summary>
        /// <returns>An IActionResult representing the result of the operation.</returns>
        [HttpGet("Category")]
        public async Task<IActionResult> viewProductcategory()
        {
            try
            {
                var product = await _productService.GetProductswithCategory();

                if (product == null)
                {
                    return BadRequest();
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        //get product by category

        /// <summary>
        /// Retrieves a specific product category by its ID.
        /// </summary>
        /// <param name="id">The category ID  to retrieve the response.</param>
        /// <returns>An IActionResult representing the result of the operation.</returns>
        [HttpGet("Product-Category/{categoryid}")]
        public async Task<IActionResult> viewProductCategory(int categoryid)
        {
            try
            {
                var productCategory = await _productService.getProductCategory(categoryid);
                if (productCategory == null)
                {
                    return BadRequest();
                }

                return Ok(productCategory);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        /// <summary>
        /// Retrieves a specific product by its ID.
        /// </summary>
        /// <param name="Product_id">The ID of the product to retrieve.</param>
        /// <returns>An IActionResult representing the result of the operation.</returns>
        [HttpGet("product-by-id/{Product_id}")]
        public async Task<IActionResult> viewProductById(int Product_id)
        {
            try
            {
                var product = await _productService.getProductById(Product_id);
                if (product == null)
                {
                    return BadRequest(new
                    {
                        Message = $"No product found with ID {Product_id}"
                    });
                }

                return Ok(product);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
