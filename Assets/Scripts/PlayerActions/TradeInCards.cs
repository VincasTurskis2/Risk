public class TradeInCards : PlayerAction
{
    public readonly TerritoryCard[] cards;

    public TradeInCards(Player Caller, GameMaster GameMaster, TerritoryCard[] Cards) : base(Caller, GameMaster)
    {
        cards = Cards;
    }
    public override bool execute()
    {    
        int result = 0;
        if(cards.Length != 3) return false;
        if(caller == null) return false;
        if(gameMaster.turnStage() != TurnStage.Deploy) return false;
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
        if(!matching3 && !different3) return false;

        result += TerritoryCard.CardSetRewards[gameMaster.state.cardSetRewardStage++];
        for(int i = 0; i < 3; i++)
        {
            if(cards[i].ReferencedTerritory != null)
            {
                if(cards[i].ReferencedTerritory.Owner == caller)
                {
                    result += 2;
                    break;
                }
            }
        }
        new DiscardCards(caller, gameMaster, cards).execute();
        gameMaster.state.cardSetRewardStage++;
        caller.SetPlaceableTroopNumber(caller.GetPlaceableTroopNumber() + result);
        return true;
    }
}