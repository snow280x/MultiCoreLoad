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
        int maxNormalfreq = 99;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                CoreCount = Environment.ProcessorCount;
                Cores = new Core[CoreCount];
                Graphs = new PictureBox[CoreCount + 1];

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
                        freqBackground.BackColor = freqFrame;
                        pic.BackColor = normal;
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

                BackColor = Color.FromArgb(32, 32, 32);
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

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020;
                return cp;
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
            double freq = Cores[0].Freq();

            Parallel.For(0, CoreCount, id =>
            {
                usage[id] = Cores[id].Load();
                parked[id] = Cores[id].Parked();
            });

            for (int i = 0; i < CoreCount + usageStartIndex; i++)
            {
                if (i == freqIndex)
                {
                    maxfreq = Math.Max(maxfreq, freq);
                    freqBackground.Width = (int)(GraphWidth / maxfreq * 100);
                    Graphs[i].Width = (int)(GraphWidth / maxfreq * freq);
                    Graphs[i].BackColor = (freq >= maxNormalfreq) ? boost : normal;
                    Console.WriteLine($"{nameof(freq)}:{freq}");
                }
                else
                {
                    Graphs[i].Width = (int)(GraphWidth / 100 * usage[i - usageStartIndex]);
                    Graphs[i].BackColor = (!parked[i - usageStartIndex]) ? active : park;
                    Console.WriteLine($"{nameof(usage)}[{i - usageStartIndex}]:{usage[i - usageStartIndex]}");
                }
            }
        }

        private void LocationSet()
        {
            if (WindowState != FormWindowState.Normal ||
                Height != (GraphHeight + 1) * (CoreCount + usageStartIndex) ||
                Width != GraphWidth ||
                Top != Screen.PrimaryScreen.WorkingArea.Height - Height + Screen.PrimaryScreen.WorkingArea.Top ||
                Left != Screen.PrimaryScreen.WorkingArea.Width - Width + Screen.PrimaryScreen.WorkingArea.Left)
            {
                int oldTop = Top;
                int oldLeft = Left;

                Console.WriteLine($"{nameof(TopLevel)}:{TopLevel = true}");
                Console.WriteLine($"{nameof(TopMost)}:{TopMost = true}");

                //TopLevel = true;
                //TopMost = true;
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

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            Activate();
        }
    }
}
