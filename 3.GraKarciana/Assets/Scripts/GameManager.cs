using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public enum GameStage
{
    PreFlop,
    Flop,
    Turn,
    River,
    Showdown
}

public class GameManager : MonoBehaviour
{
    public int ConfigMinimumAmount = 4;
    public int ConfigSmallBlind = 4;
    public int ConfigBigBling => ConfigSmallBlind * 2;

    public static List<Player> Players = new List<Player>();
    public int ActivePlayerCount => Players.Where(x => x.IsFold == false).Count();
    public Transform PlayerTransf;

    public Player PlayerDealer;
    public Player PlayerSmallBlind => PlayerDealer.next;
    public Player PlayerBigBlind => PlayerDealer.next.next;

    [SerializeField] private Card CardTemplate;

    public ChipStack ChipStackComp;
    private int _coinPot;
    public int CoinPot {
        set {
            _coinPot = value;
            ChipStackComp.StackCount = value;
        }
        get => _coinPot;
    }

    public Transform CardPotContainer;
    public Container<Card> CardPot = new Container<Card>();

    public Transform CardDeckContainer;
    public Container<Card> Deck = new Container<Card>();

    public CommunityCards CommunityCardsComp;
    public Transform CommunityCardsContainer => CommunityCardsComp.CommunityCardsTransf;
    public Container<Card> CommunityCards => CommunityCardsComp.CommunityCardContainer;

    public Indicator IndicatorComp;
    public Transform DealerButton;
    public Text CurrentStageText;

    public GameEndWnd GameEndWndComp;
    public Player HumanPlayerComp;


    // Start is called before the first frame update
    void Start()
    {
        Deck.storage = CardDeckContainer;
        //FlopDeck.storage = FlopDeckContainer;
        CommunityCards.storage = CommunityCardsContainer;
        CardPot.storage = CardPotContainer;
        
    }

    private void OnValidate()
    {
        Players.Clear();
        foreach (var i in PlayerTransf.GetComponentsInChildren<Player>())
        {
            Players.Add(i);
        }
    }


    private void Fold(Player player, int minAmount)
    {
        player.IsFold = true;
        CardPot.TakeFrom(player.HoldCards, 0);
        CardPot.TakeFrom(player.HoldCards, 0);

        var pc = player.Coins;
        player.Coins -= minAmount;
        if (player.Coins < 0)
        {
            CoinPot += pc;
            player.Coins = 0;
        }
        else
        {
            CoinPot += minAmount;
        }
    }

    private void Raise(Player player, ref int minAmount, ref int argCoins)
    {
        minAmount += argCoins;
        CoinPot += argCoins;
        

        player.Coins -= argCoins;
    }

    private void Check(Player player, int minAmount)
    {
        player.Coins -= minAmount;
    }



    private int dealerIndex = 0;
    public void GameStart(bool isNextGame = false)
    {
        print("** Game started");

        // czyszczenie tekstu botów
        foreach (var player in Players)
        {
            var ai = player as AIPlayer;
            if (ai != null)
                ai.ClearText();

            if (isNextGame) // reaktywowanie graczy
            {
                player.IsFold = false;

                player.HoldCards.TryRemove(0);
                player.HoldCards.TryRemove(0);
            }
        }

        // kolejka
        int ctr = 0;
        foreach (var player in Players)
        {
            if (ctr == Players.Count - 1) // gracz ostatni
                player.next = Players[0];
            else
                player.next = Players[ctr + 1];
            player.index = ctr;
            ctr++;
        }

        // wybieranie dilera
        dealerIndex = Random.Range(0, Players.Count - 1);
        PlayerDealer = Players[dealerIndex];

        print($"Dealer selected: ${dealerIndex}");
        DealerButton.position = Players[dealerIndex].DealerPlacement.position;

        if (isNextGame) // usuwanie starych kart
        {
            Deck.Clear();
        }

        // tworzenie talii kart
        for (int suit_i = 0; suit_i < 4; suit_i++)
        {
            for (int rank_i = 0; rank_i < 13; rank_i++)
            {
                Card newCard = Instantiate(CardTemplate.transform.GetComponent<Card>());
                newCard.Rank = (Card.CardRank)rank_i;
                newCard.Suit = (Card.CardSuit)suit_i;
                Deck.Add(newCard);
            }
        }

        print($"Tail created with {Deck.Count} cards");

        // Tasowanie
        Deck.Shuffle();
        print($"Shuffled");

        // rozdanie kart
        foreach (var player in Players)
        {
            player.HoldCards.TakeFrom(Deck, 0);
            player.HoldCards.TakeFrom(Deck, 0);
        }

        print($"Cards given away");

        print("Setup done");

        var preFlop = Game.Stages.Preflop;
        var roundPreFlop = Round(preFlop);
        StartCoroutine(roundPreFlop);

        
    }

    public void GameContinue()
    {
        GameStart(isNextGame: true);
    }

