using Factories;
using Misc;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class FigureSpawner : MonoBehaviour
    {
        [SerializeField]
        private Transform figuresContainer;

        private IFigureFactory _figureFactory;
        private BoardService _boardService;
        private EventDeliveryService _eventDeliveryService;

        [Inject]
        public void Construct(IFigureFactory figureFactory, BoardService boardService, EventDeliveryService eventDeliveryService)
        {
            _boardService = boardService;
            _figureFactory = figureFactory;
            _figureFactory.Load();
            _eventDeliveryService = eventDeliveryService;
            _eventDeliveryService.OnBoardRestart += SpawnFiguresToStart;
            SpawnFiguresToStart();
        }

        private void SpawnFiguresToStart()
        {
            for (int i = 0; i < BoardService.Rows; ++i)
            {
                for (int j = 0; j < BoardService.Columns; ++j)
                {
                    FigureMeta figureMeta = _boardService.StartFigureData[i, j];
                    if (figureMeta == null) continue;
                    
                    Quaternion rotation = figureMeta.color == FigureColor.Black 
                        ? Quaternion.Euler(new Vector3(0, 180f, 0))
                        : Quaternion.identity;
                    
                    _figureFactory.Create(
                        figureMeta.type,
                        figureMeta.color,
                        new BoardPosition(i, j),
                        new Vector3(j * BoardService.FieldOffset, 0.5f, i * BoardService.FieldOffset),
                        rotation,
                        figuresContainer
                    );
                }
            }
        }
    }
}