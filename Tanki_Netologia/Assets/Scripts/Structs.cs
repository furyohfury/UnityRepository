namespace Tanks
{
    public struct BulletData
    {
        public int Damage { get; set; }
        public float Speed { get; set; }
        public Tank Owner { get; set; }
        public BulletData(int damage, float speed, Tank owner)
        {
            Damage = damage;
            Speed = speed;
            Owner = owner;
        }
    }
}
