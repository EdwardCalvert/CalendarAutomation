using System;
using Newtonsoft.Json;

namespace MonitorBrightnessDriver
{
    public class WeatherResponse
    {
    }
}

public class WeatherObject
{
    public Weather[] weather { get; set; }
    public Source[] sources { get; set; }
}

public class Weather
{
    [JsonProperty(PropertyName = "timestamp")]
    public DateTime TimeStamp { get; set; }

    [JsonProperty(PropertyName = "source_id")]
    public int SourceId { get; set; }

    [JsonProperty(PropertyName = "precipitation")]
    public float? Precipitation { get; set; }

    [JsonProperty(PropertyName = "pressure_msl")]
    public float PressureMsl { get; set; }

    [JsonProperty(PropertyName = "sunshine")]
    public float? Sunshine { get; set; }

    [JsonProperty(PropertyName = "temperature")]
    public float Temperature { get; set; }

    [JsonProperty(PropertyName = "wind_direction")]
    public int WindDirection { get; set; }

    [JsonProperty(PropertyName = "wind_speed")]
    public float WindSpeed { get; set; }

    [JsonProperty(PropertyName = "cloud_cover")]
    public int? CloudCover { get; set; }

    [JsonProperty(PropertyName = "dew_point")]
    public float DewPoint { get; set; }

    [JsonProperty(PropertyName = "relative_humidity")]
    public int? RelativeHumidity { get; set; }

    [JsonProperty(PropertyName = "visibility")]
    public int Visibility { get; set; }

    [JsonProperty(PropertyName = "wind_gust_direction")]
    public object WindGustDirection { get; set; }

    [JsonProperty(PropertyName = "wind_gust_speed")]
    public float? WindGustSpeed { get; set; }

    [JsonProperty(PropertyName = "condition")]
    public string Condition { get; set; }

    [JsonProperty(PropertyName = "icon")]
    public string Icon { get; set; }

    [JsonProperty(PropertyName = "fallback_source_ids")]
    public Fallback_Source_Ids FallbackSourceIds { get; set; }
}

public class Fallback_Source_Ids
{
    public int wind_gust_speed { get; set; }
    public int sunshine { get; set; }
    public int condition { get; set; }
    public int cloud_cover { get; set; }
    public int precipitation { get; set; }
}

public class Source
{
    public int id { get; set; }
    public object dwd_station_id { get; set; }
    public string observation_type { get; set; }
    public float lat { get; set; }
    public float lon { get; set; }
    public float height { get; set; }
    public string station_name { get; set; }
    public string wmo_station_id { get; set; }
    public DateTime first_record { get; set; }
    public DateTime last_record { get; set; }
    public float distance { get; set; }
}
