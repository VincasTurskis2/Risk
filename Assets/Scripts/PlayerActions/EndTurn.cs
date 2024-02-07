using System.Collections.Generic;

public class EndTurn : PlayerAction
{

    public EndTurn(Player Caller) : base(Caller)
    {
    }
    public override bool execute()
    {    
        if(GameMaster.Instance.state.players[GameMaster.Instance.state.currentPlayerNo] != caller) return false;
        if(caller.IsCardEligible() == true)
        {
            List<TerritoryCard> newCard = new()
            {
                GameMaster.Instance.state.cardDeck.DrawCard()
            };
            caller.AddCardsToHand(newCard);
            caller.SetCardEligible(false);
        }
        GameMaster.Instance.EndTurn();
        return true;
    }
}