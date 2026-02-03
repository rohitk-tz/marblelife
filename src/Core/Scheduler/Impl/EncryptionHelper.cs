using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Core.Scheduler.Impl
{
    public static class EncryptionHelper
    {

        private const string Secret = "ˠˠˤ̡ʤʥʨʧʯʭʬͶͷζЉЊϹϸϸ³ÂÖÙîæíÊó¶djD̆ЊЋЌЖẂệՃև₧₯₡₹₱┐ ﬕﬗỮбб";

        static readonly string PasswordHash = "Fr0nT5tr3@mCRM";
        static readonly string SaltKey = "UH8Od2Xe";
        static readonly string VIKey = "9G0pthTOgZ2kJBc0";



        private static RijndaelManaged GetRijndaelManaged(string secretKey)
        {
            var keyBytes = new byte[16];
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            Array.Copy(secretKeyBytes, keyBytes, Math.Min(keyBytes.Length, secretKeyBytes.Length));
            return new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = 128,
                BlockSize = 128,
                Key = keyBytes,
                IV = keyBytes
            };
        }

        static byte[] Encrypt(byte[] plainBytes, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateEncryptor()
                .TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }
        static byte[] Decrypt(byte[] encryptedData, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateDecryptor()
                .TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        }

        public static String StringEncrypt(this String plainText)
        {
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(Encrypt(plainBytes, GetRijndaelManaged(Secret)));
        }

        [DebuggerStepThrough]
        public static String stringDecrypt(this String encryptedText)
        {
            var encryptedBytes = Convert.FromBase64String(encryptedText);
            var decrypt = Decrypt(encryptedBytes, GetRijndaelManaged(Secret));
            return Encoding.UTF8.GetString(decrypt);
        }

        public static String UrlEncrypt(this String plainText)
        {
            return System.Net.WebUtility.UrlEncode(plainText);
        }

        public static String UrlDecrypt(this String plainText)
        {
            return System.Net.WebUtility.UrlDecode(plainText);
        }


        public static string Encrypt(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }


        public static string Decrypt(string encryptedText)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }
        public static string AesDecryption(string privateKey, string publicKey, string data)
        {
            //privateKey= "MwIPT0u3vgKkuzpQGL3de2X03L4J0UaR";
            //publicKey =  "Gv4GpLSyDERIJ2==";
            //data = "aWa0/Lv8e1qfPrmWm+H0uIt/Gzi6DlSv5gOMj7XVfElPv/4C5pXWxQH39yDgtffoJ20oStQo1aHufmtPXwBYQdF99bKkRGGlqDk+jaG0uB4p0iaeFqqPHUV2dowxsrEm6VWX89S6BjwvC6wpQIMMtQnjjteqvUCg1aguF7aAJKkqc6y01Ro/4Ph6K6FUS1g2OX3o5gFG29WVcJTQ8yIG+e2ax+AT3aszHV11MU92HOF75HeYOVV89ZFKYinVg55D";


            byte[] encryptedData = Convert.FromBase64String(data);
            byte[] keyBytes = Encoding.ASCII.GetBytes(privateKey);
            //byte[] IVBytes = Convert.FromBase64String(publicKey);
            byte[] IVBytes = Encoding.ASCII.GetBytes(publicKey);
            var keySize = keyBytes.Length * 8;
            var blockSize = 128;//IVBytes.Length * 8;
            var symmetricKey = new RijndaelManaged()
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = keySize,
                BlockSize = blockSize,
                Key = keyBytes,
                IV = IVBytes
            };
            var decryptor = symmetricKey.CreateDecryptor();
            var text = Encoding.ASCII.GetString(decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length));
            symmetricKey.Clear();
            return text;
        }

        public static string AesEncryption(string privateKey, string publicKey, string data)
        {
            var DisablePadding = false;
            byte[] InputBytes = ASCIIEncoding.ASCII.GetBytes(data);

            byte[] KeyBytes = Encoding.ASCII.GetBytes(privateKey);
            byte[] IVBytes = Encoding.ASCII.GetBytes(publicKey);
            string encrypted = "";
            RijndaelManaged AES = new RijndaelManaged();
            AES.KeySize = (KeyBytes.Length * 8);
            AES.BlockSize = (IVBytes.Length * 8);
            AES.Mode = CipherMode.CBC;
            if (!DisablePadding)
            {
                AES.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
            }
            AES.Key = KeyBytes;
            AES.IV = IVBytes;
            ICryptoTransform AESEncrypter = AES.CreateEncryptor();
            encrypted = Convert.ToBase64String(AESEncrypter.TransformFinalBlock(InputBytes, 0, InputBytes.Length));

            AES.Clear();
            return encrypted;
        }

        public static string AesDecryptionFails(string privateKey, string publicKey, string data)
        {

            //byte[] encryptedData = Convert.FromBase64String(HttpUtility.UrlDecode(data));  - failed 
            byte[] encryptedData = Convert.FromBase64String(data);
            //var pk = HttpUtility.UrlDecode(publicKey); //failed 
            byte[] keyBytes = Encoding.ASCII.GetBytes(privateKey);
            byte[] IVBytes = Convert.FromBase64String(publicKey);
            var keySize = keyBytes.Length * 8;
            var blockSize = IVBytes.Length * 8;
            var symmetricKey = new RijndaelManaged()
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.None,
                KeySize = keySize,
                BlockSize = blockSize,
                Key = keyBytes,
                IV = IVBytes
            };
            var decryptor = symmetricKey.CreateDecryptor();
            var text = Encoding.UTF8.GetString(decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length));
            symmetricKey.Clear();
            return text;
        }

    }
}
