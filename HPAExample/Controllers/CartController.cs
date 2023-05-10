using Google.Protobuf;
using HPAExample.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;

namespace HPAExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ILogger<CartController> _logger;
        private readonly List<AddCartRequest> addCartRequests = new();
        private readonly Counter<long> _cartLineCounter;
        public CartController(ILogger<CartController> logger, Instrumentation instrumentation)
        {

            _logger = logger;
            _cartLineCounter = instrumentation.CartLinesCounter;

        }

        [HttpPost("item/add")]
        public async Task<IActionResult> AddItem([FromBody] AddCartRequest addCartRequest)
        {
         
            addCartRequests.Add(addCartRequest);
            if (addCartRequest.CartLine >= 100)
            {
                _cartLineCounter.Add(1, new KeyValuePair<string, object?>("machine", Environment.MachineName), new KeyValuePair<string, object?>("cartId", addCartRequest.CartId), new KeyValuePair<string, object?>("cartlines", addCartRequest.CartLine));
            }
                    
            await Task.Delay(5);
            return Created(Request.GetDisplayUrl(), new { status = "success" });
        }

        [HttpPost("{id}/process")]
        public async Task<IActionResult> ProcessCart([FromQuery] Guid id)
        {
            await Task.Delay(5);
            return Created(Request.GetDisplayUrl(), new { status = "success" });
        }
    }
}
