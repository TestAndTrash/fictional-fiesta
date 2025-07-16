using System;
using System.Collections;
using Assets.Script.Creatures;
using Assets.Script.HUD;
using UnityEngine;

public class CardGameManager : MonoBehaviour
{
    [SerializeField] private Board board;
    [SerializeField] private OpponentCardManager opponentCardManager;
    [SerializeField] private HandManager playerHandManager;

    private int playerCapturedLane = 0;
    private int opponentCaptureLane = 0;
    public static event Action<bool> partyIsOver;
    public bool gameOver = false;
    public bool playerWon = false;
    public bool opponentWon = false;

    public void Start()
    {
        PassTurn.playerPassedTurn += PlayerHasPassed;
        opponentCardManager.opponentPassedTurn += OpponentHasPassed;
        Base.baseIsKilled += CaptureLane;
        playerHandManager.playerDeckIsEmpty += OnPlayerDeckEmpty;
        opponentCardManager.opponentDeckIsEmpty += OnOpponentDeckEmpty;
        opponentCardManager.FillBoardInfo(board);
        LaunchGame();
    }

    public void LaunchGame()
    {
        StartCoroutine(opponentCardManager.Draw(3));
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
        StartCoroutine(opponentCardManager.Draw(1));
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
        playerHandManager.ActivatePlay(true);
        playerHandManager.DrawCard();
    }

    public void EndTheGame()
    {
        playerHandManager.ActivatePlay(false);
        if (playerWon)
        {
            Debug.Log("PLAYER WON");
        }
        else
        {
            Debug.Log("OPPONENT WON");
        }
    }

    public void CaptureLane(Base killedBase)
    {
        bool isPlayer = killedBase.team == board.playerTeamIndex;
        if (isPlayer)
        {
            opponentCaptureLane++;
            if (opponentCaptureLane >= 3)
            {
                gameOver = true;
                opponentWon = true;
            }
        }
        else
        {
            playerCapturedLane++;
            if (playerCapturedLane >= 3)
            {
                gameOver = true;
                playerWon = true;
            }
        }
    }

    public void OnPlayerDeckEmpty()
    {
        gameOver = true;
        opponentWon = true;
        EndTheGame();
    }

    public void OnOpponentDeckEmpty()
    {
        gameOver = true;
        playerWon = true;
        EndTheGame();
    }
}
