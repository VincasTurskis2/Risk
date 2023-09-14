using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


// A class that aggregates all the actions a player can take during their turn
public class PlayerActions : MonoBehaviour
{
    private GameState _gameState;
    public void Setup()
    {
        _gameState = gameObject.GetComponent<GameState>();
    }
    public bool Deploy(Territory territory)
    {
        // Guards
        if(territory == null) return false;
        if(!territory.Owner.IsMyTurn()) return false;
        if(_gameState.turnStage != TurnStage.Deploy) return false;
        if(territory.Owner.GetPlaceableTroopNumber() <= 0) return false;

        territory.Owner.DecrementPlaceableTroops();
        territory.TroopCount++;
        if(territory.Owner.GetPlaceableTroopNumber() <= 0)
        {
            territory.Owner.EndTurnStage();
        }
        return true;
    }

    public bool SetupDeploy(Territory territory, Player player)
    {
        //Guards
        if(territory == null) return false;
        if(_gameState.turnStage != TurnStage.Setup) return false;
        if(!player.IsMyTurn()) return false;
        if(player.GetPlaceableTroopNumber() < 1) return false;

        if((Object)territory.Owner == null)
        {
            territory.SetOwner(player);
            territory.TroopCount++;
            player.DecrementPlaceableTroops();
            player.EndTurn();
            return true;
        }
        else if((Object)territory.Owner == (Object) player && _gameState.allTerritoriesClaimed)
        {
            territory.TroopCount++;
            player.DecrementPlaceableTroops();
            player.EndTurn();
            return true;
        }
        else return false;
    }

    public bool Attack(Territory from, Territory to)
    {
        // Guards
        if(from == null || to == null || from.TroopCount <= 1) return false;
        if(!from.Owner.IsMyTurn()) return false;
        if(!from.IsANeighbor(to)) return false;
        if(to.Owner == from.Owner) return false;
        if(_gameState.turnStage != TurnStage.Attack) return false;

        // Note: technically, a player may choose to roll less dice in an attack; However, in reality, rolling less dice is not a good strategy for either player.
        // This function assumes both players will roll the maximum allowed amount of dice.
        int[] results = RollDice(from, to);
        from.TroopCount -= results[0];
        to.TroopCount -= results[1];
        if(to.TroopCount <= 0)
        {
            to.SetOwner(from.Owner);
        }
        _gameState.uiManager.UpdateAttackPanelResults(from, to);
        return true;
    }
    public bool Fortify(Territory from, Territory to, int numberOfTroops)
    {
        // Guards
        if(from == null || to == null || numberOfTroops < 1) return false;
        if(!from.Owner.IsMyTurn()) return false;
        if(numberOfTroops >= from.TroopCount) return false;
        if(!from.IsANeighbor(to)) return false;
        if(to.Owner != from.Owner) return false;
        if(_gameState.turnStage != TurnStage.Reinforce) return false;

        // If all the guards are passed, move the troops
        from.TroopCount -= numberOfTroops;
        to.TroopCount += numberOfTroops;
        from.Owner.EndTurnStage(); // Only 1 reinforcement can be done per turn
        return true;
    }
    public bool Occupy(Territory from, Territory to, int numberOfTroops)
    {
        // Guards
        if(from == null || to == null || numberOfTroops < 1) return false;
        if(!from.Owner.IsMyTurn()) return false;
        if(numberOfTroops >= from.TroopCount) return false;
        if(!from.IsANeighbor(to)) return false;
        if(to.Owner != from.Owner) return false;
        if(to.TroopCount != 0) return false;
        if(_gameState.turnStage != TurnStage.Attack) return false;

        from.TroopCount -= numberOfTroops;
        to.TroopCount += numberOfTroops;
        return true;
    }
    public int CalculatePlaceableTroops(Player player)
    {
        if(_gameState.turnStage == TurnStage.Setup)
        {
            if(player.GetPlaceableTroopNumber() == 0)
            {
                _gameState.turnStage = TurnStage.Deploy;
            }
            else return player.GetPlaceableTroopNumber();
        }
        int result = 0;
        result = player.GetOwnedTerritories().Count / 3;
        List<Continent> ownedContinents = GetOwnedContinents(player);
        for(int i = 0; i < ownedContinents.Count; i++)
        {
            result += GameState.ContinentValues[(int) ownedContinents[i]];
        }
        return result;
    }

    private List<Continent> GetOwnedContinents(Player player)
    {
        List<Continent> result = new List<Continent>();
        int[] continentCountLocal = new int[GameState.ContinentCount.Length];
        GameState.ContinentCount.CopyTo(continentCountLocal, 0);
        foreach(Territory t in player.GetOwnedTerritories())
        {
            continentCountLocal[(int) t.Continent]--;
        }
        for(int i = 0; i < continentCountLocal.Length; i++)
        {
            if(continentCountLocal[i] <= 0)
            {
                result.Add((Continent) i);
            }
        }
        return result;
    }

