using Zenject;
namespace Pasians
{
    public class SceneSystems : MonoInstaller
    {
        private PrefabProvider _prefabProvider;
        
        public override void InstallBindings()
        {            
            Container.BindInstance(_prefabProvider).AsSingle();
        }
    }
}