using UnityEngine;

public class GoalScoringLogic : MonoBehaviour
{
    public string teamScored;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SoccerBall"))
        {
            Debug.Log($"Goal scored in: {teamScored}");
            GameManagement.Instance.AddScore(teamScored);
            GameManagement.Instance.ResetAfterGoal();
        }
    }
}
