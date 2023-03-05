//#define TEST_UNAVAIL
//#define TEST_INVAL_CHAR

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AAXClean;
using core.audiamus.aux;
using core.audiamus.aux.ex;
using core.audiamus.booksdb;
using core.audiamus.common;
using core.audiamus.connect.ex;
using static core.audiamus.aux.Logging;
using R = core.audiamus.connect.Properties.Resources;

namespace core.audiamus.connect {
  class AudibleApi : IAudibleApi {
    const string USER_AGENT = "Audible/671 CFNetwork/1240.0.4 Darwin/20.6.0";

    //const string HTTP_AUTHORITY_AUDIBLE = @"https://api.audible.";
    const string CONTENT_PATH = "/1.0/content";
    private int _accountId;
    private string _accountAlias;

#if TEST_INVAL_CHAR
    const char ORIG = '\''; 
    const char SUBS = '.'; 
#endif

    //private string BaseUrlAudible { get; }
    //private Uri BaseUriAudible => HttpClientAudible?.BaseAddress;
    //private Uri BaseUriAmazon => HttpClientAmazon?.BaseAddress;

    private IProfile Profile { get; }
    private HttpClientEx HttpClientAudible { get; }
    private HttpClientEx HttpClientAmazon { get; }
    private BookLibrary BookLibrary { get; }
    
    private int AccountId {
      get {
        ensureAccountId ();
        return _accountId;
      }
    }

    private HttpClientEx HttpClient => Profile.PreAmazon ? HttpClientAudible : HttpClientAmazon;


    public string AccountAlias {
      get {
        ensureAccountId ();
        return _accountAlias;
      }
    }

    public ERegion Region => Profile.Region;

    public Func<AccountAliasContext, bool> GetAccountAliasFunc { private get; set; }
    public Func<Task> RefreshTokenAsyncFunc { get; private set; }

    public AudibleApi (
      IProfile profile,
      HttpClientEx httpClientAmazon,
      HttpClientEx httpClientAudible,
      BookLibrary bookLibrary,
      Func<IProfile, Task> refreshTokenAsyncFunc
    ) {
      BookLibrary = bookLibrary;
      RefreshTokenAsyncFunc = () => refreshTokenAsyncFunc (Profile);

      if (profile is null)
        return;

      Profile = profile;
      HttpClientAmazon = httpClientAmazon;
      HttpClientAudible = httpClientAudible;

      //ILocale locale = profile.Region.FromCountryCode ();
      //Uri baseUriAudible = new Uri (HTTP_AUTHORITY_AUDIBLE + locale.Domain);
      //HttpClientAudible = HttpClientEx.Create (baseUriAudible);
    }

    internal AudibleApi (
      BookLibrary bookLibrary,
      int accountId,
      ERegion region
    ) {
      BookLibrary = bookLibrary;
      _accountId = accountId;
      Profile = new Profile (region, null, null, false);
    }

    public void Dispose () {
      //HttpClientAudible?.Dispose ();
    }

    public async Task<adb.json.LibraryResponse> GetLibraryAsync (bool resync) => await GetLibraryAsync (null, resync);


