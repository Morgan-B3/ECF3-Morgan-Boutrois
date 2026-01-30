using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace BookHub.ApiGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // tous les endpoints nécessitent JWT
    public class UserController : ControllerBase
    {
        private readonly GatewayHttpClient _userClient;


        public UserController(GatewayHttpClient userClient)
        {
            _userClient = userClient;
        }


        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userClient.GetAsync<object[]>("/users");
            return Ok(users);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userClient.GetAsync<object>($"/users/{id}");
            return Ok(user);
        }
    }
}