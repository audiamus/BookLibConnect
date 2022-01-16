using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.audiamus.util {
  public enum EOnlineUpdate {
    no,
    promptForDownload,
    promptForInstall
  }

  public enum EUpdateInteract {
    newVersAvail,
    installNow,
    installLater,
  }
}
