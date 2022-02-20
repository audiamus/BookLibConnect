using System;
using core.audiamus.booksdb;

namespace core.audiamus.connect {
  public delegate void ConvertDelegate<T> (Book book, T context, Action<Conversion> onNewStateCallback) where T : ICancellation;

  delegate ConfigurationTokenResult ConfigTokenDelegate (bool enforce = false);
}