    // A helper function to calculate dice results
    // Returns an int array of {Attacker losses, Defender losses, Attacker no. of dice rolled, Defender no. of dice rolled} in this combat
    private int[] RollDice(Territory from, Territory to)
    {
        int attackerDiceNo = Mathf.Min(from.TroopCount - 1, 3), defenderDiceNo = Mathf.Min(to.TroopCount, 2);
        int[] attackerDice = new int[attackerDiceNo], defenderDice = new int[defenderDiceNo];
        int[] result = {0, 0, attackerDiceNo, defenderDiceNo};
        int[][] diceRolls = new int[2][];
        diceRolls[0] = new int[attackerDiceNo];
        diceRolls[1] = new int[defenderDiceNo];
        for(int i = 0; i < attackerDiceNo; i++)
        {
            attackerDice[i] = Random.Range(1, 7);
            diceRolls[0][i] = attackerDice[i];
        }
        for(int i = 0; i < defenderDiceNo; i++)
        {
            defenderDice[i] = Random.Range(1, 7);
            diceRolls[1][i] = defenderDice[i];
        }
        while(attackerDice[Helpers.ArrayMaxElementIndex(attackerDice)] != 0 && defenderDice[Helpers.ArrayMaxElementIndex(defenderDice)] != 0)
        {
            if(attackerDice[Helpers.ArrayMaxElementIndex(attackerDice)] > defenderDice[Helpers.ArrayMaxElementIndex(defenderDice)])
            {
                result[1]++;
            }
            else 
            {
                result[0]++;
            }
            attackerDice[Helpers.ArrayMaxElementIndex(attackerDice)] = 0;
            defenderDice[Helpers.ArrayMaxElementIndex(defenderDice)] = 0;
        }
        _gameState.uiManager.UpdateAttackPanelNumbers(diceRolls, result);
        return result;
    }

    public int TradeInCards(TerritoryCard[] cards, Player player)
    {
        int result = 0;
        if(cards.Length != 3) return 0;
        if(player == null) return 0;
        if(_gameState.turnStage != TurnStage.Deploy) return 0;
        bool matching3 = false, different3 = false;
        TroopType[] typesFound = new TroopType[3];
        for(int i = 0; i < 3; i++)
        {
            typesFound[i] = cards[i].Type;
        }
        // Check if the cards are the same
        if((typesFound[0] & typesFound[1] & typesFound[2]) != 0)
        {
            matching3 = true;
        }
        if((typesFound[0] != typesFound[1] || typesFound[0] == TroopType.WildCard)&& 
           (typesFound[1] != typesFound[2] || typesFound[1] == TroopType.WildCard)&& 
           (typesFound[0] != typesFound[2] || typesFound[2] == TroopType.WildCard)){

            different3 = true;
        }
        if(!matching3 && !different3) return 0;

        result += TerritoryCard.CardSetRewards[player.GetCardSetRewardStage()];
        for(int i = 0; i < 3; i++)
        {
            if(cards[i].ReferencedTerritory != null)
            {
                if(cards[i].ReferencedTerritory.Owner == player)
                {
                    result += 2;
                    break;
                }
            }
        }
        player.DiscardCards(cards);
        player.IncrementCardSetReward();
        return result;
    }

    public int TradeInAnyCards(Player player)
    {
        if(player == null) return 0;
        if(player.GetCardHand().Count < 3) return 0;

        TerritoryCard[] cardsToTradeIn = new TerritoryCard[3];
        List<TerritoryCard> possibleCards = new List<TerritoryCard>();
        if(player.GetCardHand().Count > 5)
        {
            possibleCards = player.GetCardHand().Take(5).ToList();
        }
        else 
        {
            possibleCards = player.GetCardHand();
        }
        for(int i = 0; i < possibleCards.Count - 2; i++)
        {
            for(int j = i + 1; j < possibleCards.Count - 1; j++)
            {
                for(int k = j + 1; k < possibleCards.Count; k++)
                {
                    cardsToTradeIn[0] = possibleCards[i];
                    cardsToTradeIn[1] = possibleCards[j];
                    cardsToTradeIn[2] = possibleCards[k];

                    int result = 0;
                    result = TradeInCards(cardsToTradeIn, player);
                    if (result != 0)
                    {
                        return result;
                    }
                }
            }
        }
        return 0;
    }

