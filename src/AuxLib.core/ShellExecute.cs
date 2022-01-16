using System;
using System.Diagnostics;

namespace core.audiamus.aux {
  public static class ShellExecute {
    public static void Url (Uri uri) => File (uri.OriginalString);

    public static void File (string url) {
      Process.Start (new ProcessStartInfo () {
        UseShellExecute = true,
        CreateNoWindow = true,
        FileName = url,
      });

    }
  }
}
