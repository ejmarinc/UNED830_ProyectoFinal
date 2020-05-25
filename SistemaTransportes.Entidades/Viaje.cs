using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTransportes.Entidades
{
    public class Viaje
    {
        public int ViajeID { set; get; }

        public string Identificacion { set; get; }
        public string LugarInicio { set; get; }
        public string LugarFinalizacion { set; get; }
        public string Carga { set; get; }
        public string TiempoEstimado { set; get; }
        public string FechaCreacion { set; get; }
        public string UbicacionActual { set; get; }
        public string Estado { set; get; }

    }
}
