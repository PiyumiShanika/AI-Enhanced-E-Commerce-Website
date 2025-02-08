using EcomApi.Application.DTO;
using EcomApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcomApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class userController : ControllerBase
    {
        private readonly IAddressService _myService;
        private readonly IAuthServices _authServices;


        public userController(IAddressService myService, IAuthServices authServices)
        {

            _myService = myService;
            _authServices = authServices;

        }

        //Sign up endpoint

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="UserRequest">The data transfer object representing the user to be registered.</param>
        /// <returns>An IActionResult representing the result of the operation.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserCreateDTO UserRequest)
        {
            try
            {
                var result = await _authServices.registerUser(UserRequest);
                if (!result.Success)
                {
                    return BadRequest(new { Message = result.ErrorMessage });
                }

                return Ok(new { Message = "User created successfully and registered in Auth0.", Data = result.Data });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        //sign in endpoint

        /// <summary>
        /// Logs in a user.
        /// </summary>
        /// <param name="loginCredentials">The data transfer object representing the login credentials of the user.</param>
        /// <returns>An IActionResult representing the result of the operation.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO loginCredentials)
        {
            try
            {
                var result = await _authServices.loginUser(loginCredentials);
                if (result.IsSuccess)
                {
                    return Ok(new { Access_Token = result.Access_Token, Refresh_token = result.Refresh_Token });
                }
                else
                {
                    return Unauthorized(new { Error = result.Error });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        //refresh token
        //[HttpPost("refresh-token")]
        //public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDTO refreshTokenRequest)
        //{
        //    try
        //    {
        //        var tokenResponse = await _authServices.RefreshTokenAsync(refreshTokenRequest.RefreshToken);
        //        return Ok(tokenResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex, "Error occurred while refreshing token");
        //        return StatusCode(500, "Internal server error");
        //    }
        //}


        /// <summary>
        /// Adds a new address.
        /// </summary>
        /// <param name="dto">The data transfer object representing the address to be added.</param>
        /// <returns>An IActionResult representing the result of the operation.</returns>
        [Authorize]
        [HttpPost("add-address")]
        public async Task<IActionResult> Post([FromBody] CreateAddressDTO dto)
        {
            try
            {
                var createdEntity = await _myService.CreateAsync(dto);
                return StatusCode(201, createdEntity);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        /// <summary>
        /// Retrieves current loged user's address details.
        /// </summary>
        /// <returns>An IActionResult representing the result of the operation.</returns>
        [Authorize]
        [HttpGet("view-address")]
        public async Task<IActionResult> GetAddressDetails()
        {
            try
            {
                var address = await _myService.GetAddressAsync();
                if (address == null || !address.Any())
                {
                    return BadRequest("Address does not exist.");
                }
                return Ok(address);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
