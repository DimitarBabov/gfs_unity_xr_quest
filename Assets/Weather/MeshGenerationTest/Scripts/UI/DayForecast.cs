using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//[ExecuteInEditMode]
public class DayForecast : MonoBehaviour
{
    
    public enum Day { Mon, Tue, Wed, Thu, Fri, Sat, Sun }
    /*public*/ Day day;
    public TextMeshProUGUI dayText;
    public string strInitTempWind = "Day:Tue-H:27-L:67-W:12-WGusts:34-WindDir:SSE-WeatherCondition:Cloudy3";

    public enum WeatherCondition
    {
        cloudy1, cloudy1_night, cloudy2, cloudy2_night, cloudy3, cloudy3_night, cloudy4_night, cloudy5,
        cloudyltrain, dunno, fog, fog_night, hail, light_rain, mist, mist_night, shower1, shower1_night,
        shower2, shower2_night, shower3, sleet, snow1, snow1_night, snow2, snow2_night, snow3,
        snow3_night, snow4, snow5, sunny_night, tstorm1, tstorm1_night, tstorm2, tstorm2_night, tstorm3, Wind
    }
    /*public*/ WeatherCondition weatherCondition;
    public Image weatherIcon;

    public DailyForecastIconsContainer iconContainer;
    public TextMeshProUGUI highTempText;
    public TextMeshProUGUI lowTempText;
    public TextMeshProUGUI windSpeedText;
    public TextMeshProUGUI windGustText;

    public enum WindDirection { S, E, W, N, SE, SSE, SW, WNW, NNW, NE, ENE, ESE, SSW, NNE, WSW,NW }
    public WindDirection windDirection;
    public TextMeshProUGUI windDirTitle;
    public Image windDirectionIcon;

    private void Start()
    {
        UpdateUI();
    }
    private void OnEnable()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        // Update Day Text
        //dayText.text = day.ToString();

