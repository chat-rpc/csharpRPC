using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Diagnostics;
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
            //localhost 
            string hostName = Dns.GetHostName();
            IPAddress[] iplist = Dns.GetHostAddresses(hostName);

            foreach (IPAddress ipa in iplist)
            {
                if (ipa.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    Console.WriteLine(ipa.ToString());
            }

            if(IPAddressUtil.GetLocalIP() != null)
            {
                hostName = IPAddressUtil.GetLocalIP();
                Console.WriteLine("Get Local IP :" + hostName);
            }
            else
            {
                hostName = "localhost";
            }

            Server server = new Server
            {
                Services = { Greeter.BindService(new GreeterImpl()) },
                Ports = { new ServerPort(hostName, Port, ServerCredentials.Insecure) }
            };

            server.Start();

            Console.WriteLine("Greeter server listening on port " + Port);
            Console.WriteLine("Press any key to stop the server ...");

            string flag = Console.ReadKey().Key.ToString();
            server.ShutdownAsync().Wait();
        }
    }

    /**
     * 
     */
    class IPAddressUtil
    {
        const bool defaultLogged = false;
        const string conndomain = "www.baidu.com";
        public static string GetLocalIP()
        {
            string result = RunApp("route", "print", true);
            System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(result, @"0.0.0.0\s+0.0.0.0\s+(\d+.\d+.\d+.\d+)\s+(\d+.\d+.\d+.\d+)");

            if (match.Success)
            {
                return match.Groups[2].Value;
            }else
            {
                try
                {
                    System.Net.Sockets.TcpClient cli = new System.Net.Sockets.TcpClient();
                    cli.Connect(conndomain, 80);
                    string ip = ((System.Net.IPEndPoint)cli.Client.LocalEndPoint).Address.ToString();
                    cli.Close();
                    return ip;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static string GetPrimaryDNS()
        {
            string result = RunApp("nslookup", "", true);

            System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(result, @"\d+\.\d+\.\d+\.\d+");
            if (match.Success)
            {
                return match.Value;
            }
            else
            {
                return null;
            }
        }

        public static string RunApp(string filename,string arg,bool logged)
        {
            try
            {
                if (logged)
                {
                    Trace.WriteLine(filename + " " + arg);
                }

                Process proc = new Process();
                proc.StartInfo.FileName = filename;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.Arguments = arg;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.UseShellExecute = false;
                proc.Start();

                using(System.IO.StreamReader sr = new System.IO.StreamReader(proc.StandardOutput.BaseStream, Encoding.Default))
                {
                    System.Threading.Thread.Sleep(100);
                    if (!proc.HasExited)
                    {
                        proc.Kill();
                    }
                    string txt = sr.ReadToEnd();
                    sr.Close();

                    if (logged)
                        Trace.WriteLine(txt);
                    return txt;
                }

            }catch(Exception ex)
            {
                Trace.WriteLine(ex);
                return ex.Message;
            }
        }
    }
}
