using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem.HID;

namespace MirrorBasics
{

    #region Class Match
    [System.Serializable]
    public class Match
    {
        public string matchID;
        public bool publicMatch;
        public bool inMatch;
        public bool matchFull;
        public List<PlayerMovementAndLookNetwork> players = new List<PlayerMovementAndLookNetwork>();

        public Match(string matchID, PlayerMovementAndLookNetwork player, bool publicMatch)
        {
            matchFull = false;
            inMatch = false;
            this.matchID = matchID;
            this.publicMatch = publicMatch;
            players.Add(player);
        }

        public Match() { }
    }

    #endregion

    public class MatchMaker : NetworkBehaviour
    {

        #region Sessions

        [SerializeField] public List<ManagerSessionSaved> managers = new List<ManagerSessionSaved>();

        #endregion

        [Space(20)]
        public static MatchMaker instance;

        public SyncList<Match> matches = new SyncList<Match>();
        public SyncList<String> matchIDs = new SyncList<String>();

        [SerializeField] int maxMatchPlayers = 12;

        void Start()
        {
            instance = this;
        }

        /// <summary>
        /// Создание матча
        /// </summary>
        /// <param name="_matchID"></param>
        /// <param name="_player"></param>
        /// <param name="publicMatch"></param>
        /// <param name="playerIndex"></param>
        /// <returns></returns>
        public bool HostGame(string _matchID, PlayerMovementAndLookNetwork _player, bool publicMatch, out int playerIndex)
        {
            playerIndex = -1;

            if (!matchIDs.Contains(_matchID))
            {
                matchIDs.Add(_matchID);
                Match match = new Match(_matchID, _player, publicMatch);
                matches.Add(match);
                Debug.Log($"Match generated");
                _player.currentMatch = match;
                playerIndex = 1;

                AddManagerSession(_matchID);

                managerSessionSavedFromCollection(_matchID).AddPlayer(_player);

                return true;
            }
            else
            {
                Debug.Log($"Match ID already exists");
                return false;
            }
        }

