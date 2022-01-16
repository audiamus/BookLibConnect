using System.Diagnostics;

namespace core.audiamus.aux {
  public interface IProcessList {
    bool Add (Process process);
    bool Remove (Process process);
  }

  public interface IUserSettings {
  }

  public interface IInitSettings {
    void Init ();
  }
}
