using System;

namespace CapitalAPI_Example
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            SKAPI sKAPI = new SKAPI("A12345678","Password");
            while (true)
            {
                Console.ReadKey();
            }
        }
    }
}