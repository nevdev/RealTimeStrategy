using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;


    [SyncVar]
    private int currenHealth;

    public event Action ServerOnDie;

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

        ServerOnDie?.Invoke();

        Debug.Log("We Died");
    }
    #endregion

    #region Client

    #endregion
}
