using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using core.audiamus.aux;
using core.audiamus.aux.ex;
using core.audiamus.booksdb;
using HtmlAgilityPack;
using static core.audiamus.aux.ApplEnv;

using R = core.audiamus.connect.Properties.Resources;

namespace core.audiamus.connect.ex {
  public static class AnonExtensions {
    public static string ToAnonString (this Credentials creds) {
      return $"usr={creds.Username.AnonymizeUsername ()}, pwd={creds.Password.AnonymizePassword ()}";
    }
  }

  public static class JsonExtensions {

    public static string CompactJson (this string json) =>
      Regex.Replace (json, "(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", "$1");

    public static bool ValidateJson (this string json) {
      if (json is null)
        return false;
      try {
        var jsonValue = JsonDocument.Parse (json);
      } catch (Exception) {
        return false;
      }
      return true;
    }
  }

  public static class JsonExtractor {
    private static JsonWriterOptions WriterOptions { get; } = new JsonWriterOptions {
      Indented = true,
      Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static string ExtractJsonStructure (this string json) {
      try {
        JsonDocument jDoc = JsonDocument.Parse (json);
        JsonElement jElem = jDoc.RootElement;

        using (MemoryStream msm = new MemoryStream ()) {
          using (Utf8JsonWriter wr = new Utf8JsonWriter (msm, WriterOptions)) {
            parseElem (null, jElem, wr);
            wr.Flush ();
            string extracted = Encoding.UTF8.GetString (msm.ToArray ());
            return extracted;
          }
        }
      } catch (Exception exc) {
        Logging.Log (1, typeof (JsonExtractor), () => exc.Summary ());
        return null;
      }
    }

    private static void parseElem (string key, JsonElement jElem, Utf8JsonWriter wr) {
      switch (jElem.ValueKind) {
        case JsonValueKind.Object:
          parseObject (key, jElem, wr);
          break;
        case JsonValueKind.Array:
          parseArray (key, jElem, wr);
          break;
        case JsonValueKind.String:
          parseString (key, jElem, wr);
          break;
        case JsonValueKind.Null:
          wr.WriteNull (key);
          break;
        case JsonValueKind.Number: {
            double val = jElem.GetDouble ();
            wr.WriteNumber (key, val);
            break;
          }
        case JsonValueKind.False:
        case JsonValueKind.True: {
            bool val = jElem.GetBoolean ();
            wr.WriteBoolean (key, val);
            break;
          }
      }
    }

    private static void parseObject (string key, JsonElement jElem, Utf8JsonWriter wr) {
      IEnumerable<JsonProperty> props = jElem.EnumerateObject ();
      if (key is null)
        wr.WriteStartObject ();
      else
        wr.WriteStartObject (key);
      foreach (var prop in props)
        parseElem (prop.Name, prop.Value, wr);
      wr.WriteEndObject ();
    }

    private static void parseArray (string key, JsonElement jElem, Utf8JsonWriter wr) {
      IEnumerable<JsonElement> elems = jElem.EnumerateArray ();
      if (key is null)
        wr.WriteStartArray ();
      else
        wr.WriteStartArray (key);
      foreach (var elem in elems)
        parseElem (null, elem, wr);
      wr.WriteEndArray ();
    }

    private static void parseString (string key, JsonElement jElem, Utf8JsonWriter wr) {
      string value = jElem.GetString ();
      string newValue = $"string {value.Length} chars";
      wr.WriteString (key, newValue);
    }


  }

  public static class FileExtensions {
    const string JSON = ".json";
    const string HTML = ".html";
    const string TXT = ".txt";

    static readonly TimeSpan ONE_MILLI_SECOND = TimeSpan.FromMilliseconds (1);

    public static string WriteTempImageFile (this byte[] bytes) {
      string ext = bytes.FindImageFormat ();
      if (ext is null)
        return null;
      string path = makePathName (ext, true);
      Directory.CreateDirectory (TempDirectory);
      File.WriteAllBytes (path, bytes);
      return path;
    }


    public static string WriteTempHtmlFile (this string html, string filenameStub = null) {
      const string DOC_HTML = "<!doctype html>";
      if (!html.Contains (DOC_HTML, StringComparison.InvariantCultureIgnoreCase))
        return null;

      return writeTempTextFile (html, filenameStub, HTML);
    }

