using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MR_metro.Class
{
    class TipoGeneros
    {
        [SQLite.PrimaryKey]
        public int Id { get; set; }
        public string Genero { get; set; }
        public int Ordem { get; set; }
    }
}
