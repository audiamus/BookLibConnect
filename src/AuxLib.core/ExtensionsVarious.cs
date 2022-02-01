using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using System.Xml;
using static System.Math;

namespace core.audiamus.aux.ex {
  public static class ExtDirInfo {
    public static void Clear (this DirectoryInfo di) {
      foreach (DirectoryInfo dir in di.GetDirectories ()) {
        try {
          dir.Clear ();
          dir.Delete (true);
        } catch (IOException) {
        }
      }
      foreach (FileInfo file in di.GetFiles ()) {
        try {
          file.Delete ();
        } catch (IOException) {
        }
      }
    }
  }


  public static class ExInt {
    public static int Digits (this Int32 n) =>
          n == 0 ? 1 : 1 + (int)Math.Log10 (Math.Abs (n));
    public static int Digits (this UInt32 n) =>
          n == 0 ? 1 : 1 + (int)Math.Log10 (Math.Abs (n));
  }

  public static class ExDouble {
    public static double MinMax (this double value, double min, double max) => 
      Min (Max (value, min), max);
  }

  public static class ExNullable {
    public static bool IsNullOrWhiteSpace (this string s) => string.IsNullOrWhiteSpace (s); 
    public static bool IsNullOrEmpty (this string s) => string.IsNullOrEmpty (s); 
    public static bool IsNullOrEmpty<T> (this IEnumerable<T> e) => e is null || e.Count() == 0; 
    public static bool IsNull (this object o) => o is null;
  }

  public static class ExEnumerable {
    public static void ForEach<T> (this IEnumerable<T> items, Action<T> action) {
      foreach (T item in items)
        action (item);
    }
  }

  public static class ExString {
    public const string SEPARATOR = "; ";
    public const char ELLIPSIS = '…';
    
    public static string FirstEtAl (this IEnumerable<string> values, char separator) =>
      values.firstEtAl ($"{separator} ");
    
    public static string FirstEtAl (this IEnumerable<string> values, string separator = SEPARATOR) =>
      values.firstEtAl (separator);

    public static string Combine (this IEnumerable<string> values, char separator) =>
      values.combine (false, $"{separator} ");
    
    public static string Combine (this IEnumerable<string> values, string separator = SEPARATOR) =>
      values.combine (false, separator);

    public static string Combine (this IEnumerable<string> values, bool newLine) =>
      values.combine (newLine, SEPARATOR);
    
    private static string firstEtAl (this IEnumerable<string> values, string separator) {
      if (values.IsNullOrEmpty())
        return null;
      if (values.Count () > 1)
        return $"{values.First ()}{separator}{ELLIPSIS}";
      else
        return values.First ();
    }

    private static string combine (this IEnumerable<string> values, bool newLine, string separator) {
      if (values is null)
        return null;
      var sb = new StringBuilder ();
      foreach (string v in values) {
        if (string.IsNullOrWhiteSpace (v))
          continue;
        if (sb.Length > 0) {
          sb.Append (separator);
          if (newLine)
            sb.AppendLine ();
        }
        sb.Append (v);
      }
      return sb.ToString ();
    }

    public static string[] SplitTrim (this string value, char separator) => value.SplitTrim (new[] { separator });

    public static string[] SplitTrim (this string value, char[] separators = null) {
      if (string.IsNullOrWhiteSpace (value))
        return new string[0];
      if (separators is null)
        separators = new[] { ',', ';' };

      var values = value.Split (separators);
      values = values.Select (v => v.Trim ()).ToArray ();
      return values;
    }

    static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars ();
    static readonly char[] DoubtfulFileNameChars = {
      '¡', '¢', '£', '¤', '¥', '¦', '§', '¨', '©', 'ª', '«', '¬', '®', '¯', '°', '±',
      '²', '³', '´', 'µ', '¶', '·', '¸', '¹', 'º', '»', '¼', '½', '¾', '¿', '×', '÷',
      '‘', '’', 'ƒ', '„', '…', '†', '‡', 'ˆ', '‰', '‹', '’', '“', '”', '•', '–', '—',
      '˜', '™', '›'
    };

    public static string Prune (this string s, char[] invalid) {
      char[] doubtful = null;
      if (s is null)
        return null;
      if (invalid is null) {
        invalid = InvalidFileNameChars;
        doubtful = DoubtfulFileNameChars;
      }
      StringBuilder sb = new StringBuilder ();
      foreach (char c in s) {
        if (invalid.Contains (c))
          continue;
        //sb.Append (',');
        else if (doubtful?.Contains (c) ?? false)
          continue;
        else
          sb.Append (c);
      }
      return sb.ToString ();
    }

    public static string Prune (this string s) {
      if (s is null)
        return null;
      string pruned = s.Prune (null);
      pruned = pruned.Trim ('.');
      return pruned;
    }

    public static string SubstitUser (this string s) {
      if (s is null)
        return null;
      string userdir = ApplEnv.UserDirectoryRoot;
      if (!s.Contains (userdir))
        return s;
      string userdir1 = userdir.Replace (ApplEnv.UserName, "USER");
      string s1 = s.Replace (userdir, userdir1);
      return s1;
    }

