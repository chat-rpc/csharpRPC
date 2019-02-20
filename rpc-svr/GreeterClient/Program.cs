using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using Helloworld;

namespace GreeterClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //
            Channel channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);

            var client = new Greeter.GreeterClient(channel);

            string user = "NBS Tech";

            var reply = client.SayHello(new HelloRequest { Name = user });

            Console.WriteLine("Greeter reply :" + reply.Message);

            var secReply = client.SayHello(new HelloRequest { Name = "lanbery" });
            Console.WriteLine("Greeting: " + secReply.Message);
            channel.ShutdownAsync().Wait();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

        }
    }
}
