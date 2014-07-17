using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MR_metro.Temp
{
    class JsonClass
    {
        public string name { get; set; }
        public List<Placemark> Placemark { get; set; }
    }
    public class LookAt
    {
        public string longitude { get; set; }
        public string latitude { get; set; }
        public string altitude { get; set; }
        public string heading { get; set; }
        public string tilt { get; set; }
        public string range { get; set; }
        public string altitudeMode { get; set; }
    }

    public class Point
    {
        public string coordinates { get; set; }
        public string altitudeMode { get; set; }
    }

    public class Placemark
    {
        public string name { get; set; }
        public LookAt LookAt { get; set; }
        public string styleUrl { get; set; }
        public Point Point { get; set; }
    }

}
