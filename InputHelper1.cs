using System;
using System.Runtime.InteropServices;

public static class InputHelper1
{
    [DllImport("user32.dll", SetLastError = true)]
    static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

    [DllImport("user32.dll", SetLastError = true)]
    static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

    private const int MOUSEEVENTF_MOVE = 0x0001;
    private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
    private const int MOUSEEVENTF_LEFTUP = 0x0004;

    private const int KEYEVENTF_KEYDOWN = 0x0000;
    private const int KEYEVENTF_KEYUP = 0x0002;
    private const byte VK_A = 0x41;

    public static void full_event()
    {
        Console.WriteLine("Moving mouse and pressing a key in 3 seconds...");
        Thread.Sleep(3000);

        // Move mouse slightly
        mouse_event(MOUSEEVENTF_MOVE, 100, 0, 0, 0);

        // Left mouse click
        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);

        // Simulate key press 'A'
        keybd_event(VK_A, 0, KEYEVENTF_KEYDOWN, 0);
        keybd_event(VK_A, 0, KEYEVENTF_KEYUP, 0);

        Console.WriteLine("Done!");
    }

}