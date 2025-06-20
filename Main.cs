using System.IO;
using System.Net;
using BepInEx;
using Photon.Pun;
using UnityEngine;

namespace SelfTracker
{
    [BepInPlugin("com.zyber.selftracker", "Self Tracker", "1.0.0")]
    public class SelfTracker : BaseUnityPlugin
    {
        public static string Name;
        public static string WebhookUrl;
        private bool wasInRoom = false;

        private void Awake()
        {
            string pluginPath = Paths.PluginPath + "\\SelfTracker";

            string webhookFile = Path.Combine(pluginPath, "webhookURL.txt");
            string nameFile = Path.Combine(pluginPath, "yourname.txt");
            WebhookUrl = File.ReadAllText(webhookFile).Trim();
            Name = File.ReadAllText(nameFile).Trim();

            Debug.Log("[Self Tracker] self-tracker loaded");
            SendWebhookMsg($"{Name} has started the game!");
        }
        private void Update()
        {
            bool inRoom = PhotonNetwork.InRoom;
            int playersInRoom = PhotonNetwork.PlayerList.Length;
            if (!wasInRoom && inRoom)
            {
                string roomNameThing = PhotonNetwork.CurrentRoom.Name;
                SendWebhookMsg($"{Name} joined code {roomNameThing}: {playersInRoom}/10 players");
            }

            if (wasInRoom && !inRoom)
            {
                SendWebhookMsg($"{Name} has left that code!");
            }

            wasInRoom = inRoom;
        }


        private void SendWebhookMsg(string msg)
        {
            if (string.IsNullOrEmpty(WebhookUrl))
            {
                Debug.LogError("[Self Tracker] Webhook URL is not set.");
                return;
            }
            string jsonPayload = "{\"content\": \"" + msg + "\"}";

            using (WebClient client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                client.UploadString(WebhookUrl, "POST", jsonPayload);
            }
        }
    }
}
