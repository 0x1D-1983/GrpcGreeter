using System;
namespace GrpcGreeter.Services
{
    public interface IGreeterCounter
    {
        void Increment(int amount);

        int Count { get; }
    }
}
