using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using DG.Tweening;
using System;
using Assets.Script.HUD;

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
    private bool buying = false;

    private int wonFight = 0;
    private bool waitingForDialogue = false;
    private bool deckInit = false;
    private bool bought = false;

    private bool deckInitDialogueClear = false;
    private bool fight0DialogueClear = false;
    private bool fight1DialogueClear = false;
    private bool fight2DialogueClear = false;
    private bool fight3DialogueClear = false;
    private bool hasntBoughtDialogueClear = false;
    private bool beforeBossDialogueClear = false;

    private string[] newLines;

    public static event Action shopOpen;

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
        Dialogue.dialogueDone += DialogueOver;
        EnhanceDeck.playerBoughtCard += BuyCard;
        LeaveShop.playerLeaveShop += PlayerLeaveShop;

        //TEST 
        /*deckInitDialogueClear = true;
        fight0DialogueClear = true;
        fight1DialogueClear = true;
        fight2DialogueClear = true;
        fight3DialogueClear = true;
        wonFight = 4;*/
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
        enhanceDeck.DrawRandCards(3, currOpponent.deck, false);
    }

    public void AddCardToDeck(CardManager cardManager)
    {
        handManager.gameObject.GetComponent<DeckManager>().AddCardToDeck(cardManager.card.id);
        drawed++;
        if (drawed < needsToDraw && currOpponent.deck.Count > 0)
        {
            if (buying)
            {
                enhanceDeck.DrawRandCards(3, godDeck, true);
            }
            else if (!deckInit)
            {
                enhanceDeck.DrawRandCards(4, godDeck, false);
            }
            else
            {
                enhanceDeck.DrawRandCards(3, currOpponent != null ? currOpponent.deck : godDeck, false);
            }
        }
        else
        {
            drawed = 0;
            if (!buying) deckInit = true;
            else
            {
                buying = false;
                bought = true;
            }      
            ManageRun();
        }
    }

    public void BuyCard(CardManager cardManager)
    {
        if (handManager.totalGold >= cardManager.card.price)
        {
            handManager.ChangeGold(-cardManager.price);
            AddCardToDeck(cardManager);
        }
        else Debug.Log("No gold, no card");
    }

    public void PlayerLeaveShop()
    {
        buying = false;
        bought = true;
        ManageRun();
    }
    public void LaunchDialogue()
    {
        waitingForDialogue = true;
        if (!deckInitDialogueClear)
        {
            newLines = new string[5];
            newLines[0] = "Hey there, welcome in the vegetable-top game championship ! I'm gonna introduce the game for you don't you worry, macaroni.";
            newLines[1] = "First it's this local tournament then It'll be a regional, and maybe the worlds?? Who knows ? (I do).";
            newLines[2] = "I know this must be weird, for you but to make things easy to understand... Let's say that I summonned you to play for this championship.";
            newLines[3] = "I believe in you... first let's crack a booster should we ?";
            newLines[4] = "Choose the card you want to add to you deck for the remaining of the championship.";
        }
        else if (!fight0DialogueClear)
        {
            newLines = new string[2];
            newLines[0] = "Stonks... as they say. Since you never played before, I'm making you play against my friend Marcus.";
            newLines[1] = "He's highly drunk so don't worry, you won't loose. And an advice protect your pots.";
        }
        else if (!fight1DialogueClear)
        {
            newLines = new string[3];
            newLines[0] = "You're a natural ! And you looted his deck too... Man you did him dirty, my man has a wife and two kids...";
            newLines[1] = "It's okay keep the thief act, I love it. You'll grow stronger as we progress through the event !";
            newLines[2] = "Now as my father said, choose your weapon but also choose your ennemy. Never got along with the old man...";
        }
        else if (!fight2DialogueClear)
        {
            newLines = new string[3];
            newLines[0] = "Man are you starting to feel the thrill ? The smell of them blood in our hands hahahaha. ha ? ha !";
            newLines[1] = "Keep looting those juicy decks. You are qualified for the next step.";
            newLines[2] = "As I am, of course.";
        }
        else if (!fight3DialogueClear)
        {
            newLines = new string[3];
            newLines[0] = "What was that ! Player !";
            newLines[1] = "Sorry never asked your name but.. don't care at this point we've been making it work without that futile info.";
            newLines[2] = "Semi-finals bruv, good luck, respect your tengland.";
        }
        else if (!hasntBoughtDialogueClear)
        {
            newLines = new string[3];
            newLines[0] = "Oh maaan this one was my favorite ! Now you know the basics and some advanced strats.";
            newLines[1] = "As my father also said : Every card game is pay to win, son.";
            newLines[2] = "I see a card market across the room, maybe we should take a look.";
        }
        else if (!beforeBossDialogueClear)
        {
            newLines = new string[3];
            newLines[0] = "So this is the time of our lives ? We finally met on the board huh ?";
            newLines[1] = "You bought all this, for nothing man, that's kinda sad, could've kept those coins for yo mama tho.";
            newLines[2] = "Imma say one last thing: Adversaire très fort et très respectable sur le terrain.";
        }
        else
        {
            newLines = new string[4];
            newLines[0] = "The heck bro ? You fooled me into thinking you were a newbie !";
            newLines[1] = "Is this game huge in your realm or what ? I tought I'd train my human pet to conquish the whole vegetable-top game scene...";
            newLines[2] = "You've killed the immortal tutorial boss, nice dude...";
            newLines[3] = "That's rought bro, my hopes and dreams... We were just getting started !";
        }
        dialogue.SetLines(newLines);
        dialogue.StartDialogue();
    }

    public void DialogueOver()
    {
        waitingForDialogue = false;
        if (!deckInitDialogueClear) deckInitDialogueClear = true;
        else if (!fight0DialogueClear) fight0DialogueClear = true;
        else if (!fight1DialogueClear) fight1DialogueClear = true;
        else if (!fight2DialogueClear) fight2DialogueClear = true;
        else if (!fight3DialogueClear) fight3DialogueClear = true;
        else if (!hasntBoughtDialogueClear) hasntBoughtDialogueClear = true;
        else if (!beforeBossDialogueClear) beforeBossDialogueClear = true;

        ManageRun();
    }

    public void ManageRun()
    {
        if (waitingForDialogue) return;
        if (!deckInitDialogueClear)
        {
            LaunchDialogue();
        }
        else if (!deckInit)
        {
            needsToDraw = 5;
            enhanceDeck.DrawRandCards(4, godDeck, false);
        }
        else if (!fight0DialogueClear)
        {
            enhanceDeck.EmptyTheDisplay();
            LaunchDialogue();
        }
        else if (wonFight == 0)
        {
            LaunchFight();
        }
        else if (!fight1DialogueClear)
        {
            enhanceDeck.EmptyTheDisplay();
            LaunchDialogue();
        }
        else if (wonFight == 1)
        {
            DisplayOpponentChoice(2);
        }
        else if (!fight2DialogueClear)
        {
            enhanceDeck.EmptyTheDisplay();
            LaunchDialogue();
        }
        else if (wonFight == 2)
        {
            DisplayOpponentChoice(2);
        }
        else if (!fight3DialogueClear)
        {
            enhanceDeck.EmptyTheDisplay();
            LaunchDialogue();
        }
        else if (wonFight == 3)
        {
            DisplayOpponentChoice(2);
        }
        else if (!hasntBoughtDialogueClear)
        {
            enhanceDeck.EmptyTheDisplay();
            LaunchDialogue();
        }
        else if (!bought)
        {
            enhanceDeck.EmptyTheDisplay();
            enhanceDeck.DrawRandCards(3, godDeck, true);
            shopOpen?.Invoke();
            buying = true;
        }
        else if (!beforeBossDialogueClear)
        {
            enhanceDeck.EmptyTheDisplay();
            LaunchDialogue();
        }
        else if (wonFight == 4)
        {
            Debug.Log("boos fight !");
            LaunchFight();
        }
        else
        {
            LaunchDialogue();
        }
    }

    public void LaunchFight()
    {
        enhanceDeck.EmptyTheDisplay();
        opponentCardManager.InitOpponent(currOpponent);
        cardGameManager.LaunchGame();
    }

    public void DisplayGameOver()
    {
        
    }

    void Update()
    {
        
    }
}
