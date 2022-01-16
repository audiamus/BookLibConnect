using System;
using System.Threading;
using core.audiamus.connect;
using core.audiamus.util;

namespace core.audiamus.connect.app.gui {
  class ConversionContext : ICancellation {
    public IProgress<ProgressMessage> Progress { get; }
    public CancellationToken CancellationToken { get; }

    public ConversionContext (IProgress<ProgressMessage> progress, CancellationToken token) {
      Progress = progress;
      CancellationToken = token;
    }

    public void Init (int nItems) {
      Progress?.Report (new (nItems, null, null, null));
    }
  }
}
