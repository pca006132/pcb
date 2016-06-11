using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pcbSimulation.nbt
{
    public class TagInt : AbstractTag, IEquatable<TagInt>
    {
        public readonly int value;
        public static TagInt parseNBT(string nbt, ref int index)
        {
            List<char> name = new List<char>();
            int value = 0;
            bool negative = false;
            bool state = true; //false: num, true: name
            bool finish = false;
            int length = nbt.Length;
            int start = index;
            int numStart = 0;
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
                } else
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
                                value += int.Parse(c.ToString());
                            else
                                value -= int.Parse(c.ToString());
                            break;
                        case ']':
                        case '}':
                            index--;
                            finish = true;
                            break;
                        case ',':
                            finish = true;
                            break;
                        default:
                            throw new FormatException("not valid number, at char " + index.ToString());
                    }
                }
                index++;
            }
            if (!finish && state)
                throw new FormatException();
            return new TagInt(new string(name.ToArray()), value);
        }
        public TagInt(string name, int value) : base(name)
        {
            this.value = value;
        }
        public bool Equals(TagInt other)
        {
            return name == other.name && value == other.value;
        }
    }
}
