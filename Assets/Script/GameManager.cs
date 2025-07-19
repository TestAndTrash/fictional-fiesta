using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using DG.Tweening;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField] private OpponentData opponentDatabase;
    [SerializeField] private GameObject opponentCardPrefab;
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private HandManager handManager;
    [SerializeField] private OpponentCardManager opponentCardManager;
    [SerializeField] private CardGameManager cardGameManager;
    [SerializeField] private EnhanceDeck enhanceDeck;
    [SerializeField] private CardData cardData;
    [SerializeField] private Dialogue dialogue;


    private List<int> godDeck = new();
    private List<Opponent> currOpponentChoice = new();
    private List<GameObject> physicalCards = new();
    private List<int> dbEntries = new();
    private Opponent currOpponent = null;

    private int needsToDraw = 0;
    private int drawed = 0;
    private int wonFight = 0;

    private bool deckInit = false;
    private bool bought = false;

    private string[] newLines;


    void Start()
    {
        currOpponent = opponentDatabase.entries[0];
        for (int i = 0; i < opponentDatabase.entries.Count; i++)
        {
            dbEntries.Add(i);
        }
        //Fill the shop/first card to add deck
        for (int i = 0; i < 120; i++)
        {
            //int randomInt = Random.Range(0, cardData.entries.Count);
            int randomInt = UnityEngine.Random.Range(1, 3);
            godDeck.Add(randomInt);
        }
        OpponentCardDisplay.OnCardDisplayClicked += OpponentChosed;
        CardGameManager.playerWonGame += RewardPlayer;
        CardGameManager.playerLostGame += DisplayGameOver;
        EnhanceDeck.playerChoseCard += AddCardToDeck;

        ManageRun();
    }

    public void DisplayOpponentChoice(int numberOfOpponent)
    {
        for (int i = 0; i < numberOfOpponent; i++)
        {
            int randomInt = UnityEngine.Random.Range(0, dbEntries.Count);
            Opponent opponent = opponentDatabase.GetOpponentById(dbEntries[randomInt]);
            currOpponentChoice.Add(opponent);
            GameObject opponentCardObject = Instantiate(opponentCardPrefab, spawnPoint.position, spawnPoint.rotation);
            OpponentCardDisplay opponentCardDisplay = opponentCardObject.GetComponent<OpponentCardDisplay>();
            opponentCardDisplay.FillCardData(opponent);
            physicalCards.Add(opponentCardObject);
            dbEntries.Remove(randomInt);
            UpdateCardPos(opponentCardDisplay);
        }
    }

    private void UpdateCardPos(OpponentCardDisplay cardDisplay)
    {
        if (physicalCards.Count == 0) return;
        float cardSpacing = 1f / 2;
        float firstCardPosition = 0.5f - (physicalCards.Count - 1) * cardSpacing / 2;
        Spline spline = splineContainer.Spline;
        for (int i = 0; i < physicalCards.Count; i++)
        {
            float position = firstCardPosition + i * cardSpacing;
            Vector3 splinePosition = spline.EvaluatePosition(position);
            Vector3 splinePositionWorld = splineContainer.transform.TransformPoint(splinePosition);
            Vector3 forward = spline.EvaluateTangent(position);
            Vector3 up = spline.EvaluateUpVector(position);
            Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);
            physicalCards[i].transform.DOLocalRotateQuaternion(rotation, 0.25f);
            physicalCards[i].transform.DOMove(splinePositionWorld, 0.25f).OnComplete(() =>
            {
                cardDisplay.traveling = false;
            });
        }
    }

    private void OpponentChosed(OpponentCardDisplay opponentCardDisplay)
    {
        currOpponent = opponentCardDisplay.opponentData;
        EmptyTheDisplay();  
        LaunchFight();
    }

    public void EmptyTheDisplay()
    {
        foreach (GameObject card in physicalCards)
        {
            Destroy(card);
        }
        physicalCards = new();
        currOpponentChoice = new();
    }

    public void RewardPlayer()
    {
        wonFight++;
        handManager.ChangeGold(currOpponent.goldGiven);
        needsToDraw = currOpponent.cardGiven;
        enhanceDeck.DrawRandCards(3, currOpponent.deck);
    }

    public void AddCardToDeck(CardManager cardManager)
    {
        handManager.gameObject.GetComponent<DeckManager>().AddCardToDeck(cardManager.card.id);
        drawed++;
        if (drawed < needsToDraw && currOpponent.deck.Count > 0)
        {
            if (!deckInit)
            {
                enhanceDeck.DrawRandCards(4, godDeck);
            }
            else
            {
                enhanceDeck.DrawRandCards(3, currOpponent != null ? currOpponent.deck : godDeck);
            }
        }
        else
        {
            drawed = 0;
            deckInit = true;
            ManageRun();
        }
    }

    public void LaunchDialogue()
    {
        newLines = new string[1];
        newLines[0] = "Hey there, welcome in the vegetable-top game championship !";
        dialogue.SetLines(newLines);
        dialogue.StartDialogue();
    }

    public void ManageRun()
    {
        if (!deckInit)
        {
            //DIALOGUE

            needsToDraw = 5;
            enhanceDeck.DrawRandCards(4, godDeck);
        }
        else if (wonFight == 0)
        {
            LaunchFight();
        }
        else if (wonFight == 1)
        {
            //DIALOGUE
            enhanceDeck.EmptyTheDisplay();
            DisplayOpponentChoice(2);
        }
        else if (wonFight == 2)
        {
            //DIALOGUE
            enhanceDeck.EmptyTheDisplay();
            DisplayOpponentChoice(2);
        }
        else if (wonFight == 3)
        {
            //DIALOGUE
            enhanceDeck.EmptyTheDisplay();
            DisplayOpponentChoice(2);
        }
        else if (!bought)
        {
            //DIALOGUE
            enhanceDeck.EmptyTheDisplay();
            //MERCHANT
        }
        else
        {
            LaunchFight();
        }
    }

    public void LaunchFight()
    {
        Debug.Log("LAUNCH FIGHT");
        enhanceDeck.EmptyTheDisplay();
        opponentCardManager.InitOpponent(currOpponent);
        cardGameManager.LaunchGame();
        //CARDGAMEMANAGER Launch game => board opponent and player display 
    }

    public void DisplayGameOver()
    {
        
    }

    void Update()
    {
        
    }
}
