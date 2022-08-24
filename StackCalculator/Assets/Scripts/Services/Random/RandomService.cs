using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class RandomService : MonoBehaviour
{
    const string API_URL = "http://www.randomnumberapi.com/api/v1.0/random";

    public void GetRandom(int min, int max, int count, UnityAction<ApiResponse<JsonArray<int>>> callback)
    {
        StartCoroutine(ProcessGetRandom(min, max, count, callback));
    }

    private IEnumerator ProcessGetRandom(int min, int max, int count, UnityAction<ApiResponse<JsonArray<int>>> callback)
    {
        var uriBuilder = new UriBuilder(API_URL);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query.Add("min", min.ToString());
        query.Add("max", max.ToString());
        query.Add("count", count.ToString());
        uriBuilder.Query = query.ToString();

        using (UnityWebRequest request = UnityWebRequest.Get(uriBuilder.ToString()))
        {
            Debug.Log($"Sending Web Request... {request.uri}");
            yield return request.SendWebRequest();

            switch (request.result)
            {
                case UnityWebRequest.Result.Success:
                    try
                    {
                        Debug.Log($"Web Request Response Body: {request.downloadHandler.text}");

                        var result = JsonUtility.FromJson<JsonArray<int>>(request.downloadHandler.text.ToJsonArrayFormat());
                        if (result != null)
                        {
                            callback?.Invoke(new ApiSuccess<JsonArray<int>>(result));
                        }
                        else
                        {
                            throw new NullReferenceException("Failed to parse object from json");
                        }
                    }
                    catch (Exception e)
                    {
                        callback?.Invoke(new ApiFailed<JsonArray<int>>($"JsonParseException '{uriBuilder}': {request.downloadHandler.text}, exception: {e}"));

                    }
                    break;
                case UnityWebRequest.Result.ConnectionError:
                    callback?.Invoke(new ApiFailed<JsonArray<int>>($"ConnectionError '{uriBuilder}' {request.error}"));
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    callback?.Invoke(new ApiFailed<JsonArray<int>>($"ProtocolError '{uriBuilder}' {request.error}"));
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    callback?.Invoke(new ApiFailed<JsonArray<int>>($"DataProcessingError '{uriBuilder}' {request.error}"));
                    break;
                default:
                    callback?.Invoke(new ApiFailed<JsonArray<int>>($"Unknown '{uriBuilder}' {request.error}"));
                    break;
            }
        }
    }
}