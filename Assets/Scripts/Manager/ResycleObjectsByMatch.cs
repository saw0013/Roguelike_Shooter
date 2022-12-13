using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ResycleObjectsByMatch
{
    [SerializeField] public List<GameObject> CreatedObjectsInMatch;

    public Guid MatchID
    {
        get => _matchid;
        set => _matchid = value;
    }

    private Guid _matchid;

    /// <summary>
    /// ��������� � ������ ������� ������� ������� ����� NetworkServer.Spawn
    /// </summary>
    /// <param name="_gameObject"></param>
    public void AddObjectWithMatch(GameObject _gameObject)
    {
        if (CreatedObjectsInMatch == null)
            CreatedObjectsInMatch = new List<GameObject>();

        CreatedObjectsInMatch.Add(_gameObject);
    }

    public void RemoveObjectWithMatch(GameObject _gameObject)
    {
        //���� ������ ��������� ��� ������ ������, ������ ������
        if (_gameObject == null || CreatedObjectsInMatch == null) return;

        CreatedObjectsInMatch.Remove(_gameObject);
    }
}
