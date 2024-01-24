public class DiscardCards : PlayerAction
{
    public readonly TerritoryCard[] cardsToDiscard;

    public DiscardCards(Player Caller, TerritoryCard[] CardsToDiscard) : base(Caller)
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
                GameMaster.Instance.state.cardDeck.DiscardCard(cardsToDiscard[i]);
                caller.GetCardHand().Remove(cardsToDiscard[i]);
            }
        }
        UIManager.Instance.RedrawCardPanel(caller);
        return true;
    }
}