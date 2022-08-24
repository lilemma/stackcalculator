using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class RandomService : MonoBehaviour
{
    const string API_URL = "https://csrng.net/csrng/csrng.php";

    private void Start()
    {
        GetRandom(0, 100, OnGetRandom);
    }

    void OnGetRandom(ApiResponse<JsonArray<RandomModel>> response)
    {
        if (response is ApiSuccess<JsonArray<RandomModel>>)
        {
            Debug.Log("Success: " + ((ApiSuccess<JsonArray<RandomModel>>)response).Result.items[0]);
        } 
        else if (response is ApiFailed<JsonArray<RandomModel>>)
        {
            Debug.Log("Failed: " + ((ApiFailed<JsonArray<RandomModel>>)response).Message);
        }
    }

    public void GetRandom(int min, int max, UnityAction<ApiResponse<JsonArray<RandomModel>>> callback)
    {
        StartCoroutine(ProcessGetRandom(min, max, callback));
    }

    private IEnumerator ProcessGetRandom(int min, int max, UnityAction<ApiResponse<JsonArray<RandomModel>>> callback)
    {
        var uriBuilder = new UriBuilder(API_URL);
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
                    try
                    {
                        var result = (JsonArray<RandomModel>)JsonUtility.FromJson<JsonArray<RandomModel>>("{\"items\":" + request.downloadHandler.text + "}");
                        if (result != null)
                        {
                            callback?.Invoke(new ApiSuccess<JsonArray<RandomModel>>(result));
                        }
                        else
                        {
                            throw new NullReferenceException("Failed to parse");
                        }
                    }
                    catch (Exception e)
                    {
                        callback?.Invoke(new ApiFailed<JsonArray<RandomModel>>($"JsonParseException '{uriBuilder}': {request.downloadHandler.text}, exception: {e}"));

                    }                    
                    break;
                case UnityWebRequest.Result.ConnectionError:
                    callback?.Invoke(new ApiFailed<JsonArray<RandomModel>>($"ConnectionError '{uriBuilder}' {request.error}"));
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    callback?.Invoke(new ApiFailed<JsonArray<RandomModel>>($"ProtocolError '{uriBuilder}' {request.error}"));

                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    callback?.Invoke(new ApiFailed<JsonArray<RandomModel>>($"DataProcessingError '{uriBuilder}' {request.error}"));
                    break;
                default:
                    callback?.Invoke(new ApiFailed<JsonArray<RandomModel>>($"Unknown '{uriBuilder}' {request.error}"));
                    break;
            }
        }
    }
}

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

[Serializable]
public class JsonArray<T>
{
    public T[] items;
}