using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.Enum
{
    public enum ResponseCode
    {
        unSuccess = 0,
        success = 1,
        SessionExpire = 2,
        exception = 99,
        ServerFailError = 90,
        ServerFailnullError = 91,
        ServerNullData = 92,
        ForcePinChanged = 5,
        AlreadyExist = -1,
        NotFound = -2,
    }

}
