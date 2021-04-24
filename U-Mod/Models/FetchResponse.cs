using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace U_Mod.Models
{
    public class FetchResponse<T>
    {
        public bool Ok { get; set; }
        public string ErrorMessage { get; set; }
        public T Data { get; set; }
    }
}
