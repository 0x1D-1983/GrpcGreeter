using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcGreeter;

namespace GrpcGreeterClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Call insecure gRPC services with .NET Core client
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            #region unary call
            //using var channel = GrpcChannel.ForAddress("http://localhost:5000");

            //var client = new Greeter.GreeterClient(channel);

            //var reply = await client.SayHelloAsync(new HelloRequest
            //{
            //    Name = "Janos"
            //});

            //Console.WriteLine($"Greeting: {reply.Message}");

            //Console.ReadKey();
            #endregion

            #region server streaming
            using var channel = GrpcChannel.ForAddress("http://localhost:5000");

            var client = new Greeter.GreeterClient(channel);
            var call = client.SayHellos(new HelloRequest
            {
                Name = "Janos"
            });

            await foreach (var response in call.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine($"Greeting: {response.Message}");
            }

            Console.ReadKey();
            #endregion


        }
    }
}
