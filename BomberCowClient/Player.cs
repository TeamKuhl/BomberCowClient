using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberCowClient
{
    public class Player
    {
        public string ID;
        public string Name;
        public int xPosition;
        public int yPosition;
        public int State; //1 = alive 2 = dead
        public int PlayerState;
    }
}
