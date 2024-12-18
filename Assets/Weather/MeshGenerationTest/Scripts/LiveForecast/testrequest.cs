using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class testrequest : MonoBehaviour
{
    public TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        RequestWeatherData("https://weathermodelstadium.ue.r.appspot.com/",text);
    }
    public void RequestWeatherData(string url, TextMeshProUGUI textElement)
    {
        StartCoroutine(GetWeatherData(url, textElement));
    }

    private IEnumerator GetWeatherData(string url, TextMeshProUGUI textElement)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error requesting weather data: {request.error}");
                Debug.Log($"Response Code: {request.responseCode}");
                textElement.text = "Error retrieving data";
            }
            else
            {
                Debug.Log($"Weather Data Response: {request.downloadHandler.text}");
                textElement.text = request.downloadHandler.text;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
