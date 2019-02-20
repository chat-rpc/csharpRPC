using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using Helloworld;

namespace GreeterServer
{
    //
    class GreeterImpl : Greeter.GreeterBase
    {
        public override Task<HelloReply> SayHello(HelloRequest request,ServerCallContext context)
        {
            Console.WriteLine("Greeting cli:" + request.Name);
            return Task.FromResult(new HelloReply { Message = "你好 " + request.Name });
        }
    }

    class Program
    {
        const int Port = 50051;
        static void Main(string[] args)
        {
            Server server = new Server
            {
                Services = { Greeter.BindService(new GreeterImpl()) },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };

            server.Start();

            Console.WriteLine("Greeter server listening on port " + Port);
            Console.WriteLine("Press any key to stop the server ...");

            string flag = Console.ReadKey().Key.ToString();
            server.ShutdownAsync().Wait();
        }
    }
}
