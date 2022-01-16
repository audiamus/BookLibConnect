using System;
using System.Windows.Forms;
using core.audiamus.util;

namespace core.audiamus.connect.app.gui {
  abstract class ProgressProcessorBase {
    public Progress<ProgressMessage> Progress { get; }

    protected ProgressProcessorBase () {
      Progress = new Progress<ProgressMessage> ();
      Progress.ProgressChanged += report;
    }

    private void report (object sender, ProgressMessage msg) => report (msg);

    protected abstract void report (ProgressMessage msg);
  }

  class ProgressProcessor1 : ProgressProcessorBase {
    private readonly ProgressBar _progbarConvert;

    private int _maxItems;
    private int _maxConvert;
    private int _accuValueConvert;

    public ProgressProcessor1 (ProgressBar progbarConvert) {
      _progbarConvert = progbarConvert;
    }

    protected override void report (ProgressMessage msg) {
      if (msg.ItemCount.HasValue) {
        _maxItems = msg.ItemCount.Value;

        _maxConvert = msg.ItemCount.Value * 1000;
        _accuValueConvert = 0;
        _progbarConvert.Value = 0;
        _progbarConvert.Maximum = _maxConvert;
      }

      if (msg.IncStepsPerMille.HasValue) {
        _accuValueConvert += msg.IncStepsPerMille.Value;
        _accuValueConvert = Math.Min (_accuValueConvert, _maxConvert);
        _progbarConvert.Value = _accuValueConvert;
      }

    }
  }

  class ProgressProcessor2 : ProgressProcessorBase {
    private readonly ProgressBar _progbarDecrypt;
    private readonly ProgressBar _progbarDnld;

    private int _nItems;
    private int _maxDecrypt;
    private int _accuValueDecrypt;
    private int _maxDnld;
    private int _accuValueDnld;

    public ProgressProcessor2 (ProgressBar progbarItems, ProgressBar progbarDnld) {
      _progbarDecrypt = progbarItems;
      _progbarDnld = progbarDnld;
    }

    protected override void report (ProgressMessage msg) {
      if (msg.ItemCount.HasValue) {
        _nItems = msg.ItemCount.Value;
        _maxDecrypt = _nItems * 100;
        _accuValueDecrypt = 0;
        _progbarDecrypt.Value = 0;
        _progbarDecrypt.Maximum = _maxDecrypt;

        _maxDnld = 1000;
        _accuValueDnld = 0;
        _progbarDnld.Value = 0;
        _progbarDnld.Maximum = _maxDnld;
      }

      if (msg.IncItem.HasValue) {
        _maxDnld = 1000;
        _accuValueDnld = 0;
        _progbarDnld.Value = 0;
        _progbarDnld.Maximum = _maxDnld;
      }

      if (msg.IncStepsPerMille.HasValue) {
        _accuValueDnld += msg.IncStepsPerMille.Value;
        _accuValueDnld = Math.Min (_accuValueDnld, _maxDnld);
        _progbarDnld.Value = _accuValueDnld;
      }

      if (msg.IncStepsPerCent.HasValue) {
        _accuValueDecrypt += msg.IncStepsPerCent.Value;
        _accuValueDecrypt = Math.Min (_accuValueDecrypt, _maxDecrypt);
        _progbarDecrypt.Value = _accuValueDecrypt;
      }
    }
  }
}
