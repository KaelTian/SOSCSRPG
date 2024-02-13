﻿using Engine.Models;

namespace Engine.Actions
{
    public class AttackWithWeapon :BaseAction, IAction
    {
        private readonly int _maximumDamage;
        private readonly int _minimumDamage;
        public AttackWithWeapon(GameItem itemInUse, int maximumDamage, int minimumDamage)
            :base(itemInUse)
        {
            if (itemInUse.Category != GameItem.ItemCategory.Weapon)
            {
                throw new ArgumentException($"{itemInUse.Name} is not a weapon");
            }
            if (minimumDamage < 0)
            {
                throw new ArgumentException("minimumDamage must be 0 or larger");
            }
            if (maximumDamage < minimumDamage)
            {
                throw new ArgumentException("maximumDamage must be >= minimumDamage");
            }
            _maximumDamage = maximumDamage;
            _minimumDamage = minimumDamage;
        }
        public void Execute(LivingEntity actor, LivingEntity target)
        {
            int damage = RandomNumberGenerator.NumberBetween(_minimumDamage, _maximumDamage);
            string actorName = (actor is Player) ? "You" : $"The {actor.Name.ToLower()}";
            string targetName = (target is Player) ? "You" : $"The {target.Name.ToLower()}";
            if (damage == 0)
            {
                ReportResult($"{actorName} missed the {targetName}.");
            }
            else
            {
                ReportResult($"{actorName} hit the {targetName} for {damage} point{(damage > 1 ? "s" : "")}.");
                target.TakeDamage(damage);
            }
        }
    }
}
