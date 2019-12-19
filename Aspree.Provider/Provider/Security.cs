using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspree.Provider.Interface;
using System.Security.Cryptography;
using System.IO;

namespace Aspree.Provider.Provider
{
   public class Security : ISecurity
    {
        public string ComputeHash(String input, String salt)
        {
            string encode = salt + input;
            ASCIIEncoding ae = new ASCIIEncoding();
            byte[] hashValue, messageBytes = ae.GetBytes(encode);
            SHA1Managed sha1Hash = new SHA1Managed();
            string strHex = "";

            hashValue = sha1Hash.ComputeHash(messageBytes);
            foreach (byte b in hashValue)
            {
                strHex += String.Format("{0:x2}", b);
            }

            return strHex;
        }
        public String GenerateNewPassword()
        {
            String randomStringLower;
            String randomStringUpper;
            String newPassword = String.Empty;
            Int32 index1 = 0;
            Int32 index2 = 0;
            Int32 index3 = 0;

            randomStringLower = Guid.NewGuid().ToString().Replace("-", String.Empty).ToLower();
            randomStringUpper = Guid.NewGuid().ToString().Replace("-", String.Empty).ToUpper();

            while (newPassword.Length < 6)
            {
                for (Int32 index = index1; index < randomStringLower.Length; index++)
                {
                    if (Char.IsDigit(randomStringLower, index))
                    {
                        newPassword = String.Format("{0}{1}", newPassword, randomStringLower.Substring(index, 1));
                        index1 = index + 1;
                        break;
                    }
                }

                for (Int32 index = index2; index < randomStringLower.Length; index++)
                {
                    if (Char.IsLower(randomStringLower, index))
                    {
                        newPassword = String.Format("{0}{1}", newPassword, randomStringLower.Substring(index, 1));
                        index2 = index + 1;
                        break;
                    }
                }

                for (Int32 index = index3; index < randomStringUpper.Length; index++)
                {
                    if (Char.IsUpper(randomStringUpper, index))
                    {
                        newPassword = String.Format("{0}{1}", newPassword, randomStringUpper.Substring(index, 1));
                        index3 = index + 1;
                        break;
                    }
                }
            }

            return newPassword;
        }


        // <summary>  
        // Encrypts a string          
        // </summary>        
        // <param name="CipherText">Text to be Encrypted</param>         
        // <param name="Password">Password to Encrypt with</param>         
        // <param name="Salt">Salt to Encrypt with</param>          
        // <param name="HashAlgorithm">Can be either SHA1 or MD5</param>         
        // <param name="PasswordIterations">Number of iterations to do</param>          
        // <param name="KeySize">Can be 128, 192, or 256</param>          
        // <returns>A decrypted string</returns>       
        public string AesEncrypt(string plainText, string password, string salt, string hashAlgorithm = "SHA1", int passwordIterations = 2, int keySize = 256)
        {
            string initialVector = salt.Substring(0, 16);

            if (string.IsNullOrEmpty(plainText))
            {
                return "The Text to be Decryped by AES must not be null...";
            }
            else if (string.IsNullOrEmpty(password))
            {
                return "The Password for AES Decryption must not be null...";
            }
            byte[] initialVectorBytes = Encoding.ASCII.GetBytes(initialVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(salt);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            PasswordDeriveBytes derivedPassword = new PasswordDeriveBytes(password, saltValueBytes, hashAlgorithm, passwordIterations);
            byte[] keyBytes = derivedPassword.GetBytes(keySize / 8);

            RijndaelManaged symmetricKey = new RijndaelManaged { Mode = CipherMode.CBC };

            byte[] cipherTextBytes = null;

            using (ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initialVectorBytes))
            {

                using (MemoryStream memStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                        cryptoStream.FlushFinalBlock();
                        cipherTextBytes = memStream.ToArray();
                        memStream.Close();
                        cryptoStream.Close();
                    }
                }
            }
            symmetricKey.Clear();
            return Convert.ToBase64String(cipherTextBytes);
        }

        // <summary>  
        // Decrypts a string          
        // </summary>        
        // <param name="CipherText">Text to be decrypted</param>         
        // <param name="Password">Password to decrypt with</param>         
        // <param name="Salt">Salt to decrypt with</param>          
        // <param name="HashAlgorithm">Can be either SHA1 or MD5</param>         
        // <param name="PasswordIterations">Number of iterations to do</param>          
        // <param name="KeySize">Can be 128, 192, or 256</param>
        // <returns>A decrypted string</returns>
        public string AesDecrypt(string cipherText, string password, string salt, string hashAlgorithm = "SHA1", int passwordIterations = 2, int keySize = 256)
        {
            string initialVector = salt.Substring(0, 16);

            if (string.IsNullOrEmpty(cipherText))
            {
                return "The Text to be Decryped by AES must not be null...";
            }
            else if (string.IsNullOrEmpty(password))
            {
                return "The Password for AES Decryption must not be null...";
            }
            byte[] initialVectorBytes = Encoding.ASCII.GetBytes(initialVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(salt);
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            PasswordDeriveBytes derivedPassword = new PasswordDeriveBytes(password, saltValueBytes, hashAlgorithm, passwordIterations);
            byte[] keyBytes = derivedPassword.GetBytes(keySize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int byteCount = 0;
            try
            {

                using (ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initialVectorBytes))
                {
                    using (MemoryStream memStream = new MemoryStream(cipherTextBytes))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read))
                        {
                            byteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                            memStream.Close();
                            cryptoStream.Close();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return "Please Enter the Correct Password and Salt..." + "The Following Error Occured: " + "/n" + e;
            }
            symmetricKey.Clear();
            return Encoding.UTF8.GetString(plainTextBytes, 0, byteCount);

        }

    }
}
