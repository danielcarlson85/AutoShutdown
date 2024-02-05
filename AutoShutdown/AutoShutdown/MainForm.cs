using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoavstägningCS
{
    public partial class MainForm : Form
    {
        private readonly Timer _messageBoxTimer = new Timer();
        private readonly Timer _keyPressTimer = new Timer();
        private readonly Timer _updateUITimer = new Timer();
        private readonly NotifyIcon notifyIcon = new NotifyIcon();

        private Keys _key = Keys.Alt;

        private int _insertkeyPressed3seconds = 0;
        private int _shutdownCounter { get; set; }
        private int _times { get; set; }
        private int _elapsedTime { get; set; } = 0;
        private string _insertkeyPressed { get; set; }

        public MainForm()
        {
            InitializeComponent();
            InitializeNotifyIcon();


            _messageBoxTimer.Interval = 1000;
            _messageBoxTimer.Tick += new System.EventHandler(ShutdownTimer_Tick);
            _messageBoxTimer.Start();

            _keyPressTimer.Interval = 100;
            _keyPressTimer.Tick += new System.EventHandler(KeyPressTimer_Tick);
            _keyPressTimer.Start();

            _updateUITimer.Interval = 1;
            _updateUITimer.Tick += new System.EventHandler(UpdateUITimer_Tick);
            _updateUITimer.Start();
        }

        private void InitializeNotifyIcon()
        {
            notifyIcon.Icon = new Icon("icon.ico");
            notifyIcon.Visible = true;
        }

        private void KeyPressTimer_Tick(object sender, EventArgs e)
        {
            if (ModifierKeys == _key)
            {
                _insertkeyPressed = _key.ToString();
            }
            else
            {
                _insertkeyPressed = string.Empty;
            }
        }


        private async void ShutdownTimer_Tick(object sender, EventArgs e)
        {
            await Task.Run(async () =>
            {
                if (_insertkeyPressed == _key.ToString())
                {
                    _insertkeyPressed3seconds++;

                    if (_insertkeyPressed3seconds.ToString() == "3")
                    {
                        await ShowTimerMessageBoxAsync();
                    }
                }
                else
                {
                    _insertkeyPressed3seconds = 0;
                }

                _shutdownCounter++;
                _elapsedTime++;

                await StartShutdownProcessAsync();

            });
        }

        private void UpdateTextBoxes()
        {
            //gör eandast att variablerna är synliga i labels (kontrollsyfte)
            label1.Text = _shutdownCounter.ToString();
            label2.Text = _times.ToString();
            label3.Text = _elapsedTime.ToString();
            label4.Text = _insertkeyPressed;
            label9.Text = _insertkeyPressed3seconds.ToString();
        }

        private async Task ShowTimerMessageBoxAsync()
        {
            await Task.Run(() =>
            {
                _insertkeyPressed = string.Empty;

                TimeSpan time = TimeSpan.FromSeconds(_elapsedTime);

                string h = time.ToString().Substring(0, 2);                                                   //tar ut timme från sekunder
                string min = time.ToString().Substring(3, 2);                                                 //tar ut timme från sekunder
                string sec = time.ToString().Substring(6, 2);                                                 //tar ut minut från sekunder

                var timeNowInMinutes = _shutdownCounter / 60;

                var isTeamsRunning = CheckIfTeamsIsRunning();

                if (h.Substring(0, 1).ToString() == "0") { h = h.Substring(1, 1); }                           //kollar ifall 0 finns med tar då bort den.
                if (min.Substring(0, 1).ToString() == "0") { min = min.Substring(1, 1); }                     //kollar ifall 0 finns med tar då bort den.
                if (sec.Substring(0, 1).ToString() == "0") { sec = sec.Substring(1, 1); }                     //kollar ifall 0 finns med tar då bort den.

                MessageBox.Show(
                        "Du har startat om datorn: " + _times + " gånger\n" +
                        "Din totala tid vid datorn är: " + h + " timmar, " + min + " min och " + sec + " sekunder. \n" +
                        "Din tid vid datorn just nu är: " + timeNowInMinutes + " minuter. \n" +
                        "Körs Teams: " + isTeamsRunning,

                        "Din tid vid datorn",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        MessageBoxIcon.Information,
                        MessageBoxDefaultButton.Button1,
                        MessageBoxOptions.DefaultDesktopOnly);

                _insertkeyPressed3seconds = 0;
            });
        }

        private async Task StartShutdownProcessAsync()
        {
            if (_shutdownCounter >= 2700)
            {
                if (!CheckIfTeamsIsRunning())
                {
                    _times++;
                    _shutdownCounter = 0;
                    await Task.Run(() => Process.Start(@"shutdown", "-h"));
                }
                else
                {
                    _shutdownCounter = 0;
                }
            }
        }

        static bool CheckIfTeamsIsRunning()
        {
            //List<Process> process = new List<Process>();
            //await Task.Run(() => process = Process.GetProcessesByName("Teams")
            //          .AsEnumerable()
            //          .ToList());

            //return process.Count != 0;

            return false;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Hide();
        }

        private void lstLog_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void UpdateUITimer_Tick(object sender, EventArgs e)
        {
            UpdateTextBoxes();
        }
    }
}