    internal async Task<adb.json.LibraryResponse> GetLibraryAsync (string json, bool resync) {
      using var _ = new LogGuard (3, this);

      const int PAGE_SIZE = 100;
      int page = 0;
      var libProducts = new List<adb.json.Product> ();

      if (json is null) {

        const string GROUPS
          = "response_groups=badge_types,category_ladders,claim_code_url,contributors,is_downloaded,is_returnable,media,"
          + "origin_asin,pdf_url,percent_complete,price,product_attrs,product_desc,product_extended_attrs,product_plan_details,"
          + "product_plans,provided_review,rating,relationships,review_attrs,reviews,sample,series,sku";

        DateTime dt = await BookLibrary.SinceLatestPurchaseDateAsync (new ProfileId (AccountId, Region), resync);

        while (true) {
          page++;
          string url = "/1.0/library"
            + $"?purchased_after={dt.ToXmlTime ()}"
            + $"&num_results={PAGE_SIZE}"
            + $"&page={page}"
            + "&"
            + GROUPS;

          string pageResult = await callAudibleApiSignedForStringAsync (url);
          if (pageResult is null)
            return null;

          if (Logging.Level >= 3) {
            string file = pageResult.WriteTempJsonFile ("LibraryResponse");
            Log (3, this, () => $"page={page}, file=\"{Path.GetFileName (file)}\"");
          }

          adb.json.LibraryResponse libraryResponse = adb.json.LibraryResponse.Deserialize (pageResult);
          if (libraryResponse is null)
            return null;

          if (!(libraryResponse?.items.Any () ?? false))
            break;

          var pageProducts = libraryResponse.items;
#if TEST_UNAVAIL
          pageProducts = pageProducts.ToList().Take(pageProducts.Length - 1).ToArray();
#endif
          Log (3, this, () => $"#items/page={pageProducts.Length}");
          libProducts.AddRange (pageProducts);


        }
      } else {
        adb.json.LibraryResponse libraryResponse = adb.json.LibraryResponse.Deserialize (json);
        libProducts.AddRange (libraryResponse.items);
      }

      libProducts = libProducts.DistinctBy (p => p.asin).ToList ();

      libProducts.Sort ((x, y) => DateTime.Compare (x.purchase_date, y.purchase_date));

#if TEST_INVAL_CHAR
      libProducts = libProducts
        .Select (p => {
          p.title = p.title.Replace (ORIG, SUBS);
          return p;
        })
        .ToList ();
#endif

      await BookLibrary.AddRemBooksAsync (libProducts, new ProfileId (AccountId, Region), resync);

      var allPagesResponse = new adb.json.LibraryResponse ();
      allPagesResponse.items = libProducts.ToArray ();
      return allPagesResponse;
    }

    private void ensureAccountId () {
      if (_accountId > 0)
        return;

      var ctxt = Profile.GetAccountAliasContext (BookLibrary, GetAccountAliasFunc, false);

      _accountId = ctxt.LocalId;
      _accountAlias = ctxt.Alias;

    }

    public async Task<string> GetUserProfileAsync () {
      using var _ = new LogGuard (3, this);

      await RefreshTokenAsyncFunc ();

      var url = $"/user/profile?access_token={Profile.Token.AccessToken}";

      var request = new HttpRequestMessage (HttpMethod.Get, url);
      return await sendForStringAsync (request, HttpClient);

    }
      

    public async Task<string> GetAccountInfoAsync () {
      using var _ = new LogGuard (3, this);

      const string GROUPS = "response_groups=migration_details,subscription_details_rodizio,subscription_details_premium,customer_segment,subscription_details_channels";

      var url = "/1.0/customer/information"
        + "?"
        + GROUPS;

      return await callAudibleApiSignedForStringAsync (url);
    }

    public async Task<bool> GetActivationBytesAsync () {
      using var _ = new LogGuard (3, this);
      var url = "/license/token?action=register&player_manuf=Audible,iPhone&player_model=iPhone";
      byte[] response = await callAudibleApiSignedForBytesAsync (url);

      return false;
    }

    public async Task<adb.json.LicenseResponse> GetDownloadLicenseAsync (string asin, EDownloadQuality quality) {
      using var _ = new LogGuard (3, this, () => $"asin={asin}");
      string response = await getDownloadLicenseAsync (asin, quality);

      if (Logging.Level >= 3) {
        string file = response.WriteTempJsonFile ($"LicenseResponse_{asin}");
        Log (3, this, () => $"asin={asin}, file=\"{Path.GetFileName (file)}\"");
      }

      adb.json.LicenseResponse license = adb.json.LicenseResponse.Deserialize (response);

      decryptLicense (license?.content_license);

      return license;
    }

