using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using R = core.audiamus.aux.win.Properties.Resources;

namespace core.audiamus.aux.win {
  public interface ICompleted {
    event EventHandler Completed;
  }

  public partial class SimpleWizard : Form {
    record Page (Control Control, bool HasCompletion);

    private readonly List<Page> _wizardPages = new List<Page> ();
    private readonly string _textNext;
    private int _index = 0;

    public IEnumerable<Control> Pages => _wizardPages.Select (p => p.Control).ToList (); 

    public SimpleWizard () {
      InitializeComponent ();
      _textNext = btnNext.Text;
    }

    public void AddPage (Control control) {
      bool hasCompletion = false;
      if (control is ICompleted compl) {
        hasCompletion = true;
        compl.Completed += compl_Completed;
      }
      _wizardPages.Add (new(control,hasCompletion));
    }

    private void compl_Completed (object sender, EventArgs e) {
      btnNext.Enabled = true;
    }

    protected override void OnLoad (EventArgs e) {
      base.OnLoad (e);
      setPage ();
    }

    private void btnPrev_Click (object sender, EventArgs e) {
      _index--;
      setPage ();
    }

    private void btnNext_Click (object sender, EventArgs e) {
      _index++;
      setPage ();
      if (_index >= _wizardPages.Count) {
        DialogResult = DialogResult.OK;
        Close ();
      }
    }

    private void setPage () {
      btnPrev.Enabled = _index > 0;
      btnNext.Text = _index < _wizardPages.Count - 1 ? _textNext : "OK";

      int step = _wizardPages.Count == 0 ? 0 : _index + 1;
      lblStep.Text = $"{R.Step} {step} {R.Of} {_wizardPages.Count}";


      if (pnlMain.Controls.Count > 0) {
        var ctrl = pnlMain.Controls[0];
        ctrl.Dock = DockStyle.None;
        pnlMain.Controls.Clear ();
      }
      if (_index < _wizardPages.Count) {
        var p = _wizardPages[_index];
        var ctrl = p.Control;
        pnlMain.Controls.Add (ctrl);
        if (ctrl.Tag is string s)
          lblStep.Text += $": {s}";
        pnlMain.Controls[0].Dock = DockStyle.Fill;
        if (p.HasCompletion)
          btnNext.Enabled = false;
      }
    }
  }
}
