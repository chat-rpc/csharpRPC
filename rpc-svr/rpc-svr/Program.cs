using System;

namespace rpc_svr
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            foreach (var arg in args)
            {
                Console.WriteLine(arg);
            }

            Console.ReadLine();
        }
    }
}
