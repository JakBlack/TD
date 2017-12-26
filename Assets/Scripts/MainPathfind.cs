using UnityEngine;
using UnityEngine.AI;

public class MainPathfind : MonoBehaviour {

    public static MainPathfind Instance { get; set; }
    public Transform goal;
    private NavMeshAgent navComponent;
    private NavMeshPath path;

    // Use this for initialization
    void Start () {
        Instance = this;
        navComponent = GetComponent<NavMeshAgent>();
        navComponent.SetDestination(goal.position);
    }

    public bool PathFound() //Check if full path to the goal is available
    {

        Recalculate();

        switch (navComponent.pathStatus)
        {
            case (NavMeshPathStatus.PathComplete):
                return true;

            default:
                return false;
        }
    }

    public void Recalculate()
    {
        navComponent.SetDestination(goal.position);
    }
}
