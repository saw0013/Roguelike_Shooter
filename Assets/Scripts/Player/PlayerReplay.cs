using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Cosmoground
{


    public class PlayerReplay : MonoBehaviour
    {
        [SerializeField] List<PlayerMovementInfo> _playerMovementInfoList = new List<PlayerMovementInfo>();


        void OnPlayerMove()
        {
            _playerMovementInfoList.Add(new PlayerMovementInfo(Time.time, transform.position, transform.rotation));
        }

        public void SaveReplay()
        {
            string json =  JsonSerializeHelper.Serialize(_playerMovementInfoList);
            //string json = JsonUtility.ToJson(_playerMovementInfoList);
            // сохраняем json в файл
            File.WriteAllText(Extensions.GetFilePath($"Cosmo_{DateTime.Now.ToString("dd_MM_yyy_HH_mm_ss")}.json"), json);
        }

        private void Update()
        {
            //OnPlayerMove();
        }

        /// <summary>
        /// Воспроизводит поведение.
        /// </summary>
        /// <returns></returns>
        IEnumerator ReplayCoroutine()
        {
            foreach (var movementInfo in _playerMovementInfoList)
            {
                transform.position = movementInfo.Position;
                transform.rotation = movementInfo.Rotation;

                yield return new WaitForSeconds(movementInfo.Timestamp - Time.time);
            }
        }
    }

    /// <summary>
    /// Информация о персонаже
    /// </summary>
    [Serializable]
    public class PlayerMovementInfo
    {
        public float Timestamp;
        public Vector3 Position;
        public Quaternion Rotation;

        public PlayerMovementInfo(float timestamp, Vector3 position, Quaternion rotation)
        {
            Timestamp = timestamp;
            Position = position;
            Rotation = rotation;
        }
    }

}