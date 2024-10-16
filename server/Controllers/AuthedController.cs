using AuthorizationDemo.Model;
using AuthorizationDemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthedController : ControllerBase
    {
        private readonly TokenService _tokenService;

        public AuthedController(TokenService tokenService)
        {
            _tokenService = tokenService;
        }

        //这个是登录
        //被鉴权的控制器必须先登录获取jwt.
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (!request.Email.Equals("user@example.com") || !request.Password.Equals("string"))
                return BadRequest("invalid emial or passowrd....");
            User demoUser = new User
            {
                UserName = "user@example.com",
                Password = "string",
                Age = 19,
                Id = 111,
            };
            var token = _tokenService.CreateToken(demoUser);
            await Task.CompletedTask;
            return Ok(new AuthResponse { Token = token, UserId = demoUser.Id.ToString(), UserName = demoUser.UserName });
        }

        [HttpGet]
        [Authorize]  //notice....this is a Authorized action.
        public string Test()
        {
            return "this is a Authed page...";
        }
    }
}
