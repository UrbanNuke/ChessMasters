using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class PossibleMove : MonoBehaviour
    {
        private BoardService _boardService;

        [Inject]
        private void Construct(BoardService boardService)
        {
            _boardService = boardService;
        }

        private void OnMouseUp()
        {
            _boardService.ActiveFigure.Move(transform.position);
        }
    }
}