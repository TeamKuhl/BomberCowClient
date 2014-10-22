using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BomberCowClient
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BomberMap BomberMap = new BomberMap(this);
            BomberMap.createMap("0:0:0:0:0:0:0:0;0:1:2:2:2:1:2:0;0:2:1:0:0:2:1:0;0:1:2:1:1:1:2:0;0:2:1:1:2:1:1:0;0:1:2:0:0:2:1:0;0:1:1:1:1:2:1:0;0:0:0:0:0:0:0:0");
        }
    }
}
