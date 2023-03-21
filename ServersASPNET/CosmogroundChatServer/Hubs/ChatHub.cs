using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace CosmogroundChatServer.Hubs
{
    public class ChatHub : Hub
    {
        public static List<string> _groups { get; set; } = new List<string>();
        public static List<string> AllMessages { get; set; } = new List<string>();

        public static Action<string>? ActionMessage;

        /// <summary>
        /// ����� ���������� �������
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task InitializeLobby(string roomId, string username)
        {
            if (roomId == null) return;

            _groups.Add(roomId);

            if (String.IsNullOrEmpty(username))
            {
                await Clients.Caller.SendAsync("UserNotify", "��� ����� � ������ �� ������ ����� �����");
            }
            else
            {
                await Clients.Group(roomId).SendAsync("UserNotify",
                    $"<color=\"red\">{username}</color> ������������� � ������ � ID <color=\"orange\">{roomId}</color>");

                await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
                if (!_groups.Contains(roomId)) Console.WriteLine($"������������ {username} ������ ������� {roomId} � ������� � ��� �����������");
                else Console.WriteLine($"������������ {username}  ������� ����������� � {roomId}");

                await Clients.Caller.SendAsync("UserNotify",
                    $"<color=\"blue\">{username}</color> ������������� � ������ � ID <color=\"orange\">{roomId}</color>");
            }
        }


        public async Task InitializeMatch(string roomId)
        {
            if (roomId == null) return;

            if (String.IsNullOrEmpty(roomId))
                await Clients.Caller.SendAsync("UserNotify", "matchId �� ����� ���� ������");
            else await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        }



        public async Task RemoveGroup(string roomId)
        {
            if (roomId == null) return;

            _groups.Remove(roomId);

            await SendMessageToAll($"������� {roomId} �������. ��� ������ �������� ������");
        }

        /// <summary>
        /// �������� ��������� � ������
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="username"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessageToLobbyGroup(string roomId, string username, string message)
        {
            if (roomId == null) return;

            await Clients.Group(roomId).SendAsync("ReceiveLobbyGroup", username, message);

            var msg = $"[{DateTime.Now.ToString("F")}] [����� {roomId}] : {username} - {message}";
            Console.WriteLine(msg);
            AllMessages.Add(msg);
            ActionMessage?.Invoke(msg);
        }


        public async Task SendMessageToGroup(string roomId, string username, string message)
        {
            if (roomId == null) return;

            await Clients.Group(roomId).SendAsync("ReceiveGroup", username, message);

            var msg = $"[{DateTime.Now.ToString("F")}] [������ {roomId}] : {username} - {message}";
            Console.WriteLine(msg);
            AllMessages.Add(msg);
        }

        public static void AddTestMessageView(string msg)
        {
            ActionMessage?.Invoke(msg);
            AllMessages.Add(msg);
        }

        /// <summary>
        /// ظ��� ��� ������������ �������
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessageToClient(string client, string message)
        {
            await Clients.Client(client).SendAsync("ReceiveClient", message);
        }

        /// <summary>
        /// ����� ������� ������ GameMaster ��� ���������� ���������� ���������
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessageToAll(string message)
        {
            await Clients.All.SendAsync("System", message);
        }

        /// <summary>
        /// �������� ���������. �� ������� �� ������ � ����� ���� ����� �����������
        /// </summary>
        /// <param name="username"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessageTest(string username, string message)
        {
            await Clients.Others.SendAsync("Receive", $"<color=\"blue\">{username}</color>", message);
        }

        /// <summary>
        /// �������� ������. ����� ������ ������ ���
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessageToGameMaster(string message)
        {
            await Clients.Groups("GameMaster").SendAsync("GameMaster", message);
        }

    }

}