        /// <summary>
        /// Присоединение к игре других игроков
        /// </summary>
        /// <param name="_matchID"></param>
        /// <param name="_player"></param>
        /// <param name="playerIndex"></param>
        /// <returns></returns>
        public bool JoinGame(string _matchID, PlayerMovementAndLookNetwork _player, out int playerIndex)
        {
            playerIndex = -1;

            if (matchIDs.Contains(_matchID))
            {

                for (int i = 0; i < matches.Count; i++)
                { //Пройдёмся по всем созданным комнатам
                    if (matches[i].matchID == _matchID)
                    { //Если наш ID совпадает с существующей комнатой
                        if (!matches[i].inMatch && !matches[i].matchFull)
                        { //Если комната ещё не стартанула и комната не полная
                            matches[i].players.Add(_player); //Добавим плеера в матч
                            _player.currentMatch = matches[i]; //Назначим плееру комнату в которую мы только что добавили
                            playerIndex = matches[i].players.Count; //Внешнюю ссылку индекса увеличим на число в комнате

                            matches[i].players[0].PlayerCountUpdated(matches[i].players.Count); //через главного игрока в комнате увеличим число в комнате

                            managerSessionSavedFromCollection(_matchID).AddPlayer(_player); ;

                            if (matches[i].players.Count == maxMatchPlayers)
                            { //Если количество игроков в комнате максимальное
                                matches[i].matchFull = true; //Закроем комнату для набора
                            }

                            break;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                Debug.Log($"Match joined");
                return true;
            }
            else
            {
                Debug.Log($"Match ID does not exist");
                return false;
            }
        }

        /// <summary>
        /// Быстрый поиск матча. Только с тем учётом что есть созданные публичные комнаты
        /// </summary>
        /// <param name="_player"></param>
        /// <param name="playerIndex"></param>
        /// <param name="matchID"></param>
        /// <returns></returns>
        public bool SearchGame(PlayerMovementAndLookNetwork _player, out int playerIndex, out string matchID)
        {
            playerIndex = -1;
            matchID = "";

            for (int i = 0; i < matches.Count; i++)
            {
                Debug.Log($"Checking match {matches[i].matchID} | inMatch {matches[i].inMatch} | matchFull {matches[i].matchFull} | publicMatch {matches[i].publicMatch}");
                if (!matches[i].inMatch && !matches[i].matchFull && matches[i].publicMatch)
                {
                    if (JoinGame(matches[i].matchID, _player, out playerIndex))
                    {
                        matchID = matches[i].matchID;
                        return true;
                    }
                }
            }

            return false;
        }

        public void BeginGame(string _matchID)
        {
            for (int i = 0; i < matches.Count; i++)
            {
                if (matches[i].matchID == _matchID)
                {
                    matches[i].inMatch = true;
                    foreach (var player in matches[i].players)
                    {
                        player.StartGame();
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Генерация ID матча
        /// </summary>
        /// <returns></returns>
        public static string GetRandomMatchID()
        {
            string _id = string.Empty;
            for (int i = 0; i < 5; i++)
            {
                int random = UnityEngine.Random.Range(0, 36);
                if (random < 26)
                {
                    _id += (char)(random + 65);
                }
                else
                {
                    _id += (random - 26).ToString();
                }
            }

            Debug.Log($"Random Match ID: {_id}");
            return _id;
        }

        /// <summary>
        /// Отключение пользователей из матча
        /// </summary>
        /// <param name="player"></param>
        /// <param name="_matchID"></param>
        public void PlayerDisconnected(PlayerMovementAndLookNetwork player, string _matchID)
        {
            for (int i = 0; i < matches.Count; i++)
            {
                if (matches[i].matchID == _matchID)
                {
                    int playerIndex = matches[i].players.IndexOf(player);
                    if (matches[i].players.Count > playerIndex)
                    {
                        matches[i].players.RemoveAt(playerIndex);
                        managerSessionSavedFromCollection(_matchID).RemovePlayer(player); ;
                    }

                    //TODO : Дальнейшее обновление. Получить доступ к Player и вывести это уведомление в Canvas
                    Debug.Log($"Player disconnected from match {_matchID} | {matches[i].players.Count} players remaining");

                    //когда последний пользователь уходит из матча, мы просто удаляем из списка доступных комнат
                    if (matches[i].players.Count == 0)
                    {
                        Debug.Log($"No more players in Match. Terminating {_matchID}");
                        matches.RemoveAt(i);
                        matchIDs.Remove(_matchID);

                        //var listObjects = FindObjectsOfType<NetworkMatch>()
                        //.Where(go => //Найти все элементы у которых MatchId совпадает с искомым и GO не является Player и MainCamera
                        //go.matchId == _matchID.ToGuid() 
                        //&& !go.gameObject.CompareTag("Player")
                        //&& !go.gameObject.CompareTag("MainCamera")
                        //).ToList();
                        if (managerSessionSavedFromCollection(_matchID).ObjectsWithMatch != null)
                        {
                            foreach (var MatchGameObject in managerSessionSavedFromCollection(_matchID)
                                         .ObjectsWithMatch)
                            {
                                var o = GameObject.Find(MatchGameObject.name);
                                if (o.GetComponent<NetworkMatch>().matchId == _matchID.ToGuid())
                                    Destroy(o);
                            }
                        }

                        instance.managers.Remove(managerSessionSavedFromCollection(_matchID));
                        // managerSessionSavedFromCollection(_matchID).ObjectsWithMatch;
                    }
                    else
                    {
                        matches[i].players[0].PlayerCountUpdated(matches[i].players.Count);
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Сохраняем сессию в виде файла и в матчмейкинге, чтобы можно было найти и подключаться для передачи данных
        /// </summary>
        /// <param name="NameRoom"></param>
        public static void AddManagerSession(string matchId)
        {
            //ManagerSessionSaved _manager = ScriptableObject.CreateInstance<ManagerSessionSaved>();
            ManagerSessionSaved _manager = new ManagerSessionSaved();
            _manager.NameManager = matchId;
            instance.managers.Add(_manager);

            BinaryFormatter bf = new BinaryFormatter();

            using (FileStream file = File.Create($"{pathToSaveAssets()}/Manager_{matchId}.asset"))
            {
                bf.Serialize(file, _manager);
                file.Close();
            }

            Debug.LogWarning("Saved manage: " + _manager.NameManager + $"\n{pathToSaveAssets()}/ Manager_{matchId}.asset");
            //#if UNITY_EDITOR
            //AssetDatabase.CreateAsset(_manager, $"{pathToSaveAssets()}/Manager_{NameRoom}.asset");
            //#endif
            //AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Удаление сессии из матчмейкинга, чтобы не висел
        /// </summary>
        /// <param name="matchId"></param>
        private static void RemoveManagerSession(string matchId)
        {
            var _manager = managerSessionSavedFromCollection(matchId);
            if (_manager != null)
            {
                //Удаляем все объекты, созданные для этого матча
                //Удаление

                instance.managers.Remove(_manager);
            }
        }

        /// <summary>
        /// Найденый <see cref="ManagerSessionSaved"/> в коллекции по <see cref="matchIDs"/>
        /// </summary>
        /// <param name="matchID"></param>
        /// <returns></returns>
        public static ManagerSessionSaved managerSessionSavedFromCollection(string matchID) //
        {
            //var msss = instance.managers.Find(x => x.NameManager == matchID);
            var mss = instance.managers.FirstOrDefault(m => m.NameManager == matchID);
            if (mss != null) return mss;
            else return null;
        }
        //TODO : Поиск по GUID

        static string pathToSaveAssets()
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

    public static class MatchExtensions
    {
        public static Guid ToGuid(this string id)
        {
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            byte[] inputBytes = Encoding.Default.GetBytes(id);
            byte[] hashBytes = provider.ComputeHash(inputBytes);

            return new Guid(hashBytes);
        }
    }

}