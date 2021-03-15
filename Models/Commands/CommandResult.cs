using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHorizon.Identity.AuthServer.Models.Commands
{
    public class CommandResult<T>
    {
        public bool Success { get; }
        public T Result { get; }
        public string ErrorCode { get; }

        public CommandResult(
            T result
        )
        {
            Success = true;
            Result = result;
            ErrorCode = string.Empty;
        }

        public CommandResult(
            bool success,
            T result,
            string errorCode = null
        )
        {
            Success = success;
            Result = result;
            ErrorCode = errorCode ?? string.Empty;
        }

        public CommandResult(
            string errorCode
        )
        {
            Success = false;
            ErrorCode = errorCode;
#pragma warning disable CS8601 // Possible null reference assignment.
            Result = default;
#pragma warning restore CS8601 // Possible null reference assignment.
        }
    }
}
