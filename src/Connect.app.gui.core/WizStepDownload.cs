using System;
using System.Windows.Forms;
using core.audiamus.aux.win;

namespace core.audiamus.connect.app.gui {
  public partial class WizStepDownload : UserControl, ICompleted {
    public event EventHandler Completed;

    private Func<bool> BtnAction { get; }

    public WizStepDownload (Func<bool> action)  {
      InitializeComponent ();
      BtnAction = action;
    }

    private void button1_Click (object sender, EventArgs e) {
      bool succ = BtnAction ();
      if (succ)
        Completed?.Invoke (this, EventArgs.Empty);

    }
  }
}
