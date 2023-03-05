
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
      this.btnOk = new System.Windows.Forms.Button();
      this.comboBox1 = new System.Windows.Forms.ComboBox();
      this.lblCaption = new System.Windows.Forms.Label();
      this.btnCancel = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // btnOk
      // 
      this.btnOk.Location = new System.Drawing.Point(14, 96);
      this.btnOk.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btnOk.Name = "btnOk";
      this.btnOk.Size = new System.Drawing.Size(88, 27);
      this.btnOk.TabIndex = 2;
      this.btnOk.Text = "OK";
      this.btnOk.UseVisualStyleBackColor = true;
      this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
      // 
      // comboBox1
      // 
      this.comboBox1.FormattingEnabled = true;
      this.comboBox1.Location = new System.Drawing.Point(14, 40);
      this.comboBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.comboBox1.Name = "comboBox1";
      this.comboBox1.Size = new System.Drawing.Size(330, 23);
      this.comboBox1.TabIndex = 1;
      this.comboBox1.TextUpdate += new System.EventHandler(this.comboBox1_TextUpdate);
      // 
      // lblCaption
      // 
      this.lblCaption.AutoSize = true;
      this.lblCaption.Location = new System.Drawing.Point(14, 10);
      this.lblCaption.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lblCaption.Name = "lblCaption";
      this.lblCaption.Size = new System.Drawing.Size(120, 15);
      this.lblCaption.TabIndex = 3;
      this.lblCaption.Text = "Set display name for: ";
      // 
      // btnCancel
      // 
      this.btnCancel.Location = new System.Drawing.Point(256, 96);
      this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(88, 27);
      this.btnCancel.TabIndex = 3;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnCancel.Visible = false;
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // AccountAliasForm
      // 
      this.AcceptButton = this.btnOk;
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(377, 224);
      this.ControlBox = false;
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.btnOk);
      this.Controls.Add(this.comboBox1);
      this.Controls.Add(this.lblCaption);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.Name = "AccountAliasForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Account Display Name";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnOk;
    private System.Windows.Forms.ComboBox comboBox1;
    private System.Windows.Forms.Label lblCaption;
    private System.Windows.Forms.Button btnCancel;
  }
}