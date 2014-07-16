using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MR_metro.Class
{
    class Mangas
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Id { get; set; }
        public int IdFonte { get; set; }
        public string Titulo { get; set; }
        public string Nome { get; set; }
        public string ANome { get; set; }
        public string Ano { get; set; }
        public string Status { get; set; }
        public string Autor { get; set; }
        public string Artista { get; set; }
        public string EndImagem { get; set; }
        public string DirecaoLeitura { get; set; }
        public string Descricao1 { get; set; }
        public string Descricao2 { get; set; }
        public string Generos { get; set; }
        public string Endereco { get; set; }
    }
}
