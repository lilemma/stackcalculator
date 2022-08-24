using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringExtensions
{
    public static string ToJsonArrayFormat(this string str)
    {
        return "{\"items\":" + str + "}";
    }
}