using System;
using System.Collections.Generic;
using System.Text;

namespace U_Mod.Shared.Models
{
    public class BasicHttpResponse<T>
    {
        public bool Ok { get; set; }

        public string Message { get; set; } = "";

        public T Data { get; set; } = default(T);
    }
}
