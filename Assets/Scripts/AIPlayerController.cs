using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public enum PlayerRole { Forward, Midfielder, Defender, Goalie }

[RequireComponent(typeof(NavMeshAgent))]
public class AIPlayerController : MonoBehaviour
{
    public GameObject ball;
    public Transform goalToDefend;
    public Transform goalToScore;
    public PlayerRole role = PlayerRole.Midfielder;

    private NavMeshAgent agent;
    private Vector3 homePosition;
    private bool movingToAssist = false;
    private Vector3 assistPosition;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = 40f;
        agent.acceleration = 80f;
        agent.angularSpeed = 1000f;
        agent.stoppingDistance = 5f;

        homePosition = transform.position;
        if (!ball) ball = GameObject.FindGameObjectWithTag("SoccerBall");
    }

    void Update()
    {
        if (!ball || BallPossessionManager.Instance == null) return;
        if (gameObject.CompareTag("SoccerBall")) return;

        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning($"{gameObject.name} is NOT on the NavMesh!");
            return;
        }

        Vector3 target = homePosition;

        switch (role)
        {
            case PlayerRole.Goalie: target = GoalieLogic(); break;
            case PlayerRole.Defender: target = DefenderLogic(); break;
            case PlayerRole.Midfielder: target = MidfielderLogic(); break;
            case PlayerRole.Forward: target = ForwardLogic(); break;
        }

        agent.SetDestination(target);
        if (HasPossession()){
            KickBallIfNeeded();
            }
    }

    Vector3 GoalieLogic()
    {
        Vector3 ballPosition = ball.transform.position;
        float goalZ = goalToDefend.position.z;
        float myCurrentZ = transform.position.z;
        float fixedGoalieX = goalToDefend.position.x > 0 ? 370f : -380f;

        float targetZ = myCurrentZ;
        float distanceBallFromGoal = Mathf.Abs(ballPosition.x - fixedGoalieX);

        if (distanceBallFromGoal < 70f)
        {
            targetZ = ballPosition.z;
            float randomOffset = Random.Range(-2f, 2f);
            targetZ += randomOffset;
            targetZ = Mathf.Clamp(targetZ, goalZ - 10f, goalZ + 10f);
        }

        return new Vector3(fixedGoalieX, transform.position.y, targetZ);
    }

    Vector3 DefenderLogic()
    {
        if (HasPossession())
        {
            movingToAssist = false;
            return PushBallTowardGoal();
        }
        else if (!IsTeamInPossession())
        {
            if (IsBallInDefensiveRange() && AmIAmongClosestToBall())
            {
                Debug.Log($"{gameObject.name}: Team lost ball, I am close enough, chasing ball!");
                return ball.transform.position;
            }
            else
            {
                Debug.Log($"{gameObject.name}: Team lost ball, staying open instead of chasing.");
                return GetOpenSpaceNearBall();
            }
        }
        else if (IsTeammateInPossession() && IsBallInDefensiveRange())
        {
            if (!movingToAssist)
            {
                assistPosition = GetOpenSpaceNearTeammate();
                movingToAssist = true;
                Debug.Log($"{gameObject.name}: Moving to assist teammate in defense at {assistPosition}.");
            }
            return assistPosition;
        }
        else
        {
            movingToAssist = false;
        }

        return homePosition;
    }

    Vector3 MidfielderLogic()
    {
        if (HasPossession())
        {
            movingToAssist = false;
            return PushBallTowardGoal();
        }
        else if (!IsTeamInPossession())
        {
            if (IsBallInMidfieldRange() && AmIAmongClosestToBall())

            {
                Debug.Log($"{gameObject.name}: Team lost ball, I am close enough, chasing ball!");
                return ball.transform.position;
            }
            else
            {
                Debug.Log($"{gameObject.name}: Team lost ball, staying open instead of chasing.");
                return GetOpenSpaceNearBall();
            }
        }
        else if (IsTeammateInPossession() && IsBallInMidfieldRange())
        {
            if (!movingToAssist)
            {
                assistPosition = GetOpenSpaceNearTeammate();
                movingToAssist = true;
                Debug.Log($"{gameObject.name}: Moving to assist teammate in midfield at {assistPosition}.");
            }
            return assistPosition;
        }
        else
        {
            movingToAssist = false;
        }

        return homePosition;
    }
    void KickBallIfNeeded()
    {
        if (!HasPossession()) return;

        float distanceToOwnGoal = Vector3.Distance(transform.position, goalToDefend.position);
        float distanceToEnemyGoal = Vector3.Distance(transform.position, goalToScore.position);

        // Defensive clear if too close to own goal
        if (distanceToOwnGoal < 25f)
        {
            Vector3 directionAwayFromGoal = (transform.position - goalToDefend.position).normalized;
            Vector3 clearTarget = transform.position + directionAwayFromGoal * 50f;

            Debug.Log($"{gameObject.name} is clearing the ball away from own goal!");
            BallPossessionManager.Instance.PassBall(clearTarget, 30f); // defensive clear
            return;
        }

        // Offensive shot if close to enemy goal
        if (distanceToEnemyGoal < 25f)
        {
            Debug.Log($"{gameObject.name} is shooting toward enemy goal!");
            BallPossessionManager.Instance.PassBall(goalToScore.position, 40f); // shoot!
            return;
        }

        // Otherwise, do nothing 
    }


    Vector3 ForwardLogic()
    {
        if (HasPossession())
        {
            movingToAssist = false;
            return PushBallTowardGoal();
        }
        else if (!IsTeamInPossession())
        {
            if (IsBallInForwardRange() && AmIAmongClosestToBall())
            {
                Debug.Log($"{gameObject.name}: Team lost ball, I am close enough, chasing ball!");
                return ball.transform.position;
            }
            else
            {
                Debug.Log($"{gameObject.name}: Team lost ball, staying open instead of chasing.");
                return GetOpenSpaceNearBall();
            }
        }
        else if (IsTeammateInPossession() && IsBallInForwardRange())
        {
            if (!movingToAssist)
            {
                assistPosition = GetOpenSpaceNearTeammate();
                movingToAssist = true;
                Debug.Log($"{gameObject.name}: Moving to assist teammate in attack at {assistPosition}.");
            }
            return assistPosition;
        }
        else
        {
            movingToAssist = false;
        }

        return homePosition;
    }

    bool HasPossession()
    {
        return BallPossessionManager.Instance.GetPossessor() == this;
    }

    bool IsTeamInPossession()
    {
        var possessor = BallPossessionManager.Instance.GetPossessor();
        return possessor != null && IsSameTeam(possessor);
    }

    bool IsTeammateInPossession()
    {
        var possessor = BallPossessionManager.Instance.GetPossessor();
        return possessor != null && possessor != this && IsSameTeam(possessor);
    }

    bool IsSameTeam(AIPlayerController other)
    {
        return goalToDefend == other.goalToDefend;
    }

    Vector3 PushBallTowardGoal()
    {
        Vector3 toGoal = (goalToScore.position - ball.transform.position).normalized;
        Vector3 pushTarget = ball.transform.position + toGoal * 15f;
        return pushTarget;
    }

    bool AmIAmongClosestToBall(int numberOfClosest = 2)
    {
        var teammates = FindObjectsOfType<AIPlayerController>()
            .Where(p => IsSameTeam(p))
            .ToList();

        var sortedByDistance = teammates.OrderBy(p => Vector3.Distance(p.transform.position, ball.transform.position)).ToList();

        return sortedByDistance.Take(numberOfClosest).Contains(this);
    }

    Vector3 GetOpenSpaceNearBall()
    {
        Vector3 offset = new Vector3(Random.Range(-8f, 8f), 0, Random.Range(-8f, 8f));
        return ball.transform.position + offset;
    }

    Vector3 GetOpenSpaceNearTeammate()
    {
        var possessor = BallPossessionManager.Instance.GetPossessor();
        if (possessor == null) return homePosition;

        Vector3 toGoal = (goalToScore.position - possessor.transform.position).normalized;
        Vector3 backwardOffset = -toGoal * Random.Range(10f, 20f);
        Vector3 sidewaysOffset = Vector3.Cross(Vector3.up, toGoal) * Random.Range(-10f, 10f);

        return possessor.transform.position + backwardOffset + sidewaysOffset;
    }

    bool IsBallInDefensiveRange()
    {
        float ballX = ball.transform.position.x;
        return (goalToDefend.position.x < 0 && ballX >= -493f && ballX <= -198f)
            || (goalToDefend.position.x > 0 && ballX >= 188f && ballX <= 466f);
    }

    bool IsBallInMidfieldRange()
    {
        float ballX = ball.transform.position.x;
        return ballX > -200f && ballX < 200f;
    }

    bool IsBallInForwardRange()
    {
        float ballX = ball.transform.position.x;
        if (goalToDefend.position.x < 0)
            return ballX > -4f;
        else
            return ballX < -5f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("SoccerBall"))
        {
            BallPossessionManager.Instance.SetPossession(this);
        }
    }
}
