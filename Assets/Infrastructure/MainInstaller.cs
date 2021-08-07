using Factories;
using Gameplay;
using Misc;
using Zenject;

namespace Infrastructure
{
    public class MainInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindFigureFactory();
            BindPossibleMovesFactory();
            BindBoardService();
            BindPossibleMovesService();
        }

        private IfNotBoundBinder BindPossibleMovesService()
        {
            return Container
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