    public static string WriteTempJsonFile (this string json, string filenameStub = null) {
      if (!json.ValidateJson ())
        return null;

      return writeTempTextFile (json, filenameStub, JSON);
    }

    public static string WriteTempTextFile (this string text, string filenameStub = null) {
      return writeTempTextFile (text, filenameStub, TXT);
    }

    public static string WriteTempJsonFile (this object any, string filenameStub = null) {
      string json = any.SerializeToJsonAny ();
      if (json is null)
        return null;

      return writeTempTextFile (json, filenameStub, JSON);
    }
    public static async Task<string> WriteJsonFileAsync (this object any, string directory, bool unique) =>
      await WriteJsonFileAsync (any, directory, null, unique);

    public static async Task<string> WriteJsonFileAsync (this object any, string directory, string filenameStub = null, bool unique = false) {
      string json = any.SerializeToJsonAny ();
      if (json is null)
        return null;

      if (filenameStub.IsNullOrWhiteSpace ())
        filenameStub = any.GetType ().Name;

      return await writeTextFileAsync (json, directory, filenameStub, JSON, unique);
    }

    public static string WriteJsonFile (this object any, string directory, bool unique) =>
      WriteJsonFile (any, directory, null, unique);

    public static string WriteJsonFile (this object any, string directory, string filenameStub = null, bool unique = false) {
      string json = any.SerializeToJsonAny ();
      if (json is null)
        return null;

      if (filenameStub.IsNullOrWhiteSpace ())
        filenameStub = any.GetType ().Name;

      return writeTextFile (json, directory, filenameStub, JSON, unique);
    }

    public static T ReadJsonFile<T> (string directory, string filename) =>
      ReadJsonFile<T> (Path.Combine (directory, filename));

    public static T ReadJsonFile<T> (string path) {
      if (!File.Exists (path))
        return default;

      try {
        string json = File.ReadAllText (path);
        if (json.IsNullOrWhiteSpace ())
          return default;

        T result = json.DeserializeJson<T> ();
        return result;
      } catch (Exception) {
        return default;
      }

    }
    public static async Task<T> ReadJsonFileAsync<T> (string directory, string filenameStub) {

      if (filenameStub.IsNullOrWhiteSpace ())
        filenameStub = typeof(T).Name;

      string ext = Path.GetExtension (filenameStub);
      if (ext.IsNullOrWhiteSpace ())
        ext = JSON;
      string filename = Path.GetFileNameWithoutExtension (filenameStub) + ext;
      string path = Path.Combine (directory, filename);
      return await ReadJsonFileAsync<T> (path);
    }

    public static async Task<T> ReadJsonFileAsync<T> (string path) {
      if (!File.Exists (path)) {
        string filename = typeof (T).Name + JSON;
        path = Path.Combine (path, filename);
        if (!File.Exists (path))
          return default;
      }

      try {
        string json = await File.ReadAllTextAsync (path);
        if (json.IsNullOrWhiteSpace ())
          return default;

        T result = json.DeserializeJson<T> ();
        return result;
      } catch (Exception) {
        return default;
      }

    }

    private static string writeTextFile (string text, string dir, string filename, string ext, bool unique) {
      if (!dir.IsNullOrWhiteSpace ()) {
        Directory.CreateDirectory (dir);
      }
      string path = makePathName (dir, filename, ext, unique);
      File.WriteAllText (path, text);
      return path;
    }

    private static async Task<string> writeTextFileAsync (string text, string dir, string filename, string ext, bool unique) {
      if (!dir.IsNullOrWhiteSpace ()) {
        Directory.CreateDirectory (dir);
      }
      string path = makePathName (dir, filename, ext, unique);
      await File.WriteAllTextAsync (path, text);
      return path;
    }

    private static string writeTempTextFile (string text, string filename, string ext) {
      string path = makePathName (null, filename, ext, true);
      Directory.CreateDirectory (TempDirectory);
      File.WriteAllText (path, text);
      return path;
    }

    private static string makePathName (string ext, bool unique) =>
      makePathName (null, null, ext, unique);

