using Mirror;
using System.Collections.Generic;
using System.Linq;
using Cosmoground;
using UnityEngine;

public class NetworkManagerGameResources : MonoBehaviour
{
    [SerializeField, Tooltip("Asset/Resources/[InsertFolderPath]\n�������� Prefab/UI")]
    private string[] folderPath = null;

    private bool isDone = false;

    private void Awake()
    {
        if (!isDone)
        {
            int count = 0;

            if(folderPath == null || folderPath.Length <= 0)
                Debug.LogWarning("��� ��������� ����� ��� �������� � ����������� ��������");

            else
            {
                for (int i = 0; i < folderPath.Length; i++)
                {
                    List<GameObject> listPrefabs = Resources.LoadAll(folderPath[i], typeof(GameObject)).Cast<GameObject>()
                        .ToList();

                    if(listPrefabs == null || listPrefabs.Count <= 0)
                        Debug.LogWarning($"������ �� ������� �� ���� \"Asset/Resources{folderPath[i]}\". ����� ����� ���������");

                    else
                    {
                        for (int _prefab = 0; _prefab < listPrefabs.Count; _prefab++)
                        {
                            gameObject.GetComponent<ShooterNetworkManager>().spawnPrefabs.Add(listPrefabs[_prefab]);
                            //gameObject.GetComponent<NetworkManager>().spawnPrefabs.Add(listPrefabs[_prefab]);
                            count++;
                        }
                    }
                }
            }

            isDone = true;

            Debug.Log($"���� ��������� {count} ��������.");
        }
    }
}
