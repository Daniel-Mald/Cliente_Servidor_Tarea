using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMS.Models
{
    public class CartelParpadeante
    {
        public int Id { get; set; }
        public string Color { get; set; } = "";
        public string ColorClaro { get; set; } = "";
        public bool Parpadeando { get; set; } = false;
    }
}
