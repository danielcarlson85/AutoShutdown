using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utilities;

namespace AutoavstägningCS
{
    public partial class MainForm : Form
    {
        readonly KeyboardHandler _keyboardKey = new();
        private readonly Timer _shutdownTimer = new();
        private readonly Timer _updateUITimer = new();
        private Keys _key = Keys.F8;
        private int _insertkeyPressed3seconds = 0;
        private int _shutdownCounter { get; set; }
        private int _times { get; set; }
        private int _elapsedTime { get; set; } = 0;
        private string _insertkeyPressed { get; set; }

        public MainForm()
        {
            InitializeComponent();

            _shutdownTimer.Enabled = true;
            _shutdownTimer.Interval = 1000;
            _shutdownTimer.Tick += new System.EventHandler(this.ShutdownTimer_Tick);

            _updateUITimer.Enabled = true;
            _updateUITimer.Interval = 1;
            _updateUITimer.Tick += new System.EventHandler(this.UpdateUITimer_Tick);

            _keyboardKey.HookedKeys.Add(_key);

            _keyboardKey.KeyDown += new KeyEventHandler((o, e) => { _insertkeyPressed = e.KeyCode.ToString(); e.Handled = true; });
            _keyboardKey.KeyUp += new KeyEventHandler((o, e) => { _insertkeyPressed = string.Empty; e.Handled = true; });
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
            await Task.Run(async () =>
            {
                _insertkeyPressed = string.Empty;

                TimeSpan time = TimeSpan.FromSeconds(_elapsedTime);

                string h = time.ToString().Substring(0, 2);                                                   //tar ut timme från sekunder
                string min = time.ToString().Substring(3, 2);                                                 //tar ut timme från sekunder
                string sec = time.ToString().Substring(6, 2);                                                 //tar ut minut från sekunder

                var timeNowInMinutes = _shutdownCounter / 60;

                var isTeamsRunning = await CheckIfTeamsIsRunning();

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
                if (!await CheckIfTeamsIsRunning())
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

        async Task<bool> CheckIfTeamsIsRunning()
        {
            List<Process> process = new();
            await Task.Run(() => process = Process.GetProcessesByName("Teams")
                      .AsEnumerable()
                      .ToList());

            return process.Count != 0;
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

