using System;
using System.Diagnostics;
using System.Windows.Forms;
using Utilities;

namespace Autoavst�gningCS
{
    public partial class MainForm : Form
    {
        KeyboardHandler keyboardKey = new KeyboardHandler();
        private Timer timer1 = new Timer();

        int insertkeyPressed3seconds = 0;
        int shutdownCounter = 0;
        int times = 0;
        int elapsedTime = 0;
        string insertkeyPressed;

        public MainForm()
        {
            InitializeComponent();

            CheckIfTeamsIsRunning();

            timer1.Enabled = true;
            timer1.Interval = 1000;
            timer1.Tick += new System.EventHandler(this.Timer1_Tick);

            keyboardKey.HookedKeys.Add(Keys.Insert);
            keyboardKey.HookedKeys.Add(Keys.Insert);

            keyboardKey.KeyUp += new KeyEventHandler((o, e) => { insertkeyPressed = string.Empty; e.Handled = true; });
            keyboardKey.KeyDown += new KeyEventHandler((o, e) => { insertkeyPressed = e.KeyCode.ToString(); e.Handled = true; });
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            UpdateTextBoxes();

            if (insertkeyPressed == Keys.Insert.ToString())
            {
                insertkeyPressed3seconds++;

                if (insertkeyPressed3seconds.ToString() == "3")
                {
                    ShowTimerMessageBox();
                }
            }
            else
            {
                insertkeyPressed3seconds = 0;
            }

            shutdownCounter++;
            elapsedTime++;

            StartShutdownProcess();
        }

        private void UpdateTextBoxes()
        {
            //g�r eandast att variablerna �r synliga i labels (kontrollsyfte)
            label1.Text = shutdownCounter.ToString();
            label2.Text = times.ToString();
            label3.Text = elapsedTime.ToString();
            label4.Text = insertkeyPressed;
            label9.Text = insertkeyPressed3seconds.ToString();
        }

        private void ShowTimerMessageBox()
        {
            TimeSpan ts = TimeSpan.FromSeconds(elapsedTime);

            string h = ts.ToString().Substring(0, 2);                                                     //tar ut timme fr�n sekunder
            string min = ts.ToString().Substring(3, 2);                                                   //tar ut timme fr�n sekunder
            string sec = ts.ToString().Substring(6, 2);                                                   //tar ut minut fr�n sekunder

            if (h.Substring(0, 1).ToString() == "0") { h = h.Substring(1, 1); }                           //kollar ifall 0 finns med tar d� bort den.
            if (min.Substring(0, 1).ToString() == "0") { min = min.Substring(1, 1); }                     //kollar ifall 0 finns med tar d� bort den.
            if (sec.Substring(0, 1).ToString() == "0") { sec = sec.Substring(1, 1); }                     //kollar ifall 0 finns med tar d� bort den.

            MessageBox.Show("Du har startat om datorn: " + times.ToString() + " g�nger\n" +
                        "Din tid vid datorn �r " + h + " timmar, " + min + " minuter och " + sec + " sekunder.\n",
                        "Din tid vid datorn", System.Windows.Forms.MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
            insertkeyPressed3seconds = 0;
        }

        private void StartShutdownProcess()
        {

            Process sh = new Process();
            if (shutdownCounter >= 1800)
            {
                if (!CheckIfTeamsIsRunning())
                {
                    times++;
                    shutdownCounter = 0;
                    Process.Start("shutdown", "/h");
                }
            }
        }

        bool CheckIfTeamsIsRunning()
        {
            Process[] localByName = Process.GetProcessesByName("Teams");

            if (localByName.Length != 0)
            {
                return true;
            }

            return false;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void lstLog_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }
    }
}

