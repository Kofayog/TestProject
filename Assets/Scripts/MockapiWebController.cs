using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public struct ButtonData
{
    public string color;
    public bool animationType;
    public string text;
    public int id;
}
public class MockapiWebController : IWebRequestController
{
    private const string API_URL = "https://65d483bd3f1ab8c63435564d.mockapi.io/button";

    public event Action<ButtonData> OnCreateSuccessful;
    public event Action<string> OnDeleteSuccessful;
    public event Action<ButtonData> OnUpdateSuccessful;
    public event Action<ButtonData> OnRefreshSuccessful;

    public async UniTask Create()
    {
        var request = new UnityWebRequest(API_URL, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes("");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        var operation = request.SendWebRequest();

        await operation;

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            var buttonType = JsonUtility.FromJson<ButtonData>(request.downloadHandler.text);
            OnCreateSuccessful?.Invoke(buttonType);

            Debug.Log("Data Created successfully!");
            Debug.Log("Response: " + request.downloadHandler.text);
        }
    }

    public async UniTask Delete(string id)
    {
        var url = string.Concat(API_URL, "/", id);
        var request = UnityWebRequest.Delete(url);
        var operation = request.SendWebRequest();

        await operation;

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            OnDeleteSuccessful?.Invoke(id);
            Debug.Log("Data Deleted successfully!");
        }
    }

    public async UniTask Update(string id)
    {
        var url = string.Concat(API_URL, "/", id);
        var request = UnityWebRequest.Put(url, "");
        var operation = request.SendWebRequest();

        await operation;

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            var buttonType = JsonUtility.FromJson<ButtonData>(request.downloadHandler.text);
            OnUpdateSuccessful?.Invoke(buttonType);
            Debug.Log("Data Updated successfully!");
            Debug.Log("Response: " + request.downloadHandler.text);
        }
    }
    
    public async UniTask Refresh(string id)
    {
        var url = GetURL(id);
        var request = UnityWebRequest.Get(url);
        var operation = request.SendWebRequest();

        await operation;

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            if (id == string.Empty)
            {
                var buttonTypes = JsonConvert.DeserializeObject<List<ButtonData>>(request.downloadHandler.text);

                foreach (var buttonType in buttonTypes)
                {
                    OnRefreshSuccessful?.Invoke(buttonType);
                    Debug.Log("Button: " + buttonType.text);
                }
            }
            else
            {
                var buttonType = JsonUtility.FromJson<ButtonData>(request.downloadHandler.text);
                OnRefreshSuccessful?.Invoke(buttonType);
            }
            
            Debug.Log("Data Refreshed successfully!");
            Debug.Log("Response: " + request.downloadHandler.text);
        }
    }
    
    private static string GetURL(string id)
    {
        return id == string.Empty ? API_URL : string.Concat(API_URL, "/", id);
    }
}
