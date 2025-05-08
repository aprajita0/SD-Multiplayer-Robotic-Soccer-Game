using UnityEngine;

public class BallPossessionManager : MonoBehaviour
{
    public static BallPossessionManager Instance;

    public GameObject ball;
    public AIPlayerController ballHolder;   
    public AIPlayerController lastHolder;   
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SetPossession(AIPlayerController newHolder)
    {
        if (newHolder == null)
            return;

        if (ballHolder != newHolder)
        {
            lastHolder = ballHolder;
            ballHolder = newHolder;

            Debug.Log($"[{Time.time:F2}s] Ball possession: {newHolder.gameObject.name}");
        }
    }
    public AIPlayerController GetPossessor()
    {
        return ballHolder;
    }

    public bool HasPossession(AIPlayerController player)
    {
        return ballHolder == player;
    }

    public void PassBall(Vector3 targetPosition, float passForce = 15f)
    {
        if (ballHolder == null || ball == null) return;

        Rigidbody ballRb = ball.GetComponent<Rigidbody>();
        Vector3 passDirection = (targetPosition - ball.transform.position).normalized;

        ballRb.velocity = passDirection * passForce;

        // Instead of instantly clearing possession, we delay it
        StartCoroutine(DelayedClearPossession());
    }

    private System.Collections.IEnumerator DelayedClearPossession()
    {
        yield return new WaitForSeconds(1.0f); 
        ClearPossession();
    }
    

    public void ClearPossession()
    {
        lastHolder = ballHolder;
        ballHolder = null;
        Debug.Log($"[{Time.time:F2}s] Ball possession cleared.");
    }
}