    const int MAXLEN_SHORTSTRING = 40;

    public static string Shorten (this string s, int maxlen = 0) {
      if (s is null)
        return null;
      if (maxlen == 0)
        maxlen = MAXLEN_SHORTSTRING;
      if (maxlen < 0 || s.Length <= maxlen)
        return s;

      int partLen1 = maxlen * 2 / 3;
      int partLen2 = maxlen - partLen1 - 1;

      int p2 = s.Length - partLen2;
      return s.Substring (0, partLen1).Trim() + '…' + s.Substring (p2).Trim();
    }

    /// <summary>
    /// Performs the ROT13 character rotation.
    /// </summary>
    public static string Rot13 (this string value) {
      const int C = 13;
      char[] array = value.ToCharArray ();
      for (int i = 0; i < array.Length; i++) {
        int number = (int)array[i];

        if (number >= 'a' && number <= 'z') {
          if (number > 'm') {
            number -= C;
          } else {
            number += C;
          }
        } else if (number >= 'A' && number <= 'Z') {
          if (number > 'M') {
            number -= C;
          } else {
            number += C;
          }
        }
        array[i] = (char)number;
      }
      return new string (array);
    }
  }

  public static class ExEncoding {
    // TODO implement encoding param
    public static byte[] GetBytes (this string s, Encoding enc = null) => Encoding.ASCII.GetBytes (s);
    public static string GetString (this byte[] bytes, Encoding enc = null) => Encoding.ASCII.GetString (bytes);
  }

  public static class JsonExtensions {
    public static JsonSerializerOptions Options { get; } = new JsonSerializerOptions {
      WriteIndented = true,
      ReadCommentHandling = JsonCommentHandling.Skip,
      AllowTrailingCommas = true,
      Converters ={
        new JsonStringEnumConverter()
      },
      Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
      DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static string SerializeToJsonAny (this object any) {
      try {
        string result = JsonSerializer.Serialize (any, any.GetType (), Options);
        return result;
      } catch (Exception) {
        return null;
      }
    }


    public static string SerializeToJson<T> (this T value) {
      try {
        string result = JsonSerializer.Serialize (value, typeof(T), Options);
        return result;
      } catch (Exception) {
        return null;
      }
    }

    public static T DeserializeJson<T> (this string json) {
      try {
        T result = JsonSerializer.Deserialize<T> (json, Options);
        return result;
      } catch (Exception) {
        return default (T);
      }
    }
  }

  public static class ExTimeSpan {
    public static string ToStringHMS (this TimeSpan value) {
      string sgn = value < TimeSpan.Zero ? "-" : string.Empty;
      int hours = Abs (value.Days) * 24 + Abs (value.Hours);
      return $"{sgn}{Abs (hours):D2}:{Abs (value.Minutes):D2}:{Abs (value.Seconds):D2}";
    }
    public static string ToStringHMSm (this TimeSpan value) => $"{value.ToStringHMS ()}.{Abs (value.Milliseconds):D3}";
  }

  public static class ExDateTime {
    public static DateTime RoundDown (this DateTime date, TimeSpan span) {
      long ticks = date.Ticks / span.Ticks;
      return new DateTime (ticks * span.Ticks, date.Kind);
    }
    public static DateTime RoundUp (this DateTime date, TimeSpan span) {
      long ticks = (date.Ticks + span.Ticks - 1) / span.Ticks;
      return new DateTime (ticks * span.Ticks, date.Kind);
    }

    public static string ToXmlTime (this DateTime dt) =>
      XmlConvert.ToString (dt, XmlDateTimeSerializationMode.Utc);

    public static DateTime FromXmlTime (this string s) =>
      XmlConvert.ToDateTime (s, XmlDateTimeSerializationMode.Utc);
  }

  public static class ExUnc {
    private const string UNC = @"UNC\";
    private const string UNC_PFX = @"\\?\";
    private const string UNC_NET = UNC_PFX + UNC;

    public static bool IsUnc (this string path) {
      string root = Path.GetPathRoot (path);

      if (root.StartsWith (UNC_PFX))
        return true;

      return false;
    }

    public static string AsUncIfLong (this string path) {
      if (path.IsUnc ())
        return path;
      path = Path.GetFullPath (path);
      if (path.Length < 250)
        return path;
      return path.AsUnc ();
    }

    public static string AsUnc (this string path) {
      if (path.IsUnc ())
        return path;
      else {
        string root = Path.GetPathRoot (path);

        if (root.StartsWith (@"\\")) {
          string s = path.Substring (2);
          return UNC_NET + s;
        } else
          return UNC_PFX + path;
      }
    }

    public static string StripUnc (this string path) {
      if (!path.IsUnc ())
        return path;
      else {
        string root = Path.GetPathRoot (path);

        if (root.StartsWith (UNC_NET)) {
          string s = path.Substring (UNC_NET.Length);
          return @"\\" + s;
        } else
          return path.Substring (UNC_PFX.Length);
      }
    }
  }

