using System;
using effectshud.src;
using System.Reflection;
using Vintagestory.API.Common.Entities;

namespace VSBalladeerClass.Effects
{
    public abstract class CustomEffect : Effect
    {
        // Retrieve the field via reflection only ONCE
        private static FieldInfo EntityField { get; } =
            typeof(Effect).GetField("entity", BindingFlags.Instance | BindingFlags.NonPublic) ??
            throw new Exception("Unable to find the entity field on the Effect type!");

        protected Entity? Entity => (Entity?)EntityField.GetValue(this);

        protected CustomEffect()
        {
        }

        protected CustomEffect(int tier, bool infinite) : base(tier, infinite)
        {
        }

        protected void SetStat(string statName, string modifierName, float effectiveness)
        {
            Entity?.Stats.Set(statName, modifierName, effectiveness);
        }

        public override bool OnDeath()
        {
            var behavior = Entity?.GetBehavior<EBEffectsAffected>();
            if (behavior == null || !removedAfterDeath)
            {
                return false;
            }

            behavior.activeEffects.Remove(this.effectTypeId);
            behavior.needUpdate = true;
            return true;
        }
    }
}