using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Nestelia.Application.Interfaces.Auth;
using Nestelia.Domain.Common.ViewModels.Util;
using Nestelia.Domain.DTO.Auth;
using System.Security.Claims;

namespace Nestelia.WebAPI.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            var userCreated = await authService.CreateAccount(userDto);
            if (!userCreated.IsSuccess) return BadRequest(userCreated);

            if (userCreated.Data is null) return BadRequest();

            authService.SetTokensInsideCookie(await authService.CreateTokens(userCreated.Data), HttpContext);

            var getUserResult = await authService.GetUserById(userCreated.Data.Id);

            return Ok(getUserResult);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto LoginDto)
        {
            var validateUser = await authService.ValidateUser(LoginDto);
            if (!validateUser.IsSuccess) return Unauthorized(validateUser);

            if (validateUser.Data is null) return BadRequest();

            authService.SetTokensInsideCookie(await authService.CreateTokens(validateUser.Data), HttpContext);

            var getUserResult = await authService.GetUserById(validateUser.Data.Id);

            return Ok(getUserResult);
        }


        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {

            if (!HttpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            {
                return BadRequest("Error: No se encontró el token de refresco.");
            }

            var responseRefresh = await authService.RefreshToken(refreshToken);

            if (!responseRefresh.IsSuccess)
            {
                return BadRequest(responseRefresh);
            }

            if (responseRefresh.Data is null) return BadRequest();

            authService.SetTokensInsideCookie(responseRefresh.Data, HttpContext);

            return Ok(responseRefresh);
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            ResponseHelper response = new() { Success = true, Message = "Logout exitoso" };

            if (!HttpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            {
                return BadRequest("Error: No se encontró el token de refresco.");
            }

            Response.Cookies.Delete("accessToken");
            Response.Cookies.Delete("refreshToken");

            return Ok(response);
        }

        [HttpGet("me")]
        [Authorize]
        [OutputCache(Duration = 3600, VaryByHeaderNames = new[] { "Authorization" })]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("No se pudo obtener el identificador del usuario.");
            }

            var userResult = await authService.GetUserById(userId);

            if (!userResult.IsSuccess)
            {
                return NotFound(userResult);
            }

            return Ok(userResult);
        }

    }
}