    private static string makePathName (string dir, string filename, string ext, bool unique) {
      if (dir.IsNullOrWhiteSpace ())
        dir = ApplEnv.TempDirectory;
      if (filename.IsNullOrWhiteSpace ())
        filename = ApplEnv.ApplName;
      else {
        string fext = Path.GetExtension (filename);
        if (ext.IsNullOrWhiteSpace ())
          ext = fext;
        filename = Path.GetFileNameWithoutExtension (filename);
      }

      if (ext.IsNullOrWhiteSpace ())
        ext = TXT;

      if (!ext.StartsWith ('.'))
        ext = '.' + ext;

      string path = Path.Combine (dir, filename + ext);
      if (unique)
        return path.GetUniqueTimeBasedFilename ();
      else
        return path;
    }
  }

  public static class HttpExtensions {

    public static async Task<byte[]> DownloadImageAsync (this HttpClient httpClient, string url) => 
      await httpClient.DownloadImageAsync (new Uri (url));

    public static async Task<byte[]> DownloadImageAsync (this HttpClient httpClient, Uri uri) {
      try {
        using (var networkStream = await httpClient.GetStreamAsync (uri)) {
          using (var memStream = new MemoryStream ()) {
            await networkStream.CopyToAsync (memStream);
            byte[] image = memStream.ToArray ();
            return image;
          }
        }
      } catch (Exception) {
        return default;
      }
    }


    public static string HeadersToString (this HttpResponseMessage response) =>
      response?.Headers.HeadersToString();

    public static string HeadersToString (this HttpRequestMessage request) =>
      request?.Headers.HeadersToString ();

    public static string HeadersToString (this HttpContent content) =>
      content?.Headers.HeadersToString ();

    public static string HeadersToString (this HttpHeaders headers) {
      if (headers is null)
        return null;
      var sb = new StringBuilder ();
      sb.Append ($"{headers.GetType().Name}:");
      var enumerator = headers.GetEnumerator ();
      while (enumerator.MoveNext ()) {
        var kvp = enumerator.Current;
        foreach (var val in kvp.Value)
          sb.Append ($"{Environment.NewLine}  {kvp.Key} = {val}");
      }

      return sb.ToString ();
    }

    public static bool IsNullOrEmpty (this HttpHeaders headers) {
      if (headers is null)
        return true;
      var enumerator = headers.GetEnumerator ();
      bool isNotEmpty = enumerator.MoveNext ();
      enumerator.Dispose ();
      return !isNotEmpty;
    }

    public static string CookiesToString (this CookieContainer cookieContainer, string url) =>
      cookieContainer.CookiesToString (new Uri (url));
    
    public static string CookiesToString (this CookieContainer cookieContainer, Uri uri) {
      if (cookieContainer is null || uri is null)
        return null;
      var cookies = cookieContainer.GetCookies (uri);
      if (cookies is null)
        return null;
      var sb = new StringBuilder ();
      sb.Append ($"{cookies.GetType().Name}:");
      for (int i = 0; i < cookies.Count; i++) {
        var cookie = cookies[i];
        sb.Append ($"{Environment.NewLine}  {cookie.Name} = {cookie.Value}");        
      }

      return sb.ToString ();
    }

    public static string StringKvpCollectionToString (this IEnumerable<KeyValuePair<string,string>> kvps) {
      var sb = new StringBuilder ();
      sb.Append ($"{kvps.GetType().Name}:");
      foreach (var kvp in kvps)
        sb.Append ($"{Environment.NewLine}  {kvp.Key} = {kvp.Value}");        


      return sb.ToString ();
    }

    public static IReadOnlyList<KeyValuePair<string, string>> CookiesToList (this CookieCollection cookieCollection) {
      if (cookieCollection is null)
        return null;
      var cookies = cookieCollection
        .Select (c => new KeyValuePair<string, string> (c.Name, c.Value))
        .ToList ();

      return cookies;
    }

    public static IReadOnlyList<KeyValuePair<string, string>> CookiesToList (this CookieContainer cookieContainer, Uri uri) {
      if (cookieContainer is null || uri is null)
        return null;
      var cookies = cookieContainer.GetCookies (uri).CookiesToList();
      return cookies;
    }

