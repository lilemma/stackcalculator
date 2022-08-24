using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RandomModel
{
    public string message;
    public int min;
    public int max;
    public int random;

    public override string ToString()
    {
        return $"Message: {message} Min: {min} Max: {max} Random: {random}";
    }
}
