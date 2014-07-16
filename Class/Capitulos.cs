using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MR_metro.Class
{
    class Capitulos
    {
        [SQLite.PrimaryKey]
        public int Id { get; set; }
        public int IdManga { get; set; }
        public int Numero { get; set; }
        public string nome { get; set; }        
        public string Data { get; set; }
        public bool Dowloaded { get; set; }
        public bool Lido { get; set; }
        public string Endereco { get; set; }
    }
}
