using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace life
{
    class Actor
    {
        public readonly char type;
        public Point<float> pos = new Point<float>(0, 0);
        public Actor(char type = 'A')
        {
            this.type = type;
        }

        public override string ToString()
        {
            return string.Format("{0}, ({1})", type, pos);
        }
    }
}
