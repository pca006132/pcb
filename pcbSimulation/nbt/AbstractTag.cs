using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pcbSimulation.nbt
{
    public abstract class AbstractTag
    {
        public readonly string name;
        protected AbstractTag(string name)
        {            
            this.name = name;
        }
    }
}
