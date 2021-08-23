using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI ;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField]private NavMeshAgent agent;
    [SerializeField] Targeter targeter = null; // mainly drag adn drop in fields inspector instead getting components through scripts
    [SerializeField] private float chaseRange = 10f; // chase the target if we're outside this value

    #region Server
    [ServerCallback] // [Server] attribute will only run the update() on the server. [ServerCallback the same but will not show warning and logs in the console.
    private void Update()
    {
        Targetable target = targeter.GetTarget();

        // chasing
        if (targeter.GetTarget())
        {

            // Simple Way - Vector3.Distance(a,b) is the same as (a-b).magnitude.
            // Problem is that it uses SquareRoots and these are quite slow as it being called every frame(REM: WE ARE IN UPDATE METHOD)
            // if(Vector3.Distance(target.transform.position, transform.position) -> Returns the distance between a and b.
            if ((target.transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange) // doing the auare lot more efficient
                // the sqrMagnitude do the square instead of doing the SquareRoot and this is lot more efficient
                // check sqrMagnitude if it is graeater than the square of our chaseRange. Here we are comparing the square result on each 
            {
                // if is checkign if we are out of the cahase range, then: chase
                agent.SetDestination(target.transform.position);
                
            }
            else if (agent.hasPath) // else, restart the path
            {
                // stop chase;
                agent.ResetPath();
            }
            return;
        }

        // DO the below if the above if() statement does not run!

        // over here the if checks if units has the paths and reached until it is removed
        // on each frame.
        if(agent.hasPath) { return; } // 
        if(agent.remainingDistance >= agent.stoppingDistance) { return; } // if unit did not reach the stopping ditance do not continue so will try to reach until the stopping distance is reached
        agent.ResetPath(); // if agent(unit) is within the stopping distance do not follow the path
        // ... so it will yield for other units still has the path to go
    }

    [Command] // this method implementation is a command running on the server invoked by the client prefixed as Cmd...
    public void CmdMove(Vector3 position)
    {
        targeter.ClearTarget();
        if(!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) { return; }
        agent.SetDestination(hit.position);
    }

    #endregion

    //#region Client
    //public override void OnStartAuthority()
    //{
    //    base.OnStartAuthority();
    //    mainCamera = Camera.main;
    //}

    //[ClientCallback]  // Prevents the server from running this mathod. Only runs on the client invoking it
    //private void Update()
    //{
    //    if (!hasAuthority) { return; }


    //    //if (!Input.GetMouseButtonDown(1)){ return; }
    //    if (!Mouse.current.rightButton.wasPressedThisFrame) { return; } // new input system
    //    //Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
    //    Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()); // new input system
    //    if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) { return; }
    //    Debug.Log("Update");
    //    CmdMove(hit.point); // Cmd prefix to call the method implemented on the server writen in this script (#region Server)
    //}


    //#endregion
}
