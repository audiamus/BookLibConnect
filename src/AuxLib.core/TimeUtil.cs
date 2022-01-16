using System;

namespace core.audiamus.aux {
  public static class TimeUtil {
    private readonly static DateTime EPOCH = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

    private static double dateTimeToDouble (DateTime dt) {
      if (dt == default) 
        return 0;

      TimeSpan ts = dt.Subtract (EPOCH);
      return ts.TotalSeconds;
    }

    public static int DateTimeToUnix32 (DateTime dt) => (int)dateTimeToDouble (dt);

    public static long DateTimeToUnix64 (DateTime dt) => (long)dateTimeToDouble (dt);

    public static long DateTimeToUnix64Msec (DateTime dt) {
      if (dt == default)
        return 0;

      TimeSpan ts = dt.Subtract (EPOCH);
      return (long)ts.TotalMilliseconds;
    }

    public static DateTime UnixToDateTime (long timestamp) {
      if (timestamp == 0)
        return default;

      DateTime dt = EPOCH.AddSeconds (timestamp);
      return dt;
    }

    public static DateTime UnixMsecToDateTime (long timestampMsec) {
      if (timestampMsec == 0)
        return default;

      DateTime dt = EPOCH.AddMilliseconds (timestampMsec);
      return dt;
    }

    public static int ToUnix32 (this DateTime dt) => DateTimeToUnix32 (dt);

    public static long ToUnix64 (this DateTime dt) => DateTimeToUnix64 (dt);

    public static long ToUnix64Msec (this DateTime dt) => DateTimeToUnix64Msec (dt);

    /// <summary>Replace time kind without changing value.</summary>
    /// <param name="dt">The DateTime value to be reinterpreted.</param>
    /// <param name="kind">The new DateTime kind.</param>
    /// <returns>Reinterpreted DateTime value.</returns>
    public static DateTime As (this DateTime dt, DateTimeKind kind) => new(dt.Ticks, kind);

    public static DateTime ToDateTimeFromUnix (this int timval) => UnixToDateTime (timval);

    public static DateTime ToDateTimeFromUnix (this long timval) => UnixToDateTime (timval);

    public static DateTime ToDateTimeFromUnixMsec (this long timvalMsec) => UnixMsecToDateTime (timvalMsec);

  }
}

