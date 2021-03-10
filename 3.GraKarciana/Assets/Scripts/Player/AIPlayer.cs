using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class AIPlayer : Player
{
    public GameManager gameManager;
    public int AIFoldMin = 9;
    public int AIFoldMax = 13;

    public float AIPreflopCheckCallRatio = 0.7f;
    public float AiPostflopCheckCallRation = 0.6f;
    public float StatRich;
    public float StatNoob;
    public float StatSkilled;

    public float SleepTime = 1f;

    public Action Answer;
    public int AnswerValue;

    public TextMeshPro ActionText;
    public void ClearText() => ActionText.text = "";

    public override IEnumerator Prompt(Stage gameStage, System.Action<Action> result)
    {
        float rand = Random.Range(0f, 1f);
        Action newAction = new Action(Game.Actions.Fold);
        int holdRankSum = (int)HoldCards.Get(0).Rank + (int)HoldCards.Get(0).Rank;

        if (gameStage == Game.Stages.Preflop)
        {

            if (holdRankSum < Random.Range(AIFoldMin, AIFoldMax)) // słabe karty
                newAction = new Action(Game.Actions.Fold);
            else
            {
                if (rand < AIPreflopCheckCallRatio)
                    newAction = new Action(Game.Actions.Check);
                else
                    newAction = new Action(Game.Actions.Call);
            }
        }
        else if (gameStage == Game.Stages.Flop || gameStage == Game.Stages.River || gameStage == Game.Stages.Turn)
        {
            if (rand < AiPostflopCheckCallRation)
                newAction = new Action(Game.Actions.Check);
            else if (rand >= AiPostflopCheckCallRation && rand < (gameManager.ActivePlayerCount <= 2 ? 1.0f : 0.9f))
                newAction = new Action(Game.Actions.Raise, Random.Range(2, 15));
            else
                newAction = new Action(Game.Actions.Fold);
        }

        result(newAction);

        ActionText.text = $"[{newAction.PokerAction.Name}]\n";
        yield return new WaitForSeconds(SleepTime);
        yield break;
    }

    private static List<string> names = new List<string>()
        {
            "Patryk Vega",
            "Kaczor Donald",
            "Użytkownik Małpka",
            "Pablo Escobar",
            "Kolo",
            "Elon Musk",
            "Kabel",
            "Bill Gates",
            "Bot Damian",
            "Charlin Chaplin",
            "Gabe Newell",
            "Jimmy Neutron",
            "Karol Strasburger",
            "Pan Mirosław",
            "Prezes Spółki",
            "Hunter1",
            "Amelinium Rider",
            "Beans",
            "Brat Pita",
            "Michael Jackson",
            "Freedie Mercury",
            "Uncle Dane",
            "Shrek",
            "James Bond",
            "Jeff Bezos",
            "James Bond",
            "Pewdiepie",
            "Myszka Miki",
            "Jeff Who",
            "Snajper",
            "Inżynier",
            "Skałt",
            "Żołnierz",
            "Szpieg",
            "Doktor Quin",
            "MacGyver"
        };

    private string GetBotName()
    {
        int randomIndex = Random.Range(0, names.Count);
        string randomName = names[randomIndex];
        names.RemoveAt(randomIndex);
        return randomName;
    }
    protected override void Start()
    {
        base.Start();
        gameManager = FindObjectOfType<GameManager>();
        StatRich = Random.Range(0.5f, 1);
        ActionText.text = "";
        Name = GetBotName();

    }
}
