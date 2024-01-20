public class DiscardCards : PlayerAction
{
    public readonly TerritoryCard[] cardsToDiscard;

    public DiscardCards(Player Caller, GameMaster GameMaster, TerritoryCard[] CardsToDiscard) : base(Caller, GameMaster)
    {
        cardsToDiscard = CardsToDiscard;
    }
    public override bool execute()
    {    
        if(caller == null) return false;
        if(!caller.IsMyTurn()) return false;
        for(int i = 0; i < cardsToDiscard.Length; i++)
        {
            if(caller.GetCardHand().Contains(cardsToDiscard[i]))
            {
                gameMaster.state.cardDeck.DiscardCard(cardsToDiscard[i]);
                caller.GetCardHand().Remove(cardsToDiscard[i]);
            }
        }
        gameMaster.uiManager.RedrawCardPanel(caller);
        return true;
    }
}