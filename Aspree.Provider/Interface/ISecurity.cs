using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Provider.Interface
{
    public interface ISecurity
    {
        String ComputeHash(String password, String salt);
        String GenerateNewPassword();
        string AesEncrypt(string plainText, string password, string salt, string hashAlgorithm = "SHA1", int passwordIterations = 2, int keySize = 256);
        string AesDecrypt(string cipherText, string password, string salt, string hashAlgorithm = "SHA1", int passwordIterations = 2, int keySize = 256);

    }
}
