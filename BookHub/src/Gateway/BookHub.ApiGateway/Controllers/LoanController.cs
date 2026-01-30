using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace BookHub.ApiGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LoanController : ControllerBase
    {
        private readonly GatewayHttpClient _loanClient;


        public LoanController(GatewayHttpClient loanClient)
        {
            _loanClient = loanClient;
        }


        [HttpPost]
        public async Task<IActionResult> CreateLoan([FromBody] object loanData)
        {
            var result = await _loanClient.PostAsync<object, object>("/loans", loanData);
            return Ok(result);
        }


        [HttpPut("{id}/return")]
        public async Task<IActionResult> ReturnLoan(int id)
        {
            var result = await _loanClient.PutAsync<object, object>($"/loans/{id}/return", new { });
            return Ok(result);
        }
    }
}