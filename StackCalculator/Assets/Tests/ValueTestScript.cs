using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ValueTestScript
{
    [Test]
    public void ValueTestScriptParsingPasses()
    {
        var calculator = new StackCalculator();

        EvaluateExpression(calculator, "1", new int[] { 1 });
        EvaluateExpression(calculator, "11", new int[] { 11 });
        EvaluateExpression(calculator, "1 1", new int[] { 1, 1});
        EvaluateExpression(calculator, "50 1 200 5 6000", new int[] { 6000, 5, 200, 1, 50 });
        EvaluateExpression(calculator, "1234567890", new int[] { 1234567890 });
        EvaluateExpression(calculator, "1234567890 0", new int[] { 0, 1234567890 });
    }

    void EvaluateExpression(StackCalculator calculator, string expression, int[] expectedValues)
    {
        var result = calculator.Evaluate(expression);
        int value;

        foreach(var expectedValue in expectedValues)
        {
            Debug.Assert(result.TryPop(out value), $"Failed to pop for expected value '{expectedValue}'");
            {
                Debug.Assert(value == expectedValue, $"Value '{value}' is not expected value '{expectedValue}' in expression '{expression}'");
            } 
        }
    }
}
