using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DebugMenu : MonoBehaviour
{
    public GameManager gameManager;
    private void Start()
    {
#if UNITY_EDITOR
        print("D");
#else
        Destroy(this);
#endif

        GiveCoins.onClick.AddListener(() =>
        {
            
        });
    }

    public Button GiveCoins;

    

   
}
