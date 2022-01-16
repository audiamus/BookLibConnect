using System;
using System.Collections.Generic;
using core.audiamus.booksdb;

namespace core.audiamus.connect.ui {
  public class BookEventArgs : EventArgs {
    public IEnumerable<Book> Books { get; }

    public BookEventArgs (IEnumerable<Book> books) => Books = books;
  }

  public delegate void BookSelectionChangedEventHandler (object sender, BookEventArgs args);

  public class ConversionEventArgs : EventArgs {
    public Conversion Conversion { get; }

    public ConversionEventArgs (Conversion conversion) => Conversion = conversion;
  }

  public delegate void ConversionUpdatedEventHandler (object sender, ConversionEventArgs args);

  public class BoolEventArgs : EventArgs {
    public bool Value { get; }

    public BoolEventArgs (bool value) => Value = value;
  }

  public delegate void BoolEventHandler (object sender, BoolEventArgs args);
  
}
