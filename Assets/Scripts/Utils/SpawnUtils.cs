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
        /// ������ ����� GameObject, ������� ������������� ���� ������� ���������. ���� �� ��� �� ����� ���������������
        /// ������ null.
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
                    // �� ������� pointName. �������, �������� ����� ����. ����, ������� ��������� ����� � ������� �����.
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
        /// �������� ��� ����� ���� � ������� ����� (���� ��� ���������).
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
        /// ���������� ��� �������� ���� ��� ���������� ���� �����.
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
        /// ���������� ���� � ���� ������� ������� ������ ��������� ��������, ������� ������� ������� �����
        /// </summary>
        /// <param name="target">������� ������, � ������� ����� ������ ����� ���� �������� ���������</param>
        /// <param name="tag">��� ��� ������</param>
        /// <returns>������ ������� �������� (�������� ��� ��������), ������� ���� ���</returns>
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
                if (targetChild.tag == tag) // ���� �������� ������ ����� ���, ������� �� �����
                {
                    targetChildren.Add(targetChild.gameObject);
                }
                if (targetChild.childCount > 0) //� ���� ������� ������ �������� ��������
                {
                    targetChildren.AddRange(targetChild.gameObject.FindGameObjectsWithTag(tag)); //����������� ����� ����� ������� � �.�.
                }
            }
            return targetChildren;
        }

        /// <summary>
        /// ������� ��� �����
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
