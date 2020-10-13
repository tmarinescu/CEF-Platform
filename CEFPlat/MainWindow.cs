using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CEFPlat
{
    public partial class MainWindow : Form
    {
        public CEF MainBrowser = new CEF();

        public MainWindow()
        {
            //Do the setup
            Debugger.DebugFunction(() =>
            {
                InitializeComponent();
            }, "Booting up");

            //Setup the main tick timer
            Debugger.DebugFunction(() =>
            {
                Timers.MainTimer = new Timer();
                Timers.MainTimer.Tick += new EventHandler(Timers.TimerTick);
                Timers.MainTimer.Interval = 1;
                Timers.MainTimer.Enabled = true;
                Timers.MainTimer.Start();
            }, "Setting up data");

            //Create the browser control
            if (MainBrowser.InitBrowser())
                this.Controls.Add(MainBrowser.Handle);
            else
                MessageBox.Show("Failed to initialize CEF");
        }
    }
    public static class Timers
    {
        public static Timer MainTimer = null;
        private static Dictionary<string, Action> _bindedEvents = new Dictionary<string, Action>();

        public static void TimerTick(object sender, EventArgs e)
        {
            foreach(KeyValuePair<string, Action> pair in _bindedEvents.ToDictionary(x => x.Key, x => x.Value))
            {
                try
                {
                    pair.Value.Invoke();
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Tick Event Failed & Removed. ID:({pair.Key}) Ex:({ex.Message})");
                    _bindedEvents.Remove(pair.Key);
                }
            }
        }

        public static bool BindTickEvent(string id, Action evt)
        {
            if(_bindedEvents.ContainsKey(id))
            {
                return false;
            }
            else
            {
                if(evt == null)
                {
                    return false;
                }
                else
                {
                    _bindedEvents.Add(id, evt);
                }
            }

            return true;
        }

        public static bool UnbindTickEvent(string id)
        {
            if(_bindedEvents.ContainsKey(id))
            {
                _bindedEvents.Remove(id);
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}
