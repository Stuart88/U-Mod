using System;
using System.Collections.Generic;
using System.Text;

namespace U_Mod.Shared.Models.ApiModels
{
    public class ApiResponse<T>
    {
        public ApiResponse() { } // JSON deserialise requires parameterless contructor

        public ApiResponse(bool ok, string message, T data)
        {
            Ok = ok;
            Message = message;
            Data = data;
        }
        public bool Ok { get; set; }

        public string Message { get; set; } = "";

        public T Data { get; set; } = default;
    }

    public class ApiResponse
    {
        public ApiResponse() { } // JSON deserialise requires parameterless contructor

        public ApiResponse(bool ok, string message)
        {
            Ok = ok;
            Message = message;
        }

        public bool Ok { get; set; }

        public string Message { get; set; } = "";

    }
}
