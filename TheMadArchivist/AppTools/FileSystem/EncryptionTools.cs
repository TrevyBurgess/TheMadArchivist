using CyberFeedForward.TheMadArchivist.AppTools.InternalTools;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CyberFeedForward.TheMadArchivist.AppTools.FileSystem;

public static class EncryptionTools
{
    public static void EncryptFile(string inputFilePath, string outputFilePath, DataProtectionScope scope = DataProtectionScope.CurrentUser)
    {
        if (string.IsNullOrWhiteSpace(inputFilePath))
        {
            throw new ArgumentException("Input file path cannot be empty.", nameof(inputFilePath));
        }

        if (string.IsNullOrWhiteSpace(outputFilePath))
        {
            throw new ArgumentException("Output file path cannot be empty.", nameof(outputFilePath));
        }

        if (!File.Exists(inputFilePath))
        {
            throw new FileNotFoundException("Input file does not exist.", inputFilePath);
        }

        var outputDirectory = Path.GetDirectoryName(outputFilePath);
        if (!string.IsNullOrWhiteSpace(outputDirectory) && !Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        var contentKey = RandomNumberGenerator.GetBytes(32);
        var iv = RandomNumberGenerator.GetBytes(16);
        var protectedKey = ProtectedData.Protect(contentKey, optionalEntropy: null, scope);

        var headerMagic = Encoding.ASCII.GetBytes("MAE1");

        using var outputStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 1024, FileOptions.SequentialScan);
        using var hmac = new HMACSHA256(contentKey);
        using var hmacStream = new HmacWriteStream(outputStream, hmac);

        hmacStream.Write(headerMagic, 0, headerMagic.Length);
        WriteInt32(hmacStream, protectedKey.Length);
        hmacStream.Write(protectedKey, 0, protectedKey.Length);
        WriteInt32(hmacStream, iv.Length);
        hmacStream.Write(iv, 0, iv.Length);

        using var aes = Aes.Create();
        aes.KeySize = 256;
        aes.Key = contentKey;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using (var cryptoStream = new CryptoStream(hmacStream, aes.CreateEncryptor(), CryptoStreamMode.Write, leaveOpen: true))
        using (var inputStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, 1024 * 1024, FileOptions.SequentialScan))
        {
            inputStream.CopyTo(cryptoStream, 1024 * 1024);
            cryptoStream.FlushFinalBlock();
        }

