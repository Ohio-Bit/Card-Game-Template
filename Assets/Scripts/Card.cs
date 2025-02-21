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
}
