namespace MultiCoreLoad
{
	partial class Form1
	{
		/// <summary>
		/// 必要なデザイナー変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナーで生成されたコード

		/// <summary>
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディターで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.Worker = new System.Windows.Forms.Timer(this.components);
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.frequencyModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.activeClockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.effectiveClockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ResetMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Worker
            // 
            this.Worker.Interval = 250;
            this.Worker.Tick += new System.EventHandler(this.Worker_Tick);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Visible = true;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.frequencyModeToolStripMenuItem,
            this.toolStripSeparator1,
            this.ResetMenuItem,
            this.ExitToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(301, 168);
            // 
            // frequencyModeToolStripMenuItem
            // 
            this.frequencyModeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.activeClockToolStripMenuItem,
            this.effectiveClockToolStripMenuItem});
            this.frequencyModeToolStripMenuItem.Name = "frequencyModeToolStripMenuItem";
            this.frequencyModeToolStripMenuItem.Size = new System.Drawing.Size(300, 38);
            this.frequencyModeToolStripMenuItem.Text = "Frequency Mode";
            // 
            // activeClockToolStripMenuItem
            // 
            this.activeClockToolStripMenuItem.CheckOnClick = true;
            this.activeClockToolStripMenuItem.Name = "activeClockToolStripMenuItem";
            this.activeClockToolStripMenuItem.Size = new System.Drawing.Size(359, 44);
            this.activeClockToolStripMenuItem.Text = "Active Clock";
            this.activeClockToolStripMenuItem.CheckedChanged += new System.EventHandler(this.activeClockToolStripMenuItem_CheckedChanged);
            // 
            // effectiveClockToolStripMenuItem
            // 
            this.effectiveClockToolStripMenuItem.Checked = true;
            this.effectiveClockToolStripMenuItem.CheckOnClick = true;
            this.effectiveClockToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.effectiveClockToolStripMenuItem.Name = "effectiveClockToolStripMenuItem";
            this.effectiveClockToolStripMenuItem.Size = new System.Drawing.Size(359, 44);
            this.effectiveClockToolStripMenuItem.Text = "Effective Clock";
            this.effectiveClockToolStripMenuItem.CheckedChanged += new System.EventHandler(this.effectiveClockToolStripMenuItem_CheckedChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(297, 6);
            // 
            // ResetMenuItem
            // 
            this.ResetMenuItem.Name = "ResetMenuItem";
            this.ResetMenuItem.Size = new System.Drawing.Size(300, 38);
            this.ResetMenuItem.Text = "Reset";
            this.ResetMenuItem.Click += new System.EventHandler(this.ResetMenuItem_Click);
            // 
            // ExitToolStripMenuItem
            // 
            this.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem";
            this.ExitToolStripMenuItem.Size = new System.Drawing.Size(300, 38);
            this.ExitToolStripMenuItem.Text = "Exit";
            this.ExitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(232, 50);
            this.ControlBox = false;
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "MultiCoreLoad";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Timer Worker;
		private System.Windows.Forms.NotifyIcon notifyIcon1;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem ExitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem ResetMenuItem;
        private System.Windows.Forms.ToolStripMenuItem frequencyModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem activeClockToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem effectiveClockToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}

