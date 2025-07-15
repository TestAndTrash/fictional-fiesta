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
        board.CaptureLane += CaptureLane;
        opponentCardManager.FillPlayerTeam(board.GetPlayerTeam());
        LaunchGame();
    }

    public void LaunchGame()
    {
        //DRAW OPPONENT CARD
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
        //DRAW OPPONENT CARD 
        //Opponent play
    }

    public void OpponentHasPassed() //EVENT
    {
        board.RunOpponentActions();
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
