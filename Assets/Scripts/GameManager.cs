using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
// using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;
    public List<Card> deck = new List<Card>();
    public List<Card> player_hand = new List<Card>();
    public List<Card> ai_hand = new List<Card>();
    public bool noplayerace = true;
    public int aceplayervalue = 11;
    public bool noAIace = true;
    public int aceAIvalue = 11;

    [SerializeField] private Canvas gameCanvas; // Reference to your main canvas
    [SerializeField] private Vector2 playerHandStartPosition = new Vector2(100f, 100f); // Player cards at bottom
    [SerializeField] private Vector2 aiHandStartPosition = new Vector2(100f, 400f); // AI cards higher up
    [SerializeField] private float cardOffset = 100f;
    private List<GameObject> playerCardObjects = new List<GameObject>();
    private List<GameObject> aiCardObjects = new List<GameObject>(); // New list for AI cards
    [SerializeField] private GameObject cardPrefab; // Assign your card prefab in the inspector
    [SerializeField] private Transform cardParent; // Optional: parent transform to keep cards organized
    [SerializeField] private TextMeshProUGUI playerTotalText;
    [SerializeField] private TextMeshProUGUI aiTotalText;
    [SerializeField] private Vector2 playerTotalOffset = new Vector2(150f, 0f); // Offset from last card
    [SerializeField] private Vector2 aiTotalOffset = new Vector2(150f, 0f);
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private TextMeshProUGUI scoreText;
    private int playerWins = 0;
    private int aiWins = 0;
    private bool gameEnded = false;
    private List<Card> discardPile = new List<Card>();

    private void Awake()
    {
        if (gm != null && gm != this)
        {
            Destroy(gameObject);
        }
        else
        {
            gm = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        InitializeDeck();
        if (deck.Count > 0)
        {
            InitializeGame();
        }
        else
        {
            Debug.LogError("Failed to initialize deck!");
        }
    }

    void InitializeDeck()
    {
        // The deck is already populated, just shuffle it
        if (deck.Count > 0)
        {
            Shuffle();
            Debug.Log($"Deck shuffled with {deck.Count} cards");
        }
        else
        {
            Debug.LogError("Deck is empty! Make sure cards are assigned in the inspector.");
        }
    }

    void InitializeGame()
    {
        // Now we clear the visual cards at the start of a new game
        ClearHandDisplays();
        
        // Clear hands
        player_hand.Clear();
        ai_hand.Clear();
        gameEnded = false;
        
        if (winnerText != null)
            winnerText.text = "";
            
        UpdateScoreDisplay();
        
        Debug.Log($"Starting new game with {deck.Count} cards in deck and {discardPile.Count} cards in discard");
        Deal();
    }

    void ResetDeck()
    {
        HashSet<Card> cardsInPlay = new HashSet<Card>();
        cardsInPlay.UnionWith(player_hand);
        cardsInPlay.UnionWith(ai_hand);

        deck.Clear();

        // Update path to match your actual folder structure
        Card_data[] allCardData = Resources.LoadAll<Card_data>("Dealer");

        foreach (Card_data cardData in allCardData)
        {
            bool isInPlay = cardsInPlay.Any(c => c.data == cardData);
            
            if (!isInPlay)
            {
                Card newCard = new Card();
                newCard.data = cardData;
                
                // Parse the card data name to get suit and rank
                string cardName = cardData.name;
                char suitChar = cardName[0];
                string rankChar = cardName.Substring(1);
                
                // Convert suit character to full name
                switch (suitChar)
                {
                    case 'c': newCard.suit = "Clubs"; break;
                    case 'd': newCard.suit = "Diamonds"; break;
                    case 'h': newCard.suit = "Hearts"; break;
                    case 's': newCard.suit = "Spades"; break;
                    default: Debug.LogError($"Unknown suit in card {cardName}"); continue;
                }
                
                // Convert rank character to full name and value
                switch (rankChar)
                {
                    case "2": case "3": case "4": case "5": case "6": 
                    case "7": case "8": case "9": case "10":
                        newCard.rank = rankChar;
                        newCard.value = int.Parse(rankChar);
                        break;
                    case "j":
                        newCard.rank = "Jack";
                        newCard.value = 10;
                        break;
                    case "q":
                        newCard.rank = "Queen";
                        newCard.value = 10;
                        break;
                    case "k":
                        newCard.rank = "King";
                        newCard.value = 10;
                        break;
                    case "a":
                        newCard.rank = "Ace";
                        newCard.value = 11;
                        break;
                    default:
                        Debug.LogError($"Unknown rank in card {cardName}");
                        continue;
                }
                
                newCard.data.value = newCard.value;
                deck.Add(newCard);
            }
        }

        if (deck.Count > 0)
        {
            Shuffle();
            Debug.Log($"Deck reshuffled with {deck.Count} cards (excluding {cardsInPlay.Count} cards in play)");
        }
        else
        {
            Debug.LogWarning("All cards are in play - cannot reshuffle!");
        }
    }

    void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Player Wins: {playerWins} | AI Wins: {aiWins}";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameEnded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Hit();
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                Stay();
            }
        }
        else if (Input.GetKeyDown(KeyCode.R)) // Only allow restart if game has ended
        {
            RestartGame();
        }
    }

    void Deal()
    {
        CheckDeckSize();
        
        if (deck.Count < 4)
        {
            Debug.LogError("Not enough cards to deal!");
            return;
        }

        // Deal 2 cards to player
        for (int i = 0; i < 2; i++)
        {
            Card currentCard = deck[0];
            if (currentCard.rank == "Ace")
            {
                currentCard.value = 11;
                currentCard.data.value = 11;
            }
            player_hand.Add(currentCard);
            deck.RemoveAt(0);

            // Instantiate the card visually as UI element
            GameObject cardObject = Instantiate(cardPrefab, gameCanvas.transform);
            RectTransform rectTransform = cardObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = playerHandStartPosition + new Vector2(playerCardObjects.Count * cardOffset, 0f);
            
            Card cardComponent = cardObject.GetComponent<Card>();
            cardComponent.data = currentCard.data;
            
            playerCardObjects.Add(cardObject);
        }

        // Deal 2 cards to AI
        for (int i = 0; i < 2; i++)
        {
            Card currentCard = deck[0];
            if (currentCard.rank == "Ace")
            {
                currentCard.value = 11;
                currentCard.data.value = 11;
            }
            ai_hand.Add(currentCard);
            deck.RemoveAt(0);

            // Instantiate the card visually as UI element
            GameObject cardObject = Instantiate(cardPrefab, gameCanvas.transform);
            RectTransform rectTransform = cardObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = aiHandStartPosition + new Vector2(aiCardObjects.Count * cardOffset, 0f);
            
            Card cardComponent = cardObject.GetComponent<Card>();
            cardComponent.data = currentCard.data;
            
            aiCardObjects.Add(cardObject);
        }

        UpdateTotals();
    }

    void Shuffle()
    {
        // Fisher-Yates shuffle algorithm
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            Card temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    void Stay()
    {
        // Player stays, AI takes their full turn
        while (!gameEnded)
        {
            AI_Turn();
        }
    }

    void AI_Turn()
    {
        if (gameEnded) return;

        int ai_total = 0;
        foreach (Card card in ai_hand)
        {
            ai_total += card.value;
        }

        if (ai_total > 21)
        {
            AIBust();
            // Recalculate total after potential ace adjustments
            ai_total = 0;
            foreach (Card card in ai_hand)
            {
                ai_total += card.value;
            }
        }

        if (ai_total < 17)
        {
            AI_Hit();
        }
        else
        {
            // AI stands at 17 or higher
            DetermineWinner();
        }
    }

    void Hit()
    {
        CheckDeckSize();
        
        if (deck.Count == 0)
        {
            Debug.LogWarning("No cards available to draw!");
            return;
        }
        
        Card currentCard = deck[0];
        if (currentCard.rank == "Ace")
        {
            currentCard.value = 11;
            currentCard.data.value = 11;
        }
        player_hand.Add(currentCard);
        deck.RemoveAt(0);

        // Instantiate the card visually as UI element
        GameObject cardObject = Instantiate(cardPrefab, gameCanvas.transform);
        RectTransform rectTransform = cardObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = playerHandStartPosition + new Vector2(playerCardObjects.Count * cardOffset, 0f);
        
        Card cardComponent = cardObject.GetComponent<Card>();
        cardComponent.data = currentCard.data;
        
        playerCardObjects.Add(cardObject);

        UpdateTotals();
        PlayerBust(); // Call PlayerBust only once after adding the new card
    }

    void AI_Hit()
    {
        CheckDeckSize();
        
        if (deck.Count == 0)
        {
            Debug.LogWarning("No cards available to draw!");
            return;
        }
        
        Card currentCard = deck[0];
        if (currentCard.rank == "Ace")
        {
            currentCard.value = 11;
            currentCard.data.value = 11;
        }
        ai_hand.Add(currentCard);
        deck.RemoveAt(0);

        // Instantiate the card visually as UI element
        GameObject cardObject = Instantiate(cardPrefab, gameCanvas.transform);
        RectTransform rectTransform = cardObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = aiHandStartPosition + new Vector2(aiCardObjects.Count * cardOffset, 0f);
        
        Card cardComponent = cardObject.GetComponent<Card>();
        cardComponent.data = currentCard.data;
        
        aiCardObjects.Add(cardObject);

        UpdateTotals();
        AIBust(); // Call AIBust only once after adding the new card
    }
    void PlayerBust()
    {
        int player_total = 0;
        int numAces = 0;
        List<Card> aceCards = new List<Card>();

        // First pass: calculate total and find aces
        foreach (Card card in player_hand)
        {
            player_total += card.value;
            if (card.rank == "Ace" && card.value == 11)
            {
                aceCards.Add(card);
            }
        }

        // Convert aces from 11 to 1 until we're under 21 or run out of aces
        while (player_total > 21 && aceCards.Count > 0)
        {
            Card aceCard = aceCards[0];
            player_total -= 10; // Subtract 10 (changing from 11 to 1)
            aceCard.value = 1;
            aceCard.data.value = 1; // Update the data value as well
            aceCards.RemoveAt(0);
        }

        UpdateTotals();

        if (player_total > 21)
        {
            DetermineWinner();
        }
    }
    void AIBust()
    {
        int ai_total = 0;
        int numAces = 0;
        List<Card> aceCards = new List<Card>();

        // First pass: calculate total and find aces
        foreach (Card card in ai_hand)
        {
            ai_total += card.value;
            if (card.rank == "Ace" && card.value == 11)
            {
                aceCards.Add(card);
            }
        }

        // Convert aces from 11 to 1 until we're under 21 or run out of aces
        while (ai_total > 21 && aceCards.Count > 0)
        {
            Card aceCard = aceCards[0];
            ai_total -= 10; // Subtract 10 (changing from 11 to 1)
            aceCard.value = 1;
            aceCard.data.value = 1; // Update the data value as well
            aceCards.RemoveAt(0);
        }

        UpdateTotals();

        if (ai_total > 21)
        {
            DetermineWinner();
        }
    }

    public void ClearHandDisplays()
    {
        foreach (GameObject cardObj in playerCardObjects)
        {
            Destroy(cardObj);
        }
        playerCardObjects.Clear();

        foreach (GameObject cardObj in aiCardObjects)
        {
            Destroy(cardObj);
        }
        aiCardObjects.Clear();
    }

    void UpdateTotals()
    {
        // Calculate and display player total
        int playerTotal = 0;
        foreach (Card card in player_hand)
        {
            playerTotal += card.value;
        }
        
        if (playerTotalText != null)
        {
            playerTotalText.text = $"Total: {playerTotal}";
        }

        // Calculate and display AI total
        int aiTotal = 0;
        foreach (Card card in ai_hand)
        {
            aiTotal += card.value;
        }
        
        if (aiTotalText != null)
        {
            aiTotalText.text = $"Total: {aiTotal}";
        }
    }

    private void RestartGame()
    {
        // Move all cards from hands to discard pile
        MoveToDiscardPile(player_hand);
        MoveToDiscardPile(ai_hand);
        
        // Only check if we need to reshuffle
        CheckDeckSize();
        
        InitializeGame();
    }

    void MoveToDiscardPile(List<Card> cards)
    {
        if (cards == null || cards.Count == 0) return;
        
        foreach (Card card in cards.ToList()) // Use ToList() to avoid modification during enumeration
        {
            discardPile.Add(card);
            Debug.Log($"Moving {card.rank} of {card.suit} to discard pile");
        }
        cards.Clear();
        Debug.Log($"Discard pile now contains {discardPile.Count} cards");
    }

    void DetermineWinner()
    {
        int playerTotal = 0;
        int aiTotal = 0;

        foreach (Card card in player_hand)
        {
            playerTotal += card.value;
        }

        foreach (Card card in ai_hand)
        {
            aiTotal += card.value;
        }

        string winner = "";

        if (playerTotal > 21)
        {
            winner = "AI Wins!";
            aiWins++;
        }
        else if (aiTotal > 21)
        {
            winner = "Player Wins!";
            playerWins++;
        }
        else if (playerTotal > aiTotal)
        {
            winner = "Player Wins!";
            playerWins++;
        }
        else if (aiTotal > playerTotal)
        {
            winner = "AI Wins!";
            aiWins++;
        }
        else
        {
            winner = "It's a Tie!";
        }

        if (winnerText != null)
        {
            winnerText.text = winner;
        }
        
        // Move cards to discard pile (but keep visuals)
        MoveToDiscardPile(player_hand);
        MoveToDiscardPile(ai_hand);
        
        Debug.Log($"Round ended. Discard pile now has {discardPile.Count} cards, Deck has {deck.Count} cards");
        
        // Check if deck needs reshuffling after moving cards to discard
        CheckDeckSize();
        
        UpdateScoreDisplay();
        gameEnded = true;
    }

    void CheckDeckSize()
    {
        // Only shuffle in discard pile if deck is running very low
        if (deck.Count <= 10 && discardPile.Count > 0)
        {
            Debug.Log($"Deck running low ({deck.Count} cards). Shuffling in discard pile ({discardPile.Count} cards)");
            deck.AddRange(discardPile);
            discardPile.Clear();
            Shuffle();
            Debug.Log($"Deck now has {deck.Count} cards after shuffling");
        }
    }
}
