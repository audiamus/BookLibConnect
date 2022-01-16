using System;
using System.Runtime.CompilerServices;

namespace core.audiamus.aux {

  public class LogGuard : IDisposable {

    const string IN = ">>> ";
    const string OUT = "<<< ";
    readonly uint _level;
    readonly Func<string> _func;
    readonly string _msg;
    readonly string _method;
    readonly object _caller;
    readonly Type _type;
    bool _isDispose;

    public LogGuard (uint level, Type type, Func<string> func, [CallerMemberName] string method = null) {
      _level = level;
      _type = type;
      _func = func;
      _method = method;
      Logging.Log (level, type, getFuncMsg, method);
    }

    public LogGuard (uint level, object caller, Func<string> func, [CallerMemberName] string method = null) {
      _level = level;
      _caller = caller;
      _func = func;
      _method = method;
      Logging.Log (level, caller, getFuncMsg, method);
    }

    public LogGuard (uint level, Type type, string msg, [CallerMemberName] string method = null) {
      _level = level;
      _type = type;
      _msg = msg;
      _method = method;
      Logging.Log (level, type, getMsg, method);
    }

    public LogGuard (uint level, object caller, string msg, [CallerMemberName] string method = null) {
      _level = level;
      _caller = caller;
      _msg = msg;
      _method = method;
      Logging.Log (level, caller, getMsg (), method);
    }

    public LogGuard (uint level, Type type, [CallerMemberName] string method = null) {
      _level = level;
      _type = type;
      _method = method;
      Logging.Log (level, type, () => IN, method);
    }

    public LogGuard (uint level, object caller, [CallerMemberName] string method = null) {
      _level = level;
      _caller = caller;
      _method = method;
      Logging.Log (level, caller, () => IN, method);
    }

    public void Dispose () {
      _isDispose = true;
      if (_type is null) {
        if (_func is null) {
          if (_msg is null)
            Logging.Log (_level, _caller, () => OUT, _method);
          else
            Logging.Log (_level, _caller, getMsg (), _method);
        } else
          Logging.Log (_level, _caller, getFuncMsg, _method);
      } else if (_type is not null) {
        if (_func is null) {
          if (_msg is null)
            Logging.Log (_level, _type, () => OUT, _method);
          else
            Logging.Log (_level, _type, getMsg (), _method);
        } else
          Logging.Log (_level, _type, getFuncMsg, _method);
      }
    }

    private string getFuncMsg () {
      string prefix = _isDispose ? OUT : IN;
      return prefix + _func?.Invoke ();
    }

    private string getMsg () {
      string prefix = _isDispose ? OUT : IN;
      return prefix + _msg;
    }
  }
}
