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
            //using var channel = GrpcChannel.ForAddress("http://localhost:5000");

            //var client = new Greeter.GreeterClient(channel);
            //var call = client.SayHellos(new HelloRequest
            //{
            //    Name = "Janos"
            //});

            //await foreach (var response in call.ResponseStream.ReadAllAsync())
            //{
            //    Console.WriteLine($"Greeting: {response.Message}");
            //}

            //Console.ReadKey();
            #endregion

            #region client streaming
            //var channel = GrpcChannel.ForAddress("http://localhost:5000");
            //var client = new Greeter.GreeterClient(channel);
            //using var call = client.SayHelloCounter();

            //var rnd = new Random();

            //for (int i = 0; i < 3; i++)
            //{
            //    int times = rnd.Next(10);

            //    await Task.Delay(rnd.Next(1000));

            //    var msg = new HelloCounterRequest
            //    {
            //        Message = $"Love you {times} times!",
            //        MessageCount = times
            //    };

            //    Console.WriteLine($"Sending message: {msg.Message}");

            //    await call.RequestStream.WriteAsync(msg);
            //}

            //await call.RequestStream.CompleteAsync();

            //var response = await call;

            //Console.WriteLine($"Greeting sent {response.Count} times.");

            #endregion

            #region bi-directional streaming
            var channel = GrpcChannel.ForAddress("http://localhost:5000");
            var client = new Greeter.GreeterClient(channel);
            using var call = client.SayHelloEcho();

            Console.WriteLine("Starting background task to receive messages from the server");
            var readTask = Task.Run(async () =>
            {
                await foreach(var response in call.ResponseStream.ReadAllAsync())
                {
                    Console.WriteLine(response.Message);
                }
            });

            var rnd = new Random();

            for (int i = 0; i < 5; i++)
            {
                int times = rnd.Next(10);

                await Task.Delay(rnd.Next(1000));

                var msg = new HelloEcho
                {
                    Message = $"Love you {times} times!"
                };

                Console.WriteLine($"Sending message: {msg.Message}");
                await call.RequestStream.WriteAsync(msg);
            }

            Console.WriteLine("Disconnecting client");
            await call.RequestStream.CompleteAsync();

            await readTask;

            #endregion
        }
    }
}
