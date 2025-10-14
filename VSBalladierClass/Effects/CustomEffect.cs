using effectshud.src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common.Entities;

namespace VSBalladeerClass.Effects
{
    public abstract class CustomEffect : Effect
    {
        protected Entity? Entity
        {
            get
            {
                return (Entity?)typeof(Effect).GetField("entity", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(this);
            }
        }

        protected CustomEffect() { }

        protected CustomEffect(int tier, bool infinite) : base(tier, infinite) { }

        protected void SetStat(string statName, string modifierName, float effectiveness)
        {
            Entity?.Stats.Set(statName, modifierName, effectiveness);
        }

        public override bool OnDeath()
        {
            EBEffectsAffected? ebea = Entity?.GetBehavior<EBEffectsAffected>();
            if (ebea == null)
            {
                return false;
            }
            if (this.removedAfterDeath)
            {
                ebea.activeEffects.Remove(this.effectTypeId);
                ebea.needUpdate = true;
                return true;
            }
            return false;
        }
    }
}
