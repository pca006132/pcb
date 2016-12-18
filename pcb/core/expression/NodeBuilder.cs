using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace pcb.core.expression
{
    public class NodeBuilder
    {
        public static void insertUpper(Node modify, Node insert)
        {
            insert.setLeft(modify);
            if (modify.getParent() != null)
            {
                insert.setParent(modify.getParent());
                if (modify.getParent().getLeft() == modify)
                {
                    modify.getParent().setLeft(insert);
                }
                else {
                    modify.getParent().setRight(insert);
                }
            }
            modify.setParent(insert);
        }
        public static void insertLower(Node modify, Node insert)
        {
            insert.setParent(modify);

            if (modify.getRight() != null)
            {
                insert.setLeft(modify.getRight());
                modify.getRight().setParent(insert);
            }

            modify.setRight(insert);
        }
        private enum Token
        {
            operate, constant, selector, fakePlayer, bracket, none
        }
        Match matcher;

        Token lastToken = Token.none;
        Token currentToken;
        public NodeBuilder(String input)
        {
            matcher = tokens.Match(input);

        }
        public Node buildNode()
        {
            Stack<Node> bracketNode = new Stack<Node>();
            Stack<Operator> operatorStack = new Stack<Operator>();
            Operator lastOperator = Operator.none;
            Node currentNode = null;

            string token;
            while ((token = getNextToken()) != "")
            {
                Node newNode;
                switch (currentToken)
                {
                    case Token.operate:
                        Operator _operator = OperatorMethod.parseOperator(token);

                        if (lastToken == Token.operate || lastToken == Token.none)
                        {
                            if (_operator == Operator.minus)
                            {
                                newNode = new OperatorNode(Operator.multiply);
                                if (currentNode == null)
                                {
                                    currentNode = new ConstantNode("-1");
                                    insertUpper(currentNode, newNode);
                                }
                                else {
                                    currentNode.setRight(new ConstantNode("-1"));
                                    insertLower(currentNode, newNode);
                                }
                            }
                            else
                                throw new Exception("invalid syntax");
                        }
                        else
                        {
                            if (currentNode == null)
                                throw new Exception("invalid syntax");
                            newNode = new OperatorNode(_operator);
                            if (lastOperator == Operator.none || !OperatorMethod.isLarger
                                (_operator, lastOperator))
                            {
                                insertUpper(currentNode, newNode);
                            }
                            else
                            {
                                insertLower(currentNode, newNode);
                            }
                        }
                        lastOperator = _operator;
                        currentNode = newNode;
                        break;
                    case Token.constant:
                        if (lastToken != Token.operate && lastToken != Token.none && lastToken != Token.bracket)
                        throw new Exception("invalid syntax");
                        newNode = new ConstantNode(token);
                        if (currentNode == null)
                        {
                            currentNode = newNode;
                        }
                        else {
                            currentNode.setRight(newNode);
                        }
                        break;
                    case Token.selector:
                        if (lastToken != Token.operate && lastToken != Token.none && lastToken != Token.bracket)
                        throw new Exception("invalid syntax");
                        string[] pair = token.Split('.');
                        newNode = new EntityNode(pair[0], pair[1]);
                        if (currentNode == null)
                        {
                            currentNode = newNode;
                        }
                        else {
                            currentNode.setRight(newNode);
                        }
                        break;
                    case Token.fakePlayer:
                        if (lastToken != Token.operate && lastToken != Token.none && lastToken != Token.bracket)
                        throw new Exception("invalid syntax");
                        string[] pair2 = token.Split('.');
                        newNode = new FakePlayerNode(pair2[0], pair2[1]);
                        if (currentNode == null)
                        {
                            currentNode = newNode;
                        }
                        else {
                            currentNode.setRight(newNode);
                        }
                        break;
                    case Token.bracket:
                        if (token == "(")
                        {
                            bracketNode.Push(currentNode);
                            operatorStack.Push(lastOperator);
                            currentNode = null;
                            lastOperator = Operator.none;
                            continue;
                        }
                        else {
                            if (bracketNode.Count == 0)
                                throw new Exception("imbalance bracket");
                            else {
                                Node _node = bracketNode.Pop();
                                if (currentNode != null)
                                {
                                    while (currentNode.getParent() != null)
                                        currentNode = currentNode.getParent();
                                    insertLower(_node, currentNode);
                                    currentNode = _node;
                                }
                                lastOperator = operatorStack.Pop();
                            }
                        }
                        break;
                }
                lastToken = currentToken;
            }
            Node node = currentNode;
            if (node == null)
                return null;
            while (node.getParent() != null)
            {
                node = node.getParent();
            }

            if (!node.isComplete())
                throw new Exception("missing value(fake player/selector/constant)");
            node.checkLast();

            return node;
        }
        private string getNextToken()
        {
            string captured = matcher.ToString();
            if (matcher.Groups["operator"].Length > 0 )
                currentToken = Token.operate;
            else if (matcher.Groups["constant"].Length > 0)
                currentToken = Token.constant;
            else if (matcher.Groups["selector"].Length > 0)
                currentToken = Token.selector;
            else if (matcher.Groups["fakePlayer"].Length > 0)
                currentToken = Token.fakePlayer;
            else if (matcher.Groups["bracket"].Length > 0)
                currentToken = Token.bracket;
            else if (matcher.Groups["error"].Length > 0)
                throw new Exception("illegal token at " + matcher.Index.ToString());

            matcher = matcher.NextMatch();
            while (matcher.Groups["space"].Length > 0)
                matcher = matcher.NextMatch();

            return captured;
        }
        private static Regex tokens = new Regex("(?<space>\\s)|(?<constant>-?\\d+)|" +
            "(?<operator>(\\+=|-=|\\*=|/=|%=|\\+|-|\\*|/|%|=))|" +
            "(?<selector>(@[aepr](\\[[^.\\s]+\\])?)\\.(\\w+))|" +
            "(?<bracket>[()])|(?<fakePlayer>(\\w+)\\.(\\w+))|(?<error>.+)");
    }
}
