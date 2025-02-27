using System;
using System.Collections;
using System.Collections.Generic;
// using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private Vector2 playerHandStartPosition = new Vector2(100f, 100f); // Update to use Vector2 for UI positioning
    [SerializeField] private float cardOffset = 100f; // Increased offset for UI space
    private List<GameObject> playerCardObjects = new List<GameObject>();
    [SerializeField] private GameObject cardPrefab; // Assign your card prefab in the inspector
    [SerializeField] private Transform cardParent; // Optional: parent transform to keep cards organized

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
            ai_hand.Add(deck[0]);
            deck.RemoveAt(0);
        }
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

        if (ai_total < 17)
        {
            AI_Hit();
        }
    }

    void Hit()
    {
        Card currentCard = deck[0];
        player_hand.Add(currentCard);
        deck.RemoveAt(0);
        PlayerBust();
        
        foreach (Card card in player_hand)
        {
            if (card.rank == "Ace")
            {
                noplayerace = false;
                card.value = aceplayervalue;
            }
        }

        // Instantiate the card visually as UI element
        GameObject cardObject = Instantiate(cardPrefab, gameCanvas.transform);
        RectTransform rectTransform = cardObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = playerHandStartPosition + new Vector2(playerCardObjects.Count * cardOffset, 0f);
        
        Card cardComponent = cardObject.GetComponent<Card>();
        cardComponent.data = currentCard.data;
        
        playerCardObjects.Add(cardObject);
    }

    void AI_Hit()
    {
        Card currentCard = deck[0];
        ai_hand.Add(currentCard);
        deck.RemoveAt(0);
        AIBust();
        
        foreach (Card card in ai_hand)
        {
            if (card.rank == "Ace")
            {
                noAIace = false;
                card.value = aceAIvalue;
            }
        }

        // Instantiate the card visually as UI element
        GameObject cardObject = Instantiate(cardPrefab, gameCanvas.transform);
        RectTransform rectTransform = cardObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = playerHandStartPosition + new Vector2(playerCardObjects.Count * cardOffset, 0f);
        
        Card cardComponent = cardObject.GetComponent<Card>();
        cardComponent.data = currentCard.data;
        
        playerCardObjects.Add(cardObject);
    }
    void PlayerBust()
    {
        int player_total = 0;
        foreach (Card card in player_hand)
        {
            player_total += card.value;
        }

        if (player_total > 21 && noplayerace == true)
        {
            // Player busts
        }
        else if (player_total > 21 && noplayerace == false)
        {
            aceplayervalue = 1;
            PlayerBust();
        }
    }
    void AIBust()
    {
        int ai_total = 0;
        foreach (Card card in ai_hand)
        {
            ai_total += card.value;
        }
        if (ai_total > 21 && noAIace == true)
        {
            // AI busts
        }
        else if (ai_total > 21 && noAIace == false)
        {
            aceAIvalue = 1;
            AIBust();
        }
    }

    public void ClearPlayerHandDisplay()
    {
        foreach (GameObject cardObj in playerCardObjects)
        {
            Destroy(cardObj);
        }
        playerCardObjects.Clear();
    }
}
