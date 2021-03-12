using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ManipuladorDeEvantos
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            WinHandler.WinHandler notepad = new WinHandler.WinHandler("Sem título", WinHandler.WinHandler.SearchType.ContainsTitle);
            notepad.Text = "meu manipulador de janelas";
            var campodoserialnumber = notepad.GetControl(0);
            campodoserialnumber.SendKey(Keys.Z);
            // notepad.Close();
        }
    }

}
