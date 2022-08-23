using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField]
    TMP_InputField inputField;
    
    [SerializeField]
    TMP_Text outputText;

    string expression;

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
        } else
        {
            expression = String.Empty;
        }
        inputField.text = expression;
    }

    public void Evaluate()
    {
        try
        {
            var stackCalculator = new StackCalculator();
            var result = stackCalculator.Evaluate(expression);
            var sb = new StringBuilder();
            while (result.Count > 0)
            {
                sb.Insert(0, $"[{result.Pop()}]"); // Note to self, remember to pop not peek
            }

            outputText.text = $"RESULT\n{sb}";
        } catch (Exception ex)
        {
            outputText.text = $"<color=red>INVALID EXPRESSION</color>\n{ex.Message}";
        }
    }
}