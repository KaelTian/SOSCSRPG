using SOSCSRPG.Services.Factories;
using SOSCSRPG.Models;
using SOSCSRPG.Services;
using Newtonsoft.Json;
using SOSCSRPG.Core;
using System.ComponentModel;

namespace SOSCSRPG.ViewModels
{
    public class GameSession : INotifyPropertyChanged
    {
        private readonly MessageBroker _messageBroker = MessageBroker.GetInstance();
        private Battle _currentBattle;
        #region Properties
        private Player _currentPlayer;
        private Location _currentLocation;
        private Monster _currentMonster;

        public event PropertyChangedEventHandler? PropertyChanged;

        [JsonIgnore]
        public GameDetails GameDetails
        {
            get; private set;
        }
        public string Version { get; } = "0.1.000";
        public Player CurrentPlayer
        {
            get { return _currentPlayer; }
            set
            {
                if (_currentPlayer != null)
                {
                    _currentPlayer.OnLevelUp -= OnCurrentPlayerLevelUp;
                    _currentPlayer.OnKilled -= OnPlayerKilled;
                }
                _currentPlayer = value;
                if (_currentPlayer != null)
                {
                    _currentPlayer.OnLevelUp += OnCurrentPlayerLevelUp;
                    _currentPlayer.OnKilled += OnPlayerKilled;
                }
            }
        }

        public Location CurrentLocation
        {
            get { return _currentLocation; }
            set
            {
                _currentLocation = value;
                CompleteQuestsAtLocation();
                GivePlayerQuestsAtLocation();
                CurrentMonster = MonsterFactory.GetMonsterFromLocation(CurrentLocation);
                CurrentTrader = CurrentLocation.TraderHere;
            }
        }
        [JsonIgnore]
        public Trader CurrentTrader
        {
            get; private set;
        }
        public PopupDetails InventoryDetails { get; set; }
        [JsonIgnore]
        public bool HasLocationToNorth => MoveNorthOnStep() != null;
        [JsonIgnore]
        public bool HasLocationToEast => MoveEastOnStep() != null;
        [JsonIgnore]
        public bool HasLocationToSouth => MoveSouthOnStep() != null;
        [JsonIgnore]
        public bool HasLocationToWest => MoveWestOnStep() != null;
        [JsonIgnore]
        public World CurrentWorld { get; }
        [JsonIgnore]
        public Monster CurrentMonster
        {
            get { return _currentMonster; }
            set
            {
                if (_currentBattle != null)
                {
                    _currentBattle.OnCombatVictory -= OnCurrentMonsterKilled;
                    _currentBattle.Dispose();
                    _currentBattle = null;
                }
                _currentMonster = value;
                if (_currentMonster != null)
                {
                    _currentBattle = new Battle(CurrentPlayer, CurrentMonster);
                    _currentBattle.OnCombatVictory += OnCurrentMonsterKilled;
                }
            }
        }
        [JsonIgnore]
        public bool HasMonster => CurrentMonster != null;
        [JsonIgnore]
        public bool HasTrader => CurrentTrader != null;
        #endregion
        public GameSession(Player player, int xCoordinate, int yCoordinate)
        {
            PopulateGameDetails();
            CurrentWorld = WorldFactory.CreateWorld();
            CurrentPlayer = player;
            CurrentLocation = CurrentWorld.LocationAt(xCoordinate, yCoordinate);
        }

        public void MoveNorth()
        {
            if (HasLocationToNorth)
            {
                CurrentLocation = MoveNorthOnStep();
            }
        }

        private Location MoveNorthOnStep() => CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate + 1);

        public void MoveEast()
        {
            if (HasLocationToEast)
            {
                CurrentLocation = MoveEastOnStep();
            }
        }

        private Location MoveEastOnStep() => CurrentWorld.LocationAt(CurrentLocation.XCoordinate + 1, CurrentLocation.YCoordinate);

        public void MoveSouth()
        {
            if (HasLocationToSouth)
            {
                CurrentLocation = MoveSouthOnStep();
            }
        }

