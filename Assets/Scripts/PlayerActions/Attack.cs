using UnityEngine;

public class Attack : PlayerAction
{
    public readonly ITerritoryPlayerView IFrom;
    public readonly ITerritoryPlayerView ITo;

    public Attack(Player Caller, IGameStatePlayerView GameMaster, ITerritoryPlayerView from, ITerritoryPlayerView to) : base(Caller, GameMaster)
    {
        IFrom = from;
        ITo = to;
    }
    public override bool execute()
    {    
        if(IFrom == null || ITo == null) return false;
        TerritoryData from = (TerritoryData) IFrom, to = (TerritoryData) ITo;
        // Guards
        if(from == null || to == null || from.TroopCount <= 1) return false;
        if(!from.Owner.IsMyTurn()) return false;
        if(!from.IsANeighbor(to)) return false;
        if(to.Owner == from.Owner) return false;
        if(gameMaster.turnStage() != TurnStage.Attack) return false;

        // Note: technically, a player may choose to roll less dice in an attack; However, in reality, rolling less dice is not a good strategy for either player.
        // This function assumes both players will roll the maximum allowed amount of dice.
        int[] results = RollDice(from, to);
        from.TroopCount -= results[0];
        to.TroopCount -= results[1];
        if(to.TroopCount <= 0)
        {
            Player loser = to.Owner;
            to.SetOwner(from.Owner, true);
            if(loser.GetOwnedTerritories().Count == 0)
            {
                from.Owner.AddCardsToHand(loser.GetCardHand());
                new DiscardCards(loser, gameMaster, loser.GetCardHand().ToArray()).execute();
                gameMaster.OnPlayerLoss(loser);
            }
        }
        gameMaster.uiManager.UpdateAttackPanelResults(from, to);
        return true;
    }
    private int[] RollDice(TerritoryData from, TerritoryData to)
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
        gameMaster.uiManager.UpdateAttackPanelNumbers(diceRolls, result);
        return result;
    }
}