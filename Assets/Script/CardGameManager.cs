using System;
using Assets.Script.HUD;
using UnityEngine;

public class CardGameManager : MonoBehaviour
{
    [SerializeField] private Board board;
    [SerializeField] private HandManager playerHandManager;

    private int playerCapturedLane = 0;
    private int opponentCaptureLAne = 0;
    public static event Action<bool> partyIsOver;
    public bool gameOver = false;
    public bool playerWon = false;
    public bool opponentWon = false;

    public void Start()
    {
        //Sub to everything here
        //sub to oppnet passed
        //player passed
        PassTurn.playerPassedTurn += PlayerHasPassed;
        board.CaptureLane += CaptureLane;
        LaunchGame();
    }

    public void LaunchGame()
    {
        //DRAW OPPONENT CARD
        playerHandManager.DrawFirstHand(5);
        playerHandManager.ActivatePlay(true);
    }

    public void PlayerHasPassed()  //EVENT
    {
        playerHandManager.ActivatePlay(false);
        board.RunPlayerActions();
        if (gameObject) EndTheGame();
        else LaunchOpponentTurn();
    }

    public void OpponentHasPassed() //EVENT
    {
        board.RunOpponentActions();
        if (gameObject) EndTheGame();
        else LaunchPlayerTurn();
    }

    public void LaunchOpponentTurn()
    {
        //DRAW OPPONENT CARD 
        //Opponent play
    }

    public void LaunchPlayerTurn()
    {
        playerHandManager.DrawCard();
        playerHandManager.ActivatePlay(true);
    }

    public void EndTheGame()
    {
        if (playerWon)
        {

        }
        else
        {

        }
    }

    public void CaptureLane(bool isPlayer)
    {
        if (isPlayer)
        {
            playerCapturedLane++;
            if (playerCapturedLane >= 3)
            {
                gameOver = true;
                playerWon = true;
            } 
        }
        else
        {
            opponentCaptureLAne++;
            if (opponentCaptureLAne >= 3)
            {
                gameOver = true;
                opponentWon = true;
            } 
        }
    }
}
