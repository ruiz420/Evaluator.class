namespace Evaluator.Core;

public static class ExpressionEvaluator
{
    public static double Evaluate(string infix)
    {
        var postfix = InfixToPostfix(infix);
        return calculate(postfix);
    }

    private static string InfixToPostfix(string infix) // POSTFIJA
    {
        var stack = new Stack<char>();
        var postfix = string.Empty;  //=> de aca o de esta manera podemos crear el evaluador en calculador 

        foreach (char item in infix)
        {
            if (isOperator(item))  // operador (signos +,-, etc)
            {
                if (item == ')')
                {
                    do
                    {
                        postfix += stack.Pop();
                    } while (stack.Peek() != '(');
                    stack.Pop(); // sacar el '(' de la pila
                }
                else
                {
                    if (stack.Count > 0)
                    {
                        if (priorityInfix(item) > prioritystack(stack.Peek())) // mayor prioridad
                        {
                            stack.Push(item);
                        }
                        else // menor o igual prioridad
                        {
                            postfix += stack.Pop();
                            stack.Push(item);
                        }
                    }
                    else
                    {
                        stack.Push(item);
                    }
                }
            }
            else // operando (números)
            {
                postfix += item;
            }
        }

        while (stack.Count > 0) // si el stack no está vacío
        {
            postfix += stack.Pop(); // sacar todo lo que queda en la pila
        }

        return postfix;
    }

    private static bool isOperator(char item) => item is '^' or '/' or '*' or '%' or '+' or '-' or '(' or ')';

    private static int priorityInfix(char op) => op switch // INFIJA - prioridad de los operadores
    {
        '^' => 4,
        '*' or '/' or '%' => 2,
        '-' or '+' => 1,
        '(' => 5,
        _ => throw new Exception("Invalid Expression."),
    };

    private static int prioritystack(char op) => op switch // PILA
    {
        '^' => 3,
        '*' or '/' or '%' => 2,
        '-' or '+' => 1,
        '(' => 0,
        _ => throw new Exception("Invalid Expression."),
    };

    private static double calculate(string postfix)
    {
        var stack = new Stack<double>();
        foreach (char item in postfix)
        {
            if (isOperator(item))
            {
                var op2 = stack.Pop();
                var op1 = stack.Pop();
                stack.Push(calculate(op1, item, op2));
            }
            else
            {
                stack.Push(Convert.ToDouble(item.ToString()));
            }
        }
        return stack.Peek();
    }

    private static double calculate(double op1, char item, double op2)
    {
        return item switch
        {
            '*' => op1 * op2,
            '/' => op1 / op2,
            '^' => Math.Pow(op1, op2),
            '+' => op1 + op2,
            '-' => op1 - op2,
            _ => throw new Exception("Invalid Expression."),
        };
    }

}

