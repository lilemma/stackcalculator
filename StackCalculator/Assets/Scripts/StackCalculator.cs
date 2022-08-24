using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class StackCalculator
{
    Stack<double> stack = new Stack<double>();

    int index;
    double tempValueA;
    double tempValueB;
    int calculationsUntilServiceCall = 10;

    public void EvaluateWithChanceOfRandom(string expression, RandomService randomService, UnityAction<Stack<double>> onSuccess, UnityAction<string> onFailure)
    {
        try
        {
            calculationsUntilServiceCall--;

            if (calculationsUntilServiceCall == 0)
            {
                calculationsUntilServiceCall = 10;
                randomService.GetRandom(0, 100, 5, (response) =>
                {
                    OnGetRandom(response, onSuccess, onFailure);
                });
            }
            else if (expression == null || expression.Trim().Length == 0)
            {
                onFailure.Invoke("Expression is empty");
            }
            else
            {
                onSuccess?.Invoke(Evaluate(expression.Replace(" ", "")));
            }
        }
        catch (Exception ex)
        {
            onFailure?.Invoke(ex.Message);
        }
    }

    private void OnGetRandom(ApiResponse<JsonArray<int>> response, UnityAction<Stack<double>> onSuccess, UnityAction<string> onFailure)
    {
        if (response is ApiSuccess<JsonArray<int>>)
        {
            var items = ((ApiSuccess<JsonArray<int>>)response).Result.items;
            onSuccess?.Invoke(new Stack<double>(items.Select(x => (double)x).ToList()));
        }
        else if (response is ApiFailed<JsonArray<int>>)
        {
            var message = ((ApiFailed<JsonArray<int>>)response).Message;
            Debug.Log($"Random Response Failed: {message}");
            onFailure?.Invoke(message);
        }
    }

    public Stack<double> Evaluate(string expression)
    {
        try
        {
            stack.Clear();
            index = 0;

            while (index < expression.Length)
            {
                if (double.TryParse(expression[index].ToString(), out tempValueA))
                {
                    stack.Push(tempValueA);
                }
                else
                {
                    EvaluteOperator(expression[index]);
                }
                index++;
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            throw e;
        }
        return stack;
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
            default:
                throw new NotImplementedException($"Failed to evaluate operator '{operatorSymbol}'");
        }
    }

    void EvaluateAdditionOperation()
    {
        if (stack.TryPop(out tempValueA) && stack.TryPop(out tempValueB))
        {
            stack.Push(tempValueA + tempValueB);
        }
        else
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
