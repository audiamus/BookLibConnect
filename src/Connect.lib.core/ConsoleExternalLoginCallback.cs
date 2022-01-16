using System;
using System.Diagnostics;
using core.audiamus.aux;

namespace core.audiamus.connect {
  public static class ConsoleExternalLogin {
    public static Uri Callback (Uri uri) {

      ShellExecute.Url (uri);

      Console.WriteLine ("Paste final URL from browser:");
      while (true) {
        string finalUrl = Console.ReadLine ();
        bool succ = Uri.TryCreate (finalUrl, UriKind.Absolute, out Uri finalUri);
        if (!succ) {
          Console.WriteLine ("Invalid URL. Try again:");
          continue;
        }
        Authorization auth = Authorization.Create (finalUri);
        if (auth is null) {
          Console.WriteLine ("URL does not contain authorization. Try again:");
          continue;
        }
        //TokenBearer token = TokenBearer.Create (finalUri);
        //if (token is null) {
        //  Console.WriteLine ("URL does not contain token. Try again:");
        //  continue;
        //}
        return finalUri;
        
      }
    }
  }
}
