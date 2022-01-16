using System.Collections.Generic;
using core.audiamus.aux.ex;

namespace core.audiamus.connect.ex {
  static class Anonymizer {

    private static Dictionary<uint, string> Usernames { get; } = new Dictionary<uint, string> ();
    private static Dictionary<uint, string> Passwords { get; } = new Dictionary<uint, string> ();

    public static string AnonymizeCredentials (this string source, Credentials creds) {
      if (creds is null)
        return source;
      return source.AnonymizeUsernamePassword (creds.Username, creds.Password);
    }

    public static string AnonymizeUsernamePassword (this string source, string username, string password) {
      string intermed = source.AnonymizeUsername (username);
      string result = intermed.AnonymizePassword (password);
      return result;
    } 

    public static string AnonymizeUsername (this string source, string username) {
      if (username.IsNullOrWhiteSpace ())
        return source;
      const string STUB  = "ACCNT";
      return replaceWithSubstitute (Usernames, source, username, STUB);
    }

    public static string AnonymizePassword (this string source, string password) {
      if (password.IsNullOrWhiteSpace ())
        return source;
      const string STUB = "PASSW";
      return replaceWithSubstitute (Passwords, source, password, STUB);
    }

    public static string AnonymizeUsername (this string username) => AnonymizeUsername (null, username);
    public static string AnonymizePassword (this string password) => AnonymizePassword (null, password);

    private static string replaceWithSubstitute (Dictionary<uint, string> dict, string source, string password, string STUB) {
      if (source is null)
        return null;
      string subst = getSubstitute (dict, password, STUB);
      if (source is null)
        return subst;
      else
        return source.Replace (password, subst);
    }

    private static string getSubstitute (Dictionary<uint, string> dict, string key, string stub) {
      const char C = '¿';
      string lkey = key.ToLower ();
      uint ukey = lkey.Checksum32 ();
      lock (dict) {
        bool succ = dict.TryGetValue (ukey, out string subst);
        if (!succ) {
          int n = dict.Count + 1;
          subst = $"{C}{stub}{n}{C}";
          dict[ukey] = subst;
        }
        return subst;
      }
    }
  }
}
