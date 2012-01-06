using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Keyboard;
using Keyboard.PInvoke;
using Keyboard.PInvoke.Messaging;

namespace SampleUI
{
    public partial class MainForm : Form
    {
        private readonly Dictionary<Process, IKeyboardDriver> _drivers = new Dictionary<Process, IKeyboardDriver>();

        public MainForm()
        {
            InitializeComponent();
        }

        private void ProcessRefresherTick(object sender, EventArgs e)
        {
            IEnumerable<Process> newProcs = NewWowProcesses();

            foreach (Process newProc in newProcs)
            {
                Process proc = newProc;
                if (proc.MainWindowHandle == IntPtr.Zero) continue;
                _drivers[proc] = CreateKeyboardDriver(proc);
                _lstProcesses.Items.Add(proc.Id);
                //_drivers[proc].Write("/hithere \n"); // '/' doesn't work
                proc.Exited += (s, ev) =>
                {
                    _drivers.Remove(proc);
                    _lstProcesses.Items.Remove(proc.Id);
                };
            }
        }

        private static KeyboardDriver CreateKeyboardDriver(Process proc)
        {
            return
                new KeyboardDriver(
                    new MessagingKeyboard(proc.MainWindowHandle, new WindowsMessageEmitter(), new KeyCodeMapper()),
                    new KeyCodeMapper());
        }

        private IEnumerable<Process> NewWowProcesses()
        {
            IEnumerable<Process> newProcs = from p in Process.GetProcessesByName("wow")
                                            where _drivers.Keys.All(o => o.Id != p.Id)
                                            select p;
            return newProcs;
        }

        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            foreach (IKeyboardDriver driver in _drivers.Values)
            {
                try
                {
                    driver.Write(e.KeyChar.ToString());
                }
                catch (Win32Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error while simulating key press");
                }
            }
        }
    }
}