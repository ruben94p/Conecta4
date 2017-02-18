using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Conecta4
{
    public partial class Form1 : Form
    {
        private Conecta4 conecta4;
        Button[] but;

        public Form1()
        {
            InitializeComponent();
            but = new Button[]{ button1, button2, button3, button4, button5, button6, button7 };
            conecta4 = new Conecta4(this);
            conecta4.empezar();
        }

        private int getColumn(object sender)
        {
            for(int i = 0; i < but.Length; i++)
            {
                if (sender.Equals(but[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        private void button_Click(object sender, EventArgs e)
        {
            int col = getColumn(sender);
            conecta4.movimiento(col, 1);
            
        }

        public void checarOpciones()
        {
            bool e = true;
            for (int i = 0; i < conecta4.columnas; i++)
            {
                if (!conecta4.posibleMovimiento(i))
                {
                    but[i].Enabled = false;
                }else
                {
                    e = false;
                }
            }
            if (e)
            {
                conecta4.terminarJuego();
            }
        }
    }
}
