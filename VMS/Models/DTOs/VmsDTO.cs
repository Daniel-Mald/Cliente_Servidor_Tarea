using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMS.Models.DTOs
{
    public class VmsDTO
    {
        public string Contenido { get; set; } = "";
        public string Color { get; set; } = "";
        public int CartelId { get; set; }
        public string Remitente { get; set; } = "";
    }
}
