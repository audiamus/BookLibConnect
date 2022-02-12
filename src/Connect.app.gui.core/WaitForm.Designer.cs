
namespace core.audiamus.connect.app.gui {
  partial class WaitForm {
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
      this.progressBar1 = new System.Windows.Forms.ProgressBar();
      this.label1 = new System.Windows.Forms.Label();
      this.LabelTask = new System.Windows.Forms.Label();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.tableLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // progressBar1
      // 
      this.progressBar1.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.progressBar1.Location = new System.Drawing.Point(19, 129);
      this.progressBar1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.progressBar1.Name = "progressBar1";
      this.progressBar1.Size = new System.Drawing.Size(482, 17);
      this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
      this.progressBar1.TabIndex = 0;
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
      this.label1.Location = new System.Drawing.Point(24, 19);
      this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(472, 27);
      this.label1.TabIndex = 1;
      this.label1.Text = "Starting up, please wait";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // LabelTask
      // 
      this.LabelTask.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.LabelTask.Location = new System.Drawing.Point(21, 75);
      this.LabelTask.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.LabelTask.Name = "LabelTask";
      this.LabelTask.Size = new System.Drawing.Size(478, 27);
      this.LabelTask.TabIndex = 2;
      this.LabelTask.Text = "Initializing database ...";
      this.LabelTask.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.progressBar1, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.LabelTask, 0, 1);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 3;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(520, 163);
      this.tableLayoutPanel1.TabIndex = 3;
      // 
      // WaitForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(520, 163);
      this.ControlBox = false;
      this.Controls.Add(this.tableLayoutPanel1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.Name = "WaitForm";
      this.Text = "Book Lib Connect";
      this.tableLayoutPanel1.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ProgressBar progressBar1;
    private System.Windows.Forms.Label label1;
    public System.Windows.Forms.Label LabelTask;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
  }
}