    public async Task<bool> GetDownloadLicenseAndSaveAsync (Conversion conversion, EDownloadQuality quality) {
      using var _ = new LogGuard (3, this, () => $"{conversion}");
      Log (3, this, () => $"{conversion}; desired quality: {quality}");
      adb.json.LicenseResponse licresp;
      // get license
      try {
        licresp = await GetDownloadLicenseAsync (conversion.Asin, quality);
      } catch (Exception exc) {
        conversion.State = EConversionState.license_denied;
        Log (3, this, () => $"{conversion}; {exc.Summary ()}");
        return false;
      }

      var lic = licresp?.content_license;
      if (lic?.voucher is null) {
        conversion.State = EConversionState.license_denied;
        Log (3, this, () => $"{conversion}; license decryption failed.");
        return false;
      }

      bool succ = Enum.TryParse<ELicenseStatusCode> (lic.status_code, out var status);
      if (!succ || status != ELicenseStatusCode.Granted) {
        conversion.State = EConversionState.license_denied;
        Log (3, this, () => $"{conversion}; license not granted.");
        return false;
      }


      // save license to DB, including chapters
      // update state
      var aq = BookLibrary.UpdateLicenseAndChapters (lic, conversion, quality);
      Log (3, this, () => $"{conversion}; done, {aq}");

      return true;
    }


