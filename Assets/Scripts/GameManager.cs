using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

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

    [SerializeField] private Vector3 playerHandStartPosition = new Vector3(-2f, -3f, 0f); // Adjust these values as needed
    [SerializeField] private float cardOffset = 0.5f; // Horizontal spacing between cards
    private List<GameObject> playerCardObjects = new List<GameObject>();

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
            player_hand.Add(deck[0]);
            deck.RemoveAt(0);
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
            int randomIndex = Random.Range(0, i + 1);
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
        player_hand.Add(deck[0]);
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

        // Create the visual representation of the card
        GameObject cardObject = deck[0].CreateCardObject(); // Assuming you have a method to create card GameObjects
        cardObject.transform.position = playerHandStartPosition + new Vector3(playerCardObjects.Count * cardOffset, 0f, 0f);
        
        playerCardObjects.Add(cardObject);
    }

    void AI_Hit()
    {
        ai_hand.Add(deck[0]);
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

        // Create the visual representation of the card
        GameObject cardObject = deck[0].CreateCardObject(); // Assuming you have a method to create card GameObjects
        cardObject.transform.position = playerHandStartPosition + new Vector3(playerCardObjects.Count * cardOffset, 0f, 0f);
        
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
