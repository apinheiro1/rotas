using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Rota
    {
  
        public string id { get; set; }
        public string origem { get; set; }
        public string destino { get; set; }
        public int valor { get; set; }
    }
   
}
