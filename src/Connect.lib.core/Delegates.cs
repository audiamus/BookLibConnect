using System;
using core.audiamus.booksdb;

namespace core.audiamus.connect {
  public delegate void ConvertDelegate<T> (Book book, T context, Action<Conversion> onNewStateCallback) where T : ICancellation;

  public delegate string ConfigTokenDelegate (bool enforce = false);
}
