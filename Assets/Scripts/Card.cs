using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Card_data data;

    public string suit;
  
    public string rank;
    
    public int value;
    public Sprite sprite;
    public TextMeshProUGUI suitText;
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI valueText;
    public Image spriteImage;
        

    // Start is called before the first frame update
    void Start()
    {
        value = data.value;
        suit = data.suit;
        sprite = data.sprite;
        rank = data.rank;
        suitText.text = suit;
        rankText.text = rank;
        valueText.text = value.ToString();
        
        spriteImage.sprite = sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject CreateCardObject()
    {
        GameObject cardObj = new GameObject($"Card_{suit}_{rank}");
        SpriteRenderer spriteRenderer = cardObj.AddComponent<SpriteRenderer>();
        
        // Assuming you have card sprites named appropriately
        string spritePath = $"Cards/{suit}_{rank}"; // Adjust path according to your project structure
        Sprite cardSprite = Resources.Load<Sprite>(spritePath);
        spriteRenderer.sprite = cardSprite;
        
        return cardObj;
    }
}
