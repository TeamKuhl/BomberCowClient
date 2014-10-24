using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private PictureBox Player2PictureBox = new PictureBox();

        // Size ofe the textures
        public int BlockSize = 32;
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

            Bitmap map = new Bitmap(MapXSize * BlockSize, MapYSize * BlockSize);
            Graphics g = Graphics.FromImage(map);
            Size imgSize = new Size(BlockSize, BlockSize);
            Image img1 = BomberCowClient.Properties.Resources.png1;
            Image img2 = BomberCowClient.Properties.Resources.png2;
            Image back = BomberCowClient.Properties.Resources.back;
            Image player = BomberCowClient.Properties.Resources.Player1;

            img1 = ResizeImage(img1, imgSize);//, true);
            img2 = ResizeImage(img2, imgSize);
            back = ResizeImage(back, imgSize);
            player = ResizeImage(player, imgSize);

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
            foreach (Player oplayer in mainForm.players)
            {
                g.DrawImage(player, new Point((oplayer.xPosition - 1) * BlockSize, (oplayer.yPosition - 1) * BlockSize));
            }

            g.Dispose();
            img1.Dispose();
            img2.Dispose();

            //map.Save("map.png", System.Drawing.Imaging.ImageFormat.Png);

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
    }
}
