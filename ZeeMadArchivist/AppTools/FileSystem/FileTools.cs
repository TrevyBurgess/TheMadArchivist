using System;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CyberFeedForward.TheMadArchivist.AppTools.FileSystem;

public static class FileTools
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

    public static void SaveIcon(Icon icon, string filePath)
    {
        ArgumentNullException.ThrowIfNull(icon);

        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be empty.", nameof(filePath));
        }

        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        icon.Save(fileStream);
    }

    public static bool IsIdentical(string filePath1, string filePath2)
    {
        if (string.IsNullOrWhiteSpace(filePath1) || string.IsNullOrWhiteSpace(filePath2))
        {
            return false;
        }

        if (string.Equals(filePath1, filePath2, StringComparison.OrdinalIgnoreCase))
        {
            return File.Exists(filePath1);
        }

        var fileInfo1 = new FileInfo(filePath1);
        var fileInfo2 = new FileInfo(filePath2);

        if (!fileInfo1.Exists || !fileInfo2.Exists)
        {
            return false;
        }

        if (fileInfo1.Length != fileInfo2.Length)
        {
            return false;
        }

        const int bufferSize = 1024 * 1024;

        using var stream1 = new FileStream(filePath1, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, FileOptions.SequentialScan);
        using var stream2 = new FileStream(filePath2, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, FileOptions.SequentialScan);

        var buffer1 = new byte[bufferSize];
        var buffer2 = new byte[bufferSize];

        while (true)
        {
            var read1 = stream1.Read(buffer1, 0, buffer1.Length);
            var read2 = stream2.Read(buffer2, 0, buffer2.Length);

            if (read1 != read2)
            {
                return false;
            }

            if (read1 == 0)
            {
                return true;
            }

            if (!buffer1.AsSpan(0, read1).SequenceEqual(buffer2.AsSpan(0, read2)))
            {
                return false;
            }
        }
    }

    private static void WriteInt32(Stream stream, int value)
    {
        Span<byte> bytes = stackalloc byte[4];
        BitConverter.TryWriteBytes(bytes, value);
        stream.Write(bytes);
    }

    private sealed class HmacWriteStream : Stream
    {
        private readonly Stream _inner;
        private readonly HMAC _hmac;
        private bool _hashFinalized;

        public HmacWriteStream(Stream inner, HMAC hmac)
        {
            _inner = inner;
            _hmac = hmac;
        }

        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => _inner.Length;
        public override long Position
        {
            get => _inner.Position;
            set => throw new NotSupportedException();
        }

        public override void Flush() => _inner.Flush();

        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        public override void SetLength(long value) => _inner.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_hashFinalized)
            {
                throw new InvalidOperationException("Cannot write after hash has been finalized.");
            }

            _hmac.TransformBlock(buffer, offset, count, null, 0);
            _inner.Write(buffer, offset, count);
        }

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            if (_hashFinalized)
            {
                throw new InvalidOperationException("Cannot write after hash has been finalized.");
            }

            if (buffer.Length == 0)
            {
                return;
            }

            var temp = buffer.ToArray();
            _hmac.TransformBlock(temp, 0, temp.Length, null, 0);
            _inner.Write(buffer);
        }

        public void FinalizeHash()
        {
            if (_hashFinalized)
            {
                return;
            }

            _hashFinalized = true;
            _hmac.TransformFinalBlock(Array.Empty<byte>(), 0, 0);

            if (_hmac.Hash is null)
            {
                throw new CryptographicException("HMAC hash was not computed.");
            }

            _inner.Write(_hmac.Hash, 0, _hmac.Hash.Length);
            _inner.Flush();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_hashFinalized)
            {
                _hmac.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
            }

            base.Dispose(disposing);
        }
    }

}
