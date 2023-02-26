using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using Mirror;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.Networking.Match;
using NetworkMatch = Mirror.NetworkMatch;

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

        #region Managers

        [SerializeField] public List<ResycleObjectsByMatch> resycles = new List<ResycleObjectsByMatch>();
        [SerializeField] public List<GameManagerLogic> GameManagersLogic = new List<GameManagerLogic>();

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

                AddToListResycleObjects(_matchID);
                AddToListGameManagers(_matchID);
                ManagerLogic(_matchID.ToGuid()).AddPlayerInGameManager(_player);

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

                            ManagerLogic(_matchID.ToGuid()).AddPlayerInGameManager(_player);

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
                    }

                    //TODO : Дальнейшее обновление. Получить доступ к Player и вывести это уведомление в Canvas
                    Debug.Log($"Player disconnected from match {_matchID} | {matches[i].players.Count} players remaining");

                    ManagerLogic(_matchID.ToGuid()).RemovePlayerInGameManager(player);

                    //когда последний пользователь уходит из матча, мы просто удаляем из списка доступных комнат
                    if (matches[i].players.Count == 0)
                    {
                        Debug.Log($"No more players in Match. Terminating {_matchID}");
                        matches.RemoveAt(i);
                        matchIDs.Remove(_matchID);

                        #region Linq поиск объектов на сцене
                        //var listObjects = FindObjectsOfType<NetworkMatch>()
                        //.Where(go => //Найти все элементы у которых MatchId совпадает с искомым и GO не является Player и MainCamera
                        //go.matchId == _matchID.ToGuid() 
                        //&& !go.gameObject.CompareTag("Player")
                        //&& !go.gameObject.CompareTag("MainCamera")
                        //).ToList();
                        #endregion

                        //RemoveResycleObjectsInList(_matchID.ToGuid());

                        instance.GameManagersLogic.Remove(ManagerLogic(_matchID.ToGuid()));
                    }
                    else
                    {
                        matches[i].players[0].PlayerCountUpdated(matches[i].players.Count);
                    }
                    break;
                }
            }
        }

        public static void AddToListGameManagers(string matchId)
        {
            GameManagerLogic gml = new GameManagerLogic(matchId.ToGuid());
            //gml.MatchID = matchId.ToGuid();
            instance.GameManagersLogic.Add(gml);
        }

        public static GameManagerLogic ManagerLogic(Guid matchID) => instance.GameManagersLogic.FirstOrDefault(gml => gml.MatchID == matchID);
       

        /// <summary>
        /// Сохраняем сессию в матчмейкинге, чтобы можно было найти заспавненные объекты
        /// </summary>
        /// <param name="NameRoom"></param>
        public static void AddToListResycleObjects(string matchId)
        {
            ResycleObjectsByMatch _resycle = new ResycleObjectsByMatch();
            _resycle.MatchID = matchId.ToGuid();
            instance.resycles.Add(_resycle);

            // BinaryFormatter bf = new BinaryFormatter();
            //
            // using (FileStream file = File.Create($"{pathToSaveAssets()}/Manager_{matchId}.asset"))
            // {
            //     bf.Serialize(file, _resycle);
            //     file.Close();
            // }
            //
            // Debug.LogWarning("Saved manage: " + _manager.MatchID + $"\n{pathToSaveAssets()}/ Manager_{matchId}.asset");
            //#if UNITY_EDITOR
            //AssetDatabase.CreateAsset(_manager, $"{pathToSaveAssets()}/Manager_{NameRoom}.asset");
            //#endif
            //AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Удаление сессии из матчмейкинга, чтобы не висел
        /// </summary>
        /// <param name="matchId"></param>
        private static void RemoveResycleObjectsInList(Guid matchId)
        {
            var _manager = ResycleObjects(matchId);
            if (_manager != null)
            {
                foreach (var MatchGameObject in _manager.CreatedObjectsInMatch)
                {
                    //Если игровой объект на сервере есть с именем которое мы нашли в коллекции
                    //Проверим его MatchID и если что удалим его
                    var go = GameObject.Find(MatchGameObject.name);
                    if (go.GetComponent<NetworkMatch>().matchId == MatchGameObject.GetComponent<NetworkMatch>().matchId)
                    {
                        Debug.LogWarning($"Найденый go - {go.GetComponent<NetworkMatch>().matchId}, в пуле {MatchGameObject.GetComponent<NetworkMatch>().matchId}");
                        Destroy(go);
                    }
                        
                }

                //Удаляем все объекты, созданные для этого матча
                //Удаление

                instance.resycles.Remove(_manager);
            }
        }


        /// <summary>
        /// Найденый <see cref="ResycleObjectsByMatch"/> в коллекции по <see cref="matchIDs"/>
        /// </summary>
        /// <param name="matchID"></param>
        /// <returns></returns>
        public static ResycleObjectsByMatch ResycleObjects(Guid matchID) //
        {
            var res = instance.resycles.FirstOrDefault(m => m.MatchID == matchID);
            if (res != null) return res;
            else return null;
        }

        #region Путь /game/ManagersSave
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

        #endregion

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