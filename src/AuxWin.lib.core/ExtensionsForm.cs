using System;
using System.Drawing;
using System.Windows.Forms;

namespace core.audiamus.aux.win.ex {
  /// <summary>
  /// Initial position of new form in relation to parent. 
  /// Defines edges and corners.
  /// One edge and one corner on the same side can be combined.
  /// If edge is specified, then the new form is positioned outside the parent, otherwise inside. 
  /// </summary>
  [Flags]
  public enum EFormStartPosition {
    None = 0,
    AtParentLeft = 1,
    AtParentRight = 2,
    AtParentTop = 4,
    AtParentBottom = 8,
    AtParentTopLeft = 16,
    AtParentTopRight = 32,
    AtParentBottomLeft = 64,
    AtParentBottomRight = 128,
  };

  /// <summary>
  /// Extension methods to position new form in relation to parent and screen.
  /// </summary>
  public static class FormExtensions {

    //------------------------------------------
    #region SetStartPosition
    #region private helper constants

    static readonly EFormStartPosition Corners = EFormStartPosition.AtParentTopLeft | EFormStartPosition.AtParentTopRight |
        EFormStartPosition.AtParentBottomLeft | EFormStartPosition.AtParentBottomRight;
    static readonly EFormStartPosition Edges = EFormStartPosition.AtParentTop | EFormStartPosition.AtParentRight |
        EFormStartPosition.AtParentLeft | EFormStartPosition.AtParentBottom;

    #endregion
    #region private nested class

    /// <summary>
    /// Helper class which always features screen coordinates
    /// </summary>
    class ControlRectangleOnScreen {
      public int Left {
        get;
        private set;
      }
      public int Right {
        get;
        private set;
      }
      public int Top {
        get;
        private set;
      }
      public int Bottom {
        get;
        private set;
      }
      public int Width {
        get {
          return Right - Left;
        }
      }
      public int Height {
        get {
          return Bottom - Top;
        }
      }
      public Point Location {
        get;
        private set;
      }

      public ControlRectangleOnScreen (Control control) {
        if (control == null)
          return;

        if (control is Form) {
          Left = control.Left;
          Right = control.Right;
          Top = control.Top;
          Bottom = control.Bottom;
        } else {
          Point topLeft = control.PointToScreen (new Point (0, 0));
          Point btmRght = control.PointToScreen (new Point (control.Width, control.Height));
          Left = topLeft.X;
          Right = btmRght.X;
          Top = topLeft.Y;
          Bottom = btmRght.Y;
        }
        Location = new Point (Left, Top);
      }
    }

    #endregion
    #region public static extension method

    public static void SetStartPositionCentered (this Form form) {
      form.SetStartPosition (EFormStartPosition.None, null, 0);
    }

    /// <summary>
    /// Sets the start position for the form (this), in relation to the parent (other), specified by ePosition flag(s).  
    /// The form will always be positioned inside the current screen bounds.
    /// Will set the form owner, and overwrite.
    /// </summary>
    /// <param name="form">The form the start position will be applied to.</param>
    /// <param name="ePosition">The edge and corner specification as enum flag(s).</param>
    /// <param name="other">The optional parent control.</param>
    /// <param name="offset">The optional offset in pixels to provide some overlap.</param>
    public static void SetStartPosition (this Form form, EFormStartPosition ePosition, Control other = null, int offset = 30) {
      if (form == null)
        return;
      if (other is Form)
        form.Owner = other as Form;
      if (form.Owner == null)
        form.Owner = Form.ActiveForm;

      form.SetStartPositionWithoutFormOwnership (ePosition, other, offset); 
    }

    /// <summary>
    /// Sets the start position for the form (this), in relation to the parent (other), specified by ePosition flag(s).
    /// The form will always be positioned inside the current screen bounds.
    /// Will not set the form owner, if not already set.
    /// </summary>
    /// <param name="form">The form the start position will be applied to.</param>
    /// <param name="ePosition">The edge and corner specification as enum flag(s).</param>
    /// <param name="other">The temporary parent for this position.</param>
    /// <param name="offset">The optional offset in pixels to provide some overlap.</param>
    public static void SetStartPositionWithoutFormOwnership (this Form form, EFormStartPosition ePosition, Control other, int offset = 30) {
      if (form == null)
        return;
      
      ControlRectangleOnScreen parent = new ControlRectangleOnScreen (other ?? form.Owner);

      if (parent == null)
        return;

      form.StartPosition = FormStartPosition.Manual;

      form.Location = position (form, parent, ePosition, offset);

      form.AdjustLocationToScreenBounds ();
    }

    #endregion
    #region private methods

    //------------------------------------------
    private static Point position (Form form, ControlRectangleOnScreen parent, EFormStartPosition ePosition, int offset) {

      EFormStartPosition edges = Edges & ePosition;
      EFormStartPosition corners = Corners & ePosition;

      if (edges != EFormStartPosition.None) {
        return positionEdges (form, parent, offset, edges, corners);
      } else {
        return positionCentered (form, parent, offset, edges, corners);
      }

    }

