using Microsoft.AspNetCore.Mvc;


namespace BookHub.ApiGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly GatewayHttpClient _catalogClient;


        public CatalogController(GatewayHttpClient catalogClient)
        {
            _catalogClient = catalogClient;
        }


        [HttpGet]
        public async Task<IActionResult> GetBooks()
        {
            var books = await _catalogClient.GetAsync<object[]>("/books");
            return Ok(books);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetBook(int id)
        {
            var book = await _catalogClient.GetAsync<object>($"/books/{id}");
            return Ok(book);
        }
    }
}