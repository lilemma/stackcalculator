using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class StackCalculator
{
    Stack<int> stack = new Stack<int>();

    int index;
    StringBuilder valueStringBuilder = new StringBuilder();
    int tempIntA;
    int tempIntB;

    public Stack<int> Evaluate(string expression)
    {
        try
        {
            stack.Clear();

            index = 0;
            valueStringBuilder.Clear();

            while (index < expression.Length)
            {
                if (int.TryParse(expression[index].ToString(), out tempIntA))
                {
                    valueStringBuilder.Append(tempIntA);
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
        }
        return stack;
    }

    void ParseAndPushValue()
    {
        if (valueStringBuilder.Length > 0)
        {
            if (int.TryParse(valueStringBuilder.ToString(), out tempIntA))
            {
                stack.Push(tempIntA);
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
        if (stack.TryPop(out tempIntA) && stack.TryPop(out tempIntB))
        {
            stack.Push(tempIntA + tempIntB);
        }
        else
        {
            throw new ArithmeticException("Failed to pop values from stack for addition operation");
        }
    }

    void EvaluateSubtractionOperation()
    {
        if (stack.TryPop(out tempIntA) && stack.TryPop(out tempIntB))
        {
            stack.Push(tempIntA - tempIntB);
        }
        else
        {
            throw new ArithmeticException("Failed to pop values from stack for subtraction operation");
        }
    }

    void EvaluateMultiplicationOperation()
    {
        if (stack.TryPop(out tempIntA) && stack.TryPop(out tempIntB))
        {
            stack.Push(tempIntA * tempIntB);
        }
        else
        {
            throw new ArithmeticException("Failed to pop values from stack for multiplication operation");
        }
    }

    void EvaluateDivisionOperation()
    {
        if (stack.TryPop(out tempIntA) && stack.TryPop(out tempIntB))
        {
            if (tempIntB == 0)
            {
                throw new DivideByZeroException("Cannot divide by zero");
            }
            stack.Push(tempIntA / tempIntB);
        }
        else
        {
            throw new ArithmeticException("Failed to pop values from stack for division operation");
        }
    }
}
