using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Player : MonoBehaviour
{
    [SerializeField] private List<Player> players;
    [SerializeField] private Boat playerBoat;
    [SerializeField] private Route playerRoute;
    [SerializeField] private int cardsCount = 2;

    private List<Card> drawnCards = new List<Card>();
    public bool suscribed = false;

    private void Awake()
    {
        if (TurnController.Instance == null)
        {
            Debug.LogWarning("TurnController not ready, retrying...");
            return;
        }

        if (TurnController.Instance != null)
        {
            TurnController.Instance.OnTurnChanged += HandleTurnChanged;
            Card.OnCardClicked += PlayCard;
            suscribed = true;
            Debug.Log(this + " Suscribed to the events.");
        }
        else
        {
            Debug.LogWarning("TurnController instance is null, cannot subscribe.");
        }
    }

    private void Start() {
        players.Add(this);
        if (playerBoat != null && playerRoute != null) {
            playerBoat.OnBoatSunk += HandleBoatSunk;
            playerBoat.ResetToPort(playerRoute);
        } else {
            Debug.LogError("No boat found in the scene.");
        }
    }
    
    private void HandleBoatSunk()
    {
        Debug.Log("The boat has sunk.");
        playerBoat.ResetToPort(playerRoute);
    }

    private void OnDisable()
    {
        Debug.Log(this + " Unsuscribed from the events.");
        Card.OnCardClicked -= PlayCard;
        TurnController.Instance.OnTurnChanged -= HandleTurnChanged;
        suscribed = false;
        playerBoat.OnBoatSunk -= HandleBoatSunk;
    }

    private void Update()
    {
    }

    public void HandleTurnChanged(Player currentPlayer)
    {
        Debug.Log($"{name} received turn change. Current player is {currentPlayer.name}");
        if (currentPlayer == this)
        {
            Debug.Log(name + " can take their turn.");
            DrawCards();
        }
    }

    private void PlayCard(Card card)
    {
        Debug.Log($"Attempting to play card: {card.type}. Current player: {TurnController.Instance.CurrentPlayer().name}. This player: {name}");
        if (TurnController.Instance.IsCurrentPlayer(this))
        {
            Debug.Log("Current player confirmed.");
            Debug.Log(name + " used the card: " + card.type);
            StartCoroutine(CardPlayAction(card));
        }
        else
        {
            Debug.Log("It's not the turn of " + name);
        }
    }

    private IEnumerator CardPlayAction(Card card)
    {
        yield return StartCoroutine(GetCardAction(card, playerRoute));
        playerBoat.GetBoatDeck().DiscardAll();      
        TurnController.Instance.SwitchTurn();
    }

    private IEnumerator GetCardAction(Card card, Route route)
    {

        CaptainCard captainCard = card as CaptainCard;
        switch (card.action)
        {
            case CardAction.HEALTH:
            if (captainCard ) { playerBoat.TakeWater(captainCard.secondValue); }
                playerBoat.Repair(card.firstValue);
                yield break;
            case CardAction.MOVEMENT:
            if (captainCard) { playerBoat.TakeWater(captainCard.secondValue); }
                playerBoat.Move(route, card.firstValue);
                yield break;
            case CardAction.EMPTY:
                playerBoat.Empty(card.firstValue);
                yield break;
            case CardAction.ATTACK:
                foreach(Player otherPlayer in players) {
                    if(otherPlayer != TurnController.Instance.CurrentPlayer()) {
                        otherPlayer.playerBoat.TakeDamage(card.firstValue);
                    }
                }
                yield break;
            case CardAction.BUOY:
                // Do anchor action
                yield break;
        }
    }

    public void DrawCards()
    {
        // Now we get the Cards component from the boat
        Cards deck = playerBoat.GetBoatDeck();
        if (deck)   
        {
            // Draw cards and add them to the player's drawn cards list
            drawnCards.Clear();
            List<Card> newCards = deck.DrawCards(cardsCount);
            drawnCards.AddRange(newCards);
            Debug.Log(name + " has drawn " + newCards.Count + " cards.");
        }
        else
        {
            Debug.LogError("No deck found for the player boat!");
        }
    }

    public void ClearDrawnCards()
    {
        Debug.Log("Before clearing: " + drawnCards.Count);
        foreach (Card card in drawnCards)
        {
            card.gameObject.SetActive(false);
            Debug.Log("Clearing the drawn cards...");
        }
        drawnCards.Clear();
        Debug.Log("After clearing: " + drawnCards.Count);
    }

    public Boat GetPlayerBoat() {
        return playerBoat;
    }
}