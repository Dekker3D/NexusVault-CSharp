using System;
using System.Diagnostics;
using System.Linq;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
          var zigzag =   Nexusvault.Tex.ZigZag.CalculateRowFirstZigZagIndices(8, 8);
            Debug.WriteLine($"[{string.Join(", ", zigzag)}]");

        }
    }
}
