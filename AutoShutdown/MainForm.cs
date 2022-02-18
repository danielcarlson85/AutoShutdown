using System;
using System.Diagnostics;
using System.Windows.Forms;
using Utilities;

namespace Autoavst�gningCS
{
    public partial class MainForm : Form
    {
        readonly KeyboardHandler _keyboardKey = new();

        private readonly Timer _timer1 = new();
        private int _insertkeyPressed3seconds = 0;
        private int _shutdownCounter { get;set; }
        private int _times { get; set; }
        private int _elapsedTime { get; set; }  = 0;
        private string _insertkeyPressed { get; set; }

        public MainForm()
        {
            InitializeComponent();

            _timer1.Enabled = true;
            _timer1.Interval = 1000;
            _timer1.Tick += new System.EventHandler(this.Timer1_Tick);

            _keyboardKey.HookedKeys.Add(Keys.Insert);
            _keyboardKey.HookedKeys.Add(Keys.Insert);

            _keyboardKey.KeyUp += new KeyEventHandler((o, e) => { _insertkeyPressed = string.Empty; e.Handled = true; });
            _keyboardKey.KeyDown += new KeyEventHandler((o, e) => { _insertkeyPressed = e.KeyCode.ToString(); e.Handled = true; });
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (_insertkeyPressed == Keys.Insert.ToString())
            {
                _insertkeyPressed3seconds++;

                if (_insertkeyPressed3seconds.ToString() == "3")
                {
                    ShowTimerMessageBox();
                }
            }
            else
            {
                _insertkeyPressed3seconds = 0;
            }

            _shutdownCounter++;
            _elapsedTime++;

            StartShutdownProcess();

            UpdateTextBoxes();
        }

        private void UpdateTextBoxes()
        {
            //g�r eandast att variablerna �r synliga i labels (kontrollsyfte)
            label1.Text = _shutdownCounter.ToString();
            label2.Text = _times.ToString();
            label3.Text = _elapsedTime.ToString();
            label4.Text = _insertkeyPressed;
            label9.Text = _insertkeyPressed3seconds.ToString();
        }

        private void ShowTimerMessageBox()
        {
            TimeSpan time = TimeSpan.FromSeconds(_elapsedTime);

            string h = time.ToString().Substring(0, 2);                                                   //tar ut timme fr�n sekunder
            string min = time.ToString().Substring(3, 2);                                                 //tar ut timme fr�n sekunder
            string sec = time.ToString().Substring(6, 2);                                                 //tar ut minut fr�n sekunder

            if (h.Substring(0, 1).ToString() == "0") { h = h.Substring(1, 1); }                           //kollar ifall 0 finns med tar d� bort den.
            if (min.Substring(0, 1).ToString() == "0") { min = min.Substring(1, 1); }                     //kollar ifall 0 finns med tar d� bort den.
            if (sec.Substring(0, 1).ToString() == "0") { sec = sec.Substring(1, 1); }                     //kollar ifall 0 finns med tar d� bort den.

            MessageBox.Show("Du har startat om datorn: " + _times.ToString() + " g�nger\n" +
                        "Din tid vid datorn �r " + h + " timmar, " + min + " minuter och " + sec + " sekunder.\n",
                        "Din tid vid datorn", System.Windows.Forms.MessageBoxButtons.OK,
                        MessageBoxIcon.Information,
                        MessageBoxDefaultButton.Button1,
                        MessageBoxOptions.DefaultDesktopOnly);
            _insertkeyPressed3seconds = 0;
        }

        private void StartShutdownProcess()
        {
            if (_shutdownCounter >= 2700)
            {
                if (!CheckIfTeamsIsRunning())
                {
                    _times++;
                    _shutdownCounter = 0;
                    Process.Start(@"C:\WINDOWS\system32\rundll32.exe", "user32.dll,LockWorkStation");
                }
                else
                {
                    _shutdownCounter = 0;
                }
            }
        }

        bool CheckIfTeamsIsRunning()
        {
            var process = Process.GetProcessesByName("Teams");
            return process.Length != 0;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void lstLog_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }
    }
}

