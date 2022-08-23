using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class RandomService : MonoBehaviour
{
    const string API_URL = "http://www.randomnumberapi.com/api/v1.0";

    private void Start()
    {
        GetRandom(0, 100, OnGetRandom);
    }

    void OnGetRandom(ApiResult<int> result)
    {
        if (result is ApiSuccess<int>)
        {
            Debug.Log("Success: " + ((ApiSuccess<int>)result).result);
        } 
        else if (result is ApiFailed<int>)
        {
            Debug.Log("Failed: " + ((ApiFailed<int>)result).message);
        }
    }

    public void GetRandom(int min, int max, UnityAction<ApiResult<int>> callback)
    {
        StartCoroutine(ProcessGetRandom(min, max, callback));
    }

    private IEnumerator ProcessGetRandom(int min, int max, UnityAction<ApiResult<int>> callback)
    {
        var uriBuilder = new UriBuilder(API_URL);
        uriBuilder.Path += "/random";
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query.Add("min", min.ToString());
        query.Add("max", max.ToString());
        uriBuilder.Query = query.ToString();

        using (UnityWebRequest request = UnityWebRequest.Get(uriBuilder.ToString()))
        {
            yield return request.SendWebRequest();

            switch (request.result)
            {
                case UnityWebRequest.Result.Success:
                    int result;
                    if (int.TryParse(request.downloadHandler.text, out result))
                    {
                        callback?.Invoke(new ApiSuccess<int>(result));
                    }
                    else
                    {
                        callback?.Invoke(new ApiFailed<int>($"Failed to parse: {request.downloadHandler.text}"));
                    }
                    break;
                case UnityWebRequest.Result.ConnectionError:
                    callback?.Invoke(new ApiFailed<int>($"ConnectionError '{uriBuilder}' {request.error}"));
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    callback?.Invoke(new ApiFailed<int>($"ProtocolError '{uriBuilder}' {request.error}"));

                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    callback?.Invoke(new ApiFailed<int>($"DataProcessingError '{uriBuilder}' {request.error}"));
                    break;
                default:
                    callback?.Invoke(new ApiFailed<int>($"Unknown '{uriBuilder}' {request.error}"));
                    break;
            }
        }
    }
}

public abstract class ApiResult<T>
{
}

public class ApiSuccess<T> : ApiResult<T>
{
    public ApiSuccess(T result)
    {
        this.result = result;
    }

    public T result { get; private set; }
}

public class ApiFailed<T> : ApiResult<T>
{
    public ApiFailed(string message)
    {
        this.message = message;
    }

    public string message { get; private set; }
}