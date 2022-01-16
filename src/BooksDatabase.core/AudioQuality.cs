using System.Collections.Generic;
using System.Linq;
using core.audiamus.aux.ex;
using core.audiamus.common;

namespace core.audiamus.booksdb {
  public record AudioQuality (int? SampleRate, int? BitRate) : IAudioQuality;

  namespace ex {
    public static class ExCodec {

      public static AudioQuality ToQuality (this Codec codec) => codec.Name.ToQuality ();

      public static AudioQuality ToQuality(this ECodec codec) {
        string name = codec.ToString ();
        if (!name.StartsWith ("aax"))
          return default;

        var parts = name.SplitTrim ('_');
        if (parts.Length < 3)
          return default;

        var qual = new AudioQuality (int.Parse (parts[1]), int.Parse (parts[2]));
        qual = qual with { 
          SampleRate = qual.SampleRate * 22050 / 22
        };

        return qual;
      }

      public static AudioQuality MaxQuality (this IEnumerable<Codec> codecs) {
        var aaxCodecs = codecs
          .Select (c => c.ToQuality())
          .Where (c => c is not null)
          .ToList ();
        if (!aaxCodecs.Any ())
          return default;

        var max = aaxCodecs.Aggregate ((a, s) => {
          if (a is null)
            return s;

          if (s.SampleRate >= a.SampleRate && s.BitRate >= a.BitRate)
            return s;
          else
            return a;
        });

        return max;
      }
    }
  }
}
