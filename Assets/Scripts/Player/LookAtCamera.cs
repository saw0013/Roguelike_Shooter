using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [Tooltip("��������� ���������, ����� ������ ���������� ������ ��������")]
    public bool alignUp = false;
    [Tooltip("������ ������������ ������ �������� \n!!(��������� � Realtime)!!")]
    public float height = 1;
    [Tooltip("����������� �������� ��� ������� \n!!(���� alignUp �� �������, ������ �� ������� �� ���������)")]
    public bool detachOnStart;
    [Tooltip("������������ �����������")]
    public bool useSmothRotation = true;
    protected Transform parent;
    public bool justY;
    internal Camera cameraMain;
    void Start()
    {
        if (detachOnStart)
        {
            parent = transform.parent;
            transform.SetParent(null);
        }
        cameraMain = Camera.main;
    }

    void FixedUpdate()
    {
        if (alignUp && parent)
            transform.position = parent.position + Vector3.up * height;
        if (!cameraMain) return;
        var lookPos = cameraMain.transform.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        if (useSmothRotation)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 4f);
            transform.eulerAngles = new Vector3(justY ? 0 : transform.eulerAngles.x, transform.eulerAngles.y, 0);
        }
        else
        {
            transform.eulerAngles = new Vector3(justY ? 0 : rotation.eulerAngles.x, rotation.eulerAngles.y, 0);
        }
    }
}
