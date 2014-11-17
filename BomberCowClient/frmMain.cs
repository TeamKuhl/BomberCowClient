using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommunicationLibrary;

namespace BomberCowClient
{
    public partial class frmMain : Form
    {
        private Client client = new Client();
        private BomberMap BomberMap;

        // Create a list of players
        public List<Player> players = new List<Player>();

        // Create a list od items
        public List<Item> items = new List<Item>();

        // Updates PalyerList and Map
        private Boolean getUpdates = false;

        // If false don't disconnect
        private Boolean connected = false;

        // IP of the Server
        public string ServerIP;

        // Client name to connect with
        public string PlayerName;

        // Mapstring
        private string sMapString;

        // Set size of form
        private Boolean fitForm = false;

        // ID des clients
        private string myid;

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
            tmrUpdate.Enabled = false;
            // Create Eventhandler
            client.onReceive += new ClientReceiveHandler(ReceiveEvent);
            try
            {
                // Connect with given IP
                connected = client.connect(ServerIP, 45454);
                if (!connected)
                {
                    // Can't connect
                    MessageBox.Show("Es konnte keine Verbindung zum Server hergestellt werden", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
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
                // Initialize
                case "YourId":
                    // Set my ID
                    myid = message;

                    // Set Client name = PlayerName
                    client.send("PlayerInfo", PlayerName);
                    break;

                // Player joined Server
                case "Join":
                    string[] PlayerJoin = message.Split(':');

                    players.Add(new Player() { ID = PlayerJoin[0], Name = PlayerJoin[1], State = 1 });
                    if (!getUpdates)
                    {
                        lstChat.Invoke(new emptyFunction(delegate() { lstChat.Items.Add("You joined the Server"); }));
                        client.send("GetMap", "");
                        client.send("GetPlayerList", "");
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

                    // Reload Map
                    if (sMapString != null)
                    {
                        BomberMap.createMap(sMapString);
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

                    // Reload Map
                    if (sMapString != null)
                    {
                        BomberMap.createMap(sMapString);
                    }
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

                // Got player state
                case "PlayerStatus":
                    string[] status = message.Split(':');
                    foreach (Player oPlayer in players)
                    {
                        if (oPlayer.ID == status[0])
                        {
                            oPlayer.PlayerState = Convert.ToInt32(status[1]);
                        }
                    }

                    // Reload Map
                    if (sMapString != null)
                    {
                        BomberMap.createMap(sMapString);
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

                    // Reload Map
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
                            players.Add(new Player() { ID = PlayerandId[0], Name = PlayerandId[1] });
                            client.send("GetPlayerPosition", PlayerandId[0]);
                            client.send("GetPlayerStatus", PlayerandId[0]);
                        }
                    }
                    break;

                // Got bomb
                case "BombPlaced":
                    string[] BombPosition = message.Split(':');
                    items.Add(new Item() { type = "bomb", xPosition = Convert.ToInt32(BombPosition[0]), yPosition = Convert.ToInt32(BombPosition[1]) });

                    // Reload Map
                    if (sMapString != null)
                    {
                        BomberMap.createMap(sMapString);
                    }
                    break;

                // Player died
                case "PlayerDied":
                    foreach (Player oPlayer in players)
                    {
                        if (oPlayer.ID == message)
                        {
                            oPlayer.State = 2;

                            // Reload Map
                            if (sMapString != null)
                            {
                                BomberMap.createMap(sMapString);
                            }
                        }
                    }
                    break;

                // Delete bomb
                case "BombExploded":
                    string[] DeleteBombs = message.Split(';');

                    string[] BombPos = DeleteBombs[0].Split(':');

                    foreach (Item item in items)
                    {
                        if (item.type == "bomb")
                        {
                            if (item.xPosition == Convert.ToInt32(BombPos[0]))
                            {
                                if (item.yPosition == Convert.ToInt32(BombPos[1]))
                                {
                                    items.Remove(item);
                                    break;
                                }
                            }
                        }
                    }

                    List<Item> explosions = new List<Item>();

                    foreach (string bomb in DeleteBombs)
                    {
                        if (bomb != "")
                        {
                            BombPos = bomb.Split(':');
                            Item explosion = new Item() { type = "explode", xPosition = Convert.ToInt32(BombPos[0]), yPosition = Convert.ToInt32(BombPos[1]) };
                            items.Add(explosion);
                            explosions.Add(explosion);
                        }
                    }

                    // Reload Map
                    if (sMapString != null)
                    {
                        BomberMap.createMap(sMapString);
                    }

                    ParameterizedThreadStart pts = new ParameterizedThreadStart(this.explosionHandler);
                    Thread thread = new Thread(pts);
                    thread.Start(explosions);

                    break;
            }
            lstChat.Invoke(new emptyFunction(delegate() { lstChat.SelectedIndex = lstChat.Items.Count - 1; }));
        }

        private void explosionHandler(Object explosions)
        {
            Thread.Sleep(500);

            foreach (Item item in (List<Item>)explosions)
            {
                items.Remove(item);
            }

            // Reload Map
            if (sMapString != null)
            {
                BomberMap.createMap(sMapString);
            }
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
            foreach (Player oplayer in players)
            {
                if (oplayer.ID == myid)
                {
                    if (oplayer.PlayerState == 1)
                    {
                        // Send move commands to the Server
                        if (e.KeyCode == Keys.S || e.KeyCode == Keys.Down)
                        {
                            client.send("Move", "s");
                        }
                        if (e.KeyCode == Keys.W || e.KeyCode == Keys.Up)
                        {
                            client.send("Move", "n");
                        }
                        if (e.KeyCode == Keys.A || e.KeyCode == Keys.Left)
                        {
                            client.send("Move", "w");
                        }
                        if (e.KeyCode == Keys.D || e.KeyCode == Keys.Right)
                        {
                            client.send("Move", "e");
                        }
                        if (e.KeyCode == Keys.Space)
                        {
                            client.send("BombPlace", "");
                        }
                    }
                }
            }

            // Send chat to Server
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

        private void tmrUpdate_Tick(object sender, EventArgs e)
        {
            foreach (Item oitem in items)
            {
                if (oitem.type == "explode")
                {
                    oitem.livetime = oitem.livetime + 1;
                }
            }

            foreach (Item oitem in items)
            {
                if (oitem.type == "explode")
                {
                    if (oitem.livetime >= 5)
                    {
                        items.Remove(oitem);
                        break;
                    }
                }
            }
            // Reload Map
            if (sMapString != null)
            {
                BomberMap.createMap(sMapString);
            }
        }

    }
}
