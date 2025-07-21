using System;
using System.Collections;
using Assets.Script.Creatures;
using Assets.Script.HUD;
using Unity.VisualScripting;
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
    public bool eventSent = false;

    public bool playerWon = false;
    public bool opponentWon = false;
    public static event Action playerWonGame;
    public static event Action playerLostGame;    
    public static event Action battleStart;
    public void Start()
    {
        PassTurn.playerPassedTurn += PlayerHasPassed;
        opponentCardManager.opponentPassedTurn += OpponentHasPassed;
        Base.baseIsKilled += CaptureLane;
        playerHandManager.playerDeckIsEmpty += OnPlayerDeckEmpty;
        opponentCardManager.opponentDeckIsEmpty += OnOpponentDeckEmpty;
        opponentCardManager.FillBoardInfo(board);
    }
    public void LaunchGame()
    {
        battleStart?.Invoke();
        gameOver = false;
        eventSent = false;
        board.gameObject.SetActive(true);
        playerHandManager.StartBattle();
        opponentCardManager.StartBattle();
        StartCoroutine(opponentCardManager.Draw(4));
        StartCoroutine(playerHandManager.DrawFirstHand(4));
        playerHandManager.RefillMana();
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
        playerHandManager.RefillMana();
        playerHandManager.ActivatePlay(true);
        playerHandManager.DrawCard();
    }

    public void EndTheGame()
    {
        if (eventSent) return;
        playerHandManager.ActivatePlay(false);
        if (playerWon)
        {
            playerWonGame?.Invoke();
            CurtainCall();
        }
        else
        {
            playerLostGame?.Invoke();
            CurtainCall();
        }
        eventSent = true;
    }

    public void CurtainCall()
    {
        board.gameObject.SetActive(false);
        board.ResetBoard();
        opponentCardManager.EndBattle();
        playerHandManager.EndBattle();
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