    public async Task<bool> DownloadAsync (Conversion conversion, Action<Conversion, long> progressAction, CancellationToken cancToken) {

      conversion.State = EConversionState.downloading;
      using var _ = new LogGuard (3, this, () => conversion.ToString ());

      try {
        if (conversion.DownloadUrl is null)
          return false;
        Uri requestUri = new Uri (conversion.DownloadUrl);
        var request = new HttpRequestMessage (HttpMethod.Get, requestUri);
        request.Headers.UserAgent.ParseAdd (USER_AGENT);
        var response = await HttpClientAudible.SendAsync (request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode ();

        string destfilename = (conversion.DownloadFileName + R.EncryptedFileExt).AsUncIfLong ();
        long sourceFileSize = conversion.BookCommon.FileSizeBytes ?? 0;
        if (sourceFileSize == 0)
          return false;
        Log (3, this, () => $"{conversion}; size={sourceFileSize / (1024 * 1024)} MB");

        using var networkStream = await response.Content.ReadAsStreamAsync (cancToken);
        using var rdr = new BufferedStream (networkStream);
        using var fileStream = File.OpenWrite (destfilename);
        using var wrtr = new BufferedStream (fileStream);

        long accusize = await Task.Run (async () => await copyStreams (conversion, rdr, wrtr, progressAction, cancToken), cancToken);

        bool succ = accusize >= sourceFileSize;
        if (!succ)
          conversion.State = EConversionState.download_error;
        else
          BookLibrary.SavePersistentState (conversion, EConversionState.local_locked);

        Log (3, this, () => $"{conversion}; download finished, succ={succ}.");
        return succ;
      } catch (Exception exc) {
        conversion.State = EConversionState.download_error;
        Log (1, this, () => $"{conversion}; {exc.Summary ()}");
      }
      return false;
    }


    public async Task<bool> DecryptAsync (
      Conversion conversion,
      Action<Conversion, TimeSpan> progressAction,
      CancellationToken cancToken
    ) {
      conversion.State = EConversionState.unlocking;
      using var _ = new LogGuard (3, this, () => conversion.ToString ());

      AaxFile aaxFile = null;
      var rg = new ResourceGuard (() => aaxFile?.Dispose ());


      bool succ = false;
      int numChannels = 0;
      string inputFile = (conversion.DownloadFileName + R.EncryptedFileExt).AsUncIfLong ();
      string outputFile = (conversion.DownloadFileName + R.DecryptedFileExt).AsUncIfLong ();

      
      try {
        if (!File.Exists (inputFile))
          return false;

        using (var ifStream = File.OpenRead (inputFile)) {

          aaxFile = new AaxFile (ifStream);
          aaxFile.ConversionProgressUpdate += aaxFile_ConversionProgressUpdate;
          aaxFile.SetDecryptionKey (conversion.BookCommon.LicenseKey, conversion.BookCommon.LicenseIv);

          numChannels = aaxFile.AudioChannels;

          ConversionResult result;
          using (var fileStream = File.OpenWrite (outputFile))
            result = await Task.Run (() => {
              var result = aaxFile.ConvertToMp4a (fileStream);
              if (result == ConversionResult.NoErrorsDetected)
                progressAction.Invoke (conversion, TimeSpan.FromSeconds (conversion.BookMeta.RunTimeLengthSeconds ?? 0));
              return result;
            });

          succ = result == ConversionResult.NoErrorsDetected;
        }

        if (succ)
          BookLibrary.SavePersistentState (conversion, EConversionState.local_unlocked);
        else
          conversion.State = EConversionState.unlocking_failed;

        Log (3, this, () => $"{conversion}; decryption finished, succ={succ}.");

      } catch (Exception exc) {
        conversion.State = EConversionState.unlocking_failed;
        Log (1, this, () => $"{conversion}; {exc.Summary ()}");
        return false;
      }

      if (succ && numChannels > 0) {
        try {
          string suffix = numChannels == 1 ? "_Mono" : "_Stereo";
          rename (inputFile, suffix);
          rename (outputFile, suffix);
          BookLibrary.SaveFileNameSuffix (conversion, suffix);
        } catch (Exception exc) {
          Log (1, this, () => $"{conversion}; {exc.Summary ()}");
        }
      }

      return succ;


      void rename (string file, string suffix) {
        string dir = Path.GetDirectoryName (file);
        string stub = Path.GetFileNameWithoutExtension (file);
        string ext = Path.GetExtension (file);
        string sfxfile = Path.Combine (dir, stub + suffix + ext);
        File.Move (file, sfxfile, true);
      }

      async void aaxFile_ConversionProgressUpdate (object sender, ConversionProgressEventArgs e) {
        if (cancToken.IsCancellationRequested)
          await aaxFile?.CancelAsync ();
        progressAction.Invoke (conversion, e.ProcessPosition);
      }
    }

    public async Task DownloadCoverImagesAsync () {
      using var _ = new LogGuard (3, this);
      await BookLibrary.AddCoverImagesAsync (url => HttpClientAmazon.DownloadImageAsync (url));
    }

    public async Task UpdateMetaInfo (IEnumerable<Component> components, Action<IEnumerable<Component>> onDone) {
      using var _ = new LogGuard (3, this, () => $"#comp={components.Count ()}");
      var pairs = new List<ProductComponentPair> ();
      foreach (var comp in components) {
        Log (3, this, () => comp.Conversion.ToString ());
        var prod = await GetProductInfoAsync (comp.Asin);
        if (prod is null)
          continue;
        pairs.Add (new (prod, comp));
      }
      BookLibrary.UpdateComponentProduct (pairs);
      var result = pairs.Select (p => p.Component).ToList ();
      onDone (result);
    }

    public async Task<adb.json.Product> GetProductInfoAsync (string asin) {

      const string GROUPS
        = "response_groups=contributors,media,product_attrs,product_desc,product_extended_attrs," +
          "product_plan_details,product_plans,rating,review_attrs,reviews,sample,sku";

      string url = "/1.0/catalog/products/"
        + asin
        + "?"
        + GROUPS;

      string result = await callAudibleApiSignedForStringAsync (url);

      if (Logging.Level >= 3) {
        string file = result.WriteTempJsonFile ($"ProductResponse_{asin}");
        Log (3, this, () => $"asin={asin}, file=\"{Path.GetFileName (file)}\"");
      }

      adb.json.ProductResponse productResponse = adb.json.ProductResponse.Deserialize (result);

      adb.json.Product product = productResponse?.product;
#if TEST_INVAL_CHAR
      if (product is not null) {
        product.title = product.title.Replace (ORIG, SUBS);
      }
#endif

      return product;

    }

    public IEnumerable<Book> GetBooks () {
      return BookLibrary.GetBooks (new ProfileId (AccountId, Region));
    }

    public void SavePersistentState (Conversion conversion, EConversionState state) {
      BookLibrary.SavePersistentState (conversion, state);
    }

    public void RestorePersistentState (Conversion conversion) {
      BookLibrary.RestorePersistentState (conversion);
    }

    public EConversionState GetPersistentState (Conversion conversion) {
      return BookLibrary.GetPersistentState (conversion);
    }

    public void CheckUpdateFilesAndState (
      IDownloadSettings downloadSettings,
      IExportSettings exportSettings,
      Action<IConversion> callbackRefConversion,
      IInteractionCallback<InteractionMessage<BookLibInteract>, bool?> interactCallback
    ) {
      BookLibrary.CheckUpdateFilesAndState (
        new ProfileId (AccountId, Region),
        downloadSettings,
        exportSettings,
        callbackRefConversion,
        interactCallback
      );
    }


    private async Task<string> getDownloadLicenseAsync (string asin, EDownloadQuality quality) {

      var url = $"{CONTENT_PATH}/{asin}/licenserequest";

      string jsonBody = buildlicenseRequestBody (quality);

      return await callAudibleApiSignedForStringAsync (url, jsonBody);
    }

    private static string buildlicenseRequestBody (EDownloadQuality quality) {
      string json = $@"{{ 
        ""consumption_type"": ""Download"",
        ""supported_drm_types"": [""Adrm"", ""Mpeg""],
        ""quality"": ""{quality}"",
        ""response_groups"": ""last_position_heard,pdf_url,content_reference,chapter_info""
      }}";

