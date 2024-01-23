using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TradeInAnyCards : PlayerAction
{

    public TradeInAnyCards(Player Caller, IGameMasterPlayerView GameMaster) : base(Caller, GameMaster)
    {
    }
    public override bool execute()
    {    
        if(caller == null) return false;
        if(caller.GetCardHand().Count < 3) return false;

        TerritoryCard[] cardsToTradeIn = new TerritoryCard[3];
        List<TerritoryCard> possibleCards;
        if(caller.GetCardHand().Count > 5)
        {
            possibleCards = caller.GetCardHand().Take(5).ToList();
        }
        else 
        {
            possibleCards = caller.GetCardHand();
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
                    result = TradeInCards(cardsToTradeIn, caller);
                    if (result != 0)
                    {
                        return new TradeInCards(caller, gameMaster, cardsToTradeIn).execute();   
                    }
                }
            }
        }
        return false;
    }
    private int TradeInCards(TerritoryCard[] cards, Player player)
    {
        int result = 0;
        if(cards.Length != 3) return 0;
        if(player == null) return 0;
        if(gameMaster.turnStage() != TurnStage.Deploy) return 0;
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

        result += GameMaster.CardSetRewards[gameMaster.state.cardSetRewardStage];
        for(int i = 0; i < 3; i++)
        {
            if(cards[i].ReferencedTerritory != null)
            {
                if(gameMaster.state.map.GetTerritory(cards[i].ReferencedTerritory).GetOwner().Equals(player.GetData().playerName))
                {
                    result += 2;
                    break;
                }
            }
        }
        new DiscardCards(player, gameMaster, cards).execute();
        gameMaster.state.cardSetRewardStage++;
        return result;
    }
}