﻿using System;

namespace Aspree.Core
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string message, string property = "")
        : base(message)
        {
            if (!string.IsNullOrEmpty(property))
                this.Data.Add("ValidationError", property);
        }
       
    }

    public class NotFoundException : Exception
    {
        public NotFoundException(string message, string property = "")
        : base(message)
        {
            if (!string.IsNullOrEmpty(property))
                this.Data.Add("ValidationError", property);
        }

    }

    public class AlreadyExistsException : Exception
    {
        public AlreadyExistsException(string message, string property = "")
        : base(message)
        {
            if(!string.IsNullOrEmpty(property))
                this.Data.Add("ValidationError", property);
        }

    }

    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message, string property = "")
        : base(message)
        {
            if (!string.IsNullOrEmpty(property))
                this.Data.Add("ValidationError", property);
        }

    }
}
