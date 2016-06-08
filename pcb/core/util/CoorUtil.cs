using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pcb.core.util
{
    public class CoorUtil
    {
        public static byte directionToCBDamage(Direction dir)
        {
            byte damage = (byte)0;
            switch (dir)
            {
                case Direction.positiveX:
                    damage = 5;
                    break;
                case Direction.negativeX:
                    damage = 4;
                    break;
                case Direction.positiveZ:
                    damage = 3;
                    break;
                case Direction.negativeZ:
                    damage = 2;
                    break;
                case Direction.positiveY:
                    damage = 1;
                    break;
                case Direction.negativeY:
                    damage = 0;
                    break;
            }
            return damage;
        }
        public static Direction CBDamageToDirection(int damage)
        {
            Direction dir = Direction.positiveX;
            switch (damage)
            {
                case 5:
                    dir = Direction.positiveX;
                    break;
                case 4:
                    dir = Direction.negativeX;
                    break;
                case 3:
                    dir = Direction.positiveZ;
                    break;
                case 2:
                    dir = Direction.negativeZ;
                    break;
                case 1:
                    dir = Direction.positiveY;
                    break;
                case 0:
                    dir = Direction.negativeY;
                    break;
            }
            return dir;
        }
        public static int[] move(int[] coor, Direction dir)
        {
            int[] result = (int[])coor.Clone();
            switch (dir)
            {
                case Direction.positiveX:
                    result[0]++;
                    break;
                case Direction.negativeX:
                    result[0]--;
                    break;
                case Direction.positiveZ:
                    result[2]++;
                    break;
                case Direction.negativeZ:
                    result[2]--;
                    break;
                case Direction.positiveY:
                    result[1]++;
                    break;
                case Direction.negativeY:
                    result[1]--;
                    break;
            }
            return result;
        }
        public static Direction inverseDirection(Direction dir)
        {
            return (Direction)(((int)dir) * -1);
        }
    }
}
