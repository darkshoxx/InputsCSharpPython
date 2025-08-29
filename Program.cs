// See https://aka.ms/new-console-template for more information

using System;
using System.Threading;

class Program
{
    static void Main()
    {
        Console.WriteLine("Running input test...");
        Thread.Sleep(3000);

        MyInputLibrary.InputHelper1.PressKey(0x41); // 'A'
        MyInputLibrary.InputHelper1.MoveMouseRelative(50, 50);
        MyInputLibrary.InputHelper1.MoveMouseAbsolute(50, 50);
        MyInputLibrary.InputHelper1.TypeText("sf yaow3nl86yw34pot g[d9ie8rup7yn6b43]");
        Console.WriteLine("Done!");
    }
}


