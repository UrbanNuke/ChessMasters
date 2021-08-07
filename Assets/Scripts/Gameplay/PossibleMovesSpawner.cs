using Factories;
using Misc;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class PossibleMovesSpawner : MonoBehaviour
    {
        [SerializeField]
        private Transform possibleMovesContainer;

        private BoardService _boardService;
        private IPossibleMovesFactory _possibleMovesFactory;
        private PossibleMovesService _possibleMovesService;

        [Inject]
        private void Construct(BoardService boardService,
            PossibleMovesService possibleMovesService,
            IPossibleMovesFactory possibleMovesFactory
        )
        {
            _boardService = boardService;
            _possibleMovesService = possibleMovesService;
            _possibleMovesFactory = possibleMovesFactory;
            _possibleMovesFactory.Load();
            _boardService.OnFigureSelected += SpawnPossibleMoves;
        }

        private void SpawnPossibleMoves(Figure figure)
        {
            foreach (Transform move in possibleMovesContainer)
            {
                Destroy(move.gameObject);
            }

            foreach (Vector3 move in _possibleMovesService.Get(_boardService.ActiveFigure))
            {
                _possibleMovesFactory.Create(move, Quaternion.identity, possibleMovesContainer);
            }
        }
    }
}