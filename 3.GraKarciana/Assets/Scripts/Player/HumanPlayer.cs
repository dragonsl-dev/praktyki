using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
public class HumanPlayer : Player
{
    public Button[] Buttons;
    public TMP_InputField BetAmountInput;

    private int betAmount => System.Convert.ToInt32(BetAmountInput.text);
    public Text Stats;

    private PokerAction option;
    private bool isClicked = false;

    private void SetOptions(Stage gameStage)
    {
        int c = 0;
        foreach(var button in Buttons)
        {
            button.GetComponentInChildren<Text>().text = gameStage.Actions[c].Name;

            int thisC = c;
            button.onClick.AddListener(() =>
            {
                option = gameStage.Actions[thisC];
                isClicked = true;
            });

            c++;
        }
    }

    private bool WaitForChoice()
    {
        if (isClicked)
        {
            isClicked = false;
            return isClicked;
        } else
        {
            return true;
        }

    }

    private ColorBlock defaultButtonColors;
    private ColorBlock alertButtonColors;
    public Color AlertButtonColor;
    public float AlertBlinkInterval;
    public IEnumerator AlertPlayer()
    {
        while (true)
        {
            foreach (var button in Buttons)
            {
                button.colors = alertButtonColors;
            }
            yield return new WaitForSeconds(AlertBlinkInterval);

            foreach (var button in Buttons)
            {
                button.colors = defaultButtonColors;

            }
            yield return new WaitForSeconds(AlertBlinkInterval);
        }
    }

    public override IEnumerator Prompt(Stage gameStage, System.Action<Action> result)
    {

        SetOptions(gameStage);

        var alertPlayer = AlertPlayer();
        StartCoroutine(alertPlayer);

        // Oczekiwanie na użytkownika
        while (WaitForChoice())
        {
            yield return null;
        }

        StopCoroutine(alertPlayer);

        
        result(new Action(option, Mathf.Clamp(betAmount, 2, 15)));
        yield break;

    }

    protected override void Start()
    {
        base.Start();
        Name = "( Ty )";
        defaultButtonColors = Buttons[0].colors;
        alertButtonColors = Buttons[0].colors;
        alertButtonColors.normalColor = AlertButtonColor;
    }

    public GameManager GameManangerComp;
    private void Update()
    {
        Stats.text = $"Żetony: {Coins}\n" +
            $"Pula nagród: {GameManangerComp.CoinPot}";
    }

}