        private Location MoveSouthOnStep() => CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate - 1);
        public void MoveWest()
        {
            if (HasLocationToWest)
            {
                CurrentLocation = MoveWestOnStep();
            }
        }

        private Location MoveWestOnStep() => CurrentWorld.LocationAt(CurrentLocation.XCoordinate - 1, CurrentLocation.YCoordinate);

        private void PopulateGameDetails()
        {
            GameDetails = GameDetailsService.ReadGameDetails();
        }

        private void CompleteQuestsAtLocation()
        {
            foreach (var quest in CurrentLocation.QuestsAvailableHere)
            {
                QuestStatus questToComplete =
                    CurrentPlayer.Quests.FirstOrDefault(q => q.PlayerQuest.ID == quest.ID &&
                            !q.IsCompleted);
                if (questToComplete != null)
                {
                    if (CurrentPlayer.Inventory.HasAllTheseItems(quest.ItemsToComplete))
                    {
                        // Remove the quest completion items from the player's inventory
                        CurrentPlayer.RemoveItemsFromInventory(quest.ItemsToComplete);
                        _messageBroker.RaiseMessage("");
                        _messageBroker.RaiseMessage($"You completed the '{quest.Name}' quest");
                        // Give the player the quest rewards
                        _messageBroker.RaiseMessage($"You receive {quest.RewardExperiencePoints} experience points");
                        CurrentPlayer.AddExperience(quest.RewardExperiencePoints);
                        _messageBroker.RaiseMessage($"You receive {quest.RewardGold} gold");
                        CurrentPlayer.ReceiveGold(quest.RewardGold);
                        foreach (ItemQuantity itemQuantity in quest.RewardItems)
                        {
                            GameItem rewardItem = ItemFactory.CreateGameItem(itemQuantity.ItemID);
                            CurrentPlayer.AddItemToInventory(rewardItem);
                            _messageBroker.RaiseMessage($"You receive a {rewardItem.Name}");
                        }
                        // Mark the Quest as completed
                        questToComplete.IsCompleted = true;
                    }
                }
            }
        }
        public void CraftItemUsing(Recipe recipe)
        {
            if (CurrentPlayer.Inventory.HasAllTheseItems(recipe.Ingredients))
            {
                CurrentPlayer.RemoveItemsFromInventory(recipe.Ingredients);
                foreach (ItemQuantity itemQuantity in recipe.OutputItems)
                {
                    for (int i = 0; i < itemQuantity.Quantity; i++)
                    {
                        GameItem outputItem = ItemFactory.CreateGameItem(itemQuantity.ItemID);
                        CurrentPlayer.AddItemToInventory(outputItem);
                        _messageBroker.RaiseMessage($"You craft 1 {outputItem.Name}");
                    }
                }
            }
            else
            {
                _messageBroker.RaiseMessage("You do not have the required ingredients:");
                foreach (ItemQuantity itemQuantity in recipe.Ingredients)
                {
                    _messageBroker.RaiseMessage(itemQuantity.QuantityItemDescription);
                }
            }
        }
        private void GivePlayerQuestsAtLocation()
        {
            foreach (var quest in CurrentLocation.QuestsAvailableHere)
            {
                if (!CurrentPlayer.Quests.Any(q => q.PlayerQuest.ID == quest.ID))
                {
                    CurrentPlayer.Quests.Add(new QuestStatus(quest));
                    _messageBroker.RaiseMessage("");
                    _messageBroker.RaiseMessage($"You receive the '{quest.Name}' quest");
                    _messageBroker.RaiseMessage(quest.Description);
                    _messageBroker.RaiseMessage("Return with:");
                    foreach (ItemQuantity itemQuantity in quest.ItemsToComplete)
                    {
                        _messageBroker.RaiseMessage($"   {itemQuantity.Quantity} {ItemFactory.CreateGameItem(itemQuantity.ItemID).Name}");
                    }
                    _messageBroker.RaiseMessage("And you will receive:");
                    _messageBroker.RaiseMessage($"   {quest.RewardExperiencePoints} experience points");
                    _messageBroker.RaiseMessage($"   {quest.RewardGold} gold");
                    foreach (ItemQuantity itemQuantity in quest.RewardItems)
                    {
                        _messageBroker.RaiseMessage($"   {itemQuantity.Quantity} {ItemFactory.CreateGameItem(itemQuantity.ItemID).Name}");
                    }
                }
            }
        }

        public void AttackCurrentMonster()
        {
            _currentBattle?.AttackOpponent();
        }

        private void OnPlayerKilled(Object sender, System.EventArgs eventArgs)
        {
            _messageBroker.RaiseMessage("");
            var killedMessage = CurrentMonster != null ?
                $"The {CurrentMonster.Name} killed you." :
                "You have been killed.";
            _messageBroker.RaiseMessage(killedMessage);
            CurrentLocation = CurrentWorld.LocationAt(0, -1);
            CurrentPlayer.CompletelyHeal();
        }

        private void OnCurrentMonsterKilled(object sender, System.EventArgs eventArgs)
        {
            // Get another monster to fight
            CurrentMonster = MonsterFactory.GetMonsterFromLocation(CurrentLocation);
        }

        private void OnCurrentPlayerLevelUp(object? sender, System.EventArgs e)
        {
            _messageBroker.RaiseMessage($"You are now level {CurrentPlayer.Level}!");
        }


        public void UseCurrentConsumable()
        {
            if (CurrentPlayer.CurrentConsumable != null)
            {
                if (_currentBattle == null)
                {
                    CurrentPlayer.OnActionPerformed += OnConsumableActionPerformed;
                }
                CurrentPlayer.UseCurrentConsumable();
                if (_currentBattle == null)
                {
                    CurrentPlayer.OnActionPerformed -= OnConsumableActionPerformed;
                }
            }
        }
        private void OnConsumableActionPerformed(object sender, string result)
        {
            _messageBroker.RaiseMessage(result);
        }
    }
}
