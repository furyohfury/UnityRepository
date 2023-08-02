namespace Zikkurat
{
    public struct UnitStats
    {
        public int _HP;
        public float _moveSpeed;
        public float _maxVelocity;
        public int _fastAttackDamage;
        /// <summary>
        /// Time for making fast attack and wait time after it
        /// </summary>
        public float _fastAttackTime;
        public int _heavyAttackDamage;
        /// <summary>
        /// Time for making heavy attack and wait time after it
        /// </summary
        public float _heavyAttackTime;
        /// <summary>
        /// Range - 0...1
        /// </summary>
        public float _missProbability;
        /// <summary>
        /// Range - 0...1
        /// </summary>
        public float _doubleDamageProbability;
        /// <summary>
        /// if lesser -> fast else heavy
        /// </summary>
        public float _fastHeavyProbability;
        public float _attackRange;
        public UnitStats(int hp, float ms, float maxvel, int fastattackdamage, float fastattacktime, int heavyattackdamage, float heavyattacktime, float missprob, float ddprob, float fhprob, float attackrange)
        {
            _HP = hp;
            _moveSpeed = ms;
            _maxVelocity = maxvel;
            _fastAttackDamage = fastattackdamage;
            _fastAttackTime = fastattacktime;
            _heavyAttackDamage = heavyattackdamage;
            _heavyAttackTime = heavyattacktime;
            _missProbability = missprob;
            _doubleDamageProbability = ddprob;
            _fastHeavyProbability = fhprob;
            _attackRange = attackrange;
        }
        public UnitStats(Race race)
        {
            switch (race)
            {
                case Race.Terran:
                    {
                        _HP = 10;
                        _moveSpeed = 5;
                        _maxVelocity = 3f;
                        _fastAttackDamage = 3;
                        _fastAttackTime = 2;
                        _heavyAttackDamage = 6;
                        _heavyAttackTime = 4;
                        _missProbability = 0.4f;
                        _doubleDamageProbability = 0.2f;
                        _fastHeavyProbability = 0.4f;
                        _attackRange = 3f;
                        break;
                    }
                case Race.Zerg:
                    {
                        _HP = 5;
                        _moveSpeed = 8;
                        _maxVelocity = 5;
                        _fastAttackDamage = 1;
                        _fastAttackTime = 1;
                        _heavyAttackDamage = 3;
                        _heavyAttackTime = 2;
                        _missProbability = 0.3f;
                        _doubleDamageProbability = 0.1f;
                        _fastHeavyProbability = 0.8f;
                        _attackRange = 4f;
                        break;
                    }
                case Race.Protoss:
                    {
                        _HP = 7;
                        _moveSpeed = 6;
                        _maxVelocity = 5f;
                        _fastAttackDamage = 2;
                        _fastAttackTime = 1.5f;
                        _heavyAttackDamage = 4;
                        _heavyAttackTime = 3;
                        _missProbability = 0.2f;
                        _doubleDamageProbability = 0.4f;
                        _fastHeavyProbability = 0.6f;
                        _attackRange = 3f;
                        break;
                    }
                default:
                    {
                        _HP = 10;
                        _moveSpeed = 2;
                        _maxVelocity = 2f;
                        _fastAttackDamage = 2;
                        _fastAttackTime = 2;
                        _heavyAttackDamage = 5;
                        _heavyAttackTime = 4;
                        _missProbability = 0.2f;
                        _doubleDamageProbability = 0.2f;
                        _fastHeavyProbability = 0.2f;
                        _attackRange = 3f;
                        break;
                    }
            }
        }
    }
    public struct WanderData
    {
        public float WanderCenterDistance;
        public float WanderRadius;
        public float WanderAngleRange;
        public WanderData(float wcd, float wr, float war)
        {
            WanderCenterDistance = wcd;
            WanderRadius = wr;
            WanderAngleRange = war;
        }
    }


    public struct ModelsPaths
    {
        public static string _terranPath = "Prefabs/FiremanEmp";
        public static string _zergPath = "Prefabs/ZergEmp";
        public static string _protossPath = "Prefabs/ZealotEmp";
        public static string _spawnPointPath = "Prefabs/SpawnPointFlag";
        public static string _terranBasePath = "Prefabs/TerranBase";
        public static string _zergBasePath = "Prefabs/ZergBase";
        public static string _protossBasePath = "Prefabs/ProtossBase";
        public static string _damagedMaterialPath = "Materials/Damaged";
    }
}