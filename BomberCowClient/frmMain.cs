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

        // Create a list of parts.
        public List<Player> players = new List<Player>();

        // Updates PalyerList and Map
        private Boolean getUpdates = false;

        private Boolean connected = false;
        // IP of the Server
        public string ServerIP;
        // Client name to connect with
        public string PlayerName;

        // Mapstring
        private string sMapString;

        //Client ID
        //private int ClientID;

        // Player dictionary
        //private Dictionary<int, string> playerlist = new Dictionary<int, string>();

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
                // Player joined Server
                case "Join":
                    string[] PlayerJoin = message.Split(':');

                    players.Add(new Player() { ID = PlayerJoin[0], Name = PlayerJoin[1] });
                    if (!getUpdates)
                    {
                        lstChat.Invoke(new emptyFunction(delegate() { lstChat.Items.Add("You joined the Server"); }));
                        client.send("GetMap", "");
                    }
                    else
                    {
                        foreach (Player oplayer in players)
                        {
                            if (oplayer.ID == PlayerJoin[0])
                            {
                                lstChat.Invoke(new emptyFunction(delegate() { lstChat.Items.Add(oplayer.Name + " joined the Server"); }));
                            }
                        }
                    }
                    break;

                // Player left Server
                case "Leave":
                    foreach (Player oplayer in players)
                    {
                        if (oplayer.ID == message)
                        {
                            lstChat.Invoke(new emptyFunction(delegate() { lstChat.Items.Add(oplayer.Name + " left the Server"); }));
                            players.Remove(oplayer);
                            break;
                        }
                    }
                    //client.send("GetPlayerList", "");
                    break;

                // Got message from Player
                case "ChatMessage":
                    lstChat.Invoke(new emptyFunction(delegate() { lstChat.Items.Add(message); }));
                    break;

                // Got Map strings
                case "Map":

                    if (!getUpdates)
                    {
                        lstChat.Invoke(new emptyFunction(delegate() { lstChat.Items.Add("Map Created"); }));
                        client.send("GetPlayerList", "");
                        getUpdates = true;
                    }
                    sMapString = message;
                    BomberMap.createMap(sMapString);

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
                    foreach (Player oplayer in players)
                    {
                        if (oplayer.ID == PlayerPosition[0])
                        {
                            oplayer.xPosition = Convert.ToInt32(PlayerPosition[1]);
                            oplayer.yPosition = Convert.ToInt32(PlayerPosition[2]);
                        }
                    }
                    if (sMapString != null)
                    {
                        BomberMap.createMap(sMapString);
                    }
                    break;

                // Player list update
                case "PlayerList":
                    string[] PlayerList = message.Split(';');
                    //players
                    foreach (string Player in PlayerList)
                    {
                        if (Player != "")
                        {
                            string[] PlayerandId = Player.Split(':');
                            players.Add(new Player() { ID = Convert.ToString(PlayerandId[0]), Name = PlayerandId[1] });
                        }
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
            if (e.KeyCode == Keys.Return)
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
