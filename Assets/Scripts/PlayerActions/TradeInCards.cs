using System.Linq;

public class TradeInCards : PlayerAction
{
    public readonly TerritoryCard[] cards;

    public TradeInCards(Player Caller, TerritoryCard[] Cards) : base(Caller)
    {
        cards = Cards;
    }
    public override bool execute()
    {    
        int result = 0;
        if(cards.Length != 3) return false;
        if(caller == null) return false;
        if(GameMaster.Instance.state.turnStage != TurnStage.Deploy) return false;
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
        if(GameMaster.Instance.state.cardSetRewardStage >= GameMaster.CardSetRewards.Length)
        {
            result += (GameMaster.Instance.state.cardSetRewardStage - 2) * 4;
        }
        else
        {
            result += GameMaster.CardSetRewards[GameMaster.Instance.state.cardSetRewardStage];
        }
        for(int i = 0; i < 3; i++)
        {
            if(cards[i].ReferencedTerritory != null)
            {
                if(GameMaster.Instance.state.map.GetTerritory(cards[i].ReferencedTerritory).GetOwner().Equals(caller.GetData().playerName))
                {
                    result += 2;
                    break;
                }
            }
        }
        new DiscardCards(caller, cards).execute();
        GameMaster.Instance.state.cardSetRewardStage++;
        caller.SetPlaceableTroopNumber(caller.GetPlaceableTroopNumber() + result);
        return true;
    }
}