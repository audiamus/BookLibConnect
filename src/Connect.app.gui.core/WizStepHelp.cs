using System;
using System.Windows.Forms;

namespace core.audiamus.connect.app.gui {
  public partial class WizStepHelp : UserControl {

    private Action BtnAction { get; }

    public WizStepHelp (Action action)  {
      InitializeComponent ();
      BtnAction = action;
    }

    private void button1_Click (object sender, EventArgs e) {
      BtnAction ();
    }
  }
}
