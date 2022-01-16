using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace core.audiamus.aux.win.ex {
  public static class ControlExtensions {
    public static Form GetParentForm (this Control control) {
      Control parent = control;
      while (!(parent is null || parent is Form))
        parent = parent.Parent;

      return parent as Form;
    }

    private static readonly Dictionary<Form, ToolTip> __tooltips = new Dictionary<Form, ToolTip> ();

    private const char ELLIPSIS = '…';

    public static void SetTextAsPathWithEllipsis (this Control label, string text = null) {
      if (string.IsNullOrWhiteSpace (text))
        text = label.Tag as string;
      else
        label.Tag = text;

      label.Text = text;

      Size size = new (label.Width - 8, label.Height);
      Size reqSize = TextRenderer.MeasureText (text, label.Font);
      if (reqSize.Width > size.Width) {

        var form = label.GetParentForm ();
        bool succ = __tooltips.TryGetValue (form, out var tooltip);
        if (!succ) {
          tooltip = new ToolTip ();
          __tooltips[form] = tooltip;
          label.Resize += label_Resize; // once only
        }

        tooltip.SetToolTip (label, text);

        int pos = text.LastIndexOfAny (new char[] { '/', '\\' });
        string newText;
        if (pos >= 0) {
          string left = text.Substring (0, pos);
          string right = text.Substring (pos);

          newText = shrink (left, right);
          if (newText is null) {
            pos = right.Length / 2;
            left = $"{ELLIPSIS}{right.Substring (0, pos)}";
            right = right.Substring (pos);

            newText = shrink (left, right, true);
          }
        } else {
          pos = text.Length / 2;
          string left = text.Substring (0, pos);
          string right = text.Substring (pos);

          newText = shrink (left, right, true);
        }

        if (label is Label lbl) {
          lbl.AutoEllipsis = newText is null;
        }

        if (!(newText is null))
          label.Text = newText;

        // local function
        string shrink (string left, string right, bool both = false) {
          bool fit = false;
          string newText = null;
          int i = 0;
          while (left.Length >= 4 && right.Length >= 4 && !fit) {
            if (!both || i % 2 == 0)
              left = left.Substring (0, left.Length - 1);
            else
              right = right.Substring (1);
            i++;
            newText = $"{left}{ELLIPSIS}{right}";
            Size siz = TextRenderer.MeasureText (newText, label.Font);
            fit = siz.Width <= size.Width;
          }
          if (fit)
            return newText;
          else
            return null;
        }
      }
    }

    private static void label_Resize (object sender, System.EventArgs e) => 
      (sender as Control)?.SetTextAsPathWithEllipsis ();
  }
}
