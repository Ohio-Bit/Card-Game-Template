using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;
    public List<Card> deck = new List<Card>();
    public List<Card> player_hand = new List<Card>();
    public List<Card> ai_hand = new List<Card>();

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

    }

    void Hit()
    {
        player_hand.Add(deck[0]);
        deck.RemoveAt(0);
    }

    void AI_Hit()
    {
        ai_hand.Add(deck[0]);
        deck.RemoveAt(0);
    }



    
}
