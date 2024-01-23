using System.Collections.Generic;

public class EndTurn : PlayerAction
{

    public EndTurn(Player Caller, IGameMasterPlayerView GameMaster) : base(Caller, GameMaster)
    {
    }
    public override bool execute()
    {    
        if(gameMaster.state.Players[gameMaster.state.currentPlayerNo] != caller) return false;
        if(caller.IsCardEligible() == true)
        {
            List<TerritoryCard> newCard = new()
            {
                gameMaster.state.cardDeck.DrawCard()
            };
            caller.AddCardsToHand(newCard);
            caller.SetCardEligible(false);
        }
        gameMaster.EndTurn();
        return true;
    }
}