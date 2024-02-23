﻿using Newtonsoft.Json;

namespace Engine.Models
{
    public class Location
    {
        public int XCoordinate { get; }
        public int YCoordinate { get; }
        [JsonIgnore]
        public string Name { get; }
        [JsonIgnore]
        public string Description { get; }
        [JsonIgnore]
        public string ImageName { get; }
        [JsonIgnore]
        public List<Quest> QuestsAvailableHere { get; } = new List<Quest>();
        [JsonIgnore]
        public List<MonsterEncounter> MonstersHere { get; } = new List<MonsterEncounter>();
        [JsonIgnore]
        public Trader TraderHere { get; set; }
        public Location(int xCoordinate, int yCoordinate, string name, string description, string imageName)
        {
            XCoordinate = xCoordinate;
            YCoordinate = yCoordinate;
            Name = name;
            Description = description;
            ImageName = imageName;
        }
        public void AddMonster(int monsterID, int chanceOfEncountering)
        {
            if (MonstersHere.Exists(m => m.MonsterID == monsterID))
            {
                MonstersHere.First(m => m.MonsterID == monsterID).ChanceOfEncountering = chanceOfEncountering;
            }
            else
            {
                MonstersHere.Add(new MonsterEncounter(monsterID, chanceOfEncountering));
            }
        }
    }
}
