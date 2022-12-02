using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RareItemBulletUI : MonoBehaviour
{
    private PlayerData owner;
    public void RegisterOwner(PlayerData ownerItem) => owner = ownerItem;
}
