using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Conecta4
{
    public class Agente
    {
        private Conecta4 conecta4;

        public Agente(Conecta4 conecta4)
        {
            this.conecta4 = conecta4;
        }

        private int contarCol(int i)
        {
            int contador = 0;
            for (int x = 0; x < conecta4.filas; x++)
            {
                if (conecta4.espacios[x][i].estado == Espacio.Estado.Jugador)
                { 
                    contador++;
                }
            }
            return contador;
        }

        private int contarFilas(int i)
        {
            int contador = 0;
            for (int x = 0; x < conecta4.filas; x++)
            {
                if (conecta4.espacios[i][x].estado == Espacio.Estado.Jugador)
                {
                    contador++;
                }
            }
            return contador;
        }

        public void iniciar()
        {
            while (!conecta4.juegoTerminado)
            {

                while(conecta4.turno == Conecta4.Turno.Jugador)
                {
                    Thread.Sleep(500);
                }
                if (conecta4.juegoTerminado)
                {
                    return;
                }
                //Heuristica y costo
                //Determinar que columnas estan disponibles
                List<int> columnas = new List<int>(new int[]{ 0,1,2,3,4,5,6});
                for(int i = 0; i < columnas.Count; i++)
                {
                    if (!conecta4.posibleMovimiento(columnas[i]))
                    {
                        columnas[i] = -1;
                    }
                }
                columnas.RemoveAll(i => i == -1);
                List<Costo> costos = new List<Costo>();
                //Aplicar función de costo a los espacios disponibles
                foreach(int c in columnas) {
                    Espacio e = conecta4.getEspacioLibre(c);
                    List<Espacio> con = conecta4.getEspaciosContiguos(e);
                    double cos = 0;
                    foreach(Espacio v in con)
                    {
                        if(v.estado == Espacio.Estado.Libre)
                        {
                            cos += 1;
                        }
                        else if(v.estado == Espacio.Estado.Agente)
                        {
                            cos += 0.5;
                        }
                        else {
                            cos += 0;
                        }
                    }
                    //Console.WriteLine($"Columna {c} Costo sin promediar {cos}");
                    cos /= con.Count;
                    //cos = Costo
                    //h = Heuristica
                    //Heuristica nueva: Cantidad de fichas del otro jugador por fila y columna y se pone negativa
                    double h = contarCol(e.id % 7) + contarFilas((int)Math.Floor((double)e.id / 7));
                    Costo costo = new Costo(c,e,cos,-h);
                    costos.Add(costo);
                }
                costos.Sort();
                //Debug
                foreach(Costo c in costos)
                {
                    Console.WriteLine($"Columna {c.columna} Costo {c.costo}, Heuristica {c.heuristica}, Total {c.costoTotal}");
                }

                if (costos.Count > 0)
                {
                    conecta4.movimiento(costos[0].columna, 2);
                }else
                {
                    if (conecta4.juegoTerminado)
                    {
                        return;
                    }
                    //Error
                    conecta4.terminarJuego();
                }
                
                //conecta4.turno = Conecta4.Turno.Jugador;
            }
        }
        
    }
}