    public void EndTurn(Player player)
    {
        if(_gameState.Players[_gameState.currentPlayerNo] != player) return;
        if(player.IsCardEligible() == true)
        {
            List<TerritoryCard> newCard = new()
            {
                _gameState.cardDeck.DrawCard()
            };
            player.AddCardsToHand(newCard);
            player.SetCardEligible(false);
        }
        _gameState.EndTurn();
    }
    /*public int TradeInAnyCards(Player player)
    {
        if(player == null) return 0;
        if(player.GetCardHand().Count < 3) return 0;
        
        List<TerritoryCard> cardsToTradeIn = new List<TerritoryCard>();
        
        int InfantryCount = 0, CavalryCount = 0, ArtilleryCount = 0, WildcardCount = 0;
        for(int i = 0; i < player.GetCardHand().Count; i++)
        {
            switch(player.GetCardHand()[i].Type)
            {
                case TroopType.Infantry:
                    InfantryCount++;
                    break;
                case TroopType.Cavalry:
                    CavalryCount++;
                    break;
                case TroopType.Artillery:
                    ArtilleryCount++;
                    break;
                case TroopType.WildCard:
                    cardsToTradeIn.Add(player.GetCardHand()[i]);
                    WildcardCount++;
                    break;
            }
        }

        if(WildcardCount >= 2)
        {
            for(int i = 0; i < player.GetCardHand().Count; i++)
            {
                if(player.GetCardHand()[i].Type != TroopType.WildCard)
                {
                    cardsToTradeIn.Add(player.GetCardHand()[i]);
                    break;
                }
            }
        }
        else if(InfantryCount + WildcardCount >= 3)
        {
            for(int i = 0; i < player.GetCardHand().Count; i++)
            {
                if(player.GetCardHand()[i].Type != TroopType.Infantry)
                {
                    cardsToTradeIn.Add(player.GetCardHand()[i]);
                    if(cardsToTradeIn.Count >= 3)
                    {
                        break;
                    }
                }
            }
        }
        else if(CavalryCount + WildcardCount >= 3)
        {
            for(int i = 0; i < player.GetCardHand().Count; i++)
            {
                if(player.GetCardHand()[i].Type != TroopType.Cavalry)
                {
                    cardsToTradeIn.Add(player.GetCardHand()[i]);
                    if(cardsToTradeIn.Count >= 3)
                    {
                        break;
                    }
                }
            }
        }
        else if(ArtilleryCount + WildcardCount >= 3)
        {
            for(int i = 0; i < player.GetCardHand().Count; i++)
            {
                if(player.GetCardHand()[i].Type != TroopType.Artillery)
                {
                    cardsToTradeIn.Add(player.GetCardHand()[i]);
                    if(cardsToTradeIn.Count >= 3)
                    {
                        break;
                    }
                }
            }
        }
        else if(InfantryCount > 0 && CavalryCount > 0 && ArtilleryCount > 0)
        {
            for(int i = 0; i < player.GetCardHand().Count; i++)
            {
                if(player.GetCardHand()[i].Type != TroopType.Infantry && InfantryCount != 0)
                {
                    cardsToTradeIn.Add(player.GetCardHand()[i]);
                    InfantryCount = 0;
                    if(cardsToTradeIn.Count >= 3)
                    {
                        break;
                    }
                }
                else if(player.GetCardHand()[i].Type != TroopType.Cavalry && CavalryCount != 0)
                {
                    cardsToTradeIn.Add(player.GetCardHand()[i]);
                    CavalryCount = 0;
                    if(cardsToTradeIn.Count >= 3)
                    {
                        break;
                    }
                }
                else if(player.GetCardHand()[i].Type != TroopType.Artillery && ArtilleryCount != 0)
                {
                    cardsToTradeIn.Add(player.GetCardHand()[i]);
                    ArtilleryCount = 0;
                    if(cardsToTradeIn.Count >= 3)
                    {
                        break;
                    }
                }
            }
        }
        else if(WildcardCount > 0 && InfantryCount + CavalryCount + ArtilleryCount >= 2)
        {
            for(int i = 0; i < player.GetCardHand().Count; i++)
            {
                if(player.GetCardHand()[i].Type != TroopType.Infantry && InfantryCount != 0)
                {
                    cardsToTradeIn.Add(player.GetCardHand()[i]);
                    InfantryCount = 0;
                    if(cardsToTradeIn.Count >= 3)
                    {
                        break;
                    }
                }
                else if(player.GetCardHand()[i].Type != TroopType.Cavalry && CavalryCount != 0)
                {
                    cardsToTradeIn.Add(player.GetCardHand()[i]);
                    CavalryCount = 0;
                    if(cardsToTradeIn.Count >= 3)
                    {
                        break;
                    }
                }
                else if(player.GetCardHand()[i].Type != TroopType.Artillery && ArtilleryCount != 0)
                {
                    cardsToTradeIn.Add(player.GetCardHand()[i]);
                    ArtilleryCount = 0;
                    if(cardsToTradeIn.Count >= 3)
                    {
                        break;
                    }
                }
            }
        }
        else return 0;

        return TradeInCards(cardsToTradeIn.ToArray(), player);
    }*/
}
