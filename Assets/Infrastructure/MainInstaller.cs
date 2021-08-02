using Factories;
using Gameplay;
using Zenject;

namespace Infrastructure
{
    public class MainInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindFigureFactory();
            BindBoardService();
        }

        private void BindBoardService()
        {
            Container
                .Bind<BoardService>()
                .FromInstance(new BoardService())
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
    }
}