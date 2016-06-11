using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pcbSimulation.nbt
{
    public class TagShort : AbstractTag, IEquatable<TagShort>
    {
        public readonly short value;
        public static TagShort parseNBT(string nbt, ref int index)
        {
            List<char> name = new List<char>();
            short value = 0;
            bool negative = false;
            bool state = true; //false: num, true: name
            bool finish = false;
            int length = nbt.Length;
            int start = index;
            int numStart = 0;
            int numEnd = -1;
            int typeMarker = -1;
            while (index < length && !finish)
            {
                if (name.Count > 65536)
                    throw new FormatException("the name is too long");
                if (state)
                {
                    if (nbt[index] != ':')
                    {
                        name.Add(nbt[index]);
                    }
                    else
                    {
                        state = false;
                        numStart = index + 1;
                    }
                }
                else
                {
                    char c = nbt[index];
                    switch (c)
                    {
                        case '-':
                            if (index == numStart)
                                negative = true;
                            else
                                throw new FormatException("not valid number, at char " + index.ToString());
                            break;
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                            if (negative && value > 0)
                            {
                                value *= -1;
                                negative = false;
                            }
                            value *= 10;
                            if (value >= 0)
                                value += short.Parse(c.ToString());
                            else
                                value -= short.Parse(c.ToString());
                            break;
                        case ']':
                        case '}':
                            index--;
                            numEnd = index;
                            finish = true;
                            break;
                        case ',':
                            numEnd = index - 1;
                            finish = true;
                            break;
                        case 's':
                            typeMarker = index;
                            break;
                        default:
                            throw new FormatException("not valid number, at char " + index.ToString());
                    }
                }
                index++;
            }
            if (!finish && state)
                throw new FormatException();            
            if (index != length && typeMarker != numEnd)
                throw new FormatException("type marker (s) in wrong position");
            return new TagShort(new string(name.ToArray()), value);
        }
        public TagShort(string name, short value) : base(name)
        {
            this.value = value;
        }
        public bool Equals(TagShort other)
        {
            return name == other.name && value == other.value;
        }
    }
}
