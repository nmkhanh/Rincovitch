using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RincovitchApp.Models.ModelChilds
{
    public class NMK_M_Return<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string Error { get; set; }
    }
    
}
