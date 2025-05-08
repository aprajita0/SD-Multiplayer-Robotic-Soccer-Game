using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;

public class SimpleServer : MonoBehaviour {
    TcpListener listener;
    List<TcpClient> clients = new();
    bool running = true;

    void Start() {
        listener = new TcpListener(IPAddress.Any, 7777);
        listener.Start();
        Debug.Log("[Server] Listening on port 7777");
        new Thread(AcceptClients).Start();
    }

    void AcceptClients() {
        while (running) {
            var client = listener.AcceptTcpClient();
            lock (clients) { clients.Add(client); }
            Debug.Log("[Server] Client connected");
            new Thread(() => HandleClient(client)).Start();
        }
    }

    void HandleClient(TcpClient client) {
        var stream = client.GetStream();
        var buf = new byte[256];
        while (client.Connected) {
            int len = stream.Read(buf, 0, buf.Length);
            if (len == 0) break;
            var msg = Encoding.UTF8.GetString(buf,0,len);
            Debug.Log($"[Server] Received: {msg}");
            // Echo back to all
            lock (clients) {
                foreach (var c in clients)
                    if (c.Connected)
                        c.GetStream().Write(buf,0,len);
            }
        }
        lock (clients) { clients.Remove(client); }
        client.Close();
        Debug.Log("[Server] Client disconnected");
    }

    void OnApplicationQuit() {
        running = false;
        listener.Stop();
    }
}
