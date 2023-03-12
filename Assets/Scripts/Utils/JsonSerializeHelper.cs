using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

public class JsonSerializeHelper 
{
    /// <summary>
    /// �������������� �������
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
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

    /// <summary>
    /// ����������� � ������
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <returns></returns>
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

    /// <summary>
    /// ��������� ������ ������������ ����� � ����
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="filename"></param>
    /// <param name="obj"></param>
    public static void SaveObjectToFile<T>(string filename, T obj) where T : class
    {
        try
        {
            // ����������� ������ � ������ JSON
            var json = Serialize(obj);

            // ��������� ���� ��� ������
            using (var stream = new FileStream(filename, FileMode.Create))
            {
                using (var writer = new StreamWriter(stream))
                {
                    // ���������� ��������������� ������ � ����
                    writer.Write(json);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save object to file '{filename}': {e.Message}");
        }
    }
}
