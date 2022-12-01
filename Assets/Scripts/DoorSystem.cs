using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSystem : NetworkBehaviour
{
    [SerializeField] private Animator[] _animatorsDoor;

    public int AthorOpenDoor;
    public int AthorCloseDoor;

    private int countOpenDoor;
    private int countCloseDoor;

    public void Update()
    {
        if(countOpenDoor < AthorOpenDoor)
        {
            countOpenDoor++;
            _animatorsDoor[countOpenDoor - 1].SetTrigger("Open");
        }

        if(countCloseDoor < AthorCloseDoor)
        {
            countCloseDoor++;
            _animatorsDoor[countCloseDoor - 1].SetTrigger("Close");
        }
    }

}
