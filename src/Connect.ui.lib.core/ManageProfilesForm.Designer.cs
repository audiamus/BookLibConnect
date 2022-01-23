
namespace core.audiamus.connect.ui {
  partial class ManageProfilesForm {
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
      this.comBoxProfiles = new System.Windows.Forms.ComboBox();
      this.lblAccount = new System.Windows.Forms.Label();
      this.btnRemove = new System.Windows.Forms.Button();
      this.btnAdd = new System.Windows.Forms.Button();
      this.btnOK = new System.Windows.Forms.Button();
      this.lblDevice = new System.Windows.Forms.Label();
      this.ckBoxEncrypt = new System.Windows.Forms.CheckBox();
      this.label1 = new System.Windows.Forms.Label();
      this.panelOK = new System.Windows.Forms.Panel();
      this.label2 = new System.Windows.Forms.Label();
      this.panelOK.SuspendLayout();
      this.SuspendLayout();
      // 
      // comBoxProfiles
      // 
      this.comBoxProfiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comBoxProfiles.FormattingEnabled = true;
      this.comBoxProfiles.Location = new System.Drawing.Point(13, 32);
      this.comBoxProfiles.Name = "comBoxProfiles";
      this.comBoxProfiles.Size = new System.Drawing.Size(277, 21);
      this.comBoxProfiles.TabIndex = 0;
      this.comBoxProfiles.SelectedIndexChanged += new System.EventHandler(this.comBoxProfiles_SelectedIndexChanged);
      // 
      // lblAccount
      // 
      this.lblAccount.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.lblAccount.Location = new System.Drawing.Point(13, 61);
      this.lblAccount.Name = "lblAccount";
      this.lblAccount.Size = new System.Drawing.Size(277, 23);
      this.lblAccount.TabIndex = 2;
      this.lblAccount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // btnRemove
      // 
      this.btnRemove.Location = new System.Drawing.Point(205, 128);
      this.btnRemove.Name = "btnRemove";
      this.btnRemove.Size = new System.Drawing.Size(85, 23);
      this.btnRemove.TabIndex = 3;
      this.btnRemove.Text = "Remove";
      this.btnRemove.UseVisualStyleBackColor = true;
      this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
      // 
      // btnAdd
      // 
      this.btnAdd.Location = new System.Drawing.Point(13, 128);
      this.btnAdd.Name = "btnAdd";
      this.btnAdd.Size = new System.Drawing.Size(85, 23);
      this.btnAdd.TabIndex = 4;
      this.btnAdd.Text = "Add new …";
      this.btnAdd.UseVisualStyleBackColor = true;
      this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
      // 
      // btnOK
      // 
      this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnOK.Location = new System.Drawing.Point(13, 11);
      this.btnOK.Name = "btnOK";
      this.btnOK.Size = new System.Drawing.Size(85, 23);
      this.btnOK.TabIndex = 5;
      this.btnOK.Text = "OK";
      this.btnOK.UseVisualStyleBackColor = true;
      this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
      // 
      // lblDevice
      // 
      this.lblDevice.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.lblDevice.Location = new System.Drawing.Point(14, 93);
      this.lblDevice.Name = "lblDevice";
      this.lblDevice.Size = new System.Drawing.Size(277, 23);
      this.lblDevice.TabIndex = 2;
      this.lblDevice.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // ckBoxEncrypt
      // 
      this.ckBoxEncrypt.AutoSize = true;
      this.ckBoxEncrypt.Location = new System.Drawing.Point(14, 169);
      this.ckBoxEncrypt.Name = "ckBoxEncrypt";
      this.ckBoxEncrypt.Size = new System.Drawing.Size(129, 17);
      this.ckBoxEncrypt.TabIndex = 6;
      this.ckBoxEncrypt.Text = "Encrypt configuration";
      this.ckBoxEncrypt.UseVisualStyleBackColor = true;
      this.ckBoxEncrypt.CheckedChanged += new System.EventHandler(this.ckBoxEncrypt_CheckedChanged);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(14, 15);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(200, 13);
      this.label1.TabIndex = 7;
      this.label1.Text = "Profile account alias and region / domain";
      // 
      // panelOK
      // 
      this.panelOK.Controls.Add(this.btnOK);
      this.panelOK.Controls.Add(this.label2);
      this.panelOK.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panelOK.Location = new System.Drawing.Point(0, 192);
      this.panelOK.Name = "panelOK";
      this.panelOK.Size = new System.Drawing.Size(302, 46);
      this.panelOK.TabIndex = 8;
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(104, 16);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(101, 13);
      this.label2.TabIndex = 7;
      this.label2.Text = "Use selected profile";
      // 
      // ManageProfilesForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(302, 238);
      this.Controls.Add(this.panelOK);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.ckBoxEncrypt);
      this.Controls.Add(this.btnAdd);
      this.Controls.Add(this.btnRemove);
      this.Controls.Add(this.lblDevice);
      this.Controls.Add(this.lblAccount);
      this.Controls.Add(this.comBoxProfiles);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ManageProfilesForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Manage profiles";
      this.panelOK.ResumeLayout(false);
      this.panelOK.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ComboBox comBoxProfiles;
    private System.Windows.Forms.Label lblAccount;
    private System.Windows.Forms.Button btnRemove;
    private System.Windows.Forms.Button btnAdd;
    private System.Windows.Forms.Button btnOK;
    private System.Windows.Forms.Label lblDevice;
    private System.Windows.Forms.CheckBox ckBoxEncrypt;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Panel panelOK;
    private System.Windows.Forms.Label label2;
  }
}