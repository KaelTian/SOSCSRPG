using Engine.Models;

namespace Engine.Factories
{
    public static class TraderFactory
    {
        private static readonly List<Trader> _traders=new List<Trader>();
        static TraderFactory()
        {
            Trader susan = new Trader("Susan");
            susan.AddItemToInventory(ItemFactory.CreateGameItem(1001));
            Trader farmerTed = new Trader("Farmer Ted");
            farmerTed.AddItemToInventory(ItemFactory.CreateGameItem(1001));
            Trader peteTheHerbalist = new Trader("Pete the Herbalist");
            peteTheHerbalist.AddItemToInventory(ItemFactory.CreateGameItem(1001));
            AddTraderToList(susan);
            AddTraderToList(farmerTed);
            AddTraderToList(peteTheHerbalist);
        }

        private static void AddTraderToList(Trader trader)
        {
            if(_traders.Any(t=> string.Equals(t.Name, trader.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException($"There is already a trader named '{trader.Name}'");
            }
            _traders.Add(trader);   
        }

        public static Trader GetTraderByName(string name)
        {
            return _traders.FirstOrDefault(t => string.Equals(t.Name,name,StringComparison.OrdinalIgnoreCase));
        }


    }
}
