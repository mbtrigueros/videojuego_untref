using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [SerializeField] Player[] players;
    [SerializeField] TextMeshProUGUI currentPlayerTurnText;

    [SerializeField] Image[] avatars;
    [SerializeField] TextMeshProUGUI[] integrityTexts, capacityTexts, playerTitles;

    [SerializeField] Color lorenzoColor;
    [SerializeField] Color carpiColor;

    // New Image components for integrity and capacity
    [SerializeField] Image[] integrityImage, capacityImage; 
    [SerializeField] Sprite[] integritySprites; // Array of sprites for integrity levels (sliced from the sprite sheet)
    [SerializeField] Sprite[] capacitySprites; // Array of sprites for capacity levels (sliced from the sprite sheet)

    void Start()
    {
        if (players != null)
        {
            for (int i = 0; i < players.Length; i++)
            {
                var boat = players[i].GetPlayerBoat();
                int playerIndex = i;
                boat.OnIntegrityChanged += (currentIntegrity) => UpdateBoatUI(playerIndex, currentIntegrity, boat.GetMaxIntegrity(), true);
                boat.OnCapacityChanged += (currentCapacity) => UpdateBoatUI(playerIndex, currentCapacity, boat.GetMaxCapacity(), false);
                TurnController.Instance.OnTurnChanged += HandleTurnChanged;
            }
            HandleTurnChanged(players[0]);

            InitializeHUD();
        }
    }

    private void HandleTurnChanged(Player currentPlayer)
    {
        for (int i = 0; i < players.Length; i++)
        {
            currentPlayerTurnText.GetComponent<Animator>().Play("fade_out");
            if (players[i] == currentPlayer)
            {
                SetCurrentPlayerAvatarAnimation(i);
                SetCurrentPlayerText(i);
            }
        }
    }

    // Unsubscribe from events when the HUDController is destroyed
    private void OnDestroy()
    {
        for (int i = 0; i < players.Length; i++)
        {
            var boat = players[i].GetPlayerBoat();
            int playerIndex = i;
            boat.OnIntegrityChanged -= (currentIntegrity) => UpdateBoatUI(playerIndex, currentIntegrity, boat.GetMaxIntegrity(), true);
            boat.OnCapacityChanged -= (currentCapacity) => UpdateBoatUI(playerIndex, currentCapacity, boat.GetMaxCapacity(), false);
        }
    }

    // Initialize the HUD with the current values for each boat
    void InitializeHUD()
    {
        for (int i = 0; i < players.Length; i++)
        {
            SetPlayerTitle(i);

            var boat = players[i].GetPlayerBoat();
            if (boat != null)
            {
                // Update the UI with the initial values for integrity and capacity
                UpdateBoatUI(i, players[i].GetPlayerBoat().Integrity, boat.GetMaxIntegrity(), true);
                UpdateBoatUI(i, players[i].GetPlayerBoat().Capacity, boat.GetMaxCapacity(), false);
            }
        }
    }

    void SetPlayerTitle(int playerIndex)
    {
        playerTitles[playerIndex].text = playerIndex == 0 ? "Lorenzo" : "Carpi";
    }

    void SetCurrentPlayerText(int playerIndex)
    {
        currentPlayerTurnText.text = playerIndex == 0 ? "Turno de Lorenzo" : "Turno de Carpi";
        currentPlayerTurnText.color = playerIndex == 0 ? lorenzoColor : carpiColor;
    }

    void SetCurrentPlayerAvatarAnimation(int playerIndex)
    {
        if (playerIndex == 0)
        {
            var animatorCurrent = avatars[0].GetComponent<Animator>();
            animatorCurrent.Rebind();
            animatorCurrent.Play("rotate");
            var animatorOther = avatars[1].GetComponent<Animator>();
            animatorOther.Rebind();
            animatorOther.Play("turn_gray");
        }
        else
        {
            var animatorCurrent = avatars[1].GetComponent<Animator>();
            animatorCurrent.Rebind();
            animatorCurrent.Play("rotate");
            var animatorOther = avatars[0].GetComponent<Animator>();
            animatorOther.Rebind();
            animatorOther.Play("turn_gray");
        }
    }

    // Update the UI for integrity or capacity based on the stat value
    void UpdateBoatUI(int playerIndex, int currentValue, int maxValue, bool isIntegrity)
    {
        // Update the sprite based on the stat value (integrity or capacity)
        if (isIntegrity)
        {
            UpdateIntegrityImage(playerIndex, currentValue, maxValue);
        }
        else
        {
            UpdateCapacityImage(playerIndex, currentValue, maxValue);
        }
    }

    void UpdateIntegrityImage(int playerIndex, int currentIntegrity, int maxValue)
    {
        // Ensure the integrity value stays within the bounds of the sprite array
        int spriteIndex = Mathf.Clamp(currentIntegrity, 0, integritySprites.Length - 1);

        // Update the sprite based on the current integrity value
        integrityImage[playerIndex].sprite = integritySprites[spriteIndex];
        integrityTexts[playerIndex].text = $"{currentIntegrity} / {maxValue}";
    }

    void UpdateCapacityImage(int playerIndex, int currentCapacity, int maxValue)
    {
        // Ensure the capacity value stays within the bounds of the sprite array
        int spriteIndex = Mathf.Clamp(currentCapacity, 0, capacitySprites.Length - 1);

        // Update the sprite based on the current capacity value
        capacityImage[playerIndex].sprite = capacitySprites[spriteIndex];
        capacityTexts[playerIndex].text = $"{currentCapacity} / {maxValue}";
    }
}
