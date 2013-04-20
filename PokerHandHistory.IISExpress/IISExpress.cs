#region License
// Copyright (c) 2012 Jerome Terry
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using JeromeTerry.PokerHandHistory.Core;
using JeromeTerry.PokerHandHistory.Native;

namespace JeromeTerry.PokerHandHistory.Core.IIS
{
    public sealed class IISExpress : IDisposable
    {
        const string IIS_DIR = "IIS Express";
        const string IIS_EXE = "iisexpress.exe";
        const string PATH = "path";
        const string PORT = "port";
        const string SYSTRAY = "systray";

        public string Path { get; protected set; }
        public int Port { get; protected set; }
        public bool SysTray { get; protected set; }

        public static string ProgramFilesFolder
        {
            get
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                return path;
            }
        }

        public static string IISExpressExecutable
        {
            get
            {
                string path = System.IO.Path.Combine(ProgramFilesFolder, IIS_DIR, IIS_EXE);
                return path;
            }
        }

        public static void SendStopMessageToProcess(int PID)
        {
            try
            {
                for (IntPtr ptr = NativeMethods.GetTopWindow(IntPtr.Zero); ptr != IntPtr.Zero; ptr = NativeMethods.GetWindow(ptr, 2))
                {
                    uint num;
                    NativeMethods.GetWindowThreadProcessId(ptr, out num);
                    if (PID == num)
                    {
                        HandleRef hWnd = new HandleRef(null, ptr);
                        NativeMethods.PostMessage(hWnd, 0x12, IntPtr.Zero, IntPtr.Zero);
                        return;
                    }
                }
            }
            catch (ArgumentException)
            {
            }
        }

        public IISExpress(string path, int port, bool sysTray)
        {
            this.Path = path;
            this.Port = port;
            this.SysTray = sysTray;
        }

        public bool Start()
        {
            Logger.Info("Starting IIS Express.");

            string exe = IISExpressExecutable;
            if (System.IO.File.Exists(exe) == false)
            {
                Logger.Error("IIS Express not found at {0}. IIS Express doesn't appear to be installed.",
                    exe);
                return false;
            }

            if (this.IsRunning)
            {
                Logger.Info("Starting IIS Express failed. IIS Express appears to be running.");
                return false;
            }

            StringBuilder arguments = new StringBuilder();
            arguments.AppendFormat("/{0}:\"{1}\" ", PATH, this.Path);
            arguments.AppendFormat("/{0}:{1} ", PORT, this.Port);
            arguments.AppendFormat("/{0}:{1} ", SYSTRAY, this.SysTray);

            IntPtr res = NativeMethods.ShellExecute(IntPtr.Zero, "open", IISExpressExecutable,
                arguments.ToString(), "", NativeMethods.ShowCommands.SW_HIDE);

            return true;
        }

        public IEnumerable<Process> GetIISExpressProcesses()
        {
            Process[] processes = Process.GetProcesses();

            IEnumerable<Process> results =
                from p in processes
                where p.ProcessName.Contains("iisexpress")
                select p;

            return results;
        }

        public bool IsRunning
        {
            get
            {
                IEnumerable<Process> results = this.GetIISExpressProcesses();
                int count = 0;
                if (results != null)
                {
                    count = results.Count();
                }

                bool running = count > 0;
                return running;
            }
        }

        public void Stop()
        {
            Logger.Info("Stopping all IIS Express instances.");

            IEnumerable<Process> results = this.GetIISExpressProcesses();

            int count = results.Count();
            if (count == 0)
            {
                Logger.Info("No IIS Express instances running.");
            }
            else
            {
                Logger.Info("Found {0} IIS Express instances", count);
            }

            foreach (Process p in results)
            {
                Logger.Info("Stopping IIS Express process {0}, id: {1}",
                    p.ProcessName, p.Id);

                SendStopMessageToProcess(p.Id);
                p.Close();
            }

            Logger.Info("Stopping all IIS Express instances complete.");
        }

        public void Dispose()
        {
        }
    }
}
