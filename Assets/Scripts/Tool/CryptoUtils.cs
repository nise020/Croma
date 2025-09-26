using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class CryptoUtils
{
    private static readonly byte[] aesKey = Encoding.UTF8.GetBytes("GameDestroyer504"); // 16바이트
    private static readonly byte[] aesIV = Encoding.UTF8.GetBytes("Dontcomeinhere25"); // 16바이트

    public static string EncryptToBase64(byte[] plainData)
    {
        byte[] encryptedAndHashed = EncryptAndHash(plainData);
        return Convert.ToBase64String(encryptedAndHashed);
    }

    public static byte[] DecryptFromBase64(string base64Data)
    {
        byte[] dataWithHash = Convert.FromBase64String(base64Data);
        return DecryptAndVerify(dataWithHash);
    }

    public static byte[] EncryptAndHash(byte[] plainData)
    {
        using var aes = Aes.Create();
        aes.Key = aesKey;
        aes.IV = aesIV;

        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            cs.Write(plainData, 0, plainData.Length);

        byte[] encryptedData = ms.ToArray();
        byte[] hash = SHA256.Create().ComputeHash(encryptedData);

        return Combine(encryptedData, hash);
    }

    public static byte[] DecryptAndVerify(byte[] dataWithHash)
    {
        int hashSize = 32;
        int dataSize = dataWithHash.Length - hashSize;

        byte[] encryptedData = new byte[dataSize];
        byte[] storedHash = new byte[hashSize];
        Array.Copy(dataWithHash, 0, encryptedData, 0, dataSize);
        Array.Copy(dataWithHash, dataSize, storedHash, 0, hashSize);

        byte[] computedHash = SHA256.Create().ComputeHash(encryptedData);
        if (!CompareHashes(storedHash, computedHash))
            throw new Exception("파일이 변조되었거나 손상되었습니다.");

        using var aes = Aes.Create();
        aes.Key = aesKey;
        aes.IV = aesIV;

        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
            cs.Write(encryptedData, 0, encryptedData.Length);

        return ms.ToArray();
    }

    private static byte[] Combine(byte[] a, byte[] b)
    {
        byte[] result = new byte[a.Length + b.Length];
        Buffer.BlockCopy(a, 0, result, 0, a.Length);
        Buffer.BlockCopy(b, 0, result, a.Length, b.Length);
        return result;
    }

    private static bool CompareHashes(byte[] a, byte[] b)
    {
        if (a.Length != b.Length) return false;
        for (int i = 0; i < a.Length; i++)
            if (a[i] != b[i]) return false;
        return true;
    }
}
