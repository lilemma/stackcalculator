using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ApiResponse<T>
{
}

public class ApiSuccess<T> : ApiResponse<T>
{
    public ApiSuccess(T result)
    {
        Result = result;
    }

    public T Result { get; private set; }
}

public class ApiFailed<T> : ApiResponse<T>
{
    public ApiFailed(string message)
    {
        Message = message;
    }

    public string Message { get; private set; }
}