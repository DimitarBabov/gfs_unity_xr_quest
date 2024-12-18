using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;  // Import TextMeshPro namespace
using SimpleJSON;
using System.Collections.Generic;  // Import SimpleJSON for JSON parsing

public class WeatherRequest : MonoBehaviour
{
    public bool useInternet = true;
    //public TMP_InputField zipCodeInputField;  // Reference to the InputField for ZIP code
    public TMP_Text responseText;             // Reference to a TextMeshPro Text element to display the response
    public string initZipCode = "08401";
    // URLs for weather data and forecast
    public string apiUrl = "https://weathermodelstadium.ue.r.appspot.com//weather/zip?zip=";  // Local server URL for current weather
    public string forecastApiUrl = "https://weathermodelstadium.ue.r.appspot.com//weather/forecast/zip?zip="; // Local server URL for forecast

    //public string apiUrl = "http://127.0.0.1:5000//weather/zip?zip=";  // Local server URL for current weather
    //public string forecastApiUrl = "http://127.0.0.1:5000//weather/forecast/zip?zip="; //
    private void Start()
    {
        //zipCodeInputField.text = initZipCode;
        OnGetWeatherButtonClick();
    }
    // Function called when the user clicks the "Get Weather" button
    public void OnGetWeatherButtonClick()
    {
        if (!useInternet)
        {
            //responseText.text = PlayerPrefs.GetString("SavedWeatherData", "No saved data available");
            return;
        }
        // Get the ZIP code from the input field
        string zipCode = initZipCode;// zipCodeInputField.text;

        // If the ZIP code is not empty, start the request
        if (!string.IsNullOrEmpty(zipCode))
        {
            string requestUrl = apiUrl + zipCode;
            StartCoroutine(GetWeatherData(requestUrl,false));
        }
        else
        {
            responseText.text = "Please enter a valid ZIP code.";
        }
    }
    public void OnGetWeatherDetailedButtonClick()
    {
        // Get the ZIP code from the input field
        string zipCode = initZipCode;// zipCodeInputField.text;

        // If the ZIP code is not empty, start the request
        if (!string.IsNullOrEmpty(zipCode))
        {
            string requestUrl = apiUrl + zipCode;
            StartCoroutine(GetWeatherData(requestUrl,true));
        }
        else
        {
            responseText.text = "Please enter a valid ZIP code.";
        }
    }

    // Function called when the user clicks the "Get Forecast" button
    public void OnForecastButtonClick()
    {
        // Get the ZIP code from the input field
        string zipCode = initZipCode;// zipCodeInputField.text;

        // If the ZIP code is not empty, start the request
        if (!string.IsNullOrEmpty(zipCode))
        {
            string requestUrl = forecastApiUrl + zipCode;
            StartCoroutine(GetForecastData(requestUrl));
        }
        else
        {
            responseText.text = "Please enter a valid ZIP code.";
        }
    }

    // Coroutine to make an HTTP request and get the weather data
    IEnumerator GetWeatherData(string url, bool isDetailed)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // Send the request and wait for a response
            yield return request.SendWebRequest();

            // Check for network errors
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                responseText.text ='*'+ PlayerPrefs.GetString("SavedWeatherData", "No saved data available in the prefrences");

                Debug.LogError("Error: " + request.error);
                responseText.text = "Error: " + request.error;
            }
            else
            {
                // Get the JSON response
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("Weather Data: " + jsonResponse);

                // Parse the weather data
                var jsonData = SimpleJSON.JSON.Parse(jsonResponse);

                // Get the station name directly from the server response
                string stationName = jsonData["stationName"];

                
                responseText.text = FormatJsonResponse(jsonData, stationName, useInternet);
            }
        }
    }

    // Coroutine to make an HTTP request and get the forecast data
    IEnumerator GetForecastData(string url)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // Send the request and wait for a response
            yield return request.SendWebRequest();

            // Check for network errors
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
                responseText.text = "Error: " + request.error;
            }
            else
            {
                // Get the JSON response
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("Forecast Data: " + jsonResponse);

                // Parse the forecast data
                var jsonData = SimpleJSON.JSON.Parse(jsonResponse);

                //Need to  Format and display the forecast data
                Debug.Log(responseText.text);
            }
        }
    }

    // Function to format the JSON response for weather data
    string FormatJsonResponse(JSONNode jsonData, string stationName, bool useInternet)
    {
        string formattedResponse;

       
        {
            // Create formatted response from live data
            formattedResponse = $"Live Weather Data\n Zip:{initZipCode}\n{stationName.Split(',')[0]}\n";

            // Extract temperature in Celsius and convert to Fahrenheit
            float temperatureCelsius = jsonData["temperature"]["value"].AsFloat;
            float temperatureFahrenheit = (temperatureCelsius * 9 / 5) + 32;

            // Extract wind speed, humidity, pressure, and precipitation
            float windSpeed = jsonData["windSpeed"]["value"].AsFloat * 0.621371f;
            float windGusts = jsonData["windGust"]["value"].AsFloat * 0.621371f;

            float humidity = jsonData["relativeHumidity"]["value"].AsFloat;
            float pressure = jsonData["barometricPressure"]["value"].AsFloat / 100;
            float precipitation = jsonData["precipitationLast3Hours"]["value"].AsFloat* 0.0393701f;

            // Extract dew point in Celsius and convert to Fahrenheit if available
            string dewPoint = "N/A";
            if (jsonData.HasKey("dewpoint") && jsonData["dewpoint"].HasKey("value"))
            {
                float dewPointCelsius = jsonData["dewpoint"]["value"].AsFloat;
                float dewPointFahrenheit = (dewPointCelsius * 9 / 5) + 32;
                dewPoint = dewPointFahrenheit.ToString("F1") + " °F";
            }

            // Extract heat index in Celsius and convert to Fahrenheit if available
            string heatIndex = "N/A";
            if (jsonData.HasKey("heatIndex") && jsonData["heatIndex"].HasKey("value"))
            {
                float heatIndexCelsius = jsonData["heatIndex"]["value"].AsFloat;
                float heatIndexFahrenheit = (heatIndexCelsius * 9 / 5) + 32;
                heatIndex = heatIndexFahrenheit.ToString("F1") + " °F";
            }

            // Format the response to display the values in Fahrenheit
            formattedResponse += $"Temperature: {temperatureFahrenheit:F1} °F\n";
            if (windSpeed > 0)
                formattedResponse += $"Wind Speed: {windSpeed:F1} mph\n";
            if (humidity > 0)
                formattedResponse += $"Humidity: {humidity:F1} %\n";
            if (pressure > 0)
                formattedResponse += $"Barometric Pressure: {pressure:F1} hPa\n";
            if (precipitation > 0)
                formattedResponse += $"Precipitation (last 3 hours): {precipitation:F1} in\n";
            if (dewPoint != "N/A")
                formattedResponse += $"Dew Point: {dewPoint}\n";
            if (heatIndex != "N/A")
                formattedResponse += $"Heat Index: {heatIndex}\n";

            // Save formatted response to PlayerPrefs for offline use
            PlayerPrefs.SetString("SavedWeatherData", formattedResponse);

        }

        return formattedResponse;
    }




}
