using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace BomberCowClient
{
    class BomberMap
    {
        private Form mainForm;
        private PictureBox AllPictureBox = new PictureBox();

        public BomberMap(Form1 mainform)
        {
            this.mainForm = mainform;
        }

        public void createMap(string MapString)
        {
            AllPictureBox.Location = new Point(10, 10);
            AllPictureBox.Name = "MapPictureBox";
            AllPictureBox.Size = new Size(8 * 50, 8 * 50);
            AllPictureBox.Visible = true;
            AllPictureBox.BackgroundImage = BomberCowClient.Properties.Resources.back;
            mainForm.Controls.Add(AllPictureBox);

            // split map rows
            String[] rows = MapString.Split(';');

            // set height
            //this.height = rows.Length;

            // set up row counter
            int rowCounter = 0;

            for (int yCounter = 0; yCounter < 8; yCounter++)
            {
                String dummy = rows[yCounter];
                String[] currow = dummy.Split(':');
                for (int xCounter = 0; xCounter < 8; xCounter++)
                {
                    if (currow[xCounter] == "0")
                    {
                        createBlock(xCounter, yCounter, 0);
                    }
                    if (currow[xCounter] == "2")
                    {
                        createBlock(xCounter, yCounter, 1);
                    }
                }
            }
        }

        private void createBlock(int xCount, int yCount, int img)
        {
            PictureBox MapPictureBox = new PictureBox();
            MapPictureBox.Location = new Point(xCount * 50, yCount * 50);
            MapPictureBox.Name = "MapPictureBox_" + xCount + "_" + yCount;
            MapPictureBox.Size = new Size(50, 50);
            MapPictureBox.Visible = true;
            if (img == 0)
            {
                MapPictureBox.BackgroundImage = BomberCowClient.Properties.Resources.png1;
            }
            if (img == 1)
            {
                MapPictureBox.BackgroundImage = BomberCowClient.Properties.Resources.png2;
            }
            AllPictureBox.Controls.Add(MapPictureBox);
        }
    }
}
