using effectshud.src;

namespace VSBalladeerClass.Effects
{
    public class BalladeerEffect : CustomEffect
    {
        private static string BALLADEER_EFFECT = "balladeerbuff";

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
            SetStat("healingeffectivness", BALLADEER_EFFECT, 0.15f * tier);
            SetStat("hungerrate", BALLADEER_EFFECT, -0.20f * tier);
            SetStat("cantemporalcharge", BALLADEER_EFFECT, 0.10f * tier);
            SetStat("walkspeed", BALLADEER_EFFECT, 0.10f * tier);
            SetStat("meleeWeaponsDamage", BALLADEER_EFFECT, 0.25f * tier);
            SetStat("rangedWeaponsDamage", BALLADEER_EFFECT, 0.25f * tier);
            SetStat("rangedWeaponsAcc", BALLADEER_EFFECT, 0.1f * tier);
            SetStat("rangedWeaponsSpeed", BALLADEER_EFFECT, 0.1f * tier);
            SetStat("animalSeekingRange", BALLADEER_EFFECT, 0.35f * tier);
        }

        public override void OnStack(Effect otherEffect)
        {
            if (tier <= otherEffect.Tier)
            {
                if (tier < otherEffect.Tier)
                {
                    tier = otherEffect.Tier;
                    SetStat("healingeffectivness", BALLADEER_EFFECT, 0.15f * tier);
                    SetStat("hungerrate", BALLADEER_EFFECT, -0.20f * tier);
                    SetStat("cantemporalcharge", BALLADEER_EFFECT, 0.10f * tier);
                    SetStat("walkspeed", BALLADEER_EFFECT, 0.10f * tier);
                    SetStat("meleeWeaponsDamage", BALLADEER_EFFECT, 0.25f * tier);
                    SetStat("rangedWeaponsDamage", BALLADEER_EFFECT, 0.25f * tier);
                    SetStat("rangedWeaponsAcc", BALLADEER_EFFECT, 0.1f * tier);
                    SetStat("rangedWeaponsSpeed", BALLADEER_EFFECT, 0.1f * tier);
                    SetStat("animalSeekingRange", BALLADEER_EFFECT, 0.35f * tier);
                }
                ExpireTick = otherEffect.ExpireTick;
                TickCounter = otherEffect.TickCounter;
            }
        }

        public override void OnExpire()
        {
            SetStat("healingeffectivness", BALLADEER_EFFECT, 0f);
            SetStat("hungerrate", BALLADEER_EFFECT, 0f);
            SetStat("cantemporalcharge", BALLADEER_EFFECT, 0f);
            SetStat("walkspeed", BALLADEER_EFFECT, 0f);
            SetStat("meleeWeaponsDamage", BALLADEER_EFFECT, 0f);
            SetStat("rangedWeaponsDamage", BALLADEER_EFFECT, 0f);
            SetStat("rangedWeaponsAcc", BALLADEER_EFFECT, 0f);
            SetStat("rangedWeaponsSpeed", BALLADEER_EFFECT, 0f);
            SetStat("animalSeekingRange", BALLADEER_EFFECT, 0f);
        }

        public override bool OnDeath()
        {
            SetStat("healingeffectivness", BALLADEER_EFFECT, 0f);
            SetStat("hungerrate", BALLADEER_EFFECT, 0f);
            SetStat("cantemporalcharge", BALLADEER_EFFECT, 0f);
            SetStat("walkspeed", BALLADEER_EFFECT, 0f);
            SetStat("meleeWeaponsDamage", BALLADEER_EFFECT, 0f);
            SetStat("rangedWeaponsDamage", BALLADEER_EFFECT, 0f);
            SetStat("rangedWeaponsAcc", BALLADEER_EFFECT, 0f);
            SetStat("rangedWeaponsSpeed", BALLADEER_EFFECT, 0f);
            SetStat("animalSeekingRange", BALLADEER_EFFECT, 0f);
            return base.OnDeath();
        }
    }
}
