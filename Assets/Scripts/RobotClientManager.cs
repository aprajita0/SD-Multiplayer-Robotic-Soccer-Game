// Handles the networking setup for each player, allowing them to communicate with a server.
using UnityEngine;

public class RobotClientManager : MonoBehaviour
{
    public GameObject[] players;
    public Transform blueGoal;
    public Transform redGoal;

    void Start()
    {
        foreach (var player in players)
        {
            var aiControllers = player.GetComponents<AIPlayerController>();
            for (int i = 1; i < aiControllers.Length; i++)
            {
                Destroy(aiControllers[i]);
            }

            
            if (aiControllers.Length == 0)
            {
                var ai = player.AddComponent<AIPlayerController>();
                ai.ball = GameObject.FindGameObjectWithTag("SoccerBall");

                
                if (player.name == "BlueTeamPlayer4")
                {
                    ai.role = PlayerRole.Goalie;
                    ai.goalToDefend = blueGoal;
                    ai.goalToScore = redGoal;
                }
                else if (player.name == "RedTeamPlayer4")
                {
                    ai.role = PlayerRole.Goalie;
                    ai.goalToDefend = redGoal;
                    ai.goalToScore = blueGoal;
                }
                else
                {
                    if (player.name.Contains("Player1") || player.name.Contains("Player2"))
                    {
                        ai.role = PlayerRole.Forward;
                    }
                    else if (player.name.Contains("Player3"))
                    {
                        ai.role = PlayerRole.Midfielder;
                    }
                    else if (player.name.Contains("Player5"))
                    {
                        ai.role = PlayerRole.Defender;
                    }

                    
                    ai.goalToDefend = player.name.StartsWith("Blue") ? blueGoal : redGoal;
                    ai.goalToScore = player.name.StartsWith("Blue") ? redGoal : blueGoal;
                }

                
                var client = player.GetComponent<RobotClient>();
                if (client == null)
                {
                    client = player.AddComponent<RobotClient>();
                    client.id = player.name;
                    client.serverIp = "127.0.0.1";
                    client.serverPort = 7777;
                }
            }
        }
    }
}

