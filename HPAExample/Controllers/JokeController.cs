using HPAExample.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;

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
                _logger.LogWarning("Getting random joke started");
                var httpClient = _httpClientFactory.CreateClient("test");
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                var result = await httpClient.GetAsync(_configuration.GetValue<string>("Api2Url"), HttpCompletionOption.ResponseContentRead);
                if (result.IsSuccessStatusCode)
                {
                    var result1=JsonSerializer.Deserialize<JokeModel>(await result.Content.ReadAsStringAsync());
                    return Ok(result1);
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

        [HttpGet("list")]
        public async Task<IActionResult> GetJokes()
        {
            var watch = new Stopwatch();
            try
            {
                _logger.LogWarning("Getting random joke started");
                var taskList = new List<Task<List<JokeModel>>>();
                watch.Start();
                for (var i = 0; i < 100; i++)
                {
                   // _logger.LogInformation($"[Jokes/List]: triggering the {i}th call");
                    taskList.Add(GetJokeListV1(i));
                }
                var result = await Task.WhenAll(taskList);
                watch.Stop();
                _logger.LogWarning($"[Jokes/List]: Total elpsed time {watch.ElapsedMilliseconds}");
                return Ok(new {result, elapsedTimeInMilliSec= watch.ElapsedMilliseconds });
            }
            catch (Exception exception)
            {
                _logger.LogError("{Exception}", exception);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

        [HttpGet("listv2")]
        public async Task<IActionResult> GetJokeList()
        {
            var watch = new Stopwatch();
            try
            {
                _logger.LogWarning("Getting random joke started");
                var taskList = new List<Task<List<JokeModel>>>();
                watch.Start();
                for (var i = 0; i < 100; i++)
                {
                    _logger.LogWarning($"[Jokes/Listv2]: triggering the {i}th call");
                    taskList.Add(GetJokeListV2(i));
                }
                var result = await Task.WhenAll(taskList);
                watch.Stop();
                _logger.LogWarning($"[Jokes/Listv2] : Total elpsed time {watch.ElapsedMilliseconds}");
                return Ok(new { result, elapsedTimeInMilliSec = watch.ElapsedMilliseconds });
            }
            catch (Exception exception)
            {
                _logger.LogError("Error while publishing to Kafka and error is {Exception}", exception);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

        private async Task<List<JokeModel>> GetJokeListV2(int i)
        {
            _logger.LogWarning($"[Jokes/Listv2] : [GetJokeListV2] {i}th call");
            var result = new List<JokeModel>();
            var resultV1 = await GetJokesV3(i);
            result.Add(resultV1);
            var result2 = await GetJokesV4(i);
            result.Add(result2);

            return result;
        }

        private async Task<List<JokeModel>> GetJokeListV1(int i)
        {
            var result = new List<JokeModel>();
            result.AddRange(await GetJokesV1(new List<int> { i }));
            result.AddRange(await GetJokesV2(new List<int> { i }));
            return result;
        }

        private async Task<List<JokeModel>> GetJokesV1(List<int> list)
        {
            var httpClient = _httpClientFactory.CreateClient("test");
            var responseList = new List<JokeModel>();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _logger.LogWarning($"[Jokes/List] : [GetJokesV1] {list[0]}th call");
            var taskList = list.Select(x => httpClient.GetAsync(_configuration.GetValue<string>("Api2Url"), HttpCompletionOption.ResponseContentRead)).ToList();
            while (taskList.Any())
            {
                var task = await Task.WhenAny(taskList);
                taskList.Remove(task);
                var result = await task;
                if (result.IsSuccessStatusCode)
                {
                    var item = await result.Content.ReadAsStringAsync();
                    if (item != null)
                    {
                        responseList.Add(JsonSerializer.Deserialize<JokeModel>(item));
                    }

                }
                else
                {
                    _logger.LogWarning($"[Jokes/Listv2] : [GetJokesV3] : status {result.StatusCode}");
                }
            }
            
            return responseList;
        }

        private async Task<List<JokeModel>> GetJokesV2(List<int> list)
        {
            var httpClient = _httpClientFactory.CreateClient("test");
            var responseList = new List<JokeModel>();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _logger.LogWarning($"[Jokes/List] : [GetJokesV2] {list[0]}th call");
            var taskList = list.Select(x => httpClient.GetAsync(_configuration.GetValue<string>("Api2Url"), HttpCompletionOption.ResponseContentRead)).ToList();
            while (taskList.Any())
            {
                var task = await Task.WhenAny(taskList);
                taskList.Remove(task);
                var result = await task;
                if (result.IsSuccessStatusCode)
                {
                    var item = await result.Content.ReadAsStringAsync();
                    if (item != null)
                    {
                        responseList.Add(JsonSerializer.Deserialize<JokeModel>(item));
                    }

                }
                else
                {
                    _logger.LogWarning($"[Jokes/Listv2] : [GetJokesV3] : status {result.StatusCode}");
                }
            }
            
            return responseList;
        }

        private async Task<JokeModel> GetJokesV3(int i)
        {
            _logger.LogWarning($"[Jokes/Listv2] : [GetJokesV3] {i}th call");
            var httpClient = _httpClientFactory.CreateClient("test");
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var result = await httpClient.GetAsync(_configuration.GetValue<string>("Api2Url"));
            if (result.IsSuccessStatusCode)
            {
                var item = await result.Content.ReadAsStringAsync();
                if (item != null)
                {
                    return JsonSerializer.Deserialize<JokeModel>(item);
                }

            }
            _logger.LogWarning($"[Jokes/Listv2] : [GetJokesV3] : status {result.StatusCode}:{result.ReasonPhrase}");
            return new JokeModel();
        }


        private async Task<JokeModel> GetJokesV4(int i)
        {
           _logger.LogWarning($"[Jokes/Listv2] : [GetJokesV4] {i}th call");
            var httpClient = _httpClientFactory.CreateClient("test");
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var result = await httpClient.GetAsync(_configuration.GetValue<string>("Api2Url"));
            if (result.IsSuccessStatusCode)
            {
                var item = await result.Content.ReadAsStringAsync();
                if (item != null)
                {
                    return JsonSerializer.Deserialize<JokeModel>(item);
                }

            }
            _logger.LogWarning($"[Jokes/Listv2] : [GetJokesV3] : status {result.StatusCode}:{result.ReasonPhrase}");
            return new JokeModel();
        }

    }
}