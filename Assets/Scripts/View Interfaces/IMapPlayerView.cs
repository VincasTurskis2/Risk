public interface IMapPlayerView
{
        public ITerritoryPlayerView[] GetTerritories();
        public ITerritoryPlayerView[] GetTerritories(string[] territories);
        public ITerritoryPlayerView GetTerritory(string territory);
}