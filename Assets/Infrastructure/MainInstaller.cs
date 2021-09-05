using Factories;
using Gameplay;
using Misc;
using UnityEngine;
using Zenject;

namespace Infrastructure
{
    public class MainInstaller : MonoInstaller
    {
        public GameObject beatenFiguresPrefab;
        
        public override void InstallBindings()
        {
            BindFigureFactory();
            BindPossibleMovesFactory();
            BindBoardService();
            BindPossibleMovesService();
            BindBeatenFigures();
            BindHistoryService();
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