        //update wind and temp
        TextInit(strInitTempWind);
        /*
        // Update Weather Icon
        Sprite selectedSprite = GetWeatherSprite();
        if (selectedSprite != null)
        {
            weatherIcon.sprite = selectedSprite;
        }

        // Update Wind Direction Icon
        Sprite selectedWindSprite = GetWindDirectionSprite();
        if (selectedWindSprite != null)
        {
            windDirectionIcon.sprite = selectedWindSprite;
        }
        */
        // You can add additional code here to update the highTempText, lowTempText, etc.
    }

    private Sprite GetWeatherSprite()
    {
        switch (weatherCondition)
        {
            case WeatherCondition.cloudy1: return iconContainer.cloudy1;
            case WeatherCondition.cloudy1_night: return iconContainer.cloudy1_night;
            case WeatherCondition.cloudy2: return iconContainer.cloudy2;
            case WeatherCondition.cloudy2_night: return iconContainer.cloudy2_night;
            case WeatherCondition.cloudy3: return iconContainer.cloudy3;
            case WeatherCondition.cloudy3_night: return iconContainer.cloudy3_night;
            case WeatherCondition.cloudy4_night: return iconContainer.cloudy4_night;
            case WeatherCondition.cloudy5: return iconContainer.cloudy5;
            case WeatherCondition.cloudyltrain: return iconContainer.cloudyltrain;
            case WeatherCondition.dunno: return iconContainer.dunno;
            case WeatherCondition.fog: return iconContainer.fog;
            case WeatherCondition.fog_night: return iconContainer.fog_night;
            case WeatherCondition.hail: return iconContainer.hail;
            case WeatherCondition.light_rain: return iconContainer.light_rain;
            case WeatherCondition.mist: return iconContainer.mist;
            case WeatherCondition.mist_night: return iconContainer.mist_night;
            case WeatherCondition.shower1: return iconContainer.shower1;
            case WeatherCondition.shower1_night: return iconContainer.shower1_night;
            case WeatherCondition.shower2: return iconContainer.shower2;
            case WeatherCondition.shower2_night: return iconContainer.shower2_night;
            case WeatherCondition.shower3: return iconContainer.shower3;
            case WeatherCondition.sleet: return iconContainer.sleet;
            case WeatherCondition.snow1: return iconContainer.snow1;
            case WeatherCondition.snow1_night: return iconContainer.snow1_night;
            case WeatherCondition.snow2: return iconContainer.snow2;
            case WeatherCondition.snow2_night: return iconContainer.snow2_night;
            case WeatherCondition.snow3: return iconContainer.snow3;
            case WeatherCondition.snow3_night: return iconContainer.snow3_night;
            case WeatherCondition.snow4: return iconContainer.snow4;
            case WeatherCondition.snow5: return iconContainer.snow5;
            case WeatherCondition.sunny_night: return iconContainer.sunny_night;
            case WeatherCondition.tstorm1: return iconContainer.tstorm1;
            case WeatherCondition.tstorm1_night: return iconContainer.tstorm1_night;
            case WeatherCondition.tstorm2: return iconContainer.tstorm2;
            case WeatherCondition.tstorm2_night: return iconContainer.tstorm2_night;
            case WeatherCondition.tstorm3: return iconContainer.tstorm3;
            case WeatherCondition.Wind: return iconContainer.Wind;
            default: return null;
        }
    }

    private Sprite GetWindDirectionSprite()
    {
        switch (windDirection)
        {
            case WindDirection.S: return iconContainer.S;
            case WindDirection.E: return iconContainer.E;
            case WindDirection.W: return iconContainer.W;
            case WindDirection.N: return iconContainer.N;
            case WindDirection.SE: return iconContainer.SE;
            case WindDirection.SSE: return iconContainer.SSE;
            case WindDirection.SW: return iconContainer.SW;
            case WindDirection.WNW: return iconContainer.WNW;
            case WindDirection.NNW: return iconContainer.NNW;
            case WindDirection.NE: return iconContainer.NE;
            case WindDirection.ENE: return iconContainer.ENE;
            case WindDirection.ESE: return iconContainer.ESE;
            case WindDirection.SSW: return iconContainer.SSW;
            case WindDirection.NNE: return iconContainer.NNE;
            case WindDirection.WSW: return iconContainer.WSW;
            case WindDirection.NW: return iconContainer.NW;
            default: return null;
        }
    }
    public void TextInit(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            Debug.LogWarning("TextInit called with null or empty input");
            return;
        }

        string[] parts = input.Split('-');
        Debug.Log(string.Join('-' ,parts));
        foreach (string part in parts)
        {
            if (string.IsNullOrEmpty(part))
                continue;
            Debug.Log(part);
            if(part.StartsWith("Day:"))
            {
                dayText.text = part.Split(':')[1];
            }
            else if (part.StartsWith("H:"))
            {
                highTempText.text = part;
            }
            else if (part.StartsWith("L:"))
            {
                lowTempText.text = part;
            }
            else if (part.StartsWith("W:"))
            {
                windSpeedText.text = part.Substring(2) + " mph";
            }
            else if (part.StartsWith("WGusts:"))
            {
                windGustText.text = part.Substring(7) + " mph gusts";
            }
            else if (part.StartsWith("WindDir:"))
            {
                string directionStr = part.Substring(8);
                Debug.Log("xxxWindDir:" + directionStr);
                if (System.Enum.TryParse(directionStr, true, out WindDirection direction))  // case-insensitive parsing
                {
                    windDirection = direction;
                    windDirectionIcon.sprite = GetWindDirectionSprite();
                    windDirTitle.text = directionStr;
                    Debug.Log(directionStr);
                }
                else
                {
                    Debug.LogWarning("Failed to parse WindDir: " + directionStr);
                }
            }
            else if (part.StartsWith("WeatherCondition:"))
            {
                string conditionStr = part.Substring(17);
                if (System.Enum.TryParse(conditionStr, true, out WeatherCondition condition))  // case-insensitive parsing
                {
                    weatherCondition = condition;
                    weatherIcon.sprite = GetWeatherSprite();
                }
                else
                {
                    Debug.LogWarning("Failed to parse WeatherCondition: " + conditionStr);
                }
            }
        }
    }


}