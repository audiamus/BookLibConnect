using System;

namespace core.audiamus.util {
  public abstract class ThreadProgressBase<T> : IDisposable {

    private readonly Action<T> _report;

    private int _accuValuePerMax;

    protected abstract int Max { get; }

    protected ThreadProgressBase (Action<T> report) {
      _report = report;
    }

    public void Dispose () {
      int inc = Max - _accuValuePerMax;
      if (inc > 0)
        _report?.Invoke (getProgressMessage(inc));
    }

    public void Report (double value) {
      int val = (int)(value * Max);
      int total = Math.Min (Max, val);
      int inc = total - _accuValuePerMax;
      _accuValuePerMax = total;
      if (inc > 0)
        _report?.Invoke (getProgressMessage (inc));
    }

    protected abstract T getProgressMessage (int inc);
  }

  public class ThreadProgressPerMille : ThreadProgressBase<ProgressMessage> {
    protected override int Max => 1000;

    public ThreadProgressPerMille (Action<ProgressMessage> report) : base (report) { }

    protected override ProgressMessage getProgressMessage (int inc) => new(null, null, null, inc);
  }

  public class ThreadProgressPerCent : ThreadProgressBase<ProgressMessage> {
    protected override int Max => 100;

    public ThreadProgressPerCent (Action<ProgressMessage> report) : base (report) { }

    protected override ProgressMessage getProgressMessage (int inc) => new(null, null, inc, null);
  }
}
