using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace MultiCoreLoad
{
    public partial class Form1 : Form
    {
        private const int freqIndex = 0;
        private const int usageStartIndex = freqIndex + 1;
        private const ProcessPriorityClass processPriority = ProcessPriorityClass.Idle;
        private int CoreCount;
        private Core[] Cores;
        private double[] usage;
        private bool[] parked;
        private double[] freq;
        private double maxfreq;
        private double[] idle;
        private double[] effective;
        private PictureBox[] Graphs;
        private PictureBox freqBackground;
        private readonly int GraphWidth = 100;
        private readonly int GraphHeight = 5;
        private readonly Color normal = Color.LightSkyBlue;
        private readonly Color boost = Color.Red;
        private readonly Color active = Color.Lime;
        private readonly Color park = Color.DarkGreen;
        private string configFile = "config.json";
        private string execPath = Application.ExecutablePath.Remove(Application.ExecutablePath.LastIndexOf('\\') + 1);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Init();
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

        private void Init()
        {
            try
            {
                if (Process.GetCurrentProcess().PriorityClass != processPriority)
                {
                    Process.GetCurrentProcess().PriorityClass = processPriority;
                }

                Opacity = 0.75;

                CoreCount = Environment.ProcessorCount;
                Cores = new Core[CoreCount];
                Graphs = new PictureBox[CoreCount + 1];

                ShowInTaskbar = false;
                LocationSet();

                usage = new double[CoreCount];
                parked = new bool[CoreCount];
                freq = new double[CoreCount];
                idle = new double[CoreCount];
                effective = new double[CoreCount];

                for (int c = 0; c < CoreCount; c++)
                {
                    Cores[c] = new Core(c);
                }

                for (int i = 0; i < CoreCount + usageStartIndex; i++)
                {
                    PictureBox pic = new PictureBox()
                    {
                        Width = GraphWidth,
                        Height = GraphHeight
                    };

                    pic.Top = pic.Height * i + i;
                    pic.Left = 0;

                    if (i == freqIndex)
                    {
                        freqBackground = new PictureBox()
                        {
                            Width = pic.Width,
                            Height = pic.Height,
                            Top = pic.Top,
                            Left = pic.Left,
                            BackColor = normal
                        };

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

                DoWork();

                Worker.Enabled = true;

                GC.Collect();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void Worker_Tick(object sender, EventArgs e)
        {
            DoWork();
            LocationSet();
        }

        private void DoWork()
        {
            Parallel.For(0, CoreCount, id =>
            {
                usage[id] = Cores[id].Load();
                parked[id] = Cores[id].Parked();
                freq[id] = Cores[id].Freq();
                idle[id] = Cores[id].Idle();
                effective[id] = (100 - idle[id]) * freq[id] / 100;
            });

            maxfreq = freq.Max();
            if (effectiveClockToolStripMenuItem.Checked)
            {
                maxfreq = effective.Max();
            }

            for (int i = 0; i < CoreCount + usageStartIndex; i++)
            {
                if (i == freqIndex)
                {
                    freqBackground.Width = Math.Min(GraphWidth, (int)Math.Round(GraphWidth / 100 * maxfreq));
                    Graphs[i].Width = Math.Max(0, (int)Math.Round(GraphWidth / 100 * (maxfreq - 100)));
                }
                else
                {
                    Graphs[i].Width = (int)Math.Round(GraphWidth / 100 * usage[i - usageStartIndex]);
                    Graphs[i].BackColor = (parked[i - usageStartIndex]) ? park : (usage[i - usageStartIndex] > 99.9) ? boost : active;
                }
            }
        }

        private void LocationSet()
        {
            if (Height != (GraphHeight + 1) * (CoreCount + usageStartIndex) ||
                Width != GraphWidth ||
                Top != Screen.PrimaryScreen.WorkingArea.Height - Height + Screen.PrimaryScreen.WorkingArea.Top ||
                Left != Screen.PrimaryScreen.WorkingArea.Width - Width + Screen.PrimaryScreen.WorkingArea.Left)
            {
                int oldTop = Top;
                int oldLeft = Left;

                Height = (GraphHeight + 1) * (CoreCount + usageStartIndex);
                Width = GraphWidth;
                Top = Screen.PrimaryScreen.WorkingArea.Height - Height + Screen.PrimaryScreen.WorkingArea.Top;
                Left = Screen.PrimaryScreen.WorkingArea.Width - Width + Screen.PrimaryScreen.WorkingArea.Left;

                Debug.WriteLine($"{nameof(Top)}:\t{oldTop}\t->\t{Top}");
                Debug.WriteLine($"{nameof(Left)}:\t{oldLeft}\t->\t{Left}");
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ResetMenuItem_Click(object sender, EventArgs e)
        {
            Reset();
        }
        private void Reset()
        {
            Debug.WriteLine("Resetting");
            Debug.WriteLine($"{nameof(TopMost)}:{TopMost}");

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

            Init();
        }
        private void activeClockToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            effectiveClockToolStripMenuItem.Checked = !activeClockToolStripMenuItem.Checked;
        }

        private void effectiveClockToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            activeClockToolStripMenuItem.Checked = !effectiveClockToolStripMenuItem.Checked;
        }

    }
}
