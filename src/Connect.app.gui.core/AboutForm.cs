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
      lblCopyright.Text += assemblyCopyright();

      static string assemblyCopyright () {
        int pos = AssemblyCopyright.IndexOf ('(');
        if (pos < 0)
          return AssemblyCopyright;
        else
          return AssemblyCopyright.Substring (0, pos).Trim();

      }
    }

    private void linkLabelHomepage_LinkClicked (object sender, LinkLabelLinkClickedEventArgs e) {
      var s = ((LinkLabel)sender).Text;
      ShellExecute.File (s);
    }
  }



}
