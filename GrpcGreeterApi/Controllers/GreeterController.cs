using System;
using System.Threading.Tasks;
using GrpcGreeter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GrpcGreeterApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GreeterController : ControllerBase
    {
        private readonly ILogger<GreeterController> _logger;
        private readonly Greeter.GreeterClient _client;

        public GreeterController(ILogger<GreeterController> logger, Greeter.GreeterClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        [HttpGet]
        public async Task<string> Get()
        {
            

            var response = await _client.SayHelloAsync(new HelloRequest
            {
                Name = "Janos"
            });

            return response.Message;
        }
    }
}
