using System;
using System.Runtime.InteropServices;

namespace MyInputLibrary
{
    
    public static class InputHelper1
    {

        [DllImport("user32.dll")]
        private static extern short VkKeyScanEx(char ch, IntPtr dwhkl);

        [DllImport("user32.dll")]
        private static extern IntPtr GetKeyboardLayout(uint idThread);
        
        public static bool TestingMode {get; set; } = false;

        private static List<INPUT> _capturedInputs = new List<INPUT>();

        public static IReadOnlyList<INPUT> CapturedInputs => _capturedInputs.AsReadOnly();

        private static uint SendInputSafe(uint nInputs, INPUT[] pInputs, int cbSize)
        {
            if (TestingMode)
            {
                _capturedInputs.AddRange(pInputs);
                return nInputs; // pretend all succeeded
            }
            else
            {
                return SendInput(nInputs, pInputs, cbSize);
            }
        }
        
        // Define Input Types
        public const int INPUT_MOUSE = 0;
        public const int INPUT_KEYBOARD = 1;

        // Keyboard event flags
        public const uint KEYEVENTF_KEYDOWN = 0x0000;
        public const uint KEYEVENTF_KEYUP = 0x0002;
        public const ushort VK_SHIFT = 0x10;

        // Mouse event flags
        public const uint MOUSEEVENTF_MOVE = 0x0001;
        public const uint MOUSEEVENTF_ABSOLUTE = 0x8000;

        public const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        public const uint MOUSEEVENTF_LEFTUP = 0x0004;
        public const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        public const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
        public const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        public const uint MOUSEEVENTF_RIGHTUP = 0x0010;

        public const uint MOUSEEVENTF_WHEEL = 0x0800;



        // Virtual Key codes
        private const ushort VK_A = 0x41; // 'A'

        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            public int type;
            public InputUnion U;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct InputUnion
        {
            [FieldOffset(0)] public MOUSEINPUT mi;
            [FieldOffset(0)] public KEYBDINPUT ki;

        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;

        }
        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;

        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);



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
                        dwExtraInfo = IntPtr.Zero

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
                        dwExtraInfo = IntPtr.Zero

                    }
                }
            };
            SendInputSafe((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
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
            SendInputSafe(1, new INPUT[] { input }, Marshal.SizeOf(typeof(INPUT)));
        }

        public static void WinLeft(){
            const ushort VK_LWIN = 0x5B;
            PressKey(VK_LWIN);
        }

        public static void SendKeyCombo(params ushort[] keys){
            // press all keys
            foreach (var key in keys)
                PressKeyAdvanced(key);
            
            // release in reverse order for no reason
            for (int i = keys.Length - 1; i >= 0; i--)
                PressKeyAdvanced(keys[i], keyUp: true);
        }

        public static void Kill(){
            const ushort VK_MENU = 0x12;
            const ushort VK_F4 = 0x73;
            SendKeyCombo(VK_MENU, VK_F4);
        }
        public static void SelectAll(){
            const ushort VK_CTRL = 0x11;
            const ushort VK_A = 0x41;
            SendKeyCombo(VK_CTRL, VK_A);
        }
        public static void WinRun(){
            const ushort VK_LWIN = 0x5B;
            const ushort VK_R = 0x52;
            SendKeyCombo(VK_LWIN, VK_R);
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
        public static ushort CharToVk(char c, out bool shift)
        {
            // key combinations are layout dependent
            IntPtr layout = GetKeyboardLayout(0); 
            short vkAndModifiers = VkKeyScanEx(c, layout);
            if (vkAndModifiers == -1)
            {
                // character cannot be typed on this layout
                shift = false;
                return 0;
            }
            shift = (vkAndModifiers & 0x0100) != 0;
            ushort vk = (ushort)(vkAndModifiers & 0xFF);

            return vk;
        }

        public static void MoveMouseRelative(int move_x, int move_y)
        {

            INPUT[] inputs = new INPUT[1];
            inputs[0] = new INPUT
            {
                type = INPUT_MOUSE,
                U = new InputUnion
                {
                    mi = new MOUSEINPUT
                    {
                        dx = move_x,
                        dy = move_y,
                        mouseData = 0,
                        dwFlags = MOUSEEVENTF_MOVE,
                        time = 0,
                        dwExtraInfo = IntPtr.Zero
                    }
                }
            };
            SendInputSafe(1, inputs, Marshal.SizeOf(typeof(INPUT)));
        }
        public static void MoveMouseAbsolute(int x, int y)
        {

            INPUT[] inputs = new INPUT[1];
            inputs[0] = new INPUT
            {
                type = INPUT_MOUSE,
                U = new InputUnion
                {
                    mi = new MOUSEINPUT
                    {
                        dx = (int)(x * 65535 / GetSystemMetrics(0)),
                        dy = (int)(y * 65535 / GetSystemMetrics(1)),
                        mouseData = 0,
                        dwFlags = MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE,
                        time = 0,
                        dwExtraInfo = IntPtr.Zero
                    }
                }
            };
            SendInputSafe(1, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);

        public static void ClickMouseLeft()
        {
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
            SendInputSafe((uint)inputs_2.Length, inputs_2, Marshal.SizeOf(typeof(INPUT)));


        }
        public static void ClickMouseMiddle()
        {
            INPUT[] inputs_2 = new INPUT[2];
            inputs_2[0] = new INPUT
            {
                type = INPUT_MOUSE,
                U = new InputUnion { mi = new MOUSEINPUT { dwFlags = MOUSEEVENTF_MIDDLEDOWN } }
            };
            inputs_2[1] = new INPUT
            {
                type = INPUT_MOUSE,
                U = new InputUnion { mi = new MOUSEINPUT { dwFlags = MOUSEEVENTF_MIDDLEUP } }

            };
            SendInputSafe((uint)inputs_2.Length, inputs_2, Marshal.SizeOf(typeof(INPUT)));


        }
        public static void ClickMouseRight()
        {
            INPUT[] inputs_2 = new INPUT[2];
            inputs_2[0] = new INPUT
            {
                type = INPUT_MOUSE,
                U = new InputUnion { mi = new MOUSEINPUT { dwFlags = MOUSEEVENTF_RIGHTDOWN } }
            };
            inputs_2[1] = new INPUT
            {
                type = INPUT_MOUSE,
                U = new InputUnion { mi = new MOUSEINPUT { dwFlags = MOUSEEVENTF_RIGHTUP } }

            };
            SendInputSafe((uint)inputs_2.Length, inputs_2, Marshal.SizeOf(typeof(INPUT)));


        }
        public static void SpinMouseWheel(int amount)
        {
            INPUT[] input_wheel = new INPUT[1];
            input_wheel[0] = new INPUT
            {
                type = INPUT_MOUSE,
                U = new InputUnion { mi = new MOUSEINPUT 
                { 
                    dwFlags = MOUSEEVENTF_WHEEL,
                    mouseData = unchecked((uint)amount)
                    } }
            };
            SendInputSafe((uint)input_wheel.Length, input_wheel, Marshal.SizeOf(typeof(INPUT)));


        }


        public static void ClearCaptured() => _capturedInputs.Clear();
    }
}
// build with "dotnet build -c Release" 
// then take from \bin\Release\netstandard2.1\InputController.dll