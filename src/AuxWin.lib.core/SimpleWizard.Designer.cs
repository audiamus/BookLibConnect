
namespace core.audiamus.aux.win {
  partial class SimpleWizard {
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
      this.pnlBottom = new System.Windows.Forms.Panel();
      this.btnNext = new System.Windows.Forms.Button();
      this.btnPrev = new System.Windows.Forms.Button();
      this.pnlMain = new System.Windows.Forms.Panel();
      this.pnlTop = new System.Windows.Forms.Panel();
      this.lblStep = new System.Windows.Forms.Label();
      this.pnlBottom.SuspendLayout();
      this.pnlTop.SuspendLayout();
      this.SuspendLayout();
      // 
      // pnlBottom
      // 
      this.pnlBottom.Controls.Add(this.btnNext);
      this.pnlBottom.Controls.Add(this.btnPrev);
      this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.pnlBottom.Location = new System.Drawing.Point(0, 224);
      this.pnlBottom.Name = "pnlBottom";
      this.pnlBottom.Size = new System.Drawing.Size(294, 51);
      this.pnlBottom.TabIndex = 0;
      // 
      // btnNext
      // 
      this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnNext.Location = new System.Drawing.Point(207, 16);
      this.btnNext.Name = "btnNext";
      this.btnNext.Size = new System.Drawing.Size(75, 23);
      this.btnNext.TabIndex = 0;
      this.btnNext.Text = "Next";
      this.btnNext.UseVisualStyleBackColor = true;
      this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
      // 
      // btnPrev
      // 
      this.btnPrev.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnPrev.Location = new System.Drawing.Point(126, 16);
      this.btnPrev.Name = "btnPrev";
      this.btnPrev.Size = new System.Drawing.Size(75, 23);
      this.btnPrev.TabIndex = 0;
      this.btnPrev.Text = "Previous";
      this.btnPrev.UseVisualStyleBackColor = true;
      this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
      // 
      // pnlMain
      // 
      this.pnlMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
      this.pnlMain.Location = new System.Drawing.Point(0, 41);
      this.pnlMain.Name = "pnlMain";
      this.pnlMain.Size = new System.Drawing.Size(294, 183);
      this.pnlMain.TabIndex = 1;
      // 
      // pnlTop
      // 
      this.pnlTop.Controls.Add(this.lblStep);
      this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
      this.pnlTop.Location = new System.Drawing.Point(0, 0);
      this.pnlTop.Name = "pnlTop";
      this.pnlTop.Size = new System.Drawing.Size(294, 41);
      this.pnlTop.TabIndex = 2;
      // 
      // lblStep
      // 
      this.lblStep.Dock = System.Windows.Forms.DockStyle.Fill;
      this.lblStep.Location = new System.Drawing.Point(0, 0);
      this.lblStep.Name = "lblStep";
      this.lblStep.Size = new System.Drawing.Size(294, 41);
      this.lblStep.TabIndex = 0;
      this.lblStep.Text = "Step 1 of n";
      this.lblStep.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // SimpleWizard
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(294, 275);
      this.Controls.Add(this.pnlMain);
      this.Controls.Add(this.pnlTop);
      this.Controls.Add(this.pnlBottom);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "SimpleWizard";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "SimpleWizard";
      this.pnlBottom.ResumeLayout(false);
      this.pnlTop.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel pnlBottom;
    private System.Windows.Forms.Button btnNext;
    private System.Windows.Forms.Button btnPrev;
    private System.Windows.Forms.Panel pnlMain;
    private System.Windows.Forms.Panel pnlTop;
    private System.Windows.Forms.Label lblStep;
  }
}