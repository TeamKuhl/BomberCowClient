using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
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

        // Size ofe the textures
        public int BlockSize = 32;
        private int MapXSize;
        private int MapYSize;

        // Player won
        private Boolean bplayerwon = false;
        private string swinnerid;
        public void playerwon(string id, Boolean bstate)
        {
            bplayerwon = bstate;
            swinnerid = id;
        }

        //not used right now
        private int HUDYSize = 0;

        // Chat
        private int ChatYSize = 95 + 30;
        private List<string> lstChat = new List<string>();
        private Boolean getInput = false;
        private string inputMsg;

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
                AllPictureBox.Size = new Size(MapXSize * BlockSize, (MapYSize * BlockSize) + HUDYSize + ChatYSize);
                AllPictureBox.Visible = true;
                AllPictureBox.BackgroundImage = mapimg;
                mainForm.Invoke(new emptyFunction(delegate() { mainForm.Controls.Add(AllPictureBox); }));
                //drawPlayer();
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

            Bitmap map = new Bitmap(MapXSize * BlockSize, (MapYSize * BlockSize) + HUDYSize + ChatYSize);
            Graphics g = Graphics.FromImage(map);
            Size imgSize = new Size(BlockSize, BlockSize);
            Image img1 = BomberCowClient.Properties.Resources.wall;
            Image img2 = BomberCowClient.Properties.Resources.breakable;
            Image back = BomberCowClient.Properties.Resources.back;
            Image player = BomberCowClient.Properties.Resources.player;
            Image playerDead = BomberCowClient.Properties.Resources.playerDead;
            Image bomb = BomberCowClient.Properties.Resources.bomb;
            Image explode = BomberCowClient.Properties.Resources.explode;

            img1 = ResizeImage(img1, imgSize);
            img2 = ResizeImage(img2, imgSize);
            back = ResizeImage(back, imgSize);
            player = ResizeImage(player, imgSize);
            playerDead = ResizeImage(playerDead, imgSize);
            bomb = ResizeImage(bomb, imgSize);
            explode = ResizeImage(explode, imgSize);

            // Clear
            g.Clear(Color.Gray);

            // Draw map
            for (int yCounter = 0; yCounter < MapYSize; yCounter++)
            {
                dummy = rows[yCounter];
                currow = dummy.Split(':');
                for (int xCounter = 0; xCounter < MapXSize; xCounter++)
                {
                    // Add textures
                    if (currow[xCounter] == "0")
                    {
                        g.DrawImage(img1, new Point(xCounter * BlockSize, (yCounter * BlockSize) + HUDYSize));
                    }
                    if (currow[xCounter] == "1")
                    {
                        g.DrawImage(back, new Point(xCounter * BlockSize, (yCounter * BlockSize) + HUDYSize));
                    }
                    if (currow[xCounter] == "2")
                    {
                        g.DrawImage(img2, new Point(xCounter * BlockSize, (yCounter * BlockSize) + HUDYSize));
                    }
                }
            }

            try
            {
                // Draw items
                foreach (Item oitem in mainForm.items)
                {
                    if (oitem.type == "bomb")
                    {
                        g.DrawImage(bomb, new Point((oitem.xPosition - 1) * BlockSize, ((oitem.yPosition - 1) * BlockSize) + HUDYSize));
                    }
                    if (oitem.type == "explode")
                    {
                        g.DrawImage(explode, new Point((oitem.xPosition - 1) * BlockSize, ((oitem.yPosition - 1) * BlockSize) + HUDYSize));
                    }
                }
            }
            catch
            {
                // unbehandelter Fehler
            }

            try
            {
                // Draw names & player
                foreach (Player oplayer in mainForm.players)
                {
                    
                    
                    if (oplayer.PlayerState == 1)
                    {
                        // Draw living player
                        if (oplayer.PlayerState == 1)
                        {
                            g.DrawImage(player, new Point((oplayer.xPosition - 1) * BlockSize, ((oplayer.yPosition - 1) * BlockSize) + HUDYSize));
                        }

                        // Draw dead player
                        if (oplayer.PlayerState == 2)
                        {
                            g.DrawImage(playerDead, new Point((oplayer.xPosition - 1) * BlockSize, ((oplayer.yPosition - 1) * BlockSize) + HUDYSize));
                        }

                        // Draw name
                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Center;
                        //stringFormat.LineAlignment = StringAlignment.Center;

                        RectangleF rectf = new RectangleF(((oplayer.xPosition - 1) * BlockSize) - 8, (((oplayer.yPosition - 1) * BlockSize) - 13) + HUDYSize, BlockSize + 16, BlockSize);

                        g.DrawString(oplayer.Name, new Font("Tahoma", 8), Brushes.Green, rectf, stringFormat);
                    }
                }
            }
            catch
            {
                // unbehandelter Fehler
            }

            // Draw message
            if (bplayerwon == true)
            {
                foreach (Player oplayer in mainForm.players)
                {
                    if (oplayer.ID == swinnerid)
                    {
                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Center;
                        stringFormat.LineAlignment = StringAlignment.Center;

                        RectangleF rects = new RectangleF(0, 0, MapXSize * BlockSize, MapYSize * BlockSize);
                        g.DrawString(oplayer.Name + " won", new Font("Tahoma", 60), Brushes.Green, rects, stringFormat);
                    }
                }
            }

            // Draw chat
            if (true)
            {
                int counter = 0;
                int textYSize = 15;
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Near;

                foreach (string oMessage in lstChat)
                {
                    string[] sDummy = oMessage.Split('&');
                    int iState = Convert.ToInt16(sDummy[sDummy.Length - 1]);
                    string sMessage = sDummy[0];

                    if (sDummy.Length > 2)
                    {
                        for (int iCounter = 1; iCounter < sDummy.Length - 1; iCounter++)
                        {
                            sMessage = sMessage + "&" + sDummy[iCounter];
                        }
                    }

                    RectangleF rects = new RectangleF(0, (MapYSize * BlockSize) + (counter * textYSize), MapXSize * BlockSize, (MapYSize * BlockSize) + ChatYSize);

                    if (iState == 1)
                    {
                        g.DrawString(sMessage, new Font("Tahoma", 10), Brushes.Red, rects, stringFormat);
                    }
                    if (iState == 2)
                    {
                        g.DrawString(sMessage, new Font("Tahoma", 10), Brushes.Blue, rects, stringFormat);
                    }
                    if (iState == 3)
                    {
                        g.DrawString(sMessage, new Font("Tahoma", 10), Brushes.Green, rects, stringFormat);
                    }
                    counter++;
                }

                if (getInput)
                {
                    Pen redPen = new Pen(Color.Red, 3);
                    Rectangle rect = new Rectangle(1, (MapYSize * BlockSize) + ChatYSize - 30, (MapXSize * BlockSize) - 3, 28);
                    g.DrawRectangle(redPen, rect);

                    RectangleF rects = new RectangleF(2, (MapYSize * BlockSize) + ChatYSize - 29, (MapXSize * BlockSize) - 4, 26);
                    g.DrawString(inputMsg, new Font("Tahoma", 14), Brushes.Green, rects, stringFormat);
                }
                else
                {
                    Pen blackPen = new Pen(Color.Black, 3);
                    Rectangle rect = new Rectangle(1, (MapYSize * BlockSize) + ChatYSize - 30, (MapXSize * BlockSize) - 3, 28);
                    g.DrawRectangle(blackPen, rect);
                }
            }

            g.Dispose();
            img1.Dispose();
            img2.Dispose();

            // Return complete image
            return map;
        }


        /// <summary>
        ///     Resize images to fit the map
        /// </summary>
        /// <param name="image">original image</param>
        /// <param name="size">size to scale to</param>
        /// <returns>resized image</returns>
        private static Image ResizeImage(Image image, Size size)
        {
            int newWidth;
            int newHeight;

            int originalWidth = image.Width;
            int originalHeight = image.Height;
            float percentWidth = (float)size.Width / (float)originalWidth;
            float percentHeight = (float)size.Height / (float)originalHeight;
            float percent = percentHeight < percentWidth ? percentHeight : percentWidth;
            newWidth = (int)(originalWidth * percent);
            newHeight = (int)(originalHeight * percent);

            Image newImage = new Bitmap(newWidth, newHeight);
            using (Graphics graphicsHandle = Graphics.FromImage(newImage))
            {
                graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsHandle.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            return newImage;
        }

        public void addInput(string input, Boolean writing)
        {
            getInput = writing;
            inputMsg = input;
        }

        public void addchat(string message, int status)
        {
            // status
            // 1 = Server (not included)
            // 2 = Client
            // 3 = Player

            if (lstChat.Count == 6)
            {
                lstChat.RemoveAt(0);
            }
            lstChat.Add(message + "&" + status);
        }
    }
}