      json = json.CompactJson ();

      if (!json.ValidateJson ())
        throw new InvalidOperationException ("invalid json");

      return json;
    }

    private void decryptLicense (adb.json.ContentLicense license) {
      // See also
      //https://patchwork.ffmpeg.org/project/ffmpeg/patch/17559601585196510@sas2-2fa759678732.qloud-c.yandex.net/

      if (license is null)
        return;

      string hashable = Profile.DeviceInfo.Type + Profile.DeviceInfo.Serial + Profile.CustomerInfo.AccountId +
        license.asin;

      byte[] hashableBytes = Encoding.ASCII.GetBytes (hashable);
      byte[] key = new byte[16];
      byte[] iv = new byte[16];

      using var sha256 = SHA256.Create ();
      byte[] hash = sha256.ComputeHash (hashableBytes);
      Array.Copy (hash, 0, key, 0, 16);
      Array.Copy (hash, 16, iv, 0, 16);

      byte[] encryptedText = Convert.FromBase64String (license.license_response);

      using var aes = Aes.Create ();
      aes.Mode = CipherMode.CBC;
      aes.Padding = PaddingMode.None;

      using var decryptor = aes.CreateDecryptor (key, iv);

      using var csDecrypt = new CryptoStream (new MemoryStream (encryptedText), decryptor, CryptoStreamMode.Read);

      csDecrypt.Read (encryptedText, 0, encryptedText.Length & 0x7ffffff0);

      string plainText = Encoding.ASCII.GetString (encryptedText.TakeWhile (b => b != 0).ToArray ());

      adb.json.Voucher voucher = adb.json.Voucher.Deserialize (plainText);

      license.voucher = voucher;
    }

    private async Task<string> callAudibleApiSignedForStringAsync (string relUrl, string jsonBody = null) {
      HttpRequestMessage request = makeSignedRequest (relUrl, jsonBody);
      return await sendForStringAsync (request, HttpClientAudible);
    }

    private async Task<byte[]> callAudibleApiSignedForBytesAsync (string relUrl, string jsonBody = null) {
      HttpRequestMessage request = makeSignedRequest (relUrl, jsonBody);
      return await sendForBytesAsync (request, HttpClientAudible);
    }

    private HttpRequestMessage makeSignedRequest (string relUrl, string jsonBody) {
      Uri relUri = new Uri (relUrl, UriKind.Relative);

      var method = jsonBody is null ? HttpMethod.Get : HttpMethod.Post;

      var request = new HttpRequestMessage (method, relUri);
      request.Headers.Add ("Accept", "application/json");

      if (jsonBody is not null) {
        HttpContent content = new StringContent (jsonBody, Encoding.UTF8, "application/json");
        request.Content = content;
      }

      signRequestAsync (request);
      return request;
    }

