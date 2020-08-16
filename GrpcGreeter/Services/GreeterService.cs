using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using GrpcGreeter.Services;
using Microsoft.Extensions.Logging;

namespace GrpcGreeter
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        private readonly IGreeterCounter _counter;

        public GreeterService(ILogger<GreeterService> logger, IGreeterCounter counter)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _counter = counter ?? throw new ArgumentNullException(nameof(counter));
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = $"Hello {request.Name}!"
            });
        }

        public override async Task SayHellos(HelloRequest request, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
        {
            var i = 0;
            var rnd = new Random();

            while (!context.CancellationToken.IsCancellationRequested)
            {
                var message = $"How are you {request.Name}? {++i}";
                _logger.LogInformation($"Sending greeting {message}.");

                await responseStream.WriteAsync(new HelloReply { Message = message });

                // Gotta look busy
                await Task.Delay(rnd.Next(1000));
            }
        }

        public override async Task<HelloCounterResponse> SayHelloCounter(IAsyncStreamReader<HelloCounterRequest> requestStream, ServerCallContext context)
        {
            await foreach (var request in requestStream.ReadAllAsync())
            {
                _counter.Increment(request.MessageCount);
            }

            return new HelloCounterResponse
            {
                Count = _counter.Count
            };
        }

        public override async Task SayHelloEcho(IAsyncStreamReader<HelloEcho> requestStream, IServerStreamWriter<HelloEcho> responseStream, ServerCallContext context)
        {
            var rnd = new Random();

            while (!context.CancellationToken.IsCancellationRequested)
            {
                await foreach (var request in requestStream.ReadAllAsync())
                {
                    // Gotta look busy
                    await Task.Delay(rnd.Next(1000));

                    await responseStream.WriteAsync(new HelloEcho
                    {
                        Message = $"Echoed from server: {request.Message}"
                    });
                }
            }
        }
    }
}
