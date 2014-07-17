using Parse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MR_metro.Parse
{
    [ParseClassName("Pluviometro")]
    class Pluviometro : ParseObject
    {
        [ParseFieldName("nomeLocal")]
        public string nomeLocal
        {
            get { return GetProperty<string>(); }
            set { SetProperty<string>(value); }
        }

        [ParseFieldName("emdereco")]
        public string endereco
        {
            get { return GetProperty<string>(); }
            set { SetProperty<string>(value); }
        }

        [ParseFieldName("idEstacao")]
        public int idEstacao
        {
            get { return GetProperty<int>(); }
            set { SetProperty<int>(value); }
        }

        [ParseFieldName("latLng")]
        public ParseGeoPoint latLng
        {
            get { return GetProperty<ParseGeoPoint>(); }
            set { SetProperty<ParseGeoPoint>(value); }
        }

        [ParseFieldName("raio")]
        public int raio
        {
            get { return GetProperty<int>(); }
            set { SetProperty<int>(value); }
        }
    }
}
