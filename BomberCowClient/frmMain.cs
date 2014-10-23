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
    public partial class frmMain : Form
    {
        private Client client = new Client();
        private BomberMap BomberMap;

        private Boolean connected = false;
        //IP of the Server
        public string ServerIP;
        //Client name to connect with
        public string PlayerName;

        public frmMain()
        {
            InitializeComponent();
            BomberMap = new BomberMap(this);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //create formLogin
            frmLogin frmLogin = new frmLogin(this);
            //show it
            frmLogin.Show();

            //for more performance
            this.DoubleBuffered = true;
        }

        //connect to the Server
        public void connect()
        {
            //create Eventhandler
            client.onReceive += new ClientReceiveHandler(ReceiveEvent);
            try
            {
                //connect with given IP
                connected = client.connect(ServerIP, 45454);
                //set Client name = PlayerName
                client.send("PlayerInfo", PlayerName);
            }
            catch
            {
                //can't connect
                MessageBox.Show("Es konnte keine Verbindung zum Server hergestellt werden", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        //handles the differetn input- types
        private void ReceiveEvent(String type, String message)
        {
            switch (type)
            {
                //Player joint Server
                case "Join":
                    string[] PlayerJoin = message.Split(':');
                    if (PlayerJoin[1] == PlayerName)
                    {
                        lstChat.Invoke(new emptyFunction(delegate() { lstChat.Items.Add("You joined the Server"); }));
                        client.send("GetMap", "");
                    }
                    else
                    {
                        lstChat.Invoke(new emptyFunction(delegate() { lstChat.Items.Add(PlayerJoin[1] + " joined the Server"); }));
                    }
                    break;
                //Player left Server
                case "Leave":
                    lstChat.Invoke(new emptyFunction(delegate() { lstChat.Items.Add(message + " left the Server"); }));
                    break;
                //got message from Player
                case "ChatMessage":
                    lstChat.Invoke(new emptyFunction(delegate() { lstChat.Items.Add(message); }));
                    break;
                //got Map strings
                case "Map":
                    BomberMap.createMap(message);
                    break;
                //got player position
                case "PlayerPosition":
                    string[] PlayerPosition = message.Split(':');
                    BomberMap.setPlayerPosition(1, Convert.ToInt32(PlayerPosition[1]), Convert.ToInt32(PlayerPosition[2]));
                    break;
            }
            lstChat.Invoke(new emptyFunction(delegate() { lstChat.SelectedIndex = lstChat.Items.Count - 1; }));
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if connected disconnect to prevent errors
            if (connected)
            {
                client.disconnect();
            }
            //close Eventhandler
            client.onReceive -= new ClientReceiveHandler(ReceiveEvent);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //Send move commands to the Server
            if (e.KeyCode == Keys.S)
            {
                client.send("Move", "s");
            }
            if (e.KeyCode == Keys.W)
            {
                client.send("Move", "n");
            }
            if (e.KeyCode == Keys.A)
            {
                client.send("Move", "w");
            }
            if (e.KeyCode == Keys.D)
            {
                client.send("Move", "e");
            }
            if (e.KeyCode == Keys.Return)
            {
                txtChat.Enabled = true;
                txtChat.Focus();
            }
        }

        private void txtChat_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Return)
            {
                if (txtChat.Text != "")
                {
                    client.send("ChatMessage", txtChat.Text);
                    txtChat.Text = "";
                }
                txtChat.Enabled = false;
            }
        }
    }
}
