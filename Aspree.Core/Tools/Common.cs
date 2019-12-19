using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.Tools
{
    public class Common
    {
        public static bool IsValidPassword(string password)
        {
            HashSet<char> specialCharacters = new HashSet<char>() { '%', '$', '#', '@' };
            if (password.Any(char.IsLower) && //Lower case 
                 password.Any(char.IsUpper) &&
                 password.Any(char.IsDigit) &&
                 password.Any(specialCharacters.Contains))
            {
                return true;
            }

            return false;
        }
    }
}
