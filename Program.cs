// See https://aka.ms/new-console-template for more information

using System;
using System.Threading;

class Program
{
    static void Main()
    {
        Console.WriteLine("Running input test...");
        Thread.Sleep(3000);

        MyInputLibrary.InputHelper2.PressKey(0x41); // 'A'
        MyInputLibrary.InputHelper2.MoveMouse();
        MyInputLibrary.InputHelper2.TypeText("sf yaow3nl86yw34pot g[d9ie8rup7yn6b43]");
        Console.WriteLine("Done!");
    }
}


