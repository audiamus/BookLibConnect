using System;
using System.Management;

namespace core.audiamus.sysmgmt {
  /// <summary>
  /// http://jai-on-asp.blogspot.com/2010/03/finding-hardware-id-of-computer.html
  /// </summary>
  public static class HardwareId {
    public static string GetCpuId () {
      //ManagementObjectCollection mbsList = null;
      try {
        ManagementObjectSearcher mbs = new ManagementObjectSearcher ("Select * From Win32_processor");
        ManagementObjectCollection mbsList = mbs.Get ();
        string id = string.Empty;
        foreach (ManagementObject mo in mbsList) {
          id = mo["ProcessorID"].ToString ();
        }
        return id;
      } catch (Exception) {
        return string.Empty;
      }
    }

    public static string GetDiskId () {
      try { 
        ManagementObject dsk = new ManagementObject (@"win32_logicaldisk.deviceid=""c:""");
        dsk.Get ();
        string id = dsk["VolumeSerialNumber"].ToString ();
        return id;
      } catch (Exception) {
        return string.Empty;
      }
    }

    public static string GetMotherboardId () {
      return MotherboardInfo.SerialNumber;
    }

  }
}
