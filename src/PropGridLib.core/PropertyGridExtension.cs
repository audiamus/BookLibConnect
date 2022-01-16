using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace core.audiamus.aux.propgrid {
  public static class PropertyGridExtensions {


    /// <summary>
    /// Moves the property Grid's splitter bar over to fit the width 
    /// of the longest display name string + padding
    ///
    /// </summary>
    /// <param name="viewControl">The property Grid whose splitter bar is to be moved</param>
    /// <param name="iPadding">Right padding to include with longest display name width</param>
    public static void MoveSplitterToLongestDisplayName (this PropertyGrid propertyGrid, int iPadding) {
      try {

        if (propertyGrid.SelectedObject is null)
          return;

        Size longestTextSize = new Size ();
        if (propertyGrid.SelectedObject is DynamicTypeDescriptor dtd) {
          var propdescs = dtd.GetProperties (new[] { new BrowsableAttribute (true) });
          foreach (PropertyDescriptor propdesc in propdescs) {
            var browsable = propdesc.Attributes[typeof (BrowsableAttribute)] as BrowsableAttribute;
            if (!(browsable?.Browsable ?? false))
              continue;
            string displayName = propdesc.DisplayName;
            Size textSize = TextRenderer.MeasureText (displayName, propertyGrid.Font);
            if (textSize.Width > longestTextSize.Width) {
              longestTextSize = textSize;
            }
          }
        }

        propertyGrid.MoveSplitterTo (longestTextSize.Width + 17 + iPadding);
      } catch (Exception) {
      }
    }

    /// <summary>
    /// Gets the width of the left column.
    /// </summary>
    /// <param name="propertyGrid">The property grid.</param>
    /// <returns>
    /// The width of the left column.
    /// </returns>
    public static int GetInternalLabelWidth (this PropertyGrid propertyGrid) {
      object gridView = getPropertyGridView (propertyGrid);

      PropertyInfo pi = gridView.GetType ().GetProperty (
        "InternalLabelWidth", BindingFlags.NonPublic | BindingFlags.Instance);
      return (int)pi.GetValue (gridView);
    }

    /// <summary>
    /// Moves the property Grid's splitter bar to given position
    /// </summary>
    /// <param name="viewControl"></param>
    /// <param name="position"></param>
    public static void MoveSplitterTo (this PropertyGrid propertyGrid, int position) {
      object propertyGridView = getPropertyGridView (propertyGrid);

      MethodInfo mi = propertyGridView.GetType ().GetMethod (
        "MoveSplitterTo", BindingFlags.NonPublic | BindingFlags.Instance);
      mi.Invoke (propertyGridView, new object[] { position });
    }


    /// <summary>
    /// Gets the (private) PropertyGridView instance.
    /// </summary>
    /// <param name="propertyGrid">The property grid.</param>
    /// <returns>The PropertyGridView instance.</returns>
    private static object getPropertyGridView (this PropertyGrid propertyGrid) {
      MethodInfo mi = typeof (PropertyGrid).GetMethod (
        "GetPropertyGridView", BindingFlags.NonPublic | BindingFlags.Instance);
      return mi.Invoke (propertyGrid, Array.Empty<object> ());
    }

  }
}
