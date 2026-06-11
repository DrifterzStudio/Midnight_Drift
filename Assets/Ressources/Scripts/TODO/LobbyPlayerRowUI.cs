using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LobbyPlayerRowUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TMP_Text nameText;

    [Header("Host Controls")]
    [SerializeField] private Button toggleButton;
    [SerializeField] private TMP_Text buttonLabel;

    [Header("Client Status")]
    [SerializeField] private TMP_Text statusText;

    public void Setup(string playerName, ulong steamId, bool isActive, bool isHost, Action<ulong> onToggleCallback)
    {
        // 1. Couleurs et textes de base
        if (isActive)
        {
            backgroundImage.color = new Color(0.15f, 0.55f, 0.15f, 0.4f); // Vert
            nameText.text = $"[J] {playerName}";
        }
        else
        {
            backgroundImage.color = new Color(0.55f, 0.15f, 0.15f, 0.4f); // Rouge
            nameText.text = $"[S] {playerName}";
        }

        // 2. Gestion de l'affichage Host vs Client
        if (isHost)
        {
            statusText.gameObject.SetActive(false);
            toggleButton.gameObject.SetActive(true);

            // Configuration du bouton
            buttonLabel.text = isActive ? "Joueur" : "Spectateur";
            toggleButton.image.color = isActive ? new Color(0.1f, 0.75f, 0.1f) : new Color(0.75f, 0.1f, 0.1f);

            // On nettoie les anciens listeners pour éviter les doublons
            toggleButton.onClick.RemoveAllListeners();
            toggleButton.onClick.AddListener(() => onToggleCallback?.Invoke(steamId));
        }
        else
        {
            toggleButton.gameObject.SetActive(false);
            statusText.gameObject.SetActive(true);

            statusText.text = isActive ? "Joueur" : "Spectateur";
            statusText.color = isActive ? Color.green : Color.red;
        }
    }
}