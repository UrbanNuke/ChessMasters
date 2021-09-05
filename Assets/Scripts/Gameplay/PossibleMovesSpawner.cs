using System.Collections;
using Factories;
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
            StartCoroutine(SpawnPossibleMovesCoroutine(figure));
        }

        private IEnumerator SpawnPossibleMovesCoroutine(Figure figure)
        {
            foreach (Transform move in possibleMovesContainer)
                Destroy(move.gameObject);
            
            // requires(!) so OnDestroy(possibleMove) should call first, only then create new possibleMoves
            // otherwise undefined behaviour when select figure 
            yield return new WaitUntil(() => possibleMovesContainer.childCount == 0);
            
            if (figure == null) 
                yield break;

            foreach (Vector3 move in _possibleMovesService.Get(figure))
                _possibleMovesFactory.Create(move, Quaternion.identity, possibleMovesContainer);
        }
    }
}