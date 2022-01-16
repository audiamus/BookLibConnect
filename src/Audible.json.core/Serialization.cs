using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using core.audiamus.aux;
using core.audiamus.aux.ex;

namespace core.audiamus.adb.json {
  public abstract class Serialization<T> {
    private static JsonSerializerOptions Options { get; } = new JsonSerializerOptions {
      WriteIndented = true,
      ReadCommentHandling = JsonCommentHandling.Skip,
      AllowTrailingCommas = true,
      Converters ={
        new JsonStringEnumConverter()
      },
      Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public string Serialize () {
      return JsonSerializer.Serialize (this, typeof(T), Options);
    }

    public static T Deserialize (string json) {
      try {
        return JsonSerializer.Deserialize<T> (json, Options);
      } catch (Exception exc) {
        Logging.Log (1, typeof (Serialization<T>), () => exc.Summary ());
        return default;
      }
    }
  }
}
