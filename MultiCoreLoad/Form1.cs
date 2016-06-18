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
        int CoreCount;
        Core[] Cores;
        PictureBox[] Graphs;
        int GraphWidth = 100;
        int GraphHeight = 5;
        Color active = Color.FromArgb(64, 255, 0);
        Color park = Color.FromArgb(32, 128, 0);

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
                Graphs = new PictureBox[CoreCount];

                for (int i = 0; i < CoreCount; i++)
                {
                    Cores[i] = new Core(i);
                    PictureBox pic = new PictureBox();
                    pic.Width = GraphWidth;
                    pic.Height = GraphHeight;
                    pic.Top = pic.Height * i + i;
                    pic.Left = 0;
                    pic.BackColor = Color.FromArgb(64, 255, 0);
                    Graphs[i] = pic;
                }

                SuspendLayout();
                Controls.AddRange(Graphs);
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

            Parallel.For(0, CoreCount, id =>
            {
                usage[id] = Cores[id].Load();
                parked[id] = Cores[id].Parked();
            });

            for (int i = 0; i < CoreCount; i++)
            {
                Graphs[i].Width = (int)(GraphWidth / 100 * usage[i]);
                //Graphs[i].Width = (int)(GraphWidth / 10 * (int)(Math.Ceiling(usage[i]) / 10));
                Graphs[i].BackColor = (!parked[i]) ? active : park;
            }
        }

        private void LocationSet()
        {
            if (Height != (GraphHeight + 1) * CoreCount || Width != GraphWidth || Top != Screen.PrimaryScreen.WorkingArea.Height - Height + Screen.PrimaryScreen.WorkingArea.Top || Left != Screen.PrimaryScreen.WorkingArea.Width - Width + Screen.PrimaryScreen.WorkingArea.Left)
            {
                int oldTop = Top;
                int oldLeft = Left;

                Console.WriteLine($"{nameof(TopLevel)}:{TopLevel = true}");
                Console.WriteLine($"{nameof(TopMost)}:{TopMost = true}");

                //TopLevel = true;
                //TopMost = true;
                Height = (GraphHeight + 1) * CoreCount;
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