    //------------------------------------------
    private static Point positionEdges (Form form, ControlRectangleOnScreen parent, int offset, EFormStartPosition edges, EFormStartPosition corners) {
      switch (edges) {
        case EFormStartPosition.AtParentLeft: {
            switch (corners) {
              case EFormStartPosition.AtParentTopLeft:
                return new Point (
                  parent.Left - form.Width + offset,
                  parent.Top + offset
                );
              case EFormStartPosition.AtParentBottomLeft:
                return new Point (
                  parent.Left - form.Width + offset,
                  parent.Bottom - form.Height - offset
                );
            }
            break;
          }
        case EFormStartPosition.AtParentRight: {
            switch (corners) {
              case EFormStartPosition.AtParentTopRight:
                return new Point (
                  parent.Right - offset,
                  parent.Top + offset
                );
              case EFormStartPosition.AtParentBottomRight:
                return new Point (
                  parent.Right - offset,
                  parent.Bottom - form.Height - offset
                );
            }
            break;
          }
        case EFormStartPosition.AtParentTop: {
            switch (corners) {
              case EFormStartPosition.AtParentTopLeft:
                return new Point (
                  parent.Left + offset,
                  parent.Top - form.Height + offset
                );
              case EFormStartPosition.AtParentTopRight:
                return new Point (
                  parent.Right - form.Width - offset,
                  parent.Top - form.Height + offset
                );
            }
            break;
          }
        case EFormStartPosition.AtParentBottom: {
            switch (corners) {
              case EFormStartPosition.AtParentBottomLeft:
                return new Point (
                  parent.Left + offset,
                  parent.Bottom - offset
                );
              case EFormStartPosition.AtParentBottomRight:
                return new Point (
                  parent.Right - form.Width - offset,
                  parent.Bottom - offset
                );
            }
            break;
          }
      }

      return positionCentered (form, parent, offset, edges, corners);
    }

    private static Point positionCentered (Form form, ControlRectangleOnScreen parent, int offset, EFormStartPosition edges, EFormStartPosition corners) {
      if (edges != EFormStartPosition.None) {
        switch (edges) {
          case EFormStartPosition.AtParentLeft:
            return new Point (
              parent.Left - form.Width + offset,
              parent.Top + (parent.Height - form.Height) / 2
            );
          case EFormStartPosition.AtParentRight:
            return new Point (
              parent.Right - offset,
              parent.Top + (parent.Height - form.Height) / 2
            );
          case EFormStartPosition.AtParentTop:
            return new Point (
              parent.Left + (parent.Width - form.Width) / 2,
              parent.Top - form.Height + offset
            );
          case EFormStartPosition.AtParentBottom:
            return new Point (
              parent.Left + (parent.Width - form.Width) / 2,
              parent.Bottom - offset
            );
        }
      } else if (corners != EFormStartPosition.None) {
        switch (corners) {
          case EFormStartPosition.AtParentTopLeft:
            return new Point (
              parent.Left + offset,
              parent.Top + offset
            );
          case EFormStartPosition.AtParentTopRight:
            return new Point (
              parent.Right - form.Width - offset,
              parent.Top + offset
            );
          case EFormStartPosition.AtParentBottomLeft:
            return new Point (
              parent.Left + offset,
              parent.Bottom - form.Height - offset
            );
          case EFormStartPosition.AtParentBottomRight:
            return new Point (
              parent.Right - form.Width - offset,
              parent.Bottom - form.Height - offset
            );
        }
      } else {
        return new Point (
          parent.Left + (parent.Width - form.Width) / 2 + offset,
          parent.Top + (parent.Height - form.Height) / 2 + offset
        );
      }
      return parent.Location;
    }

    #endregion
    #endregion
    #region other public extension methods


    /// <summary>
    /// Adjusts the location of the form to the current screen bounds. 
    /// The current screen is defined by the form owner, if present.
    /// </summary>
    /// <param name="form">The form to be adjusted.</param>
    public static void AdjustLocationToScreenBounds (this Form form) {
      if (form.StartPosition != FormStartPosition.Manual)
        return;

      Rectangle f = new Rectangle (form.Location, form.Size);
      Screen screen = form.Owner != null ? Screen.FromControl (form.Owner) : Screen.FromControl (form);
      Rectangle wa = screen.WorkingArea;

      int x = f.Left;
      int y = f.Top;
      if (f.Right > wa.Right)
        x += wa.Right - f.Right - 1;
      else if (f.Left < wa.Left)
        x += wa.Left - f.Left;
      if (f.Bottom > wa.Bottom)
        y += wa.Bottom - f.Bottom - 1;
      else if (f.Top < wa.Top)
        y += wa.Top - f.Top;

      form.Location = new Point (x, y);

    }

    #endregion

  }


}