    public static async Task<string> ContentToStringAsync (this HttpRequestMessage request, Credentials creds = null) =>
      await request.Content.ContentToStringAsync (creds);

    public static async Task<string> ContentToStringAsync (this HttpContent content, Credentials creds = null) {
      if (content is not FormUrlEncodedContent)
        return null;

      string reqContentString = await content.ReadAsStringAsync ();
      var nvc = HttpUtility.ParseQueryString (reqContentString);

      var sb = new StringBuilder ();
      sb.Append ($"{content.GetType ().Name}:");
      for (int i = 0; i < nvc.Count; i++) {  
        string key = nvc.GetKey (i);
        string[] values = nvc.GetValues (i);
        foreach (var val in values)
          sb.Append ($"{Environment.NewLine}  {key} = {val.AnonymizeCredentials (creds)}");
      }

      return sb.ToString ();

    }
  }

  public static class HtmlExtensions {
    public static string FormatLineBreaks (this string html) {
      //first - remove all the existing '\n' from HTML
      //they mean nothing in HTML, but break our logic
      html = html.Replace ("\r", "").Replace ("\n", " ");

      //now create an Html Agile Doc object
      HtmlDocument doc = new HtmlDocument ();
      doc.LoadHtml (html);

      //remove comments, head, style and script tags
      foreach (HtmlNode node in doc.DocumentNode.SafeSelectNodes ("//comment() | //script | //style | //head")) {
        node.ParentNode.RemoveChild (node);
      }

      //now remove all "meaningless" inline elements like "span"
      foreach (HtmlNode node in doc.DocumentNode.SafeSelectNodes ("//span | //label")) //add "b", "i" if required
      {
        node.ParentNode.ReplaceChild (HtmlNode.CreateNode (node.InnerHtml), node);
      }

      //block-elements - convert to line-breaks
      foreach (HtmlNode node in doc.DocumentNode.SafeSelectNodes ("//p | //div")) //you could add more tags here
      {
        //we add a "\n" ONLY if the node contains some plain text as "direct" child
        //meaning - text is not nested inside children, but only one-level deep

        //use XPath to find direct "text" in element
        var txtNode = node.SelectSingleNode ("text()");

        //no "direct" text - NOT ADDDING the \n !!!!
        if (txtNode == null || txtNode.InnerHtml.Trim () == "")
          continue;

        //"surround" the node with line breaks
        node.ParentNode.InsertBefore (doc.CreateTextNode ("\r\n"), node);
        node.ParentNode.InsertAfter (doc.CreateTextNode ("\r\n"), node);
      }

      //todo: might need to replace multiple "\n\n" into one here, I'm still testing...

      //now BR tags - simply replace with "\n" and forget
      foreach (HtmlNode node in doc.DocumentNode.SafeSelectNodes ("//br"))
        node.ParentNode.ReplaceChild (doc.CreateTextNode ("\r\n"), node);

      //finally - return the text which will have our inserted line-breaks in it
      return doc.DocumentNode.InnerText.Trim ();

      //todo - you should probably add "&code;" processing, to decode all the &nbsp; and such
    }

    //here's the extension method I use
    private static HtmlNodeCollection SafeSelectNodes (this HtmlNode node, string selector) {
      return (node.SelectNodes (selector) ?? new HtmlNodeCollection (node));
    }

  }


  static class AccessTokenExtensions {
    const string TOKEN_STUB = "Atna|";
    public static bool Validate (this TokenBearer token) {
      if (token is null)
        return false;

      if (!token.AccessToken?.StartsWith (TOKEN_STUB) ?? false)
        return false;

      //var utcnow = DateTime.UtcNow;
      //return token.Expiration < utcnow;
      return true;
    }
  }

  public static partial class NameValueExtensions {
    public static IReadOnlyList<KeyValuePair<string, string>> ToList (this NameValueCollection @this) {
      var list = new List<KeyValuePair<string, string>> ();

      if (@this is not null) {
        foreach (string key in @this.AllKeys) {
          if (key is null)
            continue;
          list.Add (new KeyValuePair<string, string>(key, @this[key]));
        }
      }

      return list;
    }
  }