  public static class ExHex {
    public static byte[] HexStringToBytes (this string hex) {
      int NumberChars = hex.Length;
      byte[] bytes = new byte[NumberChars / 2];
      for (int i = 0; i < NumberChars; i += 2)
        bytes[i / 2] = Convert.ToByte (hex.Substring (i, 2), 16);
      return bytes;
    }

    public static string BytesToHexString (this byte[] ba) {
      if (ba is null)
        return null;
      return BitConverter.ToString (ba).Replace ("-", "").ToLower ();
    }
  }

  public static class ExFile {
    private static readonly TimeSpan ONE_MS = TimeSpan.FromMilliseconds (1);

    public static string GetUniqueTimeBasedFilename (this string path, bool alwaysUseSpaceSep = false) {
      const char SPC = ' ';
      const char DSH = '-';

      string dir = Path.GetDirectoryName (path);
      string filnamstub = Path.GetFileNameWithoutExtension (path);
      string ext = Path.GetExtension (path);
      
      char c = (alwaysUseSpaceSep || filnamstub.Contains (SPC)) ? SPC : DSH;
      string fmt1 = $"{c}yyyy_MM_dd{c}HH_mm_ss";
      string fmt2 = $"{fmt1}_fff";
      string fmt = fmt1;

      string result;

      DateTime timestamp = DateTime.Now;
      while (true) {
        string sTimestamp = timestamp.ToString (fmt);
        result = Path.Combine (dir, filnamstub + sTimestamp + ext);
        if (!File.Exists (result))
          break;
        timestamp += ONE_MS;
        fmt = fmt2;
      }

      return result;
    }
  }
  public static class ExBase64 {
    public static string ToBase64StringTrimmed (this byte [] bytes) =>
      bytes.ToBase64String ().TrimBase64String ();

    public static string ToBase64String (this byte [] bytes) => 
      Convert.ToBase64String (bytes);
   
    public static string ToUrlBase64String (this byte[] bytes) =>
      bytes.ToBase64StringTrimmed ().Replace ('+', '-').Replace ('/', '_');
        
    public static string TrimBase64String (this string s) =>
      s.TrimEnd ('=');

    public static byte[] FromBase64String (this string s) {
      s = s.Trim ();
      int n = s.Length % 4;
      string padded = n switch {
        2 => s + "==",
        3 => s + "=",
        _ => s,
      };
      try {
        return Convert.FromBase64String (padded);
      } catch (Exception) {
        return null;
      }
    }

    public static byte[] FromUrlBase64String (this string s) =>
      s.Replace ('_', '/').Replace ('-', '+').FromBase64String ();

  }

  public static class ExImage {
    public static string FindImageFormat (this byte[] bytes) {
      try {
        using (var ms = new MemoryStream (bytes)) {
          using (var bitmap = new Bitmap (ms)) {
            if (bitmap.RawFormat.Equals (ImageFormat.Jpeg))
              return ".jpg";
            else if (bitmap.RawFormat.Equals (ImageFormat.Png))
              return ".png";
            else if (bitmap.RawFormat.Equals (ImageFormat.Gif))
              return ".gif";
            else if (bitmap.RawFormat.Equals (ImageFormat.Bmp))
              return ".bmp";
            else if (bitmap.RawFormat.Equals (ImageFormat.Tiff))
              return ".tif";
            else
              return null;
          }
        }
      } catch (Exception) {
        return null;
      }
    }

  }

  public static class ExList {
    public static void StableSort<T> (this List<T> list, IComparer<T> comparer) {
      var pairs = list.Select ((value, index) => Tuple.Create (value, index)).ToList ();
      pairs.Sort ((x, y) => {
        int result = comparer.Compare (x.Item1, y.Item1);
        return result != 0 ? result : x.Item2 - y.Item2;
      });
      list.Clear ();
      list.AddRange (pairs.Select (key => key.Item1));
    }
  }

  public static class ExException {
    public static string Summary (this Exception exc, bool withCRLF = false) =>
      $"{exc.GetType ().Name}:{(withCRLF ? Environment.NewLine : " ")}\"{exc.Message.SubstitUser()}\"";
  }

  public static class ExType {
    public static string PrettyName (this Type type, int? level = null, bool fullName = false) {
      int nargs = type.GetGenericArguments ().Length;
      if (nargs == 0 || (level.HasValue && nargs > level.Value))
        return typeName ();
      var genericArguments = type.GetGenericArguments ();
      var typeDefinition = type.Name;
      int idx = typeDefinition.IndexOf ("`");
      if (idx < 0)
        return typeName ();
      var unmangledName = typeDefinition.Substring (0, idx);
      return unmangledName + $"<{string.Join (",", genericArguments.Select (t => t.PrettyName(1)))}>";

      string typeName () => fullName ? type.FullName : type.Name;

    }
  }
}
