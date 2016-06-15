using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace pcb.core
{
    public class Marker
    {
        string type;
        string name;
        int[] coor;
        string[] tags;
        public Marker(string type, string name, int[] coor, string[] tags)
        {
            if (type != "ArmorStand" && type != "AreaEffectCloud")
                //should never happen
                throw new PcbException("unknown marker type");            
            if (coor.Length != 3)
                //should never happen
                throw new PcbException("wrong coor");
            this.type = type;
            this.name = name;
            this.coor = coor;
            this.tags = tags;            
        }
        public override string ToString()
        {
            string nbt = "";
            if (type.Equals("ArmorStand"))
            {
                nbt = String.Format(@"CustomName:{0},NoGravity:1b,Invisible:1b",name);
            } else if (type.Equals("AreaEffectCloud"))
            {
                nbt = String.Format(@"CustomName:{0},Duration:2147483647", name);
            }
            StringBuilder sb = new StringBuilder(nbt);
            if (tags.Length > 0)
            {
                sb.Append(",Tags:[");
                foreach (string tag in tags)
                {
                    sb.Append(tag);
                    sb.Append(",");
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append("]");
            }            

            return String.Format(@"summon {0} ~{1} ~{2} ~{3} {{{4}}}", type, coor[0], coor[1], coor[2] ,sb.ToString());
        }
    }
}
