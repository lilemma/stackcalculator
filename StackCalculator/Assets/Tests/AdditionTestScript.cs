using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class AdditionTestScript
{
    [Test]
    public void AdditionTestScriptOperationPasses()
    {
        var calculator = new StackCalculator();

        EvaluateExpression(calculator, "5 5+", 1, 10);
        EvaluateExpression(calculator, "5 5+10+", 1, 20);
        EvaluateExpression(calculator, "55 10+", 1, 65);
        EvaluateExpression(calculator, "1 1 + 1 1 ++", 1, 4);
    }

    void EvaluateExpression(StackCalculator calculator, string expression, int expectedStackSize, int expectedPeek)
    {
        var result = calculator.Evaluate(expression);
        Debug.Assert(
            result.Count == expectedStackSize && result.Peek() == expectedPeek,
            $"Failed to evaluate expression '{expression}' Size: {result.Count} (expected '{expectedStackSize}') Peek: {result.Peek()} (expected '{expectedPeek}')");
    }
}