  public static partial class ProfileExtensions {

    public static bool Matches (this IProfileKey profile, IProfileKey other) =>
      profile is not null && other is not null && 
        profile.Region == other.Region && string.Equals (profile.AccountId, other.AccountId);

    internal static string GetAccountAlias (
      this IProfile profile,
      BookLibrary bookLibrary,
      Func<AccountAliasContext, bool> getAccountAliasFunc,
      bool newAlias = false
    ) {
      var ctxt = profile.GetAccountAliasContext (bookLibrary, getAccountAliasFunc, newAlias);
      return ctxt.Alias ?? ctxt.CustomerName;
    }

    internal static AccountAliasContext GetAccountAliasContext (
      this IProfile profile,
      BookLibrary bookLibrary,
      Func<AccountAliasContext, bool> getAccountAliasFunc,
      bool newAlias
    ) {
      var ctxt = bookLibrary.GetAccountId (profile, newAlias);
      if ((ctxt.Alias.IsNullOrWhiteSpace () || newAlias) && getAccountAliasFunc is not null) {
        getAccountAliasFunc.Invoke (ctxt);
        bookLibrary.SetAccountAlias (ctxt);
      }
      return ctxt;
    }

    internal static IProfileAliasKey CreateAliasKey (
      this IProfile profile,
      BookLibrary bookLibrary,
      Func<AccountAliasContext, bool> getAccountAliasFunc
    ) {
      string alias = profile.GetAccountAlias (bookLibrary, getAccountAliasFunc);
      if (alias.IsNullOrWhiteSpace ())
        return null;
      return new ProfileAliasKey (profile.Region, alias);
    }

    internal static IProfileKey CreateKey (this IProfile profile) => 
      new ProfileKey (profile.Id, profile.Region, profile.CustomerInfo?.AccountId);
    
    internal static IProfileKeyEx CreateKeyEx (this IProfile profile) => 
      new ProfileKeyEx (
        profile.Id,
        profile.Region, 
        profile.CustomerInfo?.Name, 
        profile.CustomerInfo?.AccountId, 
        profile.DeviceInfo?.Name);

    internal static bool Matches (this IProfile profile, IProfileKey key) {
      if (profile is null || key is null)
        return false;
      return profile.Region == key.Region && 
        string.Equals (profile.CustomerInfo.AccountId, key.AccountId);
    }

    internal static bool Matches (this IProfile profile, IProfile other) {
      if (profile is null || other is null)
        return false;

      if (object.Equals (profile, other))
        return true;

      return profile.Region == other.Region && 
        string.Equals (profile.CustomerInfo.AccountId, other.CustomerInfo.AccountId);
    }

    internal static bool MatchesId (this ProfileBundle profile, IProfileKey key) =>
      MatchesId (profile?.Profile, key);

    internal static bool MatchesId (this IProfile profile, IProfileKey key) {
      if (profile is null || key is null)
        return false;

      return profile.Id == key.Id;
    }
  }

  internal static class KeyValueExtensions {
    internal static string ToQueryString (this IEnumerable<KeyValuePair<string, string>> parameters) =>
      string.Join ("&", parameters.Select (x => $"{x.Key.UrlEncode ()}={x.Value.UrlEncode ()}"));

    internal static string UrlEncode (this string value) => WebUtility.UrlEncode (value);
  }

  internal static class CopyExtensions {
    internal static Conversion Copy (this IConversion other) {
      if (other is null)
        return null;
      return new Conversion (other.Id) {
        State = other.State,
        DownloadFileName = other.DownloadFileName,
        DestDirectory = other.DestDirectory
      };
    }
  }

  internal static class DownloadFilenameExtensions {
    static readonly string[] __knownExtensions = new string[] {
      R.EncryptedFileExt, R.DecryptedFileExt, R.ExportedFileExt
    };

    public static string GetDownloadFileNameWithoutExtension (this string downloadFileName) { 
      string ext = Path.GetExtension (downloadFileName).ToLower();
      if (__knownExtensions.Contains (ext))
        return Path.GetFileNameWithoutExtension (downloadFileName);
      else
        return Path.GetFileName (downloadFileName);
    }
  }
}
