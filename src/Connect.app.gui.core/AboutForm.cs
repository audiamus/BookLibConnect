using System;
using System.Windows.Forms;
using core.audiamus.aux;
using static core.audiamus.aux.ApplEnv;

namespace core.audiamus.connect.app.gui {
  partial class AboutForm : Form {
    public AboutForm () {
      InitializeComponent ();
      Text += AssemblyTitle;

    }

    protected override void OnLoad (EventArgs e) {
      base.OnLoad (e);
      lblProduct.Text = AssemblyProduct;
      lblVersion.Text += AssemblyVersion;
      lblCopyright.Text += AssemblyCopyright;
    }

    private void linkLabelHomepage_LinkClicked (object sender, LinkLabelLinkClickedEventArgs e) {
      var s = ((LinkLabel)sender).Text;
      ShellExecute.File (s);
    }
  }



}
