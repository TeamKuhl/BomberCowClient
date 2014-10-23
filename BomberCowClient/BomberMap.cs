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
        private Form1 mainForm;
        private PictureBox AllPictureBox = new PictureBox();
        private PictureBox Player1PictureBox = new PictureBox();

        public int BlockSize = 32;
        private int MapXSize = 8;
        private int MapYSize;

        public BomberMap(Form1 mainform)
        {
            this.mainForm = mainform;
        }

        public void createMap(string MapString)
        {
            Image mapimg = drawMap(MapString);
            if (AllPictureBox.InvokeRequired)
            {
                AllPictureBox.Invoke(new emptyFunction(delegate()
                    {
                        AllPictureBox.Location = new Point(10, 10);
                        AllPictureBox.Name = "MapPictureBox";
                        AllPictureBox.Size = new Size(MapXSize * BlockSize, MapYSize * BlockSize);
                        AllPictureBox.Visible = true;
                        AllPictureBox.BackgroundImage = mapimg;
                    }));
            }
            else
            {
                AllPictureBox.Location = new Point(10, 10);
                AllPictureBox.Name = "MapPictureBox";
                AllPictureBox.Size = new Size(MapXSize * BlockSize, MapYSize * BlockSize);
                AllPictureBox.Visible = true;
                AllPictureBox.BackgroundImage = mapimg;
            }
            mainForm.Invoke(new emptyFunction(delegate() { mainForm.Controls.Add(AllPictureBox); }));
            
            //setPlayerPosition(1,1,1);
        }

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

            g.Clear(Color.Black);
            for (int yCounter = 0; yCounter < MapYSize; yCounter++)
            {
                dummy = rows[yCounter];
                currow = dummy.Split(':');
                for (int xCounter = 0; xCounter < MapXSize; xCounter++)
                {
                    if (currow[xCounter] == "0")
                    {
                        g.DrawImage(img1, new Point(xCounter*BlockSize, yCounter*BlockSize));
                    }
                    if (currow[xCounter] == "1")
                    {
                        g.DrawImage(back, new Point(xCounter * BlockSize, yCounter * BlockSize));
                    }
                    if (currow[xCounter] == "2")
                    {
                        g.DrawImage(img2, new Point(xCounter*BlockSize, yCounter*BlockSize));
                    }
                }
            }
            g.Dispose();
            img1.Dispose();
            img2.Dispose();
            
            map.Save("map.png", System.Drawing.Imaging.ImageFormat.Png);
            return map;
            //map.Dispose();
        }

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
        }

        public void setPlayerPosition(int Player, int xPosition, int yPosition)
        {
            if (Player == 1)
            {
                Player1PictureBox.Invoke(new emptyFunction(delegate() { Player1PictureBox.Location = new Point(xPosition * BlockSize, yPosition * BlockSize); }));
            }
        }
    }
}
