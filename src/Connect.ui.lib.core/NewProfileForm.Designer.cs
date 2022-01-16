
namespace core.audiamus.connect.ui {
  partial class NewProfileForm {
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
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewProfileForm));
      this.lblRegion1 = new System.Windows.Forms.Label();
      this.comBoxRegion = new System.Windows.Forms.ComboBox();
      this.btnCreateUrl = new System.Windows.Forms.Button();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.btnOpenBrowser = new System.Windows.Forms.Button();
      this.lblPaste = new System.Windows.Forms.Label();
      this.textBox2 = new System.Windows.Forms.TextBox();
      this.panelResult = new System.Windows.Forms.Panel();
      this.lblAs = new System.Windows.Forms.Label();
      this.lblDevice = new System.Windows.Forms.Label();
      this.lblRegion = new System.Windows.Forms.Label();
      this.lblRegion2 = new System.Windows.Forms.Label();
      this.lblAccount = new System.Windows.Forms.Label();
      this.lblProfile = new System.Windows.Forms.Label();
      this.btnOK = new System.Windows.Forms.Button();
      this.lblyCopy = new System.Windows.Forms.Label();
      this.btnCopy = new System.Windows.Forms.Button();
      this.ckBoxPreAmazonUsername = new System.Windows.Forms.CheckBox();
      this.panelRegion = new System.Windows.Forms.Panel();
      this.panelCreateUrl = new System.Windows.Forms.Panel();
      this.panelCreateUrl2 = new System.Windows.Forms.Panel();
      this.panelCreateUrl1 = new System.Windows.Forms.Panel();
      this.panelPasteUrl = new System.Windows.Forms.Panel();
      this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
      this.panelResult.SuspendLayout();
      this.panelRegion.SuspendLayout();
      this.panelCreateUrl.SuspendLayout();
      this.panelCreateUrl2.SuspendLayout();
      this.panelCreateUrl1.SuspendLayout();
      this.panelPasteUrl.SuspendLayout();
      this.SuspendLayout();
      // 
      // lblRegion1
      // 
      this.lblRegion1.AutoSize = true;
      this.lblRegion1.Location = new System.Drawing.Point(37, 16);
      this.lblRegion1.Name = "lblRegion1";
      this.lblRegion1.Size = new System.Drawing.Size(84, 13);
      this.lblRegion1.TabIndex = 0;
      this.lblRegion1.Text = "Region / domain";
      // 
      // comBoxRegion
      // 
      this.comBoxRegion.FormattingEnabled = true;
      this.comBoxRegion.Location = new System.Drawing.Point(128, 13);
      this.comBoxRegion.Name = "comBoxRegion";
      this.comBoxRegion.Size = new System.Drawing.Size(129, 21);
      this.comBoxRegion.TabIndex = 1;
      this.comBoxRegion.SelectedIndexChanged += new System.EventHandler(this.comBoxRegion_SelectedIndexChanged);
      // 
      // btnCreateUrl
      // 
      this.btnCreateUrl.Location = new System.Drawing.Point(36, 10);
      this.btnCreateUrl.Name = "btnCreateUrl";
      this.btnCreateUrl.Size = new System.Drawing.Size(150, 23);
      this.btnCreateUrl.TabIndex = 2;
      this.btnCreateUrl.Text = "Create login URL";
      this.btnCreateUrl.UseVisualStyleBackColor = true;
      this.btnCreateUrl.Click += new System.EventHandler(this.btnCreateUrl_Click);
      // 
      // textBox1
      // 
      this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textBox1.Location = new System.Drawing.Point(36, 6);
      this.textBox1.Multiline = true;
      this.textBox1.Name = "textBox1";
      this.textBox1.ReadOnly = true;
      this.textBox1.Size = new System.Drawing.Size(723, 65);
      this.textBox1.TabIndex = 3;
      this.textBox1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.textBox1_MouseClick);
      // 
      // btnOpenBrowser
      // 
      this.btnOpenBrowser.Location = new System.Drawing.Point(36, 82);
      this.btnOpenBrowser.Name = "btnOpenBrowser";
      this.btnOpenBrowser.Size = new System.Drawing.Size(150, 41);
      this.btnOpenBrowser.TabIndex = 4;
      this.btnOpenBrowser.Text = "Open URL in default\r\nweb browser";
      this.btnOpenBrowser.UseVisualStyleBackColor = true;
      this.btnOpenBrowser.Click += new System.EventHandler(this.btnOpenBrowser_Click);
      // 
      // lblPaste
      // 
      this.lblPaste.AutoSize = true;
      this.lblPaste.Location = new System.Drawing.Point(39, 13);
      this.lblPaste.Name = "lblPaste";
      this.lblPaste.Size = new System.Drawing.Size(321, 13);
      this.lblPaste.TabIndex = 5;
      this.lblPaste.Text = "Copy/paste final URL \"Cannot find page\" into the text box below.";
      // 
      // textBox2
      // 
      this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textBox2.Location = new System.Drawing.Point(39, 39);
      this.textBox2.Multiline = true;
      this.textBox2.Name = "textBox2";
      this.textBox2.Size = new System.Drawing.Size(720, 88);
      this.textBox2.TabIndex = 6;
      this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
      // 
      // panelResult
      // 
      this.panelResult.Controls.Add(this.lblAs);
      this.panelResult.Controls.Add(this.lblDevice);
      this.panelResult.Controls.Add(this.lblRegion);
      this.panelResult.Controls.Add(this.lblRegion2);
      this.panelResult.Controls.Add(this.lblAccount);
      this.panelResult.Controls.Add(this.lblProfile);
      this.panelResult.Dock = System.Windows.Forms.DockStyle.Top;
      this.panelResult.Location = new System.Drawing.Point(0, 362);
      this.panelResult.Name = "panelResult";
      this.panelResult.Size = new System.Drawing.Size(800, 76);
      this.panelResult.TabIndex = 8;
      this.panelResult.Visible = false;
      // 
      // lblAs
      // 
      this.lblAs.AutoSize = true;
      this.lblAs.Location = new System.Drawing.Point(459, 31);
      this.lblAs.Name = "lblAs";
      this.lblAs.Size = new System.Drawing.Size(18, 13);
      this.lblAs.TabIndex = 4;
      this.lblAs.Text = "as";
      // 
      // lblDevice
      // 
      this.lblDevice.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.lblDevice.Location = new System.Drawing.Point(483, 26);
      this.lblDevice.Name = "lblDevice";
      this.lblDevice.Size = new System.Drawing.Size(276, 23);
      this.lblDevice.TabIndex = 3;
      this.lblDevice.Text = "device";
      this.lblDevice.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // lblRegion
      // 
      this.lblRegion.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.lblRegion.Location = new System.Drawing.Point(368, 26);
      this.lblRegion.Name = "lblRegion";
      this.lblRegion.Size = new System.Drawing.Size(82, 23);
      this.lblRegion.TabIndex = 3;
      this.lblRegion.Text = "region";
      this.lblRegion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // lblRegion2
      // 
      this.lblRegion2.AutoSize = true;
      this.lblRegion2.Location = new System.Drawing.Point(302, 31);
      this.lblRegion2.Name = "lblRegion2";
      this.lblRegion2.Size = new System.Drawing.Size(58, 13);
      this.lblRegion2.TabIndex = 2;
      this.lblRegion2.Text = "and region";
      // 
      // lblAccount
      // 
      this.lblAccount.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.lblAccount.Location = new System.Drawing.Point(180, 26);
      this.lblAccount.Name = "lblAccount";
      this.lblAccount.Size = new System.Drawing.Size(113, 23);
      this.lblAccount.TabIndex = 1;
      this.lblAccount.Text = "account";
      this.lblAccount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // lblProfile
      // 
      this.lblProfile.AutoSize = true;
      this.lblProfile.Location = new System.Drawing.Point(39, 31);
      this.lblProfile.Name = "lblProfile";
      this.lblProfile.Size = new System.Drawing.Size(135, 13);
      this.lblProfile.TabIndex = 0;
      this.lblProfile.Text = "Profile created for account";
      // 
      // btnOK
      // 
      this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnOK.Enabled = false;
      this.btnOK.Location = new System.Drawing.Point(37, 462);
      this.btnOK.Name = "btnOK";
      this.btnOK.Size = new System.Drawing.Size(75, 23);
      this.btnOK.TabIndex = 10;
      this.btnOK.Text = "OK";
      this.btnOK.UseVisualStyleBackColor = true;
      this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
      // 
      // lblyCopy
      // 
      this.lblyCopy.AutoSize = true;
      this.lblyCopy.Location = new System.Drawing.Point(205, 96);
      this.lblyCopy.Name = "lblyCopy";
      this.lblyCopy.Size = new System.Drawing.Size(94, 13);
      this.lblyCopy.TabIndex = 11;
      this.lblyCopy.Text = "or copy and paste";
      // 
      // btnCopy
      // 
      this.btnCopy.Location = new System.Drawing.Point(315, 82);
      this.btnCopy.Name = "btnCopy";
      this.btnCopy.Size = new System.Drawing.Size(151, 41);
      this.btnCopy.TabIndex = 4;
      this.btnCopy.Text = "Copy URL to clipboard";
      this.btnCopy.UseVisualStyleBackColor = true;
      this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
      // 
      // ckBoxPreAmazonUsername
      // 
      this.ckBoxPreAmazonUsername.AutoSize = true;
      this.ckBoxPreAmazonUsername.Enabled = false;
      this.ckBoxPreAmazonUsername.Location = new System.Drawing.Point(279, 15);
      this.ckBoxPreAmazonUsername.Name = "ckBoxPreAmazonUsername";
      this.ckBoxPreAmazonUsername.Size = new System.Drawing.Size(160, 17);
      this.ckBoxPreAmazonUsername.TabIndex = 12;
      this.ckBoxPreAmazonUsername.Text = "with pre-Amazon user name";
      this.ckBoxPreAmazonUsername.UseVisualStyleBackColor = true;
      this.ckBoxPreAmazonUsername.CheckedChanged += new System.EventHandler(this.ckBoxPreAmazonUsername_CheckedChanged);
      // 
      // panelRegion
      // 
      this.panelRegion.Controls.Add(this.ckBoxPreAmazonUsername);
      this.panelRegion.Controls.Add(this.lblRegion1);
      this.panelRegion.Controls.Add(this.comBoxRegion);
      this.panelRegion.Dock = System.Windows.Forms.DockStyle.Top;
      this.panelRegion.Location = new System.Drawing.Point(0, 0);
      this.panelRegion.Name = "panelRegion";
      this.panelRegion.Size = new System.Drawing.Size(800, 43);
      this.panelRegion.TabIndex = 13;
      // 
      // panelCreateUrl
      // 
      this.panelCreateUrl.Controls.Add(this.panelCreateUrl2);
      this.panelCreateUrl.Controls.Add(this.panelCreateUrl1);
      this.panelCreateUrl.Dock = System.Windows.Forms.DockStyle.Top;
      this.panelCreateUrl.Enabled = false;
      this.panelCreateUrl.Location = new System.Drawing.Point(0, 43);
      this.panelCreateUrl.Name = "panelCreateUrl";
      this.panelCreateUrl.Size = new System.Drawing.Size(800, 174);
      this.panelCreateUrl.TabIndex = 14;
      // 
      // panelCreateUrl2
      // 
      this.panelCreateUrl2.Controls.Add(this.textBox1);
      this.panelCreateUrl2.Controls.Add(this.btnOpenBrowser);
      this.panelCreateUrl2.Controls.Add(this.btnCopy);
      this.panelCreateUrl2.Controls.Add(this.lblyCopy);
      this.panelCreateUrl2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.panelCreateUrl2.Enabled = false;
      this.panelCreateUrl2.Location = new System.Drawing.Point(0, 39);
      this.panelCreateUrl2.Name = "panelCreateUrl2";
      this.panelCreateUrl2.Size = new System.Drawing.Size(800, 135);
      this.panelCreateUrl2.TabIndex = 13;
      this.toolTip1.SetToolTip(this.panelCreateUrl2, resources.GetString("panelCreateUrl2.ToolTip"));
      // 
      // panelCreateUrl1
      // 
      this.panelCreateUrl1.Controls.Add(this.btnCreateUrl);
      this.panelCreateUrl1.Dock = System.Windows.Forms.DockStyle.Top;
      this.panelCreateUrl1.Enabled = false;
      this.panelCreateUrl1.Location = new System.Drawing.Point(0, 0);
      this.panelCreateUrl1.Name = "panelCreateUrl1";
      this.panelCreateUrl1.Size = new System.Drawing.Size(800, 39);
      this.panelCreateUrl1.TabIndex = 12;
      // 
      // panelPasteUrl
      // 
      this.panelPasteUrl.Controls.Add(this.lblPaste);
      this.panelPasteUrl.Controls.Add(this.textBox2);
      this.panelPasteUrl.Dock = System.Windows.Forms.DockStyle.Top;
      this.panelPasteUrl.Enabled = false;
      this.panelPasteUrl.Location = new System.Drawing.Point(0, 217);
      this.panelPasteUrl.Name = "panelPasteUrl";
      this.panelPasteUrl.Size = new System.Drawing.Size(800, 145);
      this.panelPasteUrl.TabIndex = 15;
      // 
      // toolTip1
      // 
      this.toolTip1.AutoPopDelay = 20000;
      this.toolTip1.InitialDelay = 500;
      this.toolTip1.ReshowDelay = 100;
      this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
      // 
      // NewProfileForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(800, 497);
      this.Controls.Add(this.panelResult);
      this.Controls.Add(this.panelPasteUrl);
      this.Controls.Add(this.panelCreateUrl);
      this.Controls.Add(this.panelRegion);
      this.Controls.Add(this.btnOK);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "NewProfileForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "New profile";
      this.panelResult.ResumeLayout(false);
      this.panelResult.PerformLayout();
      this.panelRegion.ResumeLayout(false);
      this.panelRegion.PerformLayout();
      this.panelCreateUrl.ResumeLayout(false);
      this.panelCreateUrl2.ResumeLayout(false);
      this.panelCreateUrl2.PerformLayout();
      this.panelCreateUrl1.ResumeLayout(false);
      this.panelPasteUrl.ResumeLayout(false);
      this.panelPasteUrl.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label lblRegion1;
    private System.Windows.Forms.ComboBox comBoxRegion;
    private System.Windows.Forms.Button btnCreateUrl;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.Button btnOpenBrowser;
    private System.Windows.Forms.Label lblPaste;
    private System.Windows.Forms.TextBox textBox2;
    private System.Windows.Forms.Panel panelResult;
    private System.Windows.Forms.Label lblRegion;
    private System.Windows.Forms.Label lblRegion2;
    private System.Windows.Forms.Label lblAccount;
    private System.Windows.Forms.Label lblProfile;
    private System.Windows.Forms.Button btnOK;
    private System.Windows.Forms.Label lblyCopy;
    private System.Windows.Forms.Button btnCopy;
    private System.Windows.Forms.Label lblAs;
    private System.Windows.Forms.Label lblDevice;
    private System.Windows.Forms.CheckBox ckBoxPreAmazonUsername;
    private System.Windows.Forms.Panel panelRegion;
    private System.Windows.Forms.Panel panelCreateUrl;
    private System.Windows.Forms.Panel panelCreateUrl2;
    private System.Windows.Forms.Panel panelCreateUrl1;
    private System.Windows.Forms.Panel panelPasteUrl;
    private System.Windows.Forms.ToolTip toolTip1;
  }
}