using System;
namespace GrpcGreeter.Services
{
    public class GreeterCounter : IGreeterCounter
    {
        public int Count { get; private set; }

        public void Increment(int amount)
        {
            Count += amount;
        }
    }
}
