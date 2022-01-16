using System;
using core.audiamus.aux.diagn;
using core.audiamus.aux.ex;

namespace core.audiamus.util {
  public class ToStringConverterActivationCode : ToStringConverter {
    public override string ToString (object o, string format = null) {
      try {
        uint? ac = (uint?)o;
        return ac.HasValue ? "XXXXXXXX" : null;
      } catch ( Exception ) {
        return null;
      }
    }
  }

  public class ToStringConverterPath : ToStringConverter {
    public override string ToString (object o, string format = null) {
      if (o is string s)
        return s.SubstitUser ();
      else
        return null;
    }
  }
}
