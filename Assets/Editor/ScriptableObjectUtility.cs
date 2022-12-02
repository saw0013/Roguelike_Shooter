using UnityEditor;
using UnityEngine;

public static class ScriptableObjectUtility 
{
    /// <summary>
    /// Создайте новый ассет из <see cref="ScriptableObject"/> типа с уникальным именем в
    /// выбранная папка в окне проекта. Создание актива можно отменить, нажав
    /// экранирующая клавиша при первоначальном названии актива.
    /// </summary>
    /// <typeparam Name="T">Тип объекта для сценария.</typeparam>
    /// 
    public static void CreateAsset<T>() where T : ScriptableObject
    {
        var asset = ScriptableObject.CreateInstance<T>();
        ProjectWindowUtil.CreateAsset(asset, "New " + typeof(T).Name + ".asset");
    }
}
