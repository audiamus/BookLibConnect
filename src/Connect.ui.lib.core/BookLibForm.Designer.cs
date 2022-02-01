
namespace core.audiamus.connect.ui {
  partial class BookLibForm {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose (bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose ();
      }
      base.Dispose (disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent () {
      this.splitContainerOuter = new System.Windows.Forms.SplitContainer();
      this.bookLibdgvControl1 = new core.audiamus.connect.ui.BookLibDGVControl();
      this.splitContainerBottom = new System.Windows.Forms.SplitContainer();
      this.splitContainerInner = new System.Windows.Forms.SplitContainer();
      this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainerOuter)).BeginInit();
      this.splitContainerOuter.Panel1.SuspendLayout();
      this.splitContainerOuter.Panel2.SuspendLayout();
      this.splitContainerOuter.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainerBottom)).BeginInit();
      this.splitContainerBottom.Panel1.SuspendLayout();
      this.splitContainerBottom.Panel2.SuspendLayout();
      this.splitContainerBottom.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainerInner)).BeginInit();
      this.splitContainerInner.Panel1.SuspendLayout();
      this.splitContainerInner.Panel2.SuspendLayout();
      this.splitContainerInner.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.SuspendLayout();
      // 
      // splitContainerOuter
      // 
      this.splitContainerOuter.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainerOuter.Location = new System.Drawing.Point(0, 0);
      this.splitContainerOuter.Name = "splitContainerOuter";
      this.splitContainerOuter.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainerOuter.Panel1
      // 
      this.splitContainerOuter.Panel1.Controls.Add(this.bookLibdgvControl1);
      // 
      // splitContainerOuter.Panel2
      // 
      this.splitContainerOuter.Panel2.Controls.Add(this.splitContainerBottom);
      this.splitContainerOuter.Size = new System.Drawing.Size(802, 453);
      this.splitContainerOuter.SplitterDistance = 260;
      this.splitContainerOuter.TabIndex = 0;
      // 
      // bookLibdgvControl1
      // 
      this.bookLibdgvControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.bookLibdgvControl1.DownloadSelectEnabled = true;
      this.bookLibdgvControl1.Location = new System.Drawing.Point(0, 0);
      this.bookLibdgvControl1.Name = "bookLibdgvControl1";
      this.bookLibdgvControl1.Size = new System.Drawing.Size(802, 260);
      this.bookLibdgvControl1.TabIndex = 0;
      this.bookLibdgvControl1.Close += new System.EventHandler(this.bookLibdgvControl1_Close);
      // 
      // splitContainerBottom
      // 
      this.splitContainerBottom.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainerBottom.Location = new System.Drawing.Point(0, 0);
      this.splitContainerBottom.Name = "splitContainerBottom";
      // 
      // splitContainerBottom.Panel1
      // 
      this.splitContainerBottom.Panel1.Controls.Add(this.splitContainerInner);
      // 
      // splitContainerBottom.Panel2
      // 
      this.splitContainerBottom.Panel2.Controls.Add(this.pictureBox1);
      this.splitContainerBottom.Size = new System.Drawing.Size(802, 189);
      this.splitContainerBottom.SplitterDistance = 634;
      this.splitContainerBottom.TabIndex = 0;
      // 
      // splitContainerInner
      // 
      this.splitContainerInner.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainerInner.Location = new System.Drawing.Point(0, 0);
      this.splitContainerInner.Name = "splitContainerInner";
      // 
      // splitContainerInner.Panel1
      // 
      this.splitContainerInner.Panel1.Controls.Add(this.propertyGrid1);
      // 
      // splitContainerInner.Panel2
      // 
      this.splitContainerInner.Panel2.Controls.Add(this.textBox1);
      this.splitContainerInner.Size = new System.Drawing.Size(634, 189);
      this.splitContainerInner.SplitterDistance = 403;
      this.splitContainerInner.TabIndex = 0;
      // 
      // propertyGrid1
      // 
      this.propertyGrid1.DisabledItemForeColor = System.Drawing.SystemColors.ControlText;
      this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.propertyGrid1.HelpVisible = false;
      this.propertyGrid1.LineColor = System.Drawing.SystemColors.GrayText;
      this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
      this.propertyGrid1.Name = "propertyGrid1";
      this.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.NoSort;
      this.propertyGrid1.Size = new System.Drawing.Size(403, 189);
      this.propertyGrid1.TabIndex = 0;
      this.propertyGrid1.ToolbarVisible = false;
      this.propertyGrid1.ViewBackColor = System.Drawing.SystemColors.Control;
      this.propertyGrid1.SelectedObjectsChanged += new System.EventHandler(this.propertyGrid1_SelectedObjectsChanged);
      // 
      // textBox1
      // 
      this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.textBox1.Location = new System.Drawing.Point(0, 0);
      this.textBox1.Multiline = true;
      this.textBox1.Name = "textBox1";
      this.textBox1.ReadOnly = true;
      this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.textBox1.Size = new System.Drawing.Size(227, 189);
      this.textBox1.TabIndex = 0;
      // 
      // pictureBox1
      // 
      this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.pictureBox1.Location = new System.Drawing.Point(0, 0);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(164, 189);
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this.pictureBox1.TabIndex = 0;
      this.pictureBox1.TabStop = false;
      // 
      // BookLibForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(802, 453);
      this.Controls.Add(this.splitContainerOuter);
      this.MinimumSize = new System.Drawing.Size(570, 270);
      this.Name = "BookLibForm";
      this.ShowIcon = false;
      this.Text = "BookLibForm";
      this.splitContainerOuter.Panel1.ResumeLayout(false);
      this.splitContainerOuter.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainerOuter)).EndInit();
      this.splitContainerOuter.ResumeLayout(false);
      this.splitContainerBottom.Panel1.ResumeLayout(false);
      this.splitContainerBottom.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainerBottom)).EndInit();
      this.splitContainerBottom.ResumeLayout(false);
      this.splitContainerInner.Panel1.ResumeLayout(false);
      this.splitContainerInner.Panel2.ResumeLayout(false);
      this.splitContainerInner.Panel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainerInner)).EndInit();
      this.splitContainerInner.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.PropertyGrid propertyGrid1;
    private System.Windows.Forms.TextBox textBox1;
    internal System.Windows.Forms.SplitContainer splitContainerOuter;
    internal System.Windows.Forms.SplitContainer splitContainerBottom;
    internal System.Windows.Forms.SplitContainer splitContainerInner;
    internal BookLibDGVControl bookLibdgvControl1;
  }
}