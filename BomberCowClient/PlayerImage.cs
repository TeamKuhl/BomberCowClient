using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace BomberCowClient
{
    class PlayerImage
    {
        public Image ImageFromBase64String(string base64)
        {
            MemoryStream memory = new MemoryStream(Convert.FromBase64String(base64));
            Image result = Image.FromStream(memory);
            memory.Close();

            return result;
        }
    }
}
