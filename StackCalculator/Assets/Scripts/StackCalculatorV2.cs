using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public class StackCalculatorV2
{
    Stack<double> stack = new Stack<double>();

    int index;
    StringBuilder valueStringBuilder = new StringBuilder();
    double tempValueA;
    double tempValueB;
    int calculationsUntilServiceCall = 10;

    public void EvaluateWithChanceOfRandom(string expression, RandomServiceV2 randomService, UnityAction<Stack<double>> callback)
    {
        calculationsUntilServiceCall--;
        if (calculationsUntilServiceCall == 0)
        {
            calculationsUntilServiceCall = 10;
            randomService.GetRandom(0, 100, 5, (result) =>
            {

            });
        }
        else
        {
            callback?.Invoke(Evaluate(expression));
        }
    }

    public Stack<double> Evaluate(string expression)
    {
            try
            {
                stack.Clear();

                index = 0;
                valueStringBuilder.Clear();

                while (index < expression.Length)
                {
                    if (double.TryParse(expression[index].ToString(), out tempValueA))
                    {
                        valueStringBuilder.Append(tempValueA);
                    }
                    else
                    {
                        ParseAndPushValue();
                        EvaluteOperator(expression[index]);
                    }
                    index++;
                }
                ParseAndPushValue();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw e;
            }
            return stack;
    }

    void ParseAndPushValue()
    {
        if (valueStringBuilder.Length > 0)
        {
            if (double.TryParse(valueStringBuilder.ToString(), out tempValueA))
            {
                stack.Push(tempValueA);
                valueStringBuilder.Clear();
            }
            else
            {
                throw new FormatException($"Failed to parse '{valueStringBuilder}' as integer");
            }
        }
    }

    void EvaluteOperator(char operatorSymbol)
    {
        switch (operatorSymbol)
        {
            case '+':
                EvaluateAdditionOperation();
                break;
            case '-':
                EvaluateSubtractionOperation();
                break;
            case '/':
                EvaluateDivisionOperation();
                break;
            case '*':
            case 'x':
                EvaluateMultiplicationOperation();
                break;
            case ' ':
                ParseAndPushValue();
                break;
            default:
                throw new NotImplementedException($"Failed to evaluate operator '{operatorSymbol}'");
        }
    }

    void EvaluateAdditionOperation()
    {
        if (stack.TryPop(out tempValueA) && stack.TryPop(out tempValueB))
        {
            stack.Push(tempValueA + tempValueB);
        } else
        {
            throw new ArithmeticException("Failed to pop values from stack for addition operation");
        }
    }

    void EvaluateSubtractionOperation()
    {
        if (stack.TryPop(out tempValueA) && stack.TryPop(out tempValueB))
        {
            stack.Push(tempValueA - tempValueB);
        }
        else
        {
            throw new ArithmeticException("Failed to pop values from stack for subtraction operation");
        }
    }

    void EvaluateMultiplicationOperation()
    {
        if (stack.TryPop(out tempValueA) && stack.TryPop(out tempValueB))
        {
            stack.Push(tempValueA * tempValueB);
        }
        else
        {
            throw new ArithmeticException("Failed to pop values from stack for multiplication operation");
        }
    }

    void EvaluateDivisionOperation()
    {
        if (stack.TryPop(out tempValueA) && stack.TryPop(out tempValueB))
        {
            if (tempValueB == 0)
            {
                throw new DivideByZeroException("Cannot divide by zero");
            }
            stack.Push(tempValueA / tempValueB);
        }
        else
        {
            throw new ArithmeticException("Failed to pop values from stack for division operation");
        }
    }
}
