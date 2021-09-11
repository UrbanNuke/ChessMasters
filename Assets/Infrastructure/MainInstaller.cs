using Factories;
using Gameplay;
using Misc;
using UI;
using UnityEngine;
using Zenject;

namespace Infrastructure
{
    public class MainInstaller : MonoInstaller
    {
        public GameObject beatenFiguresPrefab;
        public GameObject debugServicePrefab;
        public GameObject UIServicePrefab;
        
        public override void InstallBindings()
        {
            BindFigureFactory();
            BindPossibleMovesFactory();
            BindBoardService();
            BindPossibleMovesService();
            BindBeatenFigures();
            BindHistoryService();
            BindUIService();
            BindDebugService();
        }

        private void BindUIService()
        {
            Container
                .Bind<UIService>()
                .FromComponentInNewPrefab(UIServicePrefab)
                .AsSingle()
                .NonLazy();
        }

        private void BindDebugService()
        {
            #if UNITY_EDITOR
            Container
                .Bind<DebugService>()
                .FromComponentInNewPrefab(debugServicePrefab)
                .AsSingle()
                .NonLazy();
            #endif
        }

        private void BindHistoryService()
        {
            Container
                .Bind<HistoryService>()
                .AsSingle()
                .NonLazy();
        }

        private void BindBeatenFigures()
        {
            Container
                .Bind<BeatenFigures>()
                .FromComponentInNewPrefab(beatenFiguresPrefab)
                .AsSingle();
        }

        private void BindPossibleMovesService()
        {
            Container
                .Bind<PossibleMovesService>()
                .AsSingle()
                .NonLazy();
        }

        private void BindBoardService()
        {
            Container
                .Bind<BoardService>()
                .AsSingle()
                .NonLazy();
        }

        private void BindFigureFactory()
        {
            Container
                .Bind<IFigureFactory>()
                .To<FigureFactory>()
                .AsSingle();
        }

        private void BindPossibleMovesFactory()
        {
            Container
                .Bind<IPossibleMovesFactory>()
                .To<PossibleMovesFactory>()
                .AsSingle();
        }
    }
}