using System;
using System.Media;
using System.Windows.Forms;
using core.audiamus.aux.ex;

namespace core.audiamus.aux.win {
  public class InteractionCallbackHandler : IInteractionCallback<InteractionMessage, bool?> {
    private readonly Control _parent;
    private Control Parent => _parent;
    
    public InteractionCallbackHandler (Control parent) {
      _parent = parent;
    }

    public virtual bool? Interact (InteractionMessage im) {
      switch (im.Type) {
        default:
          MsgBox.Show (Parent, im.Message, Parent.Text, MessageBoxButtons.OK, MessageBoxIcon.None);
          break;
        case ECallbackType.info:
          MsgBox.Show (Parent, im.Message, Parent.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
          break;
        case ECallbackType.infoCancel:
          return MsgBox.Show (Parent, im.Message, Parent.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK;
        case ECallbackType.warning:
          MsgBox.Show (Parent, im.Message, Parent.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
          break;
        case ECallbackType.error:
          MsgBox.Show (Parent, im.Message, Parent.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
          break;
        case ECallbackType.errorQuestion:
          return MsgBox.Show (Parent, im.Message, Parent.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes;
        case ECallbackType.errorQuestion3:
          return threewayQuestion (im, true);
        case ECallbackType.question:
          return MsgBox.Show (Parent, im.Message, Parent.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        case ECallbackType.question3: {
            SystemSounds.Exclamation.Play ();
            return threewayQuestion (im);
          }
      }

      return null;
    }

    private bool? threewayQuestion (InteractionMessage im, bool errorIcon = false) {
      var result = MsgBox.Show (
        Parent, im.Message, Parent.Text, 
        MessageBoxButtons.YesNoCancel, errorIcon ? MessageBoxIcon.Error : MessageBoxIcon.Question);
      switch (result) {
        default:
          return null;
        case DialogResult.Yes:
          return true;
        case DialogResult.No:
          return false;
      }
    }
  }

  //public class InteractionCallbackHandler<T> : InteractionCallbackHandler where T : struct, Enum {
  public class InteractionCallbackHandler<T> : InteractionCallbackHandler, IInteractionCallback<InteractionMessage<T>, bool?> 
    where T : class 
  {

    private Func<InteractionMessage<T>, bool?> CustomHandler { get; }
    private Func<T, string> CustomMessage { get; }
    

    public InteractionCallbackHandler (Control parent, Func<InteractionMessage<T>, bool?> customHandler) : base (parent) {
      CustomHandler = customHandler;
    }

    public InteractionCallbackHandler (Control parent, Func<T, string> customMessage) : base (parent) {
      CustomMessage = customMessage;
    }

    public bool? Interact (InteractionMessage<T> im) {
      return Interact((InteractionMessage)im);
    }

    public override bool? Interact (InteractionMessage im) {
      switch (im) {
        case InteractionMessage<T> imt:
          if (imt.Custom is null)
            return base.Interact (imt);
          else {
            if (CustomHandler is not null)
              return CustomHandler (imt);
            else if (CustomMessage is not null) {
              string msg = CustomMessage (imt.Custom);
              if (msg.IsNullOrWhiteSpace ())
                return null;
              var im2 = new InteractionMessage (im.Type, msg);
              return base.Interact (im2);
            } else
              return null;
          }
        default:
          return base.Interact (im);
      }
    }
  }

}

