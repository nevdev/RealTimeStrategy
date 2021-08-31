using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;


    [SyncVar(hook = nameof(HandleHealthUpdated))]
    private int currenHealth;// whwnwever this syncvar changes will call to update the value from old to new

    public event Action ServerOnDie;
    public event Action<int, int> ClientOnHealthUpdated;

    #region Server
    public override void OnStartServer()
    {
        currenHealth = maxHealth;
    }

    [Server]
    public void DealDamage(int damageAmount)
    {
        if(currenHealth == 0) { return; }
    

        currenHealth = Mathf.Max(currenHealth - damageAmount, 0);// returns that max value from the two args
        // this one liner is replacing the below

        //currenHealth -= damageAmount;
        //if (currenHealth < 0)
        //{
        //    currenHealth = 0;
        //}


        if (currenHealth != 0) { return; }

        // if no one is listening (is null) do not invoke this event
        ServerOnDie?.Invoke();

        Debug.Log("We Died");
    }
    #endregion

    #region Client
    
    // old and new args are mandatory even if you do not need the old argin order to run this from the synvar->hook 
    private void HandleHealthUpdated(int oldhealth, int newHealth)
    {
        //.. after calling the method with the old param then use other required  args:)
        ClientOnHealthUpdated?.Invoke(newHealth, maxHealth);
        // when the newHealth is updated the heaplth display script will update the health UI health bar
        // remember that this class does not know who are the subscribers 
        // just checking if ClientOnHealthUpdated is null, so if there are subscribers it will result Not Null and runs Invoke
    }

    // you can alwaus add another listener like sound effect script class and inovke it from here
    #endregion
}
