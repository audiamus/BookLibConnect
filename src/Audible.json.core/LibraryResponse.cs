using System;
using System.Text.Json.Serialization;

namespace core.audiamus.adb.json {

  public interface IPerson {
    string asin { get; set; }
    string name { get; set; }
  }

  public class LibraryResponse : Serialization<LibraryResponse> {
    public Product[] items { get; set; }
    public string[] response_groups { get; set; }
  }

  public class ProductResponse : Serialization<ProductResponse> {
    public Product product { get; set; }
    public string[] response_groups { get; set; }
  }

  public class SimsBySeriesResponse : Serialization<SimsBySeriesResponse> {
    public Product[] similar_products { get; set; }
    public string[] response_groups { get; set; }
  }

  public class Product {
    public string asin { get; set; }
    public string audible_editors_summary { get; set; }
    public Author[] authors { get; set; }
    public Codec[] available_codecs { get; set; }
    public Category[] category_ladders { get; set; }
    public string content_delivery_type { get; set; }
    public Content_Rating content_rating { get; set; }
    public string content_type { get; set; }
    public Customer_Reviews[] customer_reviews { get; set; }
    public string[] editorial_reviews { get; set; }
    public string format_type { get; set; }
    public bool? has_children { get; set; }
    public bool? is_adult_product { get; set; }
    public bool? is_ayce { get; set; }
    public bool? is_downloaded { get; set; }
    public bool? is_listenable { get; set; }
    public bool? is_pdf_url_available { get; set; }
    public bool? is_pending { get; set; }
    public bool? is_playable { get; set; }
    public bool? is_preorderable { get; set; }
    public bool? is_purchasability_suppressed { get; set; }
    public bool? is_removable { get; set; }
    public bool? is_removable_by_parent { get; set; }
    public bool? is_returnable { get; set; }
    public bool? is_searchable { get; set; }
    public bool? is_visible { get; set; }
    public bool? is_world_rights { get; set; }
    public bool? is_ws4v_companion_asin_owned { get; set; }
    public bool? is_ws4v_enabled { get; set; }
    public bool? isbn { get; set; }
    public DateTime? issue_date { get; set; }
    public string language { get; set; }
    public Library_Status library_status { get; set; }
    public string merchandising_summary { get; set; }
    public object music_id { get; set; }
    public Narrator[] narrators { get; set; }
    public string origin_asin { get; set; }
    public string origin_id { get; set; }
    public string origin_marketplace { get; set; }
    public string origin_type { get; set; }
    public string pdf_url { get; set; }
    public float? percent_complete { get; set; }
    public Plan[] plans { get; set; }
    public Product_Images product_images { get; set; }
    public string publication_name { get; set; }
    public string publisher_name { get; set; }
    public string publisher_summary { get; set; }
    public DateTime purchase_date { get; set; }
    public Rating rating { get; set; }
    public Relationship[] relationships { get; set; }
    public DateTime? release_date { get; set; }
    public int? runtime_length_min { get; set; }
    public string sample_url { get; set; }
    public Series[] series { get; set; }
    public string sku { get; set; }
    public string sku_lite { get; set; }
    public Social_Media_Images social_media_images { get; set; }
    public string status { get; set; }
    public string subtitle { get; set; }
    public string[] thesaurus_subject_keywords { get; set; }
    public string title { get; set; }
    public string voice_description { get; set; }
  }

  public class Content_Rating {
    public string steaminess { get; set; }
  }

  public class Library_Status {
    public DateTime date_added { get; set; }
    public bool? is_pending { get; set; }
    public bool? is_preordered { get; set; }
    public bool? is_removable { get; set; }
    public bool? is_visible { get; set; }
  }

  public class Product_Images {
    [JsonPropertyName ("500")]
    public string _500 { get; set; }
  }

  public class Rating {
    public int? num_reviews { get; set; }
    public Distribution overall_distribution { get; set; }
    public Distribution performance_distribution { get; set; }
    public Distribution story_distribution { get; set; }
  }

  public class Distribution {
    public float? average_rating { get; set; }
    public string display_average_rating { get; set; }
    public float? display_stars { get; set; }
    public int? num_five_star_ratings { get; set; }
    public int? num_four_star_ratings { get; set; }
    public int? num_one_star_ratings { get; set; }
    public int? num_ratings { get; set; }
    public int? num_three_star_ratings { get; set; }
    public int? num_two_star_ratings { get; set; }
  }

  public class Social_Media_Images {
    public string facebook { get; set; }
    public string twitter { get; set; }
  }

  public class Author : IPerson {
    public string asin { get; set; }
    public string name { get; set; }
  }

  public class Codec {
    public string enhanced_codec { get; set; }
    public string format { get; set; }
    public bool? is_kindle_enhanced { get; set; }
    public string name { get; set; }
  }

  public class Category {
    public Ladder[] ladder { get; set; }
    public string root { get; set; }
  }

  public class Ladder {
    public string id { get; set; }
    public string name { get; set; }
  }

  public class Customer_Reviews {
    public string asin { get; set; }
    public string author_id { get; set; }
    public string author_name { get; set; }
    public string body { get; set; }
    public string format { get; set; }
    public Guided_Responses[] guided_responses { get; set; }
    public string id { get; set; }
    public string location { get; set; }
    public Ratings ratings { get; set; }
    public Review_Content_Scores review_content_scores { get; set; }
    public DateTime? submission_date { get; set; }
    public string title { get; set; }
  }

  public class Ratings {
    public int? overall_rating { get; set; }
    public int? performance_rating { get; set; }
    public int? story_rating { get; set; }
  }

  public class Review_Content_Scores {
    public int? content_quality { get; set; }
    public int? num_helpful_votes { get; set; }
    public int? num_unhelpful_votes { get; set; }
  }

  public class Guided_Responses {
    public string answer { get; set; }
    public string id { get; set; }
    public string question { get; set; }
    public string question_type { get; set; }
  }

  public class Narrator : IPerson {
    public string asin { get; set; }
    public string name { get; set; }
  }

  public class Plan {
    public DateTime? end_date { get; set; }
    public string plan_name { get; set; }
    public DateTime? start_date { get; set; }
  }

  public class Relationship {
    public string asin { get; set; }
    public string content_delivery_type { get; set; }
    public string relationship_to_product { get; set; }
    public string relationship_type { get; set; }
    public string sequence { get; set; }
    public string sku { get; set; }
    public string sku_lite { get; set; }
    public string sort { get; set; }
    public string title { get; set; }
    public string url { get; set; }
  }

  public class Series {
    public string asin { get; set; }
    public string sequence { get; set; }
    public string title { get; set; }
    public string url { get; set; }
  }

}