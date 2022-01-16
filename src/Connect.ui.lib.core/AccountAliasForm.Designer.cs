
namespace core.audiamus.connect.ui {
  partial class AccountAliasForm {
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
      this.lblCaption = new System.Windows.Forms.Label();
      this.comboBox1 = new System.Windows.Forms.ComboBox();
      this.btnOk = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // lblCaption
      // 
      this.lblCaption.AutoSize = true;
      this.lblCaption.Location = new System.Drawing.Point(23, 40);
      this.lblCaption.Name = "lblCaption";
      this.lblCaption.Size = new System.Drawing.Size(112, 13);
      this.lblCaption.TabIndex = 0;
      this.lblCaption.Text = "Set display name for: ";
      // 
      // comboBox1
      // 
      this.comboBox1.FormattingEnabled = true;
      this.comboBox1.Location = new System.Drawing.Point(23, 66);
      this.comboBox1.Name = "comboBox1";
      this.comboBox1.Size = new System.Drawing.Size(283, 21);
      this.comboBox1.TabIndex = 1;
      this.comboBox1.TextUpdate += new System.EventHandler(this.comboBox1_TextUpdate);
      // 
      // btnOk
      // 
      this.btnOk.Location = new System.Drawing.Point(124, 122);
      this.btnOk.Name = "btnOk";
      this.btnOk.Size = new System.Drawing.Size(75, 23);
      this.btnOk.TabIndex = 2;
      this.btnOk.Text = "OK";
      this.btnOk.UseVisualStyleBackColor = true;
      this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
      // 
      // AccountAliasForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(323, 194);
      this.ControlBox = false;
      this.Controls.Add(this.btnOk);
      this.Controls.Add(this.comboBox1);
      this.Controls.Add(this.lblCaption);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Name = "AccountAliasForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Account Display Name";
      this.TopMost = true;
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label lblCaption;
    private System.Windows.Forms.ComboBox comboBox1;
    private System.Windows.Forms.Button btnOk;
  }
}