using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace MR_metro.Class
{
    public class ImgPage
    {
        public string endCap { get; set; }
        public string NomeM { get; set; }
        public string NumeroCap { get; set; }
        public int Npaginas { get; set; }
        public List<Imagens> lstImg { get; set; }

    }

    public class Imagens
    {
        public string endPag { get; set; }
        public string endImg { get; set; }
        public int nmPag { get; set; }
        public BitmapImage Imagem { get; set; }
    }
}
