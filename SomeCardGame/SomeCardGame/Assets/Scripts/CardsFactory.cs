using UnityEngine;
namespace Pasians
{
    public class CardsFactory
    {
        private PrefabProvider _prefabProvider;


        public CardsFactory(PrefabProvider provider)
        {
            _prefabProvider = provider;
        }
    }
}