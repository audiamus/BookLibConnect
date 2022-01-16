using System;
using core.audiamus.aux;
using core.audiamus.aux.ex;

namespace core.audiamus.connect.ui {
  class EnumConverterRM<TEnum> : EnumConverter<TEnum>
     where TEnum : struct, Enum {
    
    public EnumConverterRM () {
      ResourceManager = this.GetDefaultResourceManager ();
    }
  }
}
