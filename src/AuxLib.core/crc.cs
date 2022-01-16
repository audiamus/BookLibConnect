using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace core.audiamus.aux {
  /// <summary>
  /// Implements a 32-bit CRC hash algorithm compatible with Zip etc.
  /// https://github.com/damieng/DamienGKit/tree/master/CSharp/DamienG.Library/Security/Cryptography
  /// </summary>
  /// <remarks>
  /// Crc32 should only be used for backward compatibility with older file formats
  /// and algorithms. It is not secure enough for new applications.
  /// If you need to call multiple times for the same data either use the HashAlgorithm
  /// interface or remember that the result of one Compute call needs to be ~ (XOR) before
  /// being passed in as the seed for the next Compute call.
  /// </remarks>
  public sealed class Crc32 : HashAlgorithm {
    public const UInt32 DefaultPolynomial = 0xedb88320u;
    public const UInt32 DefaultSeed = 0xffffffffu;

    static UInt32[] defaultTable;

    readonly UInt32 seed;
    readonly UInt32[] table;
    UInt32 hash;

    public Crc32 ()
        : this (DefaultPolynomial, DefaultSeed) {
    }

    public Crc32 (UInt32 polynomial, UInt32 seed) {
      if (!BitConverter.IsLittleEndian)
        throw new PlatformNotSupportedException ("Not supported on Big Endian processors");

      table = InitializeTable (polynomial);
      this.seed = hash = seed;
    }

    public override void Initialize () {
      hash = seed;
    }

    protected override void HashCore (byte[] array, int ibStart, int cbSize) {
      hash = CalculateHash (table, hash, array, ibStart, cbSize);
    }

    protected override byte[] HashFinal () {
      var hashBuffer = UInt32ToBigEndianBytes (~hash);
      HashValue = hashBuffer;
      return hashBuffer;
    }

    public override int HashSize { get { return 32; } }

    public static UInt32 Compute (byte[] buffer) {
      return Compute (DefaultSeed, buffer);
    }

    public static UInt32 Compute (UInt32 seed, byte[] buffer) {
      return Compute (DefaultPolynomial, seed, buffer);
    }

    public static UInt32 Compute (UInt32 polynomial, UInt32 seed, byte[] buffer) {
      return ~CalculateHash (InitializeTable (polynomial), seed, buffer, 0, buffer.Length);
    }

    static UInt32[] InitializeTable (UInt32 polynomial) {
      if (polynomial == DefaultPolynomial && defaultTable != null)
        return defaultTable;

      var createTable = new UInt32[256];
      for (var i = 0; i < 256; i++) {
        var entry = (UInt32)i;
        for (var j = 0; j < 8; j++)
          if ((entry & 1) == 1)
            entry = (entry >> 1) ^ polynomial;
          else
            entry >>= 1;
        createTable[i] = entry;
      }

      if (polynomial == DefaultPolynomial)
        defaultTable = createTable;

      return createTable;
    }

    static UInt32 CalculateHash (UInt32[] table, UInt32 seed, IList<byte> buffer, int start, int size) {
      var hash = seed;
      for (var i = start; i < start + size; i++)
        hash = (hash >> 8) ^ table[buffer[i] ^ hash & 0xff];
      return hash;
    }

    static byte[] UInt32ToBigEndianBytes (UInt32 uint32) {
      var result = BitConverter.GetBytes (uint32);

      if (BitConverter.IsLittleEndian)
        Array.Reverse (result);

      return result;
    }
  }

  /// <summary>
  /// Implements a 64-bit CRC hash algorithm for a given polynomial.
  /// https://github.com/damieng/DamienGKit/tree/master/CSharp/DamienG.Library/Security/Cryptography
  /// </summary>
  /// <remarks>
  /// For ISO 3309 compliant 64-bit CRC's use Crc64Iso.
  /// </remarks>
  public class Crc64 : HashAlgorithm {
    public const UInt64 DefaultSeed = 0x0;

    readonly UInt64[] table;

    readonly UInt64 seed;
    UInt64 hash;

    public Crc64 (UInt64 polynomial)
        : this (polynomial, DefaultSeed) {
    }

    public Crc64 (UInt64 polynomial, UInt64 seed) {
      if (!BitConverter.IsLittleEndian)
        throw new PlatformNotSupportedException ("Not supported on Big Endian processors");

      table = InitializeTable (polynomial);
      this.seed = hash = seed;
    }

    public override void Initialize () {
      hash = seed;
    }

    protected override void HashCore (byte[] array, int ibStart, int cbSize) {
      hash = CalculateHash (hash, table, array, ibStart, cbSize);
    }

    protected override byte[] HashFinal () {
      var hashBuffer = UInt64ToBigEndianBytes (hash);
      HashValue = hashBuffer;
      return hashBuffer;
    }

    public override int HashSize { get { return 64; } }

    protected static UInt64 CalculateHash (UInt64 seed, UInt64[] table, IList<byte> buffer, int start, int size) {
      var hash = seed;
      for (var i = start; i < start + size; i++)
        unchecked {
          hash = (hash >> 8) ^ table[(buffer[i] ^ hash) & 0xff];
        }
      return hash;
    }

    static byte[] UInt64ToBigEndianBytes (UInt64 value) {
      var result = BitConverter.GetBytes (value);

      if (BitConverter.IsLittleEndian)
        Array.Reverse (result);

      return result;
    }

    static UInt64[] InitializeTable (UInt64 polynomial) {
      if (polynomial == Crc64Iso.Iso3309Polynomial && Crc64Iso.Table != null)
        return Crc64Iso.Table;

      var createTable = CreateTable (polynomial);

      if (polynomial == Crc64Iso.Iso3309Polynomial)
        Crc64Iso.Table = createTable;

      return createTable;
    }

    protected static ulong[] CreateTable (ulong polynomial) {
      var createTable = new UInt64[256];
      for (var i = 0; i < 256; ++i) {
        var entry = (UInt64)i;
        for (var j = 0; j < 8; ++j)
          if ((entry & 1) == 1)
            entry = (entry >> 1) ^ polynomial;
          else
            entry >>= 1;
        createTable[i] = entry;
      }
      return createTable;
    }
  }

  public class Crc64Iso : Crc64 {
    internal static UInt64[] Table;

    public const UInt64 Iso3309Polynomial = 0xD800000000000000;

    public Crc64Iso ()
        : base (Iso3309Polynomial) {
    }

    public Crc64Iso (UInt64 seed)
        : base (Iso3309Polynomial, seed) {
    }

    public static UInt64 Compute (byte[] buffer) {
      return Compute (DefaultSeed, buffer);
    }

    public static UInt64 Compute (UInt64 seed, byte[] buffer) {
      if (Table == null)
        Table = CreateTable (Iso3309Polynomial);

      return CalculateHash (seed, Table, buffer, 0, buffer.Length);
    }
  }

  namespace ex {
    public static class Crc32Extensions {
      public static uint Checksum32 (this byte[] bytes) {
        return Crc32.Compute (bytes);
      }

      public static uint Checksum32 (this string text) =>
        text.GetBytes ().Checksum32 ();

      public static byte[] Checksum32Bytes (this byte[] bytes) =>
        BitConverter.GetBytes (bytes.Checksum32 ());
    }

    public static class Crc64Extensions {
      public static ulong Checksum64 (this byte[] bytes) {
        return Crc64Iso.Compute (bytes);
      }

      public static ulong Checksum64 (this string text) =>
        text.GetBytes ().Checksum64 ();

      public static byte[] Checksum64Bytes (this byte[] bytes) =>
        BitConverter.GetBytes (bytes.Checksum64 ());
    }

  }
}
