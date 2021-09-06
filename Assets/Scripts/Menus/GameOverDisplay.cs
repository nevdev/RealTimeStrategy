using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// this is a client side class - so we inherit from MonoBehaviour
public class GameOverDisplay : MonoBehaviour
{
    [SerializeField] private GameObject gameObjectDisplayParent = null;
    [SerializeField] private TMP_Text winnerNametext = null;

    // On Start subscribe to the ClientOnGameOver event
    private void Start()
    {
        //GameOverHandler.ClientOnGameOver means we are listining to ClientOnGameOver from GameOverHandler
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
        // .. and here we are subcribing and with this local method we 
        // .. just created for the implementation of the event that is driven from 
        // .. GameOverHandler.ClientOnGameOver
    }

    private void OnDestroy()
    {
        // unsibscribing!
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

    public void LeaveGame()
    {
        if(NetworkServer.active && NetworkClient.isConnected)
        {
            // stop hosting.. - if we are the client hosting the server and we stop all, we will goining to stop the 
            // server, and  not just the client hosting it but all the clients connected to it.
            NetworkManager.singleton.StopHost();
        }
        else
        {
            // stop client - if we are just a client not hosting the server and we're disconneted like we 
            // died or switched off the button to disconnect, the server where the 
            // client is hoting it will continue to run
            NetworkManager.singleton.StopClient();
        }

        // this method will sendus automatically to the homescreen because we are changing from
        // from Online to Offline

    }

    private void ClientHandleGameOver(string winner)
    {
        winnerNametext.text = $"{winner} Has Won!";

        gameObjectDisplayParent.SetActive(true);
    }


}
