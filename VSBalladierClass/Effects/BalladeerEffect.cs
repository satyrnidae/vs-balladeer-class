using effectshud.src;
using effectshud.src.DefaultEffects;

namespace VSBalladeerClass.Effects
{
    public class BalladeerEffect : CustomEffect
    {
        #region Constants

        private const string BALLADEER_EFFECT = "balladeerbuff";
        private const string HEALING_EFFECTIVENESS = "healingeffectivness";
        private const string HUNGER_RATE = "hungerrate";
        private const string CAN_TEMPORAL_CHARGE = "cantemporalcharge";
        private const string WALK_SPEED = "walkspeed";
        private const string MELEE_WEAPON_DAMAGE = "meleeWeaponsDamage";
        private const string RANGED_WEAPONS_DAMAGE = "rangedWeaponsDamage";
        private const string RANGED_WEAPONS_ACCURACY = "rangedWeaponsAcc";
        private const string RANGED_WEAPONS_SPEED = "rangedWeaponsSpeed";
        private const string ANIMAL_SEEKING_RANGE = "animalSeekingRange";

        #endregion Constants

        public BalladeerEffect()
        {
            effectTypeId = BALLADEER_EFFECT;
        }

        public BalladeerEffect(int seconds = 5, int tier = 1, bool infinite = false) : base(1, infinite)
        {
            SetExpiryInRealSeconds(seconds);
            positive = true;
            effectTypeId = BALLADEER_EFFECT;
        }

        public override void OnStart()
        {
            ApplyStatChange(tier);
        }

        public override void OnStack(Effect otherEffect)
        {
            if (tier <= otherEffect.Tier)
            {
                if (tier < otherEffect.Tier)
                {
                    tier = otherEffect.Tier;
                    ApplyStatChange(tier);
                }

                ExpireTick = otherEffect.ExpireTick;
                TickCounter = otherEffect.TickCounter;
            }
        }

        public override void OnExpire()
        {
            ApplyStatChange(0);
        }

        public override bool OnDeath()
        {
            ApplyStatChange(0);
            return base.OnDeath();
        }

        private void ApplyStatChange(int multiplier)
        {
            SetStat(HEALING_EFFECTIVENESS, BALLADEER_EFFECT, 0.15f * multiplier);
            SetStat(HUNGER_RATE, BALLADEER_EFFECT, -0.20f * multiplier);
            SetStat(CAN_TEMPORAL_CHARGE, BALLADEER_EFFECT, 0.2f * multiplier);
            SetStat(WALK_SPEED, BALLADEER_EFFECT, 0.10f * multiplier);
            SetStat(MELEE_WEAPON_DAMAGE, BALLADEER_EFFECT, 0.25f * multiplier);
            SetStat(RANGED_WEAPONS_DAMAGE, BALLADEER_EFFECT, 0.25f * multiplier);
            SetStat(RANGED_WEAPONS_ACCURACY, BALLADEER_EFFECT, 0.1f * multiplier);
            SetStat(RANGED_WEAPONS_SPEED, BALLADEER_EFFECT, 0.1f * multiplier);
            SetStat(ANIMAL_SEEKING_RANGE, BALLADEER_EFFECT, 0.35f * multiplier);
        }
    }
}