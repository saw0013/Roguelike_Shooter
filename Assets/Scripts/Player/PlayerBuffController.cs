using System;
using System.Collections;
using System.Collections.Generic;
using Mono.CSharp;
using UnityEngine;

public class PlayerBuffController : MonoBehaviour
{

    private float _delayTimeNewBuff = 10; //����� �������� ����� ������� ������ ����, ����� �� 10 ������ �� ������� ����� 1 ����

    private bool _giveBuffNow = true; //�������� ����� �� ��������� ���

    private List<string> _buffs = new List<string>();


    /// <summary>
    ///  ��������� ���� �� �� ������ ���
    /// <para>���� ���� �� �� ����� ����������� UI � ������ ������� �����</para>
    /// </summary>
    /// <param name="NameBuff">������� ����� nameof(Class)</param>
    /// <returns>TRUE ���� ��� ����</returns>
    public bool BuffIsExist(string NameBuff)
    {
        if (_buffs.Contains(NameBuff)) //�������� ����\��� �� ��� �� ������
            return true;
        else
        {
            _buffs.Add(NameBuff);
            return false;
        }
    }

    /// <summary>
    /// ���������� TRUE ���� ����� ���������
    /// <para>FALSE ���� ������ ����������</para>
    /// </summary>
    /// <returns></returns>
    public bool PickOrDestroy()
    {
        if (_giveBuffNow) return true;
        else return false;
    }


    private void Update()
    {
        if (!_giveBuffNow)
        {
            if (_delayTimeNewBuff > 0)
                _delayTimeNewBuff -= Time.deltaTime;
            else
            {
                _giveBuffNow = true;
                _delayTimeNewBuff = 10;
            }
        }
    }
}
