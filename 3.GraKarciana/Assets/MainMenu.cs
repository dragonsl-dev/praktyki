using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{
    public GameManager gameManager;

    public Button StartButton;
    public Button ExitButton;
    
    private void Start()
    {
        StartButton.onClick.AddListener(() =>
        {
            gameManager.GameStart();
            gameObject.SetActive(false);
        });

        ExitButton.onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });
    }
}
