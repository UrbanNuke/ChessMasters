using Gameplay;
using Misc;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Factories
{
    public class FigureFactory : IFigureFactory
    {
        private readonly IInstantiator _instantiator;
        private Object _pawnFigure;
        private Object _towerFigure;
        private Object _horseFigure;
        private Object _bishopFigure;
        private Object _queenFigure;
        private Object _kingFigure;

        private FigureFactory(IInstantiator instantiator)
        {
            _instantiator = instantiator;
        }

        public void Load()
        {
            _pawnFigure = Resources.Load("Objects/I_Pawn");
            _towerFigure = Resources.Load("Objects/I_Tower");
            _horseFigure = Resources.Load("Objects/I_Horse");
            _bishopFigure = Resources.Load("Objects/I_Bishop");
            _queenFigure = Resources.Load("Objects/I_Queen");
            _kingFigure = Resources.Load("Objects/I_King");
        }

        public void Create(FigureType type, FigureColor color, Vector2 position, Vector3 at, Quaternion rotation, Transform parent)
        {
            GameObject instantiatePrefab = type switch
            {
                FigureType.Pawn => _instantiator.InstantiatePrefab(_pawnFigure, at, rotation, parent),
                FigureType.Tower => _instantiator.InstantiatePrefab(_towerFigure, at, rotation, parent),
                FigureType.Horse => _instantiator.InstantiatePrefab(_horseFigure, at, rotation, parent),
                FigureType.Bishop => _instantiator.InstantiatePrefab(_bishopFigure, at, rotation, parent),
                FigureType.Queen => _instantiator.InstantiatePrefab(_queenFigure, at, rotation, parent),
                FigureType.King => _instantiator.InstantiatePrefab(_kingFigure, at, rotation, parent),
                _ => _instantiator.InstantiatePrefab(_pawnFigure, at, rotation, parent)
            };

            Figure figureComponent = instantiatePrefab.GetComponent<Figure>();
            figureComponent.SetType(type);
            figureComponent.SetColor(color);
            figureComponent.SetPosition(position);
        }
    }
}

