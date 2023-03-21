using CosmogroundChatServer.Hubs;
using Microsoft.AspNetCore.Components;

namespace CosmogroundChatServer
{

    public class MyIndexModel : ComponentBase
    {
        string messages { get; set; }

        public void InitPage()
        {
            ChatHub.ActionMessage += OnMessagge;
        }

        private void OnMessagge(string obj)
        {
            messages += obj + Environment.NewLine;
        }
    }
}