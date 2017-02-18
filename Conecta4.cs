using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Conecta4
{
    public class Conecta4
    {
        private Form1 form;
        private Thread thread;
        private Agente agente;

        public bool juegoTerminado = false;

        public int filas = 6;
        public int columnas = 7;

        public List<List<Espacio>> espacios = new List<List<Espacio>>();

        public enum Turno
        {
            Jugador = 1,
            Agente = 2
        }

        public Turno turno = Turno.Jugador;

        public Conecta4(Form1 form)
        {
            this.form = form;
            espacios = new List<List<Espacio>>();
            int id = 0;
            for(int i = 0; i < filas; i++)
            {
                List<Espacio> e = new List<Espacio>();
                for(int j = 0; j < columnas; j++)
                {
                    e.Add(new Espacio(id));
                    id++;
                }
                espacios.Add(e);
            }
            
        }

        public void empezar()
        {
            turno = Turno.Jugador;
            asyncdraw();
            agente = new Agente(this);
            thread = new Thread(agente.iniciar);
            thread.Start();
        }

        public void checar()
        {
            Espacio.Estado estado = Espacio.Estado.Jugador;
            int contador = 0;
            List<Espacio> con = new List<Espacio>();
            //Checar horizontalmente
            for(int i = 0; i < filas; i++)
            {
                contador = 0;
                con.Clear();
                for(int x = 0; x < columnas; x++)
                {
                    if (espacios[i][x].estado == estado)
                    {
                        con.Add(espacios[i][x]);
                        contador++;
                    }else
                    {
                        contador = 0;
                        con.Clear();
                        if(espacios[i][x].estado != Espacio.Estado.Libre)
                        {
                            estado = espacios[i][x].estado;
                            con.Add(espacios[i][x]);
                            contador++;
                        }
                    }
                    if(contador >= 4)
                    {
                        terminarJuego(con);
                    }
                }
            }
            //Checar verticalmente
            contador = 0;
            for (int i = 0; i < columnas; i++)
            {
                contador = 0;
                con.Clear();
                for (int x = 0; x < filas; x++)
                {
                    if (espacios[x][i].estado == estado)
                    {
                        con.Add(espacios[x][i]);
                        contador++;
                    }
                    else
                    {
                        contador = 0;
                        con.Clear();
                        if (espacios[x][i].estado != Espacio.Estado.Libre)
                        {
                            estado = espacios[x][i].estado;
                            con.Add(espacios[x][i]);
                            contador++;
                        }
                    }
                    if (contador >= 4)
                    {
                        terminarJuego(con);
                    }
                }
            }

            //Checar diagonalmente a la derecha
            contador = 0;
            for(int i = 0; i < filas * columnas; i++)
            {
                contador = 0;
                con.Clear();
                Espacio e = getEspacioById(i);
                int p = i;
                while(p % columnas != 6 && p < filas*columnas)
                {
                    e = getEspacioById(p);
                    if (e.estado == estado)
                    {
                        con.Add(e);
                        contador++;
                    }
                    else
                    {
                        con.Clear();
                        contador = 0;
                        if (e.estado != Espacio.Estado.Libre)
                        {
                            estado = e.estado;
                            con.Add(e);
                            contador++;
                        }
                    }
                    if (contador >= 4)
                    {
                        terminarJuego(con);
                    }
                    p += 8;
                }
            }

            //Checar diagonalmente a la izquierda
            contador = 0;
            for (int i = 0; i < filas * columnas; i++)
            {
                contador = 0;
                con.Clear();
                Espacio e = getEspacioById(i);
                int p = i;
                while (p % columnas != 0 && p < filas * columnas)
                {
                    e = getEspacioById(p);
                    if (e.estado == estado)
                    {
                        con.Add(e);
                        contador++;
                    }
                    else
                    {
                        contador = 0;
                        con.Clear();
                        if (e.estado != Espacio.Estado.Libre)
                        {
                            estado = e.estado;
                            con.Add(e);
                            contador++;
                        }
                    }
                    if (contador >= 4)
                    {
                        terminarJuego(con);
                    }
                    p += 6;
                }
            }
        }

        public void terminarJuego(List<Espacio> espaciosG = null)
        {
            if (espaciosG != null)
            {
                foreach (Espacio e in espaciosG)
                {
                    e.espacioGanador = true;
                }
            }
            juegoTerminado = true;
            bool noLibre = true;
            foreach (List<Espacio> fila in espacios)
            {
                foreach (Espacio e in fila)
                {
                    if (e.estado == Espacio.Estado.Libre)
                    {
                        noLibre = false;
                        break;
                    }
                }
            }
            string ganador = noLibre ? "Empate" : turno == Turno.Jugador ? "Gano Jugador" : "Gano Agente";
           
            MessageBox.Show($"Juego terminado\n{ganador}");
        }

        private Pen getColor(Espacio e)
        {
            if (e.espacioGanador)
            {
                return new Pen(Color.Yellow, 20);
            }
            if(e.estado == Espacio.Estado.Jugador)
            {
                return new Pen(Color.Red, 20);
            }else if(e.estado == Espacio.Estado.Agente)
            {
                return new Pen(Color.Blue, 20);
            }
            return new Pen(Color.White, 20);
        }

        public void asyncdraw()
        {
            Thread d = new Thread(draw);
            d.Start();
        }

        private void draw()
        {
            while (form.IsDisposed)
            {
                Thread.Sleep(200);
            }
            bool trydraw = false;
            do
            {
                try
                {
                    attemptDraw();
                    trydraw = true;
                }
                catch (Exception e)
                {

                }
            } while (!trydraw);
        }

        private void attemptDraw()
        {
            form.Invoke((MethodInvoker)delegate
            {
                Graphics g = form.CreateGraphics();
                g.Clear(form.BackColor);
                int x = 200, y = 400;
                foreach (List<Espacio> fila in espacios)
                {
                    x = 200;
                    foreach (Espacio e in fila)
                    {
                        g.DrawRectangle(new Pen(Color.Orange, 20), x, y, 25, 25);
                        g.DrawEllipse(getColor(e), x, y, 20, 20);
                        g.DrawString(e.id.ToString(), new Font(FontFamily.GenericSansSerif, 6), Brushes.Black, x, y);
                        x += 45;
                    }
                    y -= 45;
                }
                g.Dispose();
            });
        }

        public bool posibleMovimiento(int columna)
        {
            for (int i = 0; i < filas; i++)
            {
                if(espacios[i][columna].estado == Espacio.Estado.Libre)
                {
                    return true;
                }
            }
            return false;
        }

        private Turno getTurnoFromId(int id)
        {
            if(id == 1)
            {
                return Turno.Jugador;
            }
            return Turno.Agente;
        }

        public void movimiento(int columna, int jugador)
        {
            if(turno != getTurnoFromId(jugador))
            {
                return;
            }
            for (int i = 0; i < filas; i++)
            {
                if (espacios[i][columna].estado == Espacio.Estado.Libre)
                {
                    espacios[i][columna].estado = jugador == 1 ? Espacio.Estado.Jugador : Espacio.Estado.Agente;
                    asyncdraw();
                    checar();
                    form.Invoke((MethodInvoker)delegate
                    {
                        form.checarOpciones();
                    });
                    turno = jugador == 1 ? Turno.Agente : Turno.Jugador;
                    return;
                }
            }
        }

        public Espacio getEspacioLibre(int columna)
        {
            for (int i = 0; i < filas; i++)
            {
                if (espacios[i][columna].estado == Espacio.Estado.Libre)
                {
                    return espacios[i][columna];
                }
            }
            return null;
        }

        public Espacio getEspacioById(int id)
        {
            for (int i = 0; i < filas; i++)
            {
                for (int j = 0; j < columnas; j++)
                {
                    if(espacios[i][j].id == id)
                    {
                        return espacios[i][j];
                    }
                }
            }
            return null;
        }

        public List<Espacio> getEspaciosContiguos(Espacio espacio)
        {
            List<Espacio> espacios = new List<Espacio>();
            int i = espacio.id;
            //Espacio a la izquierda
            if(i % columnas != 0)
            {
                espacios.Add(getEspacioById(i - 1));
            }
            //Espacio a la derecha
            if(i % columnas != columnas - 1)
            {
                espacios.Add(getEspacioById(i + 1));
            }
            //Espacio abajo
            if(i >= 7)
            {
                espacios.Add(getEspacioById(i - columnas));
                //Espacio a la izquierda
                if (i % columnas != 0)
                {
                    espacios.Add(getEspacioById(i - columnas - 1));
                }
                //Espacio a la derecha
                if (i % columnas != columnas - 1)
                {
                    espacios.Add(getEspacioById(i - columnas + 1));
                }
            }
            //Espacio arriba
            if(i < columnas * filas - columnas)
            {
                espacios.Add(getEspacioById(i + columnas));
                //Espacio a la izquierda
                if (i % columnas != 0)
                {
                    espacios.Add(getEspacioById(i + columnas - 1));
                }
                //Espacio a la derecha
                if (i % columnas != columnas - 1)
                {
                    espacios.Add(getEspacioById(i + columnas + 1));
                }
            }
            return espacios;
        }

    }
}
