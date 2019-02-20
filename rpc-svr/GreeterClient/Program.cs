using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Grpc.Core;
using Helloworld;

namespace GreeterClient
{
    class Program
    {
        const string defHost = "127.0.0.1";
        static void Main(string[] args)
        {
            string svrHost = defHost;

            string line;
            string patternip = @"((25[0-5])|(2[0-4]\d)|(1\d\d)|([1-9]\d)|\d)(\.((25[0-5])|(2[0-4]\d)|(1\d\d)|([1-9]\d)|\d)){3}";
            string patterndomain = @"^[a-zA-Z0-9][-a-zA-Z0-9]{0,62}(\.[a-zA-Z0-9][-a-zA-Z0-9]{0,62})+$";
            Regex regexIP = new Regex(patternip);
            Regex domainRegex = new Regex(patterndomain);
            bool flag = true;
            do
            {
                Console.WriteLine("请输入Gprc服务端IP,或输入`c` 字符使用默认IP.");
                try
                {
                    line = Console.ReadLine();
                    if (string.IsNullOrEmpty(line)) {
                        flag = false;
                        continue;
                    };
                    if(regexIP.IsMatch(line) || domainRegex.IsMatch(line))
                    {
                        svrHost = line;
                        flag = false;
                    }
                }
                catch 
                {
                    Console.WriteLine("请输入服务IP或直接回车（Enter）使用默认IP.");
                }
            } while (flag);

            //
            string svrHostName = svrHost + ":50051";
            Channel channel;
            try
            {
                channel = new Channel(svrHostName, ChannelCredentials.Insecure);

                Console.WriteLine("Client connecting Channel " + svrHostName + ".");

                var client = new Greeter.GreeterClient(channel);
                            
                string user = "NBS Tech";

                var reply = client.SayHello(new HelloRequest { Name = user });

                Console.WriteLine("Greeter reply :" + reply.Message);

                var secReply = client.SayHello(new HelloRequest { Name = "lanbery" });
                Console.WriteLine("Greeting: " + secReply.Message);
                channel.ShutdownAsync().Wait();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Client connect channel failure. " + ex.Message);
            }
            
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

        }
    }
}
