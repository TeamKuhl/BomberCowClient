using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace BomberCowClient
{
    public delegate void emptyFunction();

    class BomberMap
    {
        private frmMain mainForm;
        private PictureBox AllPictureBox = new PictureBox();
        private PictureBox Player1PictureBox = new PictureBox();
        private PictureBox Player2PictureBox = new PictureBox();

        // Size ofe the textures
        public int BlockSize = 42;
        private int MapXSize;
        private int MapYSize;

        private Boolean MapExists = false;

        public BomberMap(frmMain mainform)
        {
            this.mainForm = mainform;
        }

        /// <summary>
        ///     Create or update Map
        /// </summary>
        /// <param name="MapString">MapSting from Server</param>
        public void createMap(string MapString)
        {
            Image mapimg = drawMap(MapString);
            if (MapExists)
            {
                // Reload Mapimage
                AllPictureBox.Invoke(new emptyFunction(delegate() { AllPictureBox.BackgroundImage = mapimg; }));
            }
            else
            {
                // Create picturebox
                AllPictureBox.Location = new Point(10, 10);
                AllPictureBox.Name = "MapPictureBox";
                AllPictureBox.Size = new Size(MapXSize * BlockSize, MapYSize * BlockSize);
                AllPictureBox.Visible = true;
                AllPictureBox.BackgroundImage = mapimg;
                mainForm.Invoke(new emptyFunction(delegate() { mainForm.Controls.Add(AllPictureBox); }));
                drawPlayer();
                MapExists = true;
            }
        }

        /// <summary>
        ///     Create Bitmap out of Map strings 
        /// </summary>
        /// <param name="MapString">MapSting from Server</param>
        /// <returns></returns>
        private Bitmap drawMap(string MapString)
        {
            String[] rows = MapString.Split(';');
            MapYSize = rows.Length - 1;
            String dummy = rows[0];
            String[] currow = dummy.Split(':');
            MapXSize = currow.Length;

            Bitmap map = new Bitmap(MapXSize * BlockSize, MapYSize * BlockSize);
            Graphics g = Graphics.FromImage(map);
            Image img1 = BomberCowClient.Properties.Resources.png1;
            Image img2 = BomberCowClient.Properties.Resources.png2;
            Image back = BomberCowClient.Properties.Resources.back;
            Image player = BomberCowClient.Properties.Resources.Player1;

            g.Clear(Color.Black);
            for (int yCounter = 0; yCounter < MapYSize; yCounter++)
            {
                dummy = rows[yCounter];
                currow = dummy.Split(':');
                for (int xCounter = 0; xCounter < MapXSize; xCounter++)
                {
                    // Add textures
                    if (currow[xCounter] == "0")
                    {
                        g.DrawImage(img1, new Point(xCounter * BlockSize, yCounter * BlockSize));
                    }
                    if (currow[xCounter] == "1")
                    {
                        g.DrawImage(back, new Point(xCounter * BlockSize, yCounter * BlockSize));
                    }
                    if (currow[xCounter] == "2")
                    {
                        g.DrawImage(img2, new Point(xCounter * BlockSize, yCounter * BlockSize));
                    }
                }
            }
            g.DrawImage(player, new Point(1 * BlockSize, 2 * BlockSize));
            g.Dispose();
            img1.Dispose();
            img2.Dispose();
            map.Save("map.png", System.Drawing.Imaging.ImageFormat.Png);
            // Return complete image
            return map;
        }

        /// <summary>
        ///     Creates imagebox for player
        /// </summary>
        public void drawPlayer()
        {
            Player1PictureBox.Location = new Point(0, 0);
            Player1PictureBox.Name = "Player1PictureBox";
            Player1PictureBox.Size = new Size(BlockSize, BlockSize);
            Player1PictureBox.Visible = true;
            Player1PictureBox.BackColor = Color.Transparent;
            Player1PictureBox.BackgroundImage = BomberCowClient.Properties.Resources.Player1;
            AllPictureBox.Invoke(new emptyFunction(delegate() { AllPictureBox.Controls.Add(Player1PictureBox); }));
            Player1PictureBox.Parent = AllPictureBox;

            Player2PictureBox.Location = new Point(0, 0);
            Player2PictureBox.Name = "Player2PictureBox";
            Player2PictureBox.Size = new Size(BlockSize, BlockSize);
            Player2PictureBox.Visible = true;
            Player2PictureBox.BackColor = Color.Transparent;
            Player2PictureBox.BackgroundImage = BomberCowClient.Properties.Resources.Player1;
            AllPictureBox.Invoke(new emptyFunction(delegate() { AllPictureBox.Controls.Add(Player2PictureBox); }));
            Player2PictureBox.Parent = AllPictureBox;
        }

        /// <summary>
        ///     Set the position of a player
        /// </summary>
        /// <param name="Player">???</param>
        /// <param name="xPosition">x Position</param>
        /// <param name="yPosition">y Position</param>
        public void setPlayerPosition(int Player, int xPosition, int yPosition)
        {
            if (Player == 1)
            {
                if (Player1PictureBox.InvokeRequired)
                {
                    Player1PictureBox.Invoke(new emptyFunction(delegate() { Player1PictureBox.Location = new Point((xPosition - 1) * BlockSize, (yPosition - 1) * BlockSize); }));
                }
                else
                {
                    Player1PictureBox.Location = new Point((xPosition - 1) * BlockSize, (yPosition - 1) * BlockSize);
                }
            }
            if (Player == 2)
            {
                if (Player2PictureBox.InvokeRequired)
                {
                    Player2PictureBox.Invoke(new emptyFunction(delegate() { Player2PictureBox.Location = new Point((xPosition - 1) * BlockSize, (yPosition - 1) * BlockSize); }));
                }
                else
                {
                    Player2PictureBox.Location = new Point((xPosition - 1) * BlockSize, (yPosition - 1) * BlockSize);
                }
            }
        }
    }
}
