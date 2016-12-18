using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pcb.core.expression
{
    public enum Operator
    {
        plus, minus, multiply, divide, mod, assign, plus_assign, minus_assign, multiply_assign, divide_assign, mod_assign, none
    }
    public class OperatorMethod
    {
        public static Operator parseOperator(string _operator)
        {
            switch (_operator) {
                case "+":
                    return Operator.plus;
                case "-":
                    return Operator.minus;
                case "*":
                    return Operator.multiply;
                case "/":
                    return Operator.divide;
                case "%":
                    return Operator.mod;
                case "=":
                    return Operator.assign;
                case "+=":
                    return Operator.plus_assign;
                case "-=":
                    return Operator.minus_assign;
                case "*=":
                    return Operator.multiply_assign;
                case "/=":
                    return Operator.divide_assign;
                case "%=":
                    return Operator.mod_assign;
                default: throw new Exception("unknown operator");
            }
        }
        public static bool isLarger(Operator a, Operator b)
        {
            switch (a)
            {
                case Operator.assign:
                case Operator.plus_assign:
                case Operator.minus_assign:
                case Operator.multiply_assign:
                case Operator.divide_assign:
                case Operator.mod_assign:
                    return false;
                case Operator.plus:
                case Operator.minus:
                    return b == Operator.assign || b == Operator.plus_assign || b == Operator.minus_assign
                        || b == Operator.multiply_assign || b == Operator.divide_assign ||
                        b == Operator.mod_assign;
                default:
                    switch (b)
                    {
                        case Operator.multiply:
                        case Operator.divide:
                        case Operator.mod:
                            return false;
                        default:
                            return true;
                    }
            }
        }
    }
}
