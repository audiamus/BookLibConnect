namespace core.audiamus.adb.json {

  public class LicenseResponse : Serialization<LicenseResponse> {
    public ContentLicense content_license { get; set; }
    public string[] response_groups { get; set; }
  }

  public class MetadataContainer : Serialization<MetadataContainer> {
    public ContentMetadata content_metadata { get; set; }
  }

  public partial class ContentLicense {
    public string acr { get; set; }
    public string asin { get; set; }
    public ContentMetadata content_metadata { get; set; }
    public string drm_type { get; set; }
    public string license_id { get; set; }
    public string license_response { get; set; }
    public string message { get; set; }
    public string request_id { get; set; }
    public bool requires_ad_supported_playback { get; set; }
    public string status_code { get; set; }
    public string voucher_id { get; set; }

    public DecryptedLicense decrypted_license_response { get; set; }
  }

  public class ContentMetadata {
    public ChapterInfo chapter_info { get; set; }
    public ContentReference content_reference { get; set; }
    public ContentUrl content_url { get; set; }
    public LastPositionHeard last_position_heard { get; set; }
  }

  public class ChapterInfo {
    public int brandIntroDurationMs { get; set; }
    public int brandOutroDurationMs { get; set; }
    public Chapter[] chapters { get; set; }
    public bool is_accurate { get; set; }
    public int runtime_length_ms { get; set; }
    public int runtime_length_sec { get; set; }
  }

  public class Chapter {
    public int length_ms { get; set; }
    public int start_offset_ms { get; set; }
    public int start_offset_sec { get; set; }
    public string title { get; set; }
  }

  public class ContentReference {
    public string acr { get; set; }
    public string asin { get; set; }
    public string content_format { get; set; }
    public long? content_size_in_bytes { get; set; }
    public string file_version { get; set; }
    public string marketplace { get; set; }
    public string sku { get; set; }
    public string tempo { get; set; }
    public string version { get; set; }
  }

  public class ContentUrl {
    public string offline_url { get; set; }
  }

  public class LastPositionHeard {
    public string last_updated { get; set; }
    public int position_ms { get; set; }
    public string status { get; set; }
  }
}
