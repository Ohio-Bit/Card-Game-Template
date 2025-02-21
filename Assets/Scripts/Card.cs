using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Card_data data;

    public string suit;
    public string description;
    public int health;
    public int cost;
    public int value;
    public Sprite sprite;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI valueText;
    public Image spriteImage;
        

    // Start is called before the first frame update
    void Start()
    {
        value = data.value;
        suit = data.suit;
        sprite = data.sprite;
        nameText.text = suit;
        descriptionText.text = description;
        healthText.text = health.ToString();
        costText.text = cost.ToString();
        valueText.text = value.ToString();
        spriteImage.sprite = sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
