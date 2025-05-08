using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class RobotClient : MonoBehaviour
{
    public string id;
    public string serverIp;
    public int serverPort;

    private TcpClient socket;
    private Thread clientThread;
    private Vector3 cachedPosition;
    private bool running = true;

    void Start()
    {
        clientThread = new Thread(RunClient) { IsBackground = true };
        clientThread.Start();
    }

    void Update()
    {
        cachedPosition = transform.position;
    }

    void RunClient()
    {
        try
        {
            socket = new TcpClient(serverIp, serverPort);
            Debug.Log($"{id} connected to server");

            var stream = socket.GetStream();
            byte[] buffer = new byte[256];
            Vector3 lastPos = cachedPosition;

            Debug.Log($"[{id}] Initial position: {lastPos:F2}");

            while (running && socket.Connected)
            {
                Vector3 pos = cachedPosition;
                if (Vector3.Distance(pos, lastPos) > 0.01f)
                {
                    string msg = $"{id}:{pos.x:F2},{pos.y:F2},{pos.z:F2}";
                    byte[] data = Encoding.UTF8.GetBytes(msg);
                    stream.Write(data, 0, data.Length);
                    lastPos = pos;
                }

                if (stream.DataAvailable)
                {
                    int len = stream.Read(buffer, 0, buffer.Length);
                    
                }

                Thread.Sleep(100);
            }

            stream?.Close();
            socket?.Close();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"{id} Error: {e.Message}");
        }
    }

    void OnDestroy()
    {
        running = false;
        socket?.Close();

        if (clientThread != null && clientThread.IsAlive)
        {
            clientThread.Join(); // Waits for the thread to exit cleanly
        }
    }
}
