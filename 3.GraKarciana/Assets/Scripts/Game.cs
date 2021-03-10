using UnityEngine;
using System.Collections.Generic;

public class Action
{
    public PokerAction PokerAction;
    public int Value;

    public Action(PokerAction action, int value)
    {
        PokerAction = action;
        Value = value;
    }

    public Action(PokerAction action) : this(action, 0) { }
}


public class Stage
{
    public string Name;
    public PokerAction[] Actions;
    public Stage NextStage;
}

public class PokerAction
{
    public string Name;
}

public class Game : MonoBehaviour
{
    public static class Actions
    {
        public static PokerAction Fold = new PokerAction { Name = "Pasuj (Fold)" };
        public static PokerAction Call = new PokerAction { Name = "Sprawdz (Call)" };
        public static PokerAction Raise = new PokerAction { Name = "Podbij (Raise)" };
        public static PokerAction Check = new PokerAction { Name = "Czekanie (Check)" };
        public static PokerAction NextRound = new PokerAction { Name = "Następna runda" };
    }

    public static class Stages
    {
        public static Stage Preflop = new Stage
        {
            Name = "Preflop",
            Actions = new PokerAction[] {
                Actions.Fold,
                Actions.Call,
                Actions.Raise
            },
        };

        public static Stage Flop = new Stage
        {
            Name = "Flop",
            Actions = new PokerAction[] {
                Actions.Fold,
                Actions.Check,
                Actions.Raise
            },
        };

        public static Stage Turn = new Stage
        {
            Name = "Turn",
            Actions = Flop.Actions,
        };

        public static Stage River = new Stage
        {
            Name = "River",
            Actions = Flop.Actions,
 
        };

        public static Stage Showdown = new Stage
        {
            Name = "Showdown",
            Actions = new PokerAction[]
            {
                Actions.NextRound
            },

        };

        public static Dictionary<Stage, Stage> NextStage = new Dictionary<Stage, Stage>
        {
            { Preflop, Flop },
            { Flop, Turn },
            { Turn, River },
            { River, Showdown }
        };
    }



}
