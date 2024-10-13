using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cards : MonoBehaviour
{
    [SerializeField] List<CardDisplay> deck = new();
    [SerializeField] Transform[] cardSlots;
    [SerializeField] bool[] availableCardSlots;
    public void DrawCard() {
        if (deck.Count >= 1) {
            CardDisplay randomCard = deck[Random.Range(0, deck.Count)];

            for (int i = 0; i < availableCardSlots.Length; i++) {
                if (availableCardSlots[i]) {
                    randomCard.gameObject.SetActive(true);
                    randomCard.gameObject.transform.position = cardSlots[i].position;
                    availableCardSlots[i] = false;
                    deck.Remove(randomCard);
                    return;
                } 
            }

        }
    }

}
