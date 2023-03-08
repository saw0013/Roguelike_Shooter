using Microsoft.AspNetCore.SignalR;

namespace ChatServer.Hubs
{
    public class ChatHub : Hub
    {
        public List<string> _groups { get; set; } = new List<string>();

        /// <summary>
        /// ����� ���������� �������
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task InitializeGroup(string roomId, string username)
        {
            if(roomId == null) return;

            _groups.Add(roomId);

            if (String.IsNullOrEmpty(username))
            {
                await Clients.Caller.SendAsync("UserNotify", "��� ����� � ������ �� ������ ����� �����");
            }
            else
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
                await Clients.Group(roomId).SendAsync("UserNotify", $"{username} ������������� � ������ � ID {roomId}");
                Console.WriteLine($"������������ {username} ������ ������� {roomId} � ������� � ��� �����������");
            }
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
        public async Task SendMessageToGroup(string roomId, string username, string message)
        {
            if (roomId == null) return;

            await Clients.Group(roomId).SendAsync("Receive", message, username);
        }

        /// <summary>
        /// ظ��� ��� ������������ �������
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessageToClient(string client, string message)
        {
            await Clients.Client(client).SendAsync("Receive", message);
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