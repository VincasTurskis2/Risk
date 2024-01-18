public class Map : IPlayerMapView
{
    public TerritoryData[] Territories { get; private set;}

    public Map(Territory[] territories)
    {
        Territories = new TerritoryData[territories.Length];
        for (int i = 0 ; i < Territories.Length ; i++)
        {
            territories[i].Setup();
            Territories[i] = territories[i].data; 
        }
    }
}