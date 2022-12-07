using Mirror;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using UnityEditor;
using UnityEngine;

public class ManagerSessions : NetworkBehaviour
{
    [SerializeField] List<ManagerSessionSaved> managers;

    private void Start()
    {
    }

    [ServerCallback]
    public void AddManagerSession(string NameRoom)
    {
        if (isServer)
        {
            ManagerSessionSaved _manager = ScriptableObject.CreateInstance<ManagerSessionSaved>();
            _manager.name = NameRoom;
            managers.Add(_manager);
            using (FileStream file = File.Create($"{pathToSaveAssets()}/Manager_{NameRoom}.asset"))
            {
                using BinaryWriter binaryWriter = new(file);
                binaryWriter.Write(_manager);
                //using var sr = new StreamWriter(file);
                //sr.Write(_manager);
            }

            //#if UNITY_EDITOR
            //AssetDatabase.CreateAsset(_manager, $"{pathToSaveAssets()}/Manager_{NameRoom}.asset");
            //#endif
            //AssetDatabase.SaveAssets();
        }
    }

    public void SaveManager(string NameRoom)
    {
        var date = DateTime.Now.ToString("ddyyMMHHmmss");
        File.Move($"Assets/Manager_{NameRoom}.asset", pathToSaveAssets() + $"/Manager_{date + NameRoom}.asset");
    }

    string pathToSaveAssets()
    {
        var path = Path.Combine(Application.dataPath, "../ManagersSave");
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
#if UNITY_EDITOR
        return path;
#elif UNITY_SERVER
        return path;
#elif UNITY_STANDALONE
        return path;
#endif

    }

}
