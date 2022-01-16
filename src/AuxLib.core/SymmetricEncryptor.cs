using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace core.audiamus.aux {
  // https://tomrucki.com/posts/aes-encryption-in-csharp/
  // possibly way over the top for some applications. 

  public static class SymmetricEncryptor {

    private const int AES_BLOCK_BYTE_SIZE = 128 / 8;

    private const int PASSWORD_SALT_BYTE_SIZE = 128 / 8;
    private const int PASSWORD_BYTE_SIZE = 256 / 8;
    private const int PASSWORD_ITERATION_COUNT = 100_000;

    private const int SIGNATURE_BYTE_SIZE = 256 / 8;

    private const int MINIMUM_ENCRYPTED_MESSAGE_BYTE_SIZE =
        PASSWORD_SALT_BYTE_SIZE + // auth salt
        PASSWORD_SALT_BYTE_SIZE + // key salt
        AES_BLOCK_BYTE_SIZE + // IV
        AES_BLOCK_BYTE_SIZE + // cipher text min length
        SIGNATURE_BYTE_SIZE; // signature tag

    private static readonly Encoding _stringEncoding = Encoding.UTF8;
    private static readonly RandomNumberGenerator _random = RandomNumberGenerator.Create ();

    public static byte[] EncryptString (string toEncrypt, string password) {
      using (var aes = Aes.Create ()) {
        // encrypt
        var keySalt = generateRandomBytes (PASSWORD_SALT_BYTE_SIZE);
        var key = getKey (password, keySalt);
        var iv = generateRandomBytes (AES_BLOCK_BYTE_SIZE);

        byte[] cipherText;
        using (var encryptor = aes.CreateEncryptor (key, iv)) {
          var plainText = _stringEncoding.GetBytes (toEncrypt);
          cipherText = encryptor
              .TransformFinalBlock (plainText, 0, plainText.Length);
        }

        // sign
        var authKeySalt = generateRandomBytes (PASSWORD_SALT_BYTE_SIZE);
        var authKey = getKey (password, authKeySalt);
        var result = mergeArrays (
            additionalCapacity: SIGNATURE_BYTE_SIZE,
            authKeySalt, keySalt, iv, cipherText);

        using (var hmac = new HMACSHA256 (authKey)) {
          var payloadToSignLength = result.Length - SIGNATURE_BYTE_SIZE;
          var signatureTag = hmac.ComputeHash (result, 0, payloadToSignLength);
          signatureTag.CopyTo (result, payloadToSignLength);
        }

        return result;
      }
    }

    public static string DecryptToString (byte[] encryptedData, string password) {
      if (encryptedData is null
          || encryptedData.Length < MINIMUM_ENCRYPTED_MESSAGE_BYTE_SIZE) {
        throw new ArgumentException ("Invalid length of encrypted data");
      }

      var authKeySalt = encryptedData
          .AsSpan (0, PASSWORD_SALT_BYTE_SIZE).ToArray ();
      var keySalt = encryptedData
          .AsSpan (PASSWORD_SALT_BYTE_SIZE, PASSWORD_SALT_BYTE_SIZE).ToArray ();
      var iv = encryptedData
          .AsSpan (2 * PASSWORD_SALT_BYTE_SIZE, AES_BLOCK_BYTE_SIZE).ToArray ();
      var signatureTag = encryptedData
          .AsSpan (encryptedData.Length - SIGNATURE_BYTE_SIZE, SIGNATURE_BYTE_SIZE).ToArray ();

      var cipherTextIndex = authKeySalt.Length + keySalt.Length + iv.Length;
      var cipherTextLength =
          encryptedData.Length - cipherTextIndex - signatureTag.Length;

      var authKey = getKey (password, authKeySalt);
      var key = getKey (password, keySalt);

      // verify signature
      using (var hmac = new HMACSHA256 (authKey)) {
        var payloadToSignLength = encryptedData.Length - SIGNATURE_BYTE_SIZE;
        var signatureTagExpected = hmac
            .ComputeHash (encryptedData, 0, payloadToSignLength);

        // constant time checking to prevent timing attacks
        var signatureVerificationResult = 0;
        for (int i = 0; i < signatureTag.Length; i++) {
          signatureVerificationResult |= signatureTag[i] ^ signatureTagExpected[i];
        }

        if (signatureVerificationResult != 0) {
          throw new CryptographicException ("Invalid signature");
        }
      }

      // decrypt
      using (var aes = Aes.Create ()) {
        using (var encryptor = aes.CreateDecryptor (key, iv)) {
          var decryptedBytes = encryptor
              .TransformFinalBlock (encryptedData, cipherTextIndex, cipherTextLength);
          return _stringEncoding.GetString (decryptedBytes);
        }
      }
    }

    private static byte[] getKey (string password, byte[] passwordSalt) {
      var keyBytes = _stringEncoding.GetBytes (password);

      using (var derivator = new Rfc2898DeriveBytes (
          keyBytes, passwordSalt,
          PASSWORD_ITERATION_COUNT, HashAlgorithmName.SHA256)) {
        return derivator.GetBytes (PASSWORD_BYTE_SIZE);
      }
    }

    private static byte[] generateRandomBytes (int numberOfBytes) {
      var randomBytes = new byte[numberOfBytes];
      _random.GetBytes (randomBytes);
      return randomBytes;
    }

    private static byte[] mergeArrays (int additionalCapacity = 0, params byte[][] arrays) {
      var merged = new byte[arrays.Sum (a => a.Length) + additionalCapacity];
      var mergeIndex = 0;
      for (int i = 0; i < arrays.GetLength (0); i++) {
        arrays[i].CopyTo (merged, mergeIndex);
        mergeIndex += arrays[i].Length;
      }

      return merged;
    }
  }
}