using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conecta4
{
    public class Espacio : IEquatable<Espacio>
    {
        public enum Estado
        {
            Libre,
            Jugador,
            Agente
        }

        public int id;

        public Estado estado;

        public bool espacioGanador = false;

        public Espacio(int id)
        {
            estado = Estado.Libre;
            this.id = id;
        }

        public void set(int jugador)
        {
            estado = jugador == 1 ? Estado.Jugador : Estado.Agente;
        }

        public bool Equals(Espacio other)
        {
            return id == other.id;
        }
    }
}
