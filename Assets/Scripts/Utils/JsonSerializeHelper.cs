using Newtonsoft.Json;
using System;
using UnityEngine;

public class JsonSerializeHelper 
{
    public static T Deserialize<T>(string value) where T : class
    {
        try
        {
            var ret = JsonConvert.DeserializeObject<T>(value);
            return ret;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return null;
        }
    }

    public static string Serialize<T>(T type) where T : class
    {
        try
        {
            var output = JsonConvert.SerializeObject(type);
            return output;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
