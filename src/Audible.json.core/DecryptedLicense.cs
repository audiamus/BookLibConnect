using System;

namespace core.audiamus.adb.json {

  public class DecryptedLicense : Serialization<DecryptedLicense> {
    public string key { get; set; }
    public string iv { get; set; }
    public Rule[] rules { get; set; }
  }

  public class Rule {
    public Parameter[] parameters { get; set; }
    public string name { get; set; }
  }

  public class Parameter {
    public DateTime expireDate { get; set; }
    public string type { get; set; }
  }

}
