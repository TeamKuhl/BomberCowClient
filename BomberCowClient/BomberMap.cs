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
        private PlayerImage PlayerImage = new PlayerImage();

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

        // PlayerStats
        private Boolean bshowStats = false;

        // Textures
        private Dictionary<string, Image> textures = new Dictionary<string, Image>();

        // ImgSize
        Size imgSize;

        public BomberMap(frmMain mainform)
        {
            this.mainForm = mainform;
            imgSize = new Size(BlockSize, BlockSize);
        }

        /// <summary>
        ///     Create or update Map
        /// </summary>
        /// <param name="MapString">MapSting from Server</param>
        public void createMap(string MapString)
        {
            //mainForm.Invoke(new Action(() =>
            //        {
            Image mapimg = drawMap(MapString);
            mainForm.BackgroundImage = mapimg;
            //}));
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

            Image player;

            // Form size
            if (mainForm.InvokeRequired)
            {
                mainForm.Invoke(new emptyFunction(delegate() { mainForm.ClientSize = new Size(MapXSize * BlockSize, MapYSize * BlockSize + ChatYSize + HUDYSize); }));
            }
            else
            {
                mainForm.ClientSize = new Size(MapXSize * BlockSize, MapYSize * BlockSize + ChatYSize + HUDYSize);
            }

            // Clear
            if (mainForm.InvokeRequired)
            {
                mainForm.Invoke(new emptyFunction(delegate() { g.Clear(ColorTranslator.FromHtml("#031634")); }));
            }
            else
            {
                g.Clear(ColorTranslator.FromHtml("#031634"));
            }

            // Draw map
            for (int yCounter = 0; yCounter < MapYSize; yCounter++)
            {
                dummy = rows[yCounter];
                currow = dummy.Split(':');
                for (int xCounter = 0; xCounter < MapXSize; xCounter++)
                {
                    // Add textures
                    if (mainForm.InvokeRequired)
                    {
                        mainForm.Invoke(new emptyFunction(delegate() { g.DrawImage(textures[currow[xCounter]], new Point(xCounter * BlockSize, (yCounter * BlockSize) + HUDYSize)); }));
                    }
                    else
                    {
                        g.DrawImage(textures[currow[xCounter]], new Point(xCounter * BlockSize, (yCounter * BlockSize) + HUDYSize));
                    }
                }
            }

            // Draw items
            foreach (Item oitem in mainForm.items)
            {
                if (mainForm.InvokeRequired)
                {
                    mainForm.Invoke(new emptyFunction(delegate() { g.DrawImage(textures[oitem.type], new Point((oitem.xPosition - 1) * BlockSize, ((oitem.yPosition - 1) * BlockSize) + HUDYSize)); }));
                }
                else
                {
                    g.DrawImage(textures[oitem.type], new Point((oitem.xPosition - 1) * BlockSize, ((oitem.yPosition - 1) * BlockSize) + HUDYSize));
                }
            }

            // Draw player
            foreach (Player oplayer in mainForm.players)
            {
                if (oplayer.PlayerState == 1)
                {
                    // Draw living player
                    if (oplayer.PlayerState == 1)
                    {
                        if (oplayer.Skin != null)
                        {
                            player = oplayer.Skin;

                            if (mainForm.InvokeRequired)
                            {
                                mainForm.Invoke(new emptyFunction(delegate() { g.DrawImage(player, new Point((oplayer.xPosition - 1) * BlockSize, ((oplayer.yPosition - 1) * BlockSize) + HUDYSize)); }));
                            }
                            else
                            {
                                g.DrawImage(player, new Point((oplayer.xPosition - 1) * BlockSize, ((oplayer.yPosition - 1) * BlockSize) + HUDYSize));
                            }
                        }
                    }

                    //// Draw dead player
                    //if (oplayer.PlayerState == 2)
                    //{
                    //    g.DrawImage(playerDead, new Point((oplayer.xPosition - 1) * BlockSize, ((oplayer.yPosition - 1) * BlockSize) + HUDYSize));
                    //}
                }
            }

            // Draw names
            foreach (Player oplayer in mainForm.players)
            {
                if (oplayer.PlayerState == 1)
                {
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Center;

                    RectangleF rectf = new RectangleF(((oplayer.xPosition - 1) * BlockSize) - 32, (((oplayer.yPosition - 1) * BlockSize) - 13) + HUDYSize, BlockSize + 64, BlockSize);

                    if (mainForm.InvokeRequired)
                    {
                        mainForm.Invoke(new emptyFunction(delegate() { g.DrawString(oplayer.Name, new Font("Tahoma", 7), Brushes.Green, rectf, stringFormat); }));
                    }
                    else
                    {
                        g.DrawString(oplayer.Name, new Font("Tahoma", 7), Brushes.Green, rectf, stringFormat);
                    }
                }
            }

            // Draw playerlist
            if (bshowStats)
            {
                Pen borderpen = new Pen(Color.FromArgb(255, 3, 22, 52), 20);
                SolidBrush backbrush = new SolidBrush(Color.FromArgb(150, 3, 54, 73));
                Rectangle rect = new Rectangle(10, 10 + HUDYSize, (MapXSize * BlockSize) - 20, (BlockSize * MapYSize) - 20);

                if (mainForm.InvokeRequired)
                {
                    mainForm.Invoke(new emptyFunction(delegate()
                        {
                            g.DrawRectangle(borderpen, rect);
                            g.FillRectangle(backbrush, rect);
                        }));
                }
                else
                {
                    g.DrawRectangle(borderpen, rect);
                    g.FillRectangle(backbrush, rect);
                }

                int Lstcounter = 0;
                foreach (Player oPlayer in mainForm.players)
                {
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Near;
                    stringFormat.LineAlignment = StringAlignment.Near;
                    StringFormat stringFormat1 = new StringFormat();
                    stringFormat1.Alignment = StringAlignment.Center;
                    stringFormat1.LineAlignment = StringAlignment.Center;

                    RectangleF rects = new RectangleF(10, 20 + (Lstcounter * 30), (MapXSize * BlockSize) - 20, 30);

                    SolidBrush textdeadbrush = new SolidBrush(Color.FromArgb(255, 205, 179, 128));
                    SolidBrush textlivebrush = new SolidBrush(Color.FromArgb(255, 232, 221, 203));

                    if (oPlayer.PlayerState == 1)
                    {
                        if (mainForm.InvokeRequired)
                        {
                            mainForm.Invoke(new emptyFunction(delegate()
                            {
                                g.DrawString(oPlayer.Name, new Font("Tahoma", 20), textlivebrush, rects, stringFormat);
                                g.DrawString("Score:" + oPlayer.Score + " Kills:" + oPlayer.Kills + " Deaths:" + oPlayer.Deaths, new Font("Tahoma", 16), textlivebrush, rects, stringFormat1);
                            }));
                        }
                        else
                        {
                            g.DrawString(oPlayer.Name, new Font("Tahoma", 20), textlivebrush, rects, stringFormat);
                            g.DrawString("Score:" + oPlayer.Score + " Kills:" + oPlayer.Kills + " Deaths:" + oPlayer.Deaths, new Font("Tahoma", 16), textlivebrush, rects, stringFormat1);
                        }
                    }
                    else
                    {
                        if (mainForm.InvokeRequired)
                        {
                            mainForm.Invoke(new emptyFunction(delegate()
                            {
                                g.DrawString(oPlayer.Name, new Font("Tahoma", 20), textdeadbrush, rects, stringFormat);
                                g.DrawString("Score:" + oPlayer.Score + " Kills:" + oPlayer.Kills + " Deaths:" + oPlayer.Deaths, new Font("Tahoma", 16), textdeadbrush, rects, stringFormat1);
                            }));
                        }
                        else
                        {
                            g.DrawString(oPlayer.Name, new Font("Tahoma", 20), textdeadbrush, rects, stringFormat);
                            g.DrawString("Score:" + oPlayer.Score + " Kills:" + oPlayer.Kills + " Deaths:" + oPlayer.Deaths, new Font("Tahoma", 16), textdeadbrush, rects, stringFormat1);
                        }
                    }
                    Lstcounter++;
                }
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

                        if (mainForm.InvokeRequired)
                        {
                            mainForm.Invoke(new emptyFunction(delegate() { g.DrawString(oplayer.Name + " won", new Font("Tahoma", 60), Brushes.Green, rects, stringFormat); }));
                        }
                        else
                        {
                            g.DrawString(oplayer.Name + " won", new Font("Tahoma", 60), Brushes.Green, rects, stringFormat);
                        }
                    }
                }
            }

            // Draw chat
            int counter = 0;
            int textYSize = 15;
            StringFormat stringFormat2 = new StringFormat();
            stringFormat2.Alignment = StringAlignment.Near;

            Brush sbSystem = new SolidBrush(ColorTranslator.FromHtml("#036564"));
            Brush sbChat = new SolidBrush(ColorTranslator.FromHtml("#E8DDCB"));

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

                RectangleF rects = new RectangleF(0, ((MapYSize * BlockSize) + (counter * textYSize)) + HUDYSize, MapXSize * BlockSize, 20);

                if (iState == 1)
                {
                    if (mainForm.InvokeRequired)
                    {
                        mainForm.Invoke(new emptyFunction(delegate() { g.DrawString(sMessage, new Font("Tahoma", 10), sbSystem, rects, stringFormat2); }));
                    }
                    else
                    {
                        g.DrawString(sMessage, new Font("Tahoma", 10), sbSystem, rects, stringFormat2);
                    }
                }
                if (iState == 2)
                {
                    if (mainForm.InvokeRequired)
                    {
                        mainForm.Invoke(new emptyFunction(delegate() { g.DrawString(sMessage, new Font("Tahoma", 10), sbSystem, rects, stringFormat2); }));
                    }
                    else
                    {
                        g.DrawString(sMessage, new Font("Tahoma", 10), sbSystem, rects, stringFormat2);
                    }
                }
                if (iState == 3)
                {
                    if (mainForm.InvokeRequired)
                    {
                        mainForm.Invoke(new emptyFunction(delegate() { g.DrawString(sMessage, new Font("Tahoma", 10), sbChat, rects, stringFormat2); }));
                    }
                    else
                    {
                        g.DrawString(sMessage, new Font("Tahoma", 10), sbChat, rects, stringFormat2);
                    }
                }
                counter++;
            }

            if (getInput)
            {
                Pen redPen = new Pen(ColorTranslator.FromHtml("#036564"), 3);
                Rectangle rect = new Rectangle(1, ((MapYSize * BlockSize) + ChatYSize - 30) + HUDYSize, (MapXSize * BlockSize) - 3, 28);

                if (mainForm.InvokeRequired)
                {
                    mainForm.Invoke(new emptyFunction(delegate() { g.DrawRectangle(redPen, rect); }));
                }
                else
                {
                    g.DrawRectangle(redPen, rect);
                }

                RectangleF rects = new RectangleF(2, ((MapYSize * BlockSize) + ChatYSize - 29) + HUDYSize, (MapXSize * BlockSize) - 4, 26);

                if (mainForm.InvokeRequired)
                {
                    mainForm.Invoke(new emptyFunction(delegate() { g.DrawString(inputMsg, new Font("Tahoma", 14), sbChat, rects, stringFormat2); }));
                }
                else
                {
                    g.DrawString(inputMsg, new Font("Tahoma", 14), sbChat, rects, stringFormat2);
                }
            }
            else
            {
                Pen blackPen = new Pen(ColorTranslator.FromHtml("#033649"), 3);
                Rectangle rect = new Rectangle(1, ((MapYSize * BlockSize) + ChatYSize - 30) + HUDYSize, (MapXSize * BlockSize) - 3, 28);

                if (mainForm.InvokeRequired)
                {
                    mainForm.Invoke(new emptyFunction(delegate() { g.DrawRectangle(blackPen, rect); }));
                }
                else
                {
                    g.DrawRectangle(blackPen, rect);
                }
            }

            if (mainForm.InvokeRequired)
            {
                mainForm.Invoke(new emptyFunction(delegate() { g.Dispose(); }));
            }
            else
            {
                g.Dispose();
            }

            // Return complete image
            return map;
        }

        /// <summary>
        ///     Resize images to fit the map
        /// </summary>
        /// <param name="image">original image</param>
        /// <param name="size">size to scale to</param>
        /// <returns>resized image</returns>
        public Image ResizeImage(Image image)
        {
            int newWidth;
            int newHeight;

            Size size = imgSize;

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
            string sDummy = message.Trim();
            // status
            // 1 = Server (not included)
            // 2 = Client
            // 3 = Player
            if (sDummy.Length > 0)
            {
                lock (lstChat)
                {
                    if (lstChat.Count == 6)
                    {
                        lstChat.RemoveAt(0);
                    }
                    lstChat.Add(message + "&" + status);
                }
            }
        }

        public void settextures(string[] sTextures)
        {
            string[] sSplitTex;

            foreach (string sTexture in sTextures)
            {
                if (sTexture != "")
                {
                    sSplitTex = sTexture.Split(':');
                    textures.Add(sSplitTex[0], PlayerImage.ImageFromBase64String(sSplitTex[1]));
                    textures[sSplitTex[0]] = ResizeImage(textures[sSplitTex[0]]);
                }
            }
        }

        public void showstats(Boolean bShow)
        {
            bshowStats = bShow;
        }
    }
}
