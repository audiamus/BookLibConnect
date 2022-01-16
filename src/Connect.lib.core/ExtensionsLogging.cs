using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using core.audiamus.aux;
using static core.audiamus.aux.Logging;

namespace core.audiamus.connect.ex {
  public static class LoggingExtensions {
    const string BEFORE = "Before request, "; 
    const string AFTER =  "After request,  "; 
     
    public static async Task LogAsync (
      this HttpRequestMessage request, 
      uint level, 
      Type caller, 
      HttpRequestHeaders requestHeaders = null,
      CookieContainer cookieContainer = null,
      Uri baseUri = null,
      Credentials credentials = null,
      [CallerMemberName] string method = null) 
    {
      Uri uri = request.RequestUri;
      if (!uri.IsAbsoluteUri && (baseUri?.IsAbsoluteUri ?? false)) 
        uri = new Uri (baseUri, request.RequestUri);

      Log (level, caller, () => $"{BEFORE}{request.Method}, {uri}", method);
 
      if (!requestHeaders.IsNullOrEmpty())
        Log (level, caller, () => $"{BEFORE}default {requestHeaders.HeadersToString ()}", method);
      if (!request.Headers.IsNullOrEmpty())
        Log (level, caller, () => $"{BEFORE}{request.HeadersToString ()}", method);
      
      if (cookieContainer is not null && cookieContainer.Count > 0 && baseUri is not null)
        Log (level, caller, () => $"{BEFORE}{cookieContainer?.CookiesToString (baseUri)}", method);
      
      if (Logging.Level >= level && request.Content is FormUrlEncodedContent) {
        string reqContentString = await request.ContentToStringAsync (credentials);
        Log (level, caller, () => $"{BEFORE}{reqContentString}", method);
      }
    }

    public static async Task LogAsync (
      this HttpRequestMessage request, 
      uint level, 
      object caller,
      HttpRequestHeaders requestHeaders = null,
      CookieContainer cookieContainer = null, 
      Uri baseUri = null,
      Credentials credentials = null,
      [CallerMemberName] string method = null
    ) => await request.LogAsync (level, caller.GetType (), requestHeaders, cookieContainer, baseUri, credentials, method);

    public static async Task LogAsync (
      this HttpRequestMessage request, 
      uint level, 
      Type caller,
      HttpRequestHeaders requestHeaders,
      CookieContainer cookieContainer, 
      string baseUriString,
      Credentials credentials,
      [CallerMemberName] string method = null
    ) => await request.LogAsync (level, caller, requestHeaders, cookieContainer, new Uri (baseUriString), credentials, method);
    
    public static async Task LogAsync (
      this HttpRequestMessage request, 
      uint level, 
      object caller,
      HttpRequestHeaders requestHeaders,
      CookieContainer cookieContainer, 
      string baseUriString,
      Credentials credentials,
      [CallerMemberName] string method = null
    ) => await request.LogAsync (level, caller.GetType (), requestHeaders, cookieContainer, new Uri (baseUriString), credentials, method);

    public static async Task LogAsync (
      this HttpRequestMessage request, 
      uint level, 
      Type caller,
      HttpRequestHeaders requestHeaders,
      CookieContainer cookieContainer, 
      CredentialsUrl credentials,
      [CallerMemberName] string method = null
    ) => await request.LogAsync (level, caller, requestHeaders, cookieContainer, new Uri (credentials.BaseUriString), credentials, method);

    public static async Task LogAsync (
      this HttpRequestMessage request, 
      uint level, 
      object caller,
      HttpRequestHeaders requestHeaders,
      CookieContainer cookieContainer, 
      CredentialsUrl credentials,
      [CallerMemberName] string method = null
    ) => await request.LogAsync (level, caller.GetType (), requestHeaders, cookieContainer, new Uri (credentials.BaseUriString), credentials, method);

    public static async Task LogAsync (
      this HttpResponseMessage response, 
      uint level, 
      Type caller, 
      CookieContainer cookieContainer = null,
      Uri baseUri = null, 
      Credentials credentials = null,
      [CallerMemberName] string method = null) 
    {
      Uri uri = response?.RequestMessage?.RequestUri;
      if (uri is not null && !uri.IsAbsoluteUri && (baseUri?.IsAbsoluteUri ?? false))
        uri = new Uri (baseUri, uri);


      Log (level, caller, () => $"{AFTER}{response.RequestMessage.Method}, status={response.StatusCode}," 
        + $" requestUri={uri}", method);

      Log (level, caller, () => $"{AFTER}{response.HeadersToString ()}", method);

      if (cookieContainer is not null && cookieContainer.Count > 0 && baseUri is not null)
        Log (level, caller, () => $"{AFTER}{cookieContainer?.CookiesToString (baseUri)}", method);

      //if (Logging.Level >= level && response.IsSuccessStatusCode) {
      // anyway
      if (Logging.Level >= level) {
        try {
          HttpContent content = response.Content;
          string result = await content.ReadAsStringAsync ();
          string file = WriteHtml (result, credentials);
          if (file is not null)
            Log (level, caller, () => 
              $"{AFTER}response content written to \"{Path.GetFileName (file)}\"", method);
        } catch (Exception) { }
      }
    }

    public static string WriteHtml (string result, Credentials credentials) {
      string anonResult = result.AnonymizeCredentials (credentials);
      string file = anonResult.WriteTempHtmlFile ();
      return file;
    }

    public static async Task LogAsync (
      this HttpResponseMessage response, 
      uint level,
      object caller,
      CookieContainer cookieContainer = null,
      Uri baseUri = null, 
      Credentials credentials = null,
      [CallerMemberName] string method = null
    ) => await response.LogAsync (level, caller.GetType (), cookieContainer, baseUri, credentials, method);

    public static async Task LogAsync (
      this HttpResponseMessage response, 
      uint level,
      Type caller,
      CookieContainer cookieContainer,
      string baseUriString, 
      Credentials credentials = null,
      [CallerMemberName] string method = null
    ) => await response.LogAsync (level, caller, cookieContainer, new Uri (baseUriString), credentials, method);

    public static async Task LogAsync (
      this HttpResponseMessage response, 
      uint level,
      object caller,
      CookieContainer cookieContainer,
      string baseUriString, 
      Credentials credentials = null,
      [CallerMemberName] string method = null
    ) => await response.LogAsync (level, caller.GetType (), cookieContainer, new Uri (baseUriString), credentials, method);
    
    public static async Task LogAsync (
      this HttpResponseMessage response, 
      uint level,
      Type caller,
      CookieContainer cookieContainer,
      CredentialsUrl credentials,
      [CallerMemberName] string method = null
    ) => await response.LogAsync (level, caller.GetType (), cookieContainer, new Uri (credentials.BaseUriString), credentials, method);
    
    public static async Task LogAsync (
      this HttpResponseMessage response, 
      uint level,
      object caller,
      CookieContainer cookieContainer,
      CredentialsUrl credentials,
      [CallerMemberName] string method = null
    ) => await response.LogAsync (level, caller.GetType (), cookieContainer, new Uri (credentials.BaseUriString), credentials, method);

  }
}
