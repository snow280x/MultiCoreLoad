using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
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
		Color normal = Color.FromArgb(0, 96, 255);
		Color boost = Color.FromArgb(255, 32, 32);
		Color freqFrame = Color.FromArgb(128, 128, 128);
		Color active = Color.FromArgb(64, 255, 0);
		Color park = Color.FromArgb(32, 128, 0);

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
				CoreCount = Environment.ProcessorCount;
				Cores = new Core[CoreCount];
				Graphs = new PictureBox[CoreCount + 1];

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

				ShowInTaskbar = false;
				LocationSet();

				DoWork();

				Worker.Enabled = true;
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
					freqBackground.Width = (avefreq <= 100) ? (int)Math.Round(GraphWidth / 100 * avefreq) : GraphWidth;
					Graphs[i].Width = (avefreq > 100) ? (int)Math.Round(GraphWidth / 100 * (avefreq - 100)) : 0;
				}
				else
				{
					Graphs[i].Width = (int)Math.Round(GraphWidth / 100 * usage[i - usageStartIndex]);
					Graphs[i].BackColor = (parked[i - usageStartIndex]) ? park : (usage[i - usageStartIndex] >= 100) ? boost : active;
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
	}
}
