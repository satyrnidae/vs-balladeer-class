namespace VSBalladeerClass.Model
{
    public class Configuration
    {
        public BalladeerEffectRadius EffectRadius = new();
    }

    public class BalladeerEffectRadius
    {
        public float Vertical = 5f;

        public float Horizontal = 20.5f;
    }
}