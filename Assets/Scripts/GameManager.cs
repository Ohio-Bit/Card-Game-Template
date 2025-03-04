using System;
using System.Collections;
using System.Collections.Generic;
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
        Shuffle();
        Deal();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Hit();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            AI_Turn();
        }
        foreach (Card card in player_hand)
        {
        
        }
    }

    void Deal()
    {
        // Deal 2 cards to player
        for (int i = 0; i < 2; i++)
        {
            Card currentCard = deck[0];
            if (currentCard.rank == "Ace")  // Set initial ace value to 11
            {
                currentCard.value = 11;
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
            if (currentCard.rank == "Ace")  // Set initial ace value to 11
            {
                currentCard.value = 11;
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
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            Card temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    void AI_Turn()
    {
        int ai_total = 0;
        foreach (Card card in ai_hand)
        {
            ai_total += card.value;
        }

        // Check for aces if over 21
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
    }

    void Hit()
    {
        Card currentCard = deck[0];
        if (currentCard.rank == "Ace")  // Set initial ace value to 11
        {
            currentCard.value = 11;
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
        Card currentCard = deck[0];
        if (currentCard.rank == "Ace")  // Set initial ace value to 11
        {
            currentCard.value = 11;
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

        // First count aces and calculate total
        foreach (Card card in player_hand)
        {
            player_total += card.value;
            if (card.rank == "Ace")
            {
                numAces++;
            }
        }

        // While we're over 21 and we have aces worth 11, convert them to 1
        while (player_total > 21 && numAces > 0)
        {
            foreach (Card card in player_hand)
            {
                if (card.rank == "Ace" && card.value == 11)
                {
                    card.value = 1;
                    player_total -= 10; // Subtract the difference between 11 and 1
                    numAces--;
                    break;
                }
            }
        }

        UpdateTotals();

        // Only check for bust after all possible ace conversions
        if (player_total > 21)
        {
            Debug.Log("Player Busted!"); // You can replace this with your bust handling logic
        }
    }
    void AIBust()
    {
        int ai_total = 0;
        int numAces = 0;

        // First count aces and calculate total
        foreach (Card card in ai_hand)
        {
            ai_total += card.value;
            if (card.rank == "Ace")
            {
                numAces++;
            }
        }

        // While we're over 21 and we have aces worth 11, convert them to 1
        while (ai_total > 21 && numAces > 0)
        {
            foreach (Card card in ai_hand)
            {
                if (card.rank == "Ace" && card.value == 11)
                {
                    card.value = 1;
                    ai_total -= 10; // Subtract the difference between 11 and 1
                    numAces--;
                    break;
                }
            }
        }

        UpdateTotals();

        // Only check for bust after all possible ace conversions
        if (ai_total > 21)
        {
            Debug.Log("AI Busted!"); // You can replace this with your bust handling logic
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
            playerTotalText.rectTransform.anchoredPosition = 
                playerHandStartPosition + new Vector2((playerCardObjects.Count - 1) * cardOffset, 0f) + playerTotalOffset;
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
            aiTotalText.rectTransform.anchoredPosition = 
                aiHandStartPosition + new Vector2((aiCardObjects.Count - 1) * cardOffset, 0f) + aiTotalOffset;
        }
    }
}
