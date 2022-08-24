using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using System.Threading.Tasks;


public class RandomServiceV2 : MonoBehaviour
{
    const string API_URL = "http://www.randomnumberapi.com/api/v1.0/random?min=100&max=1000&count=5";

    private void Start()
    {
        GetRandom(0, 100, 5, OnGetRandom);
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

    public void GetRandom(int min, int max, int count, UnityAction<ApiResponse<JsonArray<RandomModel>>> callback)
    {
        StartCoroutine(ProcessGetRandom(min, max, count, callback));
    }

    private IEnumerator ProcessGetRandom(int min, int max, int count, UnityAction<ApiResponse<JsonArray<RandomModel>>> callback)
    {
        var uriBuilder = new UriBuilder(API_URL);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query.Add("min", min.ToString());
        query.Add("max", max.ToString());
        query.Add("count", count.ToString());
        uriBuilder.Query = query.ToString();

        using (UnityWebRequest request = UnityWebRequest.Get(uriBuilder.ToString()))
        {
            yield return request.SendWebRequest();

            switch (request.result)
            {
                case UnityWebRequest.Result.Success:
                    try
                    {
                        var result = JsonUtility.FromJson<JsonArray<RandomModel>>("{\"items\":" + request.downloadHandler.text + "}");
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