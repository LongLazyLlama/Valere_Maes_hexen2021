using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CardHand : MonoBehaviour
{
    [SerializeField]
    private int _cardsInDeck = 10;
    [SerializeField]
    private GameObject[] CardPrefabs = null;
    [HideInInspector]
    public static CardHand Deck;

    public UnityEvent forward;

    public void Start() 
    {
        if (Deck == null)
        {
            Deck = this;
        }
    }

    public void CardUsed(Vector3 cardSlotPosition)
    {
        if (_cardsInDeck > 0)
        {
            var randomValue = Random.Range(0, CardPrefabs.Length);
            Instantiate(CardPrefabs[randomValue], cardSlotPosition, Quaternion.identity, this.transform);
        }
        _cardsInDeck--;

        forward?.Invoke();
    }
}
