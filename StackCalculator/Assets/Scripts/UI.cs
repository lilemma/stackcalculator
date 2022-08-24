using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RandomService))]
public class UI : MonoBehaviour
{
    [SerializeField]
    TMP_InputField inputField;

    [SerializeField]
    TMP_Text outputText;

    [SerializeField]
    Button evaluateButton;

    StackCalculator stackCalculator = new StackCalculator();
    string expression;
    bool isEvaluating = false;

    public void AddToExpression(string value)
    {
        expression += $"{value} ";
        inputField.text = expression;
    }

    public void Clear()
    {
        expression = string.Empty;
        inputField.text = expression;
    }

    public void Undo()
    {
        var index = expression.LastIndexOf(" ");
        if (index != -1)
        {
            expression = expression.Substring(0, index - 1);
        }
        else
        {
            expression = String.Empty;
        }
        inputField.text = expression;
    }

    public void Evaluate()
    {
        if (isEvaluating)
        {
            return;
        }

        isEvaluating = true;
        evaluateButton.interactable = false;
        stackCalculator.EvaluateWithChanceOfRandom(expression, GetComponent<RandomService>(), OnEvaluationSuccess, OnEvaluationFailed);
    }

    private void OnEvaluationSuccess(Stack<double> result)
    {
        var sb = new StringBuilder();
        while (result.Count > 0)
        {
            sb.Insert(0, $"[{result.Pop()}]"); // Note to self, remember to pop not peek
        }

        outputText.text = $"RESULT\n{sb}";

        isEvaluating = false;
        evaluateButton.interactable = true;
    }

    private void OnEvaluationFailed(string message)
    {
        outputText.text = $"<color=red>INVALID EXPRESSION</color>\n{message}";

        isEvaluating = false;
        evaluateButton.interactable = true;

    }
}