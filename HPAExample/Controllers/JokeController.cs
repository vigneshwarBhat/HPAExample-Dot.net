using Microsoft.AspNetCore.Mvc;


namespace HPAExample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JokeController : ControllerBase
    {

        private readonly ILogger<JokeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public JokeController(ILogger<JokeController> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }


        [HttpGet]
        public async Task<IActionResult> GetJoke()
        {
            try
            {
                _logger.LogInformation("Getting random joke started");
                var httpClient = _httpClientFactory.CreateClient("test");
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                var result = await httpClient.GetAsync(_configuration.GetValue<string>("Api2Url"), HttpCompletionOption.ResponseContentRead);
                if (result.IsSuccessStatusCode)
                {
                    return Ok(await result.Content.ReadAsStringAsync());
                }
                _logger.LogInformation("Getting random jokeEnded");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (Exception exception)
            {
                _logger.LogError("Error while publishing to Kafka and error is {Exception}", exception);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }
    }
}