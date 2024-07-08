using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using Newtonsoft.Json; // Newtonsoft.Json kütüphanesini kullanın

public class RestClient : MonoBehaviour
{
    private string baseUrl = "http://localhost:3000"; // API URL

    void Start()
    {
        // Verileri çekme
        StartCoroutine(GetData());

        // Yeni veri gönderme
        Dictionary<string, object> newData = new Dictionary<string, object>
        {
            {"name", "ExampleName"},
            {"value", 123}
        };
       // StartCoroutine(PostData(newData));
    }

    IEnumerator GetData()
    {
        UnityWebRequest request = UnityWebRequest.Get(baseUrl + "/data");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            Debug.Log(request.downloadHandler.text);
        }
    }

    IEnumerator PostData(Dictionary<string, object> data)
    {
        string json = JsonConvert.SerializeObject(data);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(baseUrl + "/data", "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            Debug.Log(request.downloadHandler.text);
        }
    }
}
