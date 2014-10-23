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
        // IP of the Server
        public string ServerIP;
        // Client name to connect with
        public string PlayerName;

        //Client ID
        private int ClientID;

        // Player dictionary
        private Dictionary<int, string> playerlist = new Dictionary<int, string>();

        //set size of form
        private Boolean fitForm = false;

        public frmMain()
        {
            InitializeComponent();
            BomberMap = new BomberMap(this);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Create formLogin
            frmLogin frmLogin = new frmLogin(this);
            // Show it
            frmLogin.Show();

            // For more performance
            this.DoubleBuffered = true;
        }

        // Connect to the Server
        public void connect()
        {
            // Create Eventhandler
            client.onReceive += new ClientReceiveHandler(ReceiveEvent);
            try
            {
                // Connect with given IP
                connected = client.connect(ServerIP, 45454);
                
                // Set Client name = PlayerName
                client.send("PlayerInfo", PlayerName);
            }
            catch
            {
                // Can't connect
                MessageBox.Show("Es konnte keine Verbindung zum Server hergestellt werden", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        // Handles the differetn input- types
        private void ReceiveEvent(String type, String message)
        {
            switch (type)
            {
                // Player joint Server
                case "Join":
                    string[] PlayerJoin = message.Split(':');
                    if (PlayerJoin[1] == PlayerName)
                    {
                        ClientID = Convert.ToInt32(PlayerJoin[0]);
                        lstChat.Invoke(new emptyFunction(delegate() { lstChat.Items.Add("You joined the Server"); }));
                        client.send("GetMap", "");
                    }
                    else
                    {
                        lstChat.Invoke(new emptyFunction(delegate() { lstChat.Items.Add(PlayerJoin[1] + " joined the Server"); }));
                    }
                    client.send("GetPlayerList", "");
                    break;
                // Player left Server
                case "Leave":
                    lstChat.Invoke(new emptyFunction(delegate() { lstChat.Items.Add(playerlist[Convert.ToInt32(message)] + " left the Server"); }));
                    client.send("GetPlayerList", "");
                    break;
                // Got message from Player
                case "ChatMessage":
                    lstChat.Invoke(new emptyFunction(delegate() { lstChat.Items.Add(message); }));
                    break;
                // Got Map strings
                case "Map":
                    BomberMap.createMap(message);
                    if (!fitForm)
                    {
                        lstChat.Invoke(new emptyFunction(delegate()
                        {
                            lstChat.Left = this.Width - 10;
                            lstChat.Size = new Size(322, 95);
                        }));
                        txtChat.Invoke(new emptyFunction(delegate()
                        {
                            txtChat.Left = lstChat.Left;
                            txtChat.Size = new Size(206, 20);
                        }));
                        fitForm = true;
                    }
                    break;
                // Got player position
                case "PlayerPosition":
                    string[] PlayerPosition = message.Split(':');
                    if (Convert.ToInt32(PlayerPosition[0]) == ClientID)
                    {
                        BomberMap.setPlayerPosition(1, Convert.ToInt32(PlayerPosition[1]), Convert.ToInt32(PlayerPosition[2]));
                    }
                    else
                    {
                        BomberMap.setPlayerPosition(2, Convert.ToInt32(PlayerPosition[1]), Convert.ToInt32(PlayerPosition[2]));
                    }
                    break;
                // Player list update
                case "PlayerList":
                    string[] PlayerList = message.Split(';');
                    playerlist = null;
                    foreach (string Player in PlayerList)
                    {
                        string[] PlayerandId = Player.Split(':');
                        playerlist.Add(Convert.ToInt32(PlayerandId[0]), PlayerandId[1]);
                    }
                    break;
            }
            lstChat.Invoke(new emptyFunction(delegate() { lstChat.SelectedIndex = lstChat.Items.Count - 1; }));
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // If connected disconnect to prevent errors
            if (connected)
            {
                client.disconnect();
            }
            // Close Eventhandler
            client.onReceive -= new ClientReceiveHandler(ReceiveEvent);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // Send move commands to the Server
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
