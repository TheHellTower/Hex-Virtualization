using System;

namespace Hex.VM.Tests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello, type anything.");
            while (true)
            {
                var line = Console.ReadLine();
                Console.WriteLine($"You wrote: {line}");

                if (line == "continue")
                    break;
            }
            var maths = new Maths(10,10);
            /*Console.WriteLine($"Add {maths.Add()}");
            Console.WriteLine($"Sub {maths.Subtract()}");
            Console.WriteLine($"Mul {maths.Multiply()}");
            Console.WriteLine($"Div {maths.Divide()}");
            Console.WriteLine($"Cgt {(double)maths.Add() > maths.Subtract()}");
            Console.WriteLine($"Clt {maths.Add() < (float)maths.Subtract()}");
            Console.WriteLine($"Or {1337 | 7331}");
            Console.WriteLine(maths.Sum);*/
            Console.WriteLine($"Add {maths.Add()}\nSub {maths.Subtract()}\nMul {maths.Multiply()}\nDiv {maths.Divide()}\nCgt {(double)maths.Add() > maths.Subtract()}\nClt {maths.Add() < (float)maths.Subtract()}\nOr {1337 | 7331}\n{maths.Sum}");
            double d = Convert.ToDouble(int.Parse(Console.ReadLine()));
            d += 0.5;
            Console.WriteLine(d);
            Console.ReadKey();
        }
    }
}