        hmacStream.FinalizeHash();
    }

    private static void WriteInt32(HmacWriteStream stream, int value)
    {
        Span<byte> bytes = stackalloc byte[4];
        BitConverter.TryWriteBytes(bytes, value);
        stream.Write(bytes);
    }

    public static void DecryptFile(string inputFilePath, string outputFilePath, DataProtectionScope scope = DataProtectionScope.CurrentUser)
    {
        if (string.IsNullOrWhiteSpace(inputFilePath))
        {
            throw new ArgumentException("Input file path cannot be empty.", nameof(inputFilePath));
        }

        if (string.IsNullOrWhiteSpace(outputFilePath))
        {
            throw new ArgumentException("Output file path cannot be empty.", nameof(outputFilePath));
        }

        if (!File.Exists(inputFilePath))
        {
            throw new FileNotFoundException("Input file does not exist.", inputFilePath);
        }

        var outputDirectory = Path.GetDirectoryName(outputFilePath);
        if (!string.IsNullOrWhiteSpace(outputDirectory) && !Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        using var inputStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, 1024 * 1024, FileOptions.SequentialScan);
        if (inputStream.Length < 4 + 4 + 16 + 32)
        {
            throw new CryptographicException("Encrypted file is too small or corrupted.");
        }

        var expectedHmac = ReadTrailingBytes(inputStream, 32);
        inputStream.Position = 0;

        var magic = ReadExactly(inputStream, 4);
        if (!magic.AsSpan().SequenceEqual(Encoding.ASCII.GetBytes("MAE1")))
        {
            throw new CryptographicException("Invalid encrypted file header.");
        }

        var protectedKeyLength = ReadInt32(inputStream);
        if (protectedKeyLength <= 0 || protectedKeyLength > 1024 * 1024)
        {
            throw new CryptographicException("Invalid protected key length.");
        }

        var protectedKey = ReadExactly(inputStream, protectedKeyLength);
        var ivLength = ReadInt32(inputStream);
        if (ivLength <= 0 || ivLength > 1024)
        {
            throw new CryptographicException("Invalid IV length.");
        }

        var iv = ReadExactly(inputStream, ivLength);
        var contentKey = ProtectedData.Unprotect(protectedKey, optionalEntropy: null, scope);

        if (contentKey.Length != 32)
        {
            throw new CryptographicException("Invalid content key.");
        }

        using var hmac = new HMACSHA256(contentKey);

        inputStream.Position = 0;
        var hmacBytesToRead = inputStream.Length - 32;
        ComputeHmacOverPrefix(inputStream, hmac, hmacBytesToRead);

        if (hmac.Hash is null)
        {
            throw new CryptographicException("HMAC hash was not computed.");
        }

        if (!CryptographicOperations.FixedTimeEquals(hmac.Hash, expectedHmac))
        {
            throw new CryptographicException("Encrypted file integrity check failed.");
        }

        inputStream.Position = 0;

        _ = ReadExactly(inputStream, 4);
        _ = ReadExactly(inputStream, 4);
        _ = ReadExactly(inputStream, protectedKeyLength);
        _ = ReadExactly(inputStream, 4);
        _ = ReadExactly(inputStream, ivLength);

        var cipherLength = inputStream.Length - 32 - inputStream.Position;
        if (cipherLength < 0)
        {
            throw new CryptographicException("Encrypted file is corrupted.");
        }

        using var limitedCipherStream = new LimitedReadStream(inputStream, cipherLength);

        using var aes = Aes.Create();
        aes.KeySize = 256;
        aes.Key = contentKey;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var cryptoStream = new CryptoStream(limitedCipherStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using var outputStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 1024, FileOptions.SequentialScan);
        cryptoStream.CopyTo(outputStream, 1024 * 1024);
    }

    private static void ComputeHmacOverPrefix(FileStream stream, HMAC hmac, long bytesToRead)
    {
        var buffer = new byte[1024 * 1024];
        long remaining = bytesToRead;

        while (remaining > 0)
        {
            var read = stream.Read(buffer, 0, (int)Math.Min(buffer.Length, remaining));
            if (read <= 0)
            {
                throw new CryptographicException("Unexpected end of file while computing HMAC.");
            }

            remaining -= read;
            hmac.TransformBlock(buffer, 0, read, null, 0);
        }

        hmac.TransformFinalBlock([], 0, 0);
    }

    private static byte[] ReadTrailingBytes(FileStream stream, int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        var buffer = new byte[count];
        stream.Position = stream.Length - count;
        var readTotal = 0;

        while (readTotal < count)
        {
            var read = stream.Read(buffer, readTotal, count - readTotal);
            if (read <= 0)
            {
                throw new EndOfStreamException();
            }

            readTotal += read;
        }

        return buffer;
    }

    private static byte[] ReadExactly(FileStream stream, int count)
    {
        var buffer = new byte[count];
        var totalRead = 0;

        while (totalRead < count)
        {
            var read = stream.Read(buffer, totalRead, count - totalRead);
            if (read <= 0)
            {
                throw new EndOfStreamException();
            }

            totalRead += read;
        }

        return buffer;
    }

    private static int ReadInt32(FileStream stream)
    {
        Span<byte> buffer = stackalloc byte[4];
        var totalRead = 0;
        while (totalRead < buffer.Length)
        {
            var read = stream.Read(buffer[totalRead..]);
            if (read <= 0)
            {
                throw new EndOfStreamException();
            }

            totalRead += read;
        }

        return BitConverter.ToInt32(buffer);
    }

}
