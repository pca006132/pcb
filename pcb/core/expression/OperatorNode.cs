using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using static pcb.core.expression.Operator;
using static pcb.core.expression.OperatorMethod;


namespace pcb.core.expression
{
    public class OperatorNode : Node
    {
        public Operator _operator;
        private EntityNode temp;
        public OperatorNode(Operator _operator)
        {
            this._operator = _operator;
        }
        public override List<string> getCommands()
        {
            List<string> leftCommands = getLeft().getCommands();
            List<string> rightCommands = getRight().getCommands();

            EntityNode left = getLeft().getTemp();
            EntityNode right = getRight().getTemp();
            bool changeable = left.isLast;
            List<string> operationCommands = new List<string>();
            operationCommands.AddRange(leftCommands);
            operationCommands.AddRange(rightCommands);
            switch (_operator)
            {
                case plus:
                    if (!changeable && right.isLast)
                    {
                        EntityNode temp = left;
                        left = right;
                        right = temp;
                        changeable = left.isLast;
                    }

                    if (changeable)
                    {
                        if (getRight() is ConstantNode)
                        {
                            operationCommands.Add(string.Format("scoreboard players Add {0} {1} {2}",
                                    left.selector, left.scbObj, right.selector));
                        }
                        else {
                            operationCommands.Add(string.Format("scoreboard players operation {0} {1} += {2} {3}",
                                    left.selector, left.scbObj, right.selector, right.scbObj));
                        }
                        temp = left;
                    }
                    else {
                        temp = new TempNode();
                        operationCommands.Add(string.Format("scoreboard players operation {0} {1} = {2} {3}",
                                temp.selector, temp.scbObj, left.selector, left.scbObj));
                        operationCommands.Add(string.Format("scoreboard players operation {0} {1} += {2} {3}",
                                temp.selector, temp.scbObj, right.selector, right.scbObj));
                    }
                    break;
                case minus:
                    if (changeable)
                    {
                        if (getRight() is ConstantNode)
                        {
                            operationCommands.Add(string.Format("scoreboard players remove {0} {1} {2}",
                                    left.selector, left.scbObj, right.selector));
                        }
                        else {
                            operationCommands.Add(string.Format("scoreboard players operation {0} {1} -= {2} {3}",
                                    left.selector, left.scbObj, right.selector, right.scbObj));
                        }
                        temp = left;
                    }
                    else {
                        temp = new TempNode();
                        operationCommands.Add(string.Format("scoreboard players operation {0} {1} = {2} {3}",
                                temp.selector, temp.scbObj, left.selector, left.scbObj));
                        operationCommands.Add(string.Format("scoreboard players operation {0} {1} -= {2} {3}",
                                temp.selector, temp.scbObj, right.selector, right.scbObj));
                    }
                    break;
                case multiply:
                    if (!changeable && right.isLast)
                    {
                        EntityNode temp = left;
                        left = right;
                        right = temp;
                        changeable = left.isLast;
                    }

                    if (changeable)
                    {
                        operationCommands.Add(string.Format("scoreboard players operation {0} {1} *= {2} {3}",
                            left.selector, left.scbObj, right.selector, right.scbObj));

                        temp = left;
                    }
                    else {
                        temp = new TempNode();
                        operationCommands.Add(string.Format("scoreboard players operation {0} {1} = {2} {3}",
                                temp.selector, temp.scbObj, left.selector, left.scbObj));
                        operationCommands.Add(string.Format("scoreboard players operation {0} {1} *= {2} {3}",
                                temp.selector, temp.scbObj, right.selector, right.scbObj));
                    }
                    break;
                case divide:
                    if (changeable)
                    {
                        operationCommands.Add(string.Format("scoreboard players operation {0} {1} /= {2} {3}",
                                left.selector, left.scbObj, right.selector, right.scbObj));

                        temp = left;
                    }
                    else {
                        temp = new TempNode();
                        operationCommands.Add(string.Format("scoreboard players operation {0} {1} = {2} {3}",
                                temp.selector, temp.scbObj, left.selector, left.scbObj));
                        operationCommands.Add(string.Format("scoreboard players operation {0} {1} /= {2} {3}",
                                temp.selector, temp.scbObj, right.selector, right.scbObj));
                    }
                    break;
                case mod:
                    if (changeable)
                    {
                        operationCommands.Add(string.Format("scoreboard players operation {0} {1} %= {2} {3}",
                                left.selector, left.scbObj, right.selector, right.scbObj));

                        temp = left;
                    }
                    else {
                        temp = new TempNode();
                        operationCommands.Add(string.Format("scoreboard players operation {0} {1} = {2} {3}",
                                temp.selector, temp.scbObj, left.selector, left.scbObj));
                        operationCommands.Add(string.Format("scoreboard players operation {0} {1} %= {2} {3}",
                                temp.selector, temp.scbObj, right.selector, right.scbObj));
                    }
                    break;
                case assign:
                    if (left.selector != right.selector || left.scbObj != right.scbObj)
                    {
                        operationCommands.Add(string.Format("scoreboard players operation {0} {1} = {2} {3}",
                                left.selector, left.scbObj, right.selector, right.scbObj));
                        temp = left;
                    }
                    break;
                case plus_assign:
                    if (getRight() is ConstantNode)
                    {
                        operationCommands.Add(string.Format("scoreboard players Add {0} {1} {2}",
                                left.selector, left.scbObj, right.selector));
                    }
                    else {
                        operationCommands.Add(string.Format("scoreboard players operation {0} {1} += {2} {3}",
                                left.selector, left.scbObj, right.selector, right.scbObj));
                    }
                    temp = left;
                    break;
                case minus_assign:
                    if (getRight() is ConstantNode)
                    {
                        operationCommands.Add(string.Format("scoreboard players remove {0} {1} {2}",
                                left.selector, left.scbObj, right.selector));
                    }
                    else {
                        operationCommands.Add(string.Format("scoreboard players operation {0} {1} -= {2} {3}",
                                left.selector, left.scbObj, right.selector, right.scbObj));
                    }
                    temp = left;
                    break;
                case multiply_assign:
                    operationCommands.Add(string.Format("scoreboard players operation {0} {1} *= {2} {3}",
                            left.selector, left.scbObj, right.selector, right.scbObj));

                    temp = left;
                    break;
                case divide_assign:
                    operationCommands.Add(string.Format("scoreboard players operation {0} {1} /= {2} {3}",
                            left.selector, left.scbObj, right.selector, right.scbObj));

                    temp = left;
                    break;
                case mod_assign:
                    operationCommands.Add(string.Format("scoreboard players operation {0} {1} %= {2} {3}",
                            left.selector, left.scbObj, right.selector, right.scbObj));

                    temp = left;
                    break;

            }
            return operationCommands;
        }
        public override EntityNode getTemp()
        {
            return temp;
        }

        public override string[] toLines(int level)
        {
            List<string> lines = new List<string>();

            string currentLine = "";
            for (int i = 0; i < level; i++)
                currentLine += "    ";
            lines.Add(currentLine + _operator.ToString());
            if (getLeft() != null)
                lines.AddRange(getLeft().toLines(level + 1));
            if (getRight() != null)
                lines.AddRange(getRight().toLines(level + 1));
            return lines.ToArray();
        }
        public override void setIsLast(List<string[]> fakeplayerseditable)
        {
            if (_operator == Operator.assign)
            {
                if (getLeft().GetType() == typeof(FakePlayerNode))
                {
                    fakeplayerseditable.Add(new string[] {((FakePlayerNode)getLeft()).selector,
                    ((FakePlayerNode)getLeft()).scbObj});
                }
                else if (getLeft().GetType() != typeof(EntityNode))
                    throw new Exception("cannot assign to constant/expression");
            }
            getRight().setIsLast(fakeplayerseditable);
            getLeft().setIsLast(fakeplayerseditable);
        }
    }
}
