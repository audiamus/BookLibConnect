using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace core.audiamus.aux {
  public static class JsonSerialization {
    private static readonly JsonSerializerOptions __jsonSerializerOptions = new JsonSerializerOptions {
      WriteIndented = true,
      ReadCommentHandling = JsonCommentHandling.Skip,
      AllowTrailingCommas = true,
      Converters ={
        new JsonStringEnumConverter()
      },
      Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static string ToJsonString<T> (this T obj) {
      return JsonSerializer.Serialize<T> (obj, __jsonSerializerOptions);
    }

    public static void ToJsonFile<T> (this T obj, string path) {
      using var fs = new FileStream (path, FileMode.Create);
      var task = Task.Run (async () => await JsonSerializer.SerializeAsync (fs, obj, __jsonSerializerOptions));
      task.Wait ();
    }

    public static T FromJsonString<T> (this string json) {
      return JsonSerializer.Deserialize<T> (json, __jsonSerializerOptions);
    }

    public static T FromJsonFile<T> (this string path) {
      using var fs = new FileStream (path, FileMode.Open, FileAccess.Read);
      Task<T> task = Task.Run (async () => await JsonSerializer.DeserializeAsync<T> (fs, __jsonSerializerOptions));
      return task.Result;
    }

  }
}