    private async Task<string> sendForStringAsync (HttpRequestMessage request, HttpClientEx httpClient) {
      string content = null;
      try {
        await request.LogAsync (4, this, httpClient.DefaultRequestHeaders, httpClient.CookieContainer, httpClient.BaseAddress);
        var response = await httpClient.SendAsync (request);
        await response.LogAsync (4, this, httpClient.CookieContainer, httpClient.BaseAddress);

        content = await response.Content.ReadAsStringAsync ();
        response.EnsureSuccessStatusCode ();

        return content;
      } catch (Exception exc) {
        Log (1, this, () => $"{exc.Summary ()}{Environment.NewLine}{content}");
        return null;
      }
    }

    private async Task<byte[]> sendForBytesAsync (HttpRequestMessage request, HttpClientEx httpClient) {
      HttpResponseMessage response = null;
      try {
        await request.LogAsync (4, this, httpClient.DefaultRequestHeaders, httpClient.CookieContainer, httpClient.BaseAddress);
        response = await httpClient.SendAsync (request);
        await response.LogAsync (4, this, httpClient.CookieContainer, httpClient.BaseAddress);

        response.EnsureSuccessStatusCode ();
        byte[] content = await response.Content.ReadAsByteArrayAsync ();

        return content;
      } catch (Exception exc) {
        string content = await response.Content.ReadAsStringAsync ();
        Log (1, this, () => $"{exc.Summary ()}{Environment.NewLine}{content}");
        return null;
      }

    }

    private void signRequestAsync (HttpRequestMessage request) {
      string signature = makeRequestSignatureAsync (request);

      request.Headers.Add ("x-adp-token", Profile.AdpToken);
      request.Headers.Add ("x-adp-alg", "SHA256withRSA:1.0");
      request.Headers.Add ("x-adp-signature", signature);
    }

    private string makeRequestSignatureAsync (HttpRequestMessage request) {
      // HACK 
      DateTime dt = DateTime.UtcNow.RoundDown (TimeSpan.FromMinutes (10));

      string method = request.Method.ToString ().ToUpper ();
      string url = request.RequestUri.OriginalString;
      string time = dt.ToXmlTime ();
      string content = request.Content?.ReadAsStringAsync ().Result;
      string adpToken = Profile.AdpToken;

      string dataString = $"{method}\n{url}\n{time}\n{content}\n{adpToken}";

      byte[] signBytes = sign (dataString);

      string encoded = Convert.ToBase64String (signBytes);
      var signature = $"{encoded}:{time}";

      return signature;
    }

    private byte[] sign (string dataString) {
      byte[] dataBytes = Encoding.UTF8.GetBytes (dataString);

      using SHA256 sha256Hash = SHA256.Create ();
      byte[] hashBytes = sha256Hash.ComputeHash (dataBytes);

      using RSA rsa = RSA.Create ();
      rsa.ImportFromPem (Profile.PrivateKey);

      byte[] signatureBytes = rsa.SignHash (hashBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

      return signatureBytes;
    }

    private static async Task<long> copyStreams (
      Conversion conversion,
      BufferedStream rdr,
      BufferedStream wrtr,
      Action<Conversion, long> progressAction,
      CancellationToken cancToken
    ) {
      const int BUF_SIZE = 16384;
      long accusize = 0;
      byte[] buffer = new byte[BUF_SIZE];

      while (true) {
        if (cancToken.IsCancellationRequested)
          return -1;
        int size = await rdr.ReadAsync (buffer, 0, BUF_SIZE, cancToken);
        if (size == 0)
          break;
        accusize += size;
        progressAction (conversion, accusize);
        await wrtr.WriteAsync (buffer, 0, size, cancToken);
      }

      return accusize;
    }

  }
}