    private void StartNextRound(Stage previousStage)
    {
        Stage nextStage;
        Game.Stages.NextStage.TryGetValue(previousStage, out nextStage);
        
        if (nextStage != null)
        {
            var nextRound = Round(nextStage);
            StartCoroutine(nextRound);
        } else
        {
            print("Null");
        }
    }

    IEnumerator Round(Stage gameStage)
    {
        print($"** Start stage {gameStage.Name}");
        CurrentStageText.text = $"Runda: {gameStage.Name}";

        foreach (var player in Players)
        {
            var ai = player as AIPlayer;
            if (ai != null)
                ai.ClearText();
            }

            Player curPlayer = PlayerBigBlind;
        int minimumAmount = 0;

        if (gameStage == Game.Stages.Flop)
        {
            print("branie kart");
            CommunityCards.TakeFrom(Deck, 0);
            CommunityCards.TakeFrom(Deck, 0);
            CommunityCards.TakeFrom(Deck, 0);
        }
        if (gameStage == Game.Stages.Turn || gameStage == Game.Stages.River)
        {
            CommunityCards.TakeFrom(Deck, 0);
        }

        #region koniec
        // Koniec gry =======================
        if (gameStage == Game.Stages.Showdown)
        {

            foreach (var player in Players)
                player.FlipCards();

            var cc = CommunityCards.GetArray();
            int lowestHandLvl = 0;

            var activePlayers = Players.Where(x => !x.IsFold);

            Dictionary<Player, (int, int[])> results = new Dictionary<Player, (int, int[])>();
            foreach (var player in activePlayers)
            {
                var hc1 = player.HoldCards.Get(0);
                var hc2 = player.HoldCards.Get(1);

                Card[] hand = new Card[] { cc[0], cc[1], cc[2], hc1, hc2 };
                var handLevel = Card.GetLevel(hand);

                int[] unsortedRanks = new int[] { (int)cc[0].Rank, (int)cc[1].Rank, (int)cc[2].Rank, (int)hc1.Rank, (int)hc2.Rank };
                int[] sortedRanks = unsortedRanks.OrderBy(x => x).ToArray();

                results.Add(player, (handLevel.Item1, sortedRanks));

                if (handLevel.Item1 > lowestHandLvl)
                    lowestHandLvl = handLevel.Item1;
            }

            List<Player> lowestLvl = new List<Player>();
            foreach (var player in activePlayers)
            {
                if (results[player].Item1 == lowestHandLvl)
                {
                    lowestLvl.Add(player);
                }
            }

            Player winner;
            if (lowestLvl.Count == 1)
            {
                print($"WINNER: {lowestLvl[0].name}");
                winner = lowestLvl[0];
            }
            else
            {
                var topLevel = lowestLvl
                    .OrderBy(x => results[x].Item2[0])
                    .OrderBy(x => results[x].Item2[1])
                    .OrderBy(x => results[x].Item2[2])
                    .OrderBy(x => results[x].Item2[3])
                    .OrderBy(x => results[x].Item2[4])
                    .ToArray();

                winner = topLevel[0];
                print($"WINNER: {winner.name}");
            }
            
            if (HumanPlayerComp == winner)
            {
                GameEndWndComp.ShowWin();
            } else
            {
                GameEndWndComp.ShowLoss(winner);
            }
        }
        #endregion koniec

        for (int i = 0; i < Players.Count; i++)
        {
            curPlayer = curPlayer.next;
            
            IndicatorComp.Highlight(curPlayer);

            if (curPlayer.IsFold)
            {
                print("Skip fold");
                continue;
            }

            // Zapytanie graczy =================

            print($"Prompting : {i} | {Players[i].gameObject.name}");

            Action action = new Action(Game.Actions.Call, 0);
            yield return StartCoroutine(curPlayer.Prompt(gameStage, result => action = result));

            print($"Result: {action.PokerAction.Name}, value: {action.Value}");
            int argCoins = action.Value;


            // Small blind / Big blind - Mala/duza slepa
            if (gameStage == Game.Stages.Preflop)
            {
                if (curPlayer == PlayerSmallBlind)
                {
                    print("big blind");
                    curPlayer.Coins -= ConfigSmallBlind;
                    CoinPot += ConfigSmallBlind;
                    continue;
                } else if (curPlayer == PlayerBigBlind)
                {
                    print("small blind");
                    curPlayer.Coins -= ConfigBigBling;
                    CoinPot += ConfigBigBling;
                    continue;
                }
            }


            // Akcje ============================

            if (action.PokerAction == Game.Actions.Raise)
            {
                Raise(curPlayer, ref minimumAmount, ref argCoins);
            } else if (action.PokerAction == Game.Actions.Fold)
            {
                Fold(curPlayer, minimumAmount);
            } else if (action.PokerAction == Game.Actions.Call)
            {
                curPlayer.Coins -= ConfigBigBling;
                CoinPot += ConfigBigBling;
            } else if (action.PokerAction == Game.Actions.Check)
            {
                Check(curPlayer, minimumAmount);
            }
        }
        print("** END");
        StartNextRound(gameStage);
    }
}
