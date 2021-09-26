using Misc;
using UI;
using UnityEngine;
using Zenject;

namespace Infrastructure
{
    public class BootstrapInstaller : MonoInstaller
    {
        public GameObject UIServicePrefab;
        
        public override void InstallBindings()
        {
            BindUIService();
            BindEventDeliveryService();
        }

        private void BindEventDeliveryService()
        {
            Container
                .Bind<EventDeliveryService>()
                .AsSingle()
                .NonLazy();
        }

        private void BindUIService()
        {
            Container
                .Bind<UIService>()
                .FromComponentInNewPrefab(UIServicePrefab)
                .AsSingle()
                .NonLazy();
        }
    }
}