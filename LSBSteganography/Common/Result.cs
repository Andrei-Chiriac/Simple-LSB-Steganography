using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSBSteganography.Common
{
    public class Result
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public static Result Ok(string message = "") => new Result() { Success = true, Message = message };

        public static Result Fail(string message = "") => new Result() { Message = message };

    }

    public class Result<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public T Value { get; set; }
    }
}
