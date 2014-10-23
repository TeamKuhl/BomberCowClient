using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommunicationLibrary;

namespace BomberCowClient
{
    public partial class Form1 : Form
    {
        Client client = new Client();
        BomberMap BomberMap;
        int posx = 1;
        int posy = 1;
        public Form1()
        {
            InitializeComponent();
            BomberMap = new BomberMap(this);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
            client.onReceive += new ClientReceiveHandler(ReceiveEvent);
            client.connect("172.25.66.17", 45454);
            client.send("PlayerInfo", "testuser");
            timer1.Enabled = true;
            //BomberMap.drawPlayer();
        }

        public void ReceiveEvent(String type, String message)
        {
            switch (type)
            {
                case "Join":
                    MessageBox.Show(message);
                    client.send("GetMap", "");
                    break;
                case "ChatMessage":
                    MessageBox.Show(message);
                    break;
                case "Map":
                    BomberMap.createMap(message);
                    break;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            client.disconnect();
            client.onReceive -= new ClientReceiveHandler(ReceiveEvent);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                posy++;
            }
            if (e.KeyCode == Keys.Up)
            {
                posy--;
            }
            if (e.KeyCode == Keys.Left)
            {
                posx--;
            }
            if (e.KeyCode == Keys.Right)
            {
                posx++;
            }
            BomberMap.setPlayerPosition(1, posx, posy);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            client.send("GetMap", "");
            textBox1.Text = Convert.ToString(Convert.ToInt32(textBox1.Text) + 1);
        }
    }
}
