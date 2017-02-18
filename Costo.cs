using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conecta4
{
    public class Costo : IComparable<Costo>
    {
        public int columna;
        public Espacio espacio;
        public double costo;
        public double heuristica;
        public double costoTotal
        {
            get
            {
                return costo + heuristica;
            }
        }

        public Costo(int columna, Espacio espacio, double costo, double heuristica)
        {
            this.columna = columna;
            this.espacio = espacio;
            this.costo = costo;
            this.heuristica = heuristica;
        }

        public int CompareTo(Costo other)
        {
            return costoTotal.CompareTo(other.costoTotal);
        }
    }
}
