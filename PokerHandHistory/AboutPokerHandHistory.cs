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
using System.Windows.Forms;
using JeromeTerry.PokerHandHistory.Core;
using JeromeTerry.PokerHandHistory.Native;

namespace JeromeTerry.PokerHandHistory
{
    public partial class AboutPokerHandHistory : Form
    {
        public AboutPokerHandHistory()
        {
            InitializeComponent();
        }

        private void _lnkCodePlex_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = "http://pokerhandhistory.codeplex.com/";
            NativeMethods.ShellExecute(IntPtr.Zero, "open",
                url, "", "", NativeMethods.ShowCommands.SW_SHOW);
        }

        private void _lnkLicense_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string exe = System.Windows.Forms.Application.ExecutablePath;
            System.IO.FileInfo info = new System.IO.FileInfo(exe);
            System.IO.DirectoryInfo dir = info.Directory;
            string file = "license.rtf";
            string license = System.IO.Path.Combine(dir.FullName, file);
            NativeMethods.ShellExecute(IntPtr.Zero, "open",
                license, "", "", NativeMethods.ShowCommands.SW_SHOW);
        }

        private void AboutPokerHandHistory_Load(object sender, EventArgs e)
        {
            System.Reflection.Assembly assembly = 
                System.Reflection.Assembly.GetExecutingAssembly();
            string version = assembly.GetName().Version.ToString();
            _lblVersion.Text = version;
        }
    }
}
