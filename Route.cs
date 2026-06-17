using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP_KiprasRudzinskas
{
    public class Route
    {
        public int BusId { get; set; }
        public List<int> Path { get; set; } = new();
        public double Cost { get; set; }
    }
}
