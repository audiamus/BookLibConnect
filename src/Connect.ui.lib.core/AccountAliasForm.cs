using System;
using System.Linq;
using System.Windows.Forms;
using core.audiamus.aux.ex;
using core.audiamus.aux.win.ex;

namespace core.audiamus.connect.ui {
  public partial class AccountAliasForm : Form {
    private AccountAliasContext Context { get; }

    public AccountAliasForm (AccountAliasContext ctxt) {
      InitializeComponent ();

      if (ctxt is null)
        return;

      Context = ctxt;

      //this.SetStartPositionCentered ();

      lblCaption.Text += $"\"{ctxt.CustomerName}\"";
      comboBox1.Items.Add ($"Account {ctxt.LocalId}");
      comboBox1.Items.Add (ctxt.CustomerName);
      comboBox1.SelectedIndex = 0;
    }

    private void btnOk_Click (object sender, EventArgs e) {
      if (Context is not null)
        Context.Alias = comboBox1.Text.Trim ();
      Close ();
    }

    private void comboBox1_TextUpdate (object sender, EventArgs e) {
      string text = comboBox1.Text.Trim ();
      bool enable = text.Length >= 3;

      uint crc = text.Checksum32 ();
      if (Context?.Hashes?.Contains (crc) ?? false)
        enable = false;

      btnOk.Enabled = enable;
    }

  }
}
