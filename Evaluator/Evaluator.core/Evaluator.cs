using System.Globalization;

namespace Evaluator.Core;

public class ExpressionEvaluator
{
    public static double Evaluate(string infix)
    {
        var postfix = InfixToPostfix(infix);
        return Calulate(postfix);
    }

    private static string InfixToPostfix(string infix)
    {
        var stack = new Stack<char>();
        var postfix = string.Empty;
        var number = string.Empty;

        foreach (char item in infix)
        {
            if ((item >= '0' && item <= '9') || item == '.')
            {
                number += item;
            }
            else
            {
                if (number != string.Empty)
                {
                    postfix += number + " ";
                    number = string.Empty;
                }

                if (IsOperator(item))
                {
                    if (item == ')')
                    {
                        do
                        {
                            postfix += stack.Pop() + " ";
                        } while (stack.Peek() != '(');
                        stack.Pop();
                    }
                    else
                    {
                        if (stack.Count > 0)
                        {
                            if (PriorityInfix(item) > PriorityStack(stack.Peek()))
                            {
                                stack.Push(item);
                            }
                            else
                            {
                                postfix += stack.Pop() + " ";
                                stack.Push(item);
                            }
                        }
                        else
                        {
                            stack.Push(item);
                        }
                    }
                }
                else if (item == '(')
                {
                    stack.Push(item);
                }
                else if (!char.IsWhiteSpace(item))
                {
                    throw new Exception($"Invalid character in infix: {item}");
                }
            }
        }

        if (number != string.Empty)
        {
            postfix += number + " ";
        }

        while (stack.Count > 0)
        {
            var op = stack.Pop();
            if (op == '(' || op == ')')
                throw new Exception("Mismatched parentheses.");
            postfix += op + " ";
        }

        return postfix;
    }

    public static double Calulate(string postfix)
    {
        var stack = new Stack<double>();
        var buffer = string.Empty;

        foreach (char item in postfix)
        {
            if ((item >= '0' && item <= '9') || item == '.')
            {
                buffer += item;
            }
            else if (item == ' ')
            {
                if (buffer != string.Empty)
                {
                    stack.Push(double.Parse(buffer, NumberStyles.Any, CultureInfo.InvariantCulture));
                    buffer = string.Empty;
                }
            }
            else if (IsOperator(item))
            {
                if (buffer != string.Empty)
                {
                    stack.Push(double.Parse(buffer, NumberStyles.Any, CultureInfo.InvariantCulture));
                    buffer = string.Empty;
                }

                if (stack.Count < 2)
                    throw new Exception("Invalid expression: insufficient operands.");

                var op2 = stack.Pop();
                var op1 = stack.Pop();
                stack.Push(Calulate(op1, item, op2));
            }
            else
            {
                throw new Exception($"Invalid character in postfix: {item}");
            }
        }

        if (buffer != string.Empty)
        {
            stack.Push(double.Parse(buffer, NumberStyles.Any, CultureInfo.InvariantCulture));
        }

        if (stack.Count != 1)
        {
            throw new Exception("Invalid expression: leftover operands.");
        }

        return stack.Peek();
    }

    private static bool IsOperator(char item) => item is '^' or '/' or '*' or '%' or '+' or '-' or '(' or ')';

    private static int PriorityInfix(char op) => op switch
    {
        '^' => 4,
        '*' or '/' or '%' => 2,
        '-' or '+' => 1,
        '(' => 5,
        _ => throw new Exception("Invalid expression."),
    };

    private static int PriorityStack(char op) => op switch
    {
        '^' => 3,
        '*' or '/' or '%' => 2,
        '-' or '+' => 1,
        '(' => 0,
        _ => throw new Exception("Invalid expression."),
    };

    private static double Calulate(double op1, char item, double op2) => item switch
    {
        '*' => op1 * op2,
        '/' => op1 / op2,
        '^' => Math.Pow(op1, op2),
        '+' => op1 + op2,
        '-' => op1 - op2,
        '%' => op1 % op2,
        _ => throw new Exception("Invalid expression."),
    };
}