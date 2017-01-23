using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiCoreLoad
{
    public partial class Form1 : Form
    {
        const int freqIndex = 0;
        const int usageStartIndex = freqIndex + 1;
        int CoreCount;
        Core[] Cores;
        PictureBox[] Graphs;
        PictureBox freqBackground;
        int GraphWidth = 100;
        int GraphHeight = 5;
        Color normal = Color.FromArgb(0, 64, 255);
        Color boost = Color.FromArgb(255, 32, 32);
        Color freqFrame = Color.FromArgb(128, 128, 128);
        Color active = Color.FromArgb(64, 255, 0);
        Color park = Color.FromArgb(32, 128, 0);
        double maxfreq = 100;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            init();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020;
                return cp;
            }
        }

        private void init()
        {
            try
            {
                float dpi = (int)Registry.GetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop\WindowMetrics", "AppliedDPI", 96);
                float scale = dpi / 96;
                Console.WriteLine($"{dpi}dpi:{(scale * 100)}%");

                GraphWidth = (int)(100 * scale);
                GraphHeight = (int)(5 * scale);

                CoreCount = Environment.ProcessorCount;
                Cores = new Core[CoreCount];
                Graphs = new PictureBox[CoreCount + 1];
                maxfreq = 100;

                for (int c = 0; c < CoreCount; c++)
                {
                    Cores[c] = new Core(c);
                }

                for (int i = 0; i < CoreCount + usageStartIndex; i++)
                {
                    PictureBox pic = new PictureBox();

                    pic.Width = GraphWidth;
                    pic.Height = GraphHeight;
                    pic.Top = pic.Height * i + i;
                    pic.Left = 0;

                    if (i == freqIndex)
                    {
                        freqBackground = new PictureBox();
                        freqBackground.Width = pic.Width;
                        freqBackground.Height = pic.Height;
                        freqBackground.Top = pic.Top;
                        freqBackground.Left = pic.Left;
                        freqBackground.BackColor = normal;
                        pic.BackColor = boost;
                    }
                    else
                    {
                        pic.BackColor = active;
                    }

                    Graphs[i] = pic;
                }

                SuspendLayout();
                Controls.AddRange(Graphs);
                Controls.Add(freqBackground);
                ResumeLayout(false);

                ShowInTaskbar = false;
                LocationSet();

                DoWork();

                Worker.Interval = 500;
                Worker.Enabled = true;

                GC.Collect();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void Worker_Tick(object sender, EventArgs e)
        {
            DoWork();
            LocationSet();
        }

        private void DoWork()
        {
            double[] usage = new double[CoreCount];
            bool[] parked = new bool[CoreCount];
            double[] freq = new double[CoreCount];

            Parallel.For(0, CoreCount, id =>
            {
                usage[id] = Cores[id].Load();
                parked[id] = Cores[id].Parked();
                freq[id] = Cores[id].Freq();
            });

            double avefreq = freq.Average();

            for (int i = 0; i < CoreCount + usageStartIndex; i++)
            {
                if (i == freqIndex)
                {
                    maxfreq = Math.Max(maxfreq, avefreq);
                    freqBackground.Width = (avefreq <= 100) ? (int)Math.Round(GraphWidth / 100 * avefreq) : GraphWidth;
                    Graphs[i].Width = (avefreq > 100) ? (int)Math.Round(GraphWidth / 100 * (avefreq - 100)) : 0;
                }
                else
                {
                    Graphs[i].Width = (int)Math.Round(GraphWidth / 100 * usage[i - usageStartIndex]);
                    Graphs[i].BackColor = (!parked[i - usageStartIndex]) ? active : park;
                }
            }
        }

        private void LocationSet()
        {
            TopLevel = true;
            TopMost = true;

            if (WindowState != FormWindowState.Normal ||
                Height != (GraphHeight + 1) * (CoreCount + usageStartIndex) ||
                Width != GraphWidth ||
                Top != Screen.PrimaryScreen.WorkingArea.Height - Height + Screen.PrimaryScreen.WorkingArea.Top ||
                Left != Screen.PrimaryScreen.WorkingArea.Width - Width + Screen.PrimaryScreen.WorkingArea.Left)
            {
                int oldTop = Top;
                int oldLeft = Left;

                WindowState = FormWindowState.Normal;
                Height = (GraphHeight + 1) * (CoreCount + usageStartIndex);
                Width = GraphWidth;
                Top = Screen.PrimaryScreen.WorkingArea.Height - Height + Screen.PrimaryScreen.WorkingArea.Top;
                Left = Screen.PrimaryScreen.WorkingArea.Width - Width + Screen.PrimaryScreen.WorkingArea.Left;

                Console.WriteLine($"{nameof(Top)}:{oldTop}->{Top}");
                Console.WriteLine($"{nameof(Left)}:{oldLeft}->{Left}");
            }
        }

        private void 終了ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void RestartMenuItem_Click(object sender, EventArgs e)
        {
            Worker.Enabled = false;
            Thread.Sleep(100);

            SuspendLayout();
            foreach (Control ctrl in Graphs)
            {
                Controls.Remove(ctrl);
            }
            Controls.Remove(freqBackground);
            ResumeLayout(false);
            Thread.Sleep(100);

            init();
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            Activate();
        }
    }
}
