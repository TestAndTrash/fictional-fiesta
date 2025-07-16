using System;
using System.Collections;
using Assets.Script.HUD;
using UnityEngine;

public class CardGameManager : MonoBehaviour
{
    [SerializeField] private Board board;
    [SerializeField] private OpponentCardManager opponentCardManager;
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
        PassTurn.playerPassedTurn += PlayerHasPassed;
        opponentCardManager.opponentPassedTurn += OpponentHasPassed;
        board.CaptureLane += CaptureLane;
        opponentCardManager.FillBoardInfo(board);
        LaunchGame();
    }

    public void LaunchGame()
    {
        opponentCardManager.Draw(5);
        StartCoroutine(playerHandManager.DrawFirstHand(5));
        playerHandManager.ActivatePlay(true);
    }

    public void PlayerHasPassed()  //EVENT
    {
        StartCoroutine(PlayerHasPassedCRT());
    }

    public IEnumerator PlayerHasPassedCRT()
    {
        playerHandManager.ActivatePlay(false);
        yield return board.CoroutinePlayerActions();
        if (gameOver) EndTheGame();
        else LaunchOpponentTurn();
    }

    public void LaunchOpponentTurn()
    {
        opponentCardManager.PlayTurn();
    }

    public void OpponentHasPassed() //EVENT
    {
        StartCoroutine(OpponenetHasPassedCRT());
    }

    public IEnumerator OpponenetHasPassedCRT()
    {
        yield return board.CoroutineOpponentActions();
        if (gameOver) EndTheGame();
        else LaunchPlayerTurn();
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
