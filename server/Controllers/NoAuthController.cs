using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationDemo.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class NoAuthController : ControllerBase
    {
        [HttpGet]
        public string Index()
        {
            return "this is a no Authorization page....not need Authorization!";
        }
    }
}
