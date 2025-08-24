using System;
using System.Runtime.InteropServices;

namespace MyInputLibrary
{
    public static class InputHelper2
    {
        // Define Input Types
        private const int INPUT_MOUSE = 0;
        private const int INPUT_KEYBOARD = 1;

        // Keyboard event flags
        private const uint KEYEVENTF_KEYDOWN = 0x0000;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const ushort VK_SHIFT = 0x10;

        // Mouse event flags
        private const uint MOUSEEVENTF_MOVE = 0x0001;
        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;

        // Virtual Key codes
        private const ushort VK_A = 0x41; // 'A'

        [StructLayout(LayoutKind.Sequential)]
        struct INPUT
        {
            public int type;
            public InputUnion U;
        }

        [StructLayout(LayoutKind.Explicit)]
        struct InputUnion
        {
            [FieldOffset(0)] public MOUSEINPUT mi;
            [FieldOffset(0)] public KEYBDINPUT ki;

        }

        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;

        }
        [StructLayout(LayoutKind.Sequential)]
        struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;

        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);



        public static void PressKey(ushort keyCode)
        {
            INPUT[] inputs = new INPUT[2];
            inputs[0] = new INPUT
            {
                type = INPUT_KEYBOARD,
                U = new InputUnion
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = keyCode,
                        wScan = 0,
                        dwFlags = KEYEVENTF_KEYDOWN,
                        time = 0,
                        dwExtraInfo = 0

                    }
                }
            };
            inputs[1] = new INPUT
            {
                type = INPUT_KEYBOARD,
                U = new InputUnion
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = keyCode,
                        wScan = 0,
                        dwFlags = KEYEVENTF_KEYUP,
                        time = 0,
                        dwExtraInfo = 0

                    }
                }
            };
            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        public static void PressKeyAdvanced(ushort keyCode, bool keyUp = false)
        {

            var input = new INPUT
            {
                type = INPUT_KEYBOARD,
                U = new InputUnion
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = keyCode,
                        dwFlags = keyUp ? KEYEVENTF_KEYUP : 0
                    }
                }
            };
            SendInput(1, new INPUT[] { input }, Marshal.SizeOf(typeof(INPUT)));
        }
        public static void TypeText(string text, int delayMs = 50)
        {
            foreach (char c in text)
            {
                bool shiftNeeded;
                ushort vk = CharToVk(c, out shiftNeeded);

                if (vk == 0) continue;

                if (shiftNeeded) PressKeyAdvanced(VK_SHIFT);

                PressKeyAdvanced(vk);
                PressKeyAdvanced(vk, keyUp: true);

                if (shiftNeeded) PressKeyAdvanced(VK_SHIFT, keyUp: true);

                Thread.Sleep(delayMs);
            }
        }
        private static ushort CharToVk(char c, out bool shift)
        {
            shift = false;
            if (c >= 'A' && c <= 'Z')
            {
                shift = true;
                return (ushort)(c - 'A' + 0x41);
            }
            else if (c >= 'a' && c <= 'z')
            {
                return (ushort)(c - 'a' + 0x41);
            }
            else if (c >= '0' && c <= '9')
            {
                return (ushort)(c - '0' + 0x30);
            }
            else
            {
                switch (c)
                {
                    case ' ': return 0x20; // space
                    case '!': shift = true; return 0x31; // Shift+1
                    case '@': shift = true; return 0x32; // Shift+2
                    case '#': shift = true; return 0x33;
                    case '$': shift = true; return 0x34;
                    case '%': shift = true; return 0x35;
                    case '^': shift = true; return 0x36;
                    case '&': shift = true; return 0x37;
                    case '*': shift = true; return 0x38;
                    case '(': shift = true; return 0x39;
                    case ')': shift = true; return 0x30;
                    case '.': return 0xBE; // period
                    case ',': return 0xBC;
                    case '?': shift = true; return 0xBF;
                    case ':': shift = true; return 0xBA;
                    case ';': return 0xBA;
                    case '\'': return 0xDE;
                    case '\"': shift = true; return 0xDE;
                    case '-': return 0xBD;
                    case '_': shift = true; return 0xBD;
                    case '=': return 0xBB;
                    case '+': shift = true; return 0xBB;
                    case '/': return 0xBF;
                    case '\\': return 0xDC;
                    case '[': return 0xDB;
                    case ']': return 0xDD;
                    default: return 0; // unsupported char
                }
            }
        }
        public static void MoveMouse()
        {

            INPUT[] inputs = new INPUT[1];
            inputs[0] = new INPUT
            {
                type = INPUT_MOUSE,
                U = new InputUnion
                {
                    mi = new MOUSEINPUT
                    {
                        dx = 50,
                        dy = 0,
                        mouseData = 0,
                        dwFlags = MOUSEEVENTF_MOVE,
                        time = 0,
                        dwExtraInfo = 0
                    }
                }
            };
            SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));

            INPUT[] inputs_2 = new INPUT[2];
            inputs_2[0] = new INPUT
            {
                type = INPUT_MOUSE,
                U = new InputUnion { mi = new MOUSEINPUT { dwFlags = MOUSEEVENTF_LEFTDOWN } }
            };
            inputs_2[1] = new INPUT
            {
                type = INPUT_MOUSE,
                U = new InputUnion { mi = new MOUSEINPUT { dwFlags = MOUSEEVENTF_LEFTUP } }

            };
            SendInput((uint)inputs_2.Length, inputs_2, Marshal.SizeOf(typeof(INPUT)));


        }
    }
}