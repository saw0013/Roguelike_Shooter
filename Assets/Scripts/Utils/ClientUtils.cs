using Mirror;
using System.Linq;
using UnityEngine;
using UnityEngine.Diagnostics;

namespace Utils
{
    public static class ClientUtils
    {
        /// <summary>
        /// Это вернет компонент ConnectionPlayer, которым вы владеете.
        /// </summary>
        /// <returns>The ClientConnection component.</returns>
        public static ConnectionPlayer GetMyClientConnectionObject()
        {
            return GameObject.FindObjectsOfType<ConnectionPlayer>().ToList().Find(x => x.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer == true || x.gameObject.GetComponent<NetworkIdentity>().hasAuthority == true);
        }

        /// <summary>
        /// Это вернет принадлежащий вам компонент PlayerMovementAndLookNetwork. Не работает, если вы сервер.
        /// </summary>
        /// <returns></returns>
        public static PlayerMovementAndLookNetwork GetMyCharacterControllerObject()
        {
            return GameObject.FindObjectsOfType<PlayerMovementAndLookNetwork>().ToList().Find(x => x.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer == true || x.gameObject.GetComponent<NetworkIdentity>().hasAuthority == true);
        }

        /// <summary>
        /// Вернет компонент подключения клиента, который соответствует этому идентификатору подключения.
        /// </summary>
        /// <param name="connId"></param>
        /// <returns></returns>
        public static PlayerMovementAndLookNetwork GetClientConnection(int connId)
        {
            if (NetworkServer.active)
                return GameObject.FindObjectsOfType<PlayerMovementAndLookNetwork>().ToList().Find(x => x.GetComponent<NetworkIdentity>().connectionToClient.connectionId == connId);
            else
                return GameObject.FindObjectsOfType<PlayerMovementAndLookNetwork>().ToList().Find(x => x.connId == connId);
        }

        ///// <summary>
        ///// Возвращает компонент подключения клиента, который соответствует переданному игроку игровому объекту.
        ///// </summary>
        ///// <param name="connId"></param>
        ///// <returns></returns>
        //public static PlayerMovementAndLookNetwork GetClientConnection(GameObject player)
        //{
        //    return GameObject.FindObjectsOfType<PlayerMovementAndLookNetwork>().ToList().Find(x => x.playerCharacter.Equals(player));
        //}
    }
}

