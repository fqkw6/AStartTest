using UnityEngine;
using System.Collections;

public enum WeatherType
{
    NONE,
    RAIN,
    WIND,
    FOG,
    SNOW,
    FLARE,      // 雨后的光晕
}

// 天气系统
public class WeatherController : MonoBehaviour {
    

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void PlayWeather(WeatherType wtype)
    {

    }

    public void StopWeather()
    {
        
    }
}
