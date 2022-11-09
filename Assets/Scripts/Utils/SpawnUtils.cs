using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils
{
    public static class SpawnUtils
    {
        public enum PointType { RespawnPoint, NetworkSpawnPoint, JumpToPoint };

        /// <summary>
        /// Вернет точку GameObject, которая соответствует всем прошлым критериям. Если всё это не может соответствовать
        /// вернет null.
        /// </summary>
        /// <param name="pointType"></param>
        /// <param name="pointName"></param>
        /// <param name="teamName"></param>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public static GameObject GetPoint(PointType pointType, string pointName = "",  string sceneName = "")
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                if (string.IsNullOrEmpty(pointName))
                {
                    List<GameObject> points = GameObject.FindGameObjectsWithTag(GetTag(pointType)).ToList();
                    if (points.Count > 0)
                        return points[Random.Range(0, points.Count)];
                }
                else
                {
                    return GameObject.FindGameObjectsWithTag(GetTag(pointType)).ToList().Find(x => x.name == pointName);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(pointName))
                {
                    // не указано pointName. Впрочем, название сцены было. Итак, найдите случайную точку в целевой сцене.
                    List<GameObject> points = GetAllPoints(pointType, sceneName);
                    if (points.Count > 0)
                        return points[Random.Range(0, points.Count)];
                }
                else
                {
                    return GetAllPoints(pointType, sceneName).Find(x => x.name == pointName);
                }
            }
            return null;
        }

        /// <summary>
        /// Получает все точки типа в текущей сцене (если она загружена).
        /// </summary>
        /// <param name="pointType"></param>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public static List<GameObject> GetAllPoints(PointType pointType, string sceneName)
        {
            List<GameObject> points = new List<GameObject>();
            foreach (GameObject target in SceneManager.GetSceneByName(sceneName).GetRootGameObjects())
            {
                points.AddRange(target.FindGameObjectsWithTag(GetTag(pointType)));
            }
            return points;
        }

        /// <summary>
        /// Возвращает имя целевого тега для указанного типа точки.
        /// </summary>
        /// <param name="pointType"></param>
        /// <returns></returns>
        public static string GetTag(PointType pointType)
        {
            switch (pointType)
            {
                case PointType.JumpToPoint:
                    return "JumpPoint";
                case PointType.NetworkSpawnPoint:
                    return "NetworkSpawnPoint";
                case PointType.RespawnPoint:
                    return "NetworkRespawnPoint";
                default:
                    return null;
            }
        }
    }

    #region Extension
    public static class Extensions
    {
        /// <summary>
        /// Рекурсивно ищет в этом игровом объекте любого дочернего элемента, который помечен целевым тегом
        /// </summary>
        /// <param name="target">Игровой объект, в котором нужно начать поиск всех дочерних элементов</param>
        /// <param name="tag">Тег для поиска</param>
        /// <returns>Список игровых объектов (дочерних или корневых), имеющих этот тег</returns>
        public static List<GameObject> FindGameObjectsWithTag(this GameObject target, string tag)
        {
            List<GameObject> targetChildren = new List<GameObject>();
            tag = tag.Trim();
            if (target.tag == tag)
            {
                targetChildren.Add(target);
            }
            for (int i = 0; i < target.transform.childCount; i++)
            {
                Transform targetChild = target.transform.GetChild(i);
                if (targetChild.tag == tag) // Этот дочерний объект имеет тег, который вы ищете
                {
                    targetChildren.Add(targetChild.gameObject);
                }
                if (targetChild.childCount > 0) //В этом ребенке больше дочерних объектов
                {
                    targetChildren.AddRange(targetChild.gameObject.FindGameObjectsWithTag(tag)); //рекурсивный поиск этого потомка и т.д.
                }
            }
            return targetChildren;
        }

        /// <summary>
        /// Очищает имя сцены
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetCleanSceneName(this string input)
        {
            if (input == null) return "";
            return input.Split('/').Last().Replace(".unity", "");
        }
    }
    #endregion
}
