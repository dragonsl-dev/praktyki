using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameEndWnd : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject WinPanel;
    public GameObject LossPanel;

    public TMP_Text WinningPlayerNameText;
    public Button ContinueButton;

    private void Start()
    {
        ContinueButton.onClick.AddListener(() =>
        {
            WinPanel.SetActive(false);
            LossPanel.SetActive(false);
            gameManager.GameContinue();
            gameObject.SetActive(false);
        });
        gameObject.SetActive(false);
    }

    public void ShowWin()
    {
        gameObject.SetActive(true);
        WinPanel.SetActive(true);
        LossPanel.SetActive(false);
    }

    public void ShowLoss(Player player)
    {
        gameObject.SetActive(true);
        WinningPlayerNameText.text = $"Wygrał gracz: {player.Name}";
        WinPanel.SetActive(false);
        LossPanel.SetActive(true);
    }
}
