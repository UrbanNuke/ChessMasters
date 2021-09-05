using Misc;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class PossibleMove : MonoBehaviour
    {
        private BoardService _boardService;
        private Figure _possibleFigure;
        private Material _figureInitialMaterial;
        private const string HighlightMaterialPath = "Materials/M_Highlight";

        [Inject]
        private void Construct(BoardService boardService)
        {
            _boardService = boardService;
        }

        private void OnMouseUp()
        {
            _boardService.ActiveFigure.Move(transform.position);
        }

        private void Start()
        {
            BoardPosition boardPosition = BoardPosition.FromBoardCoord(transform.position);
            _possibleFigure = _boardService.FiguresPosition[boardPosition.y, boardPosition.x];
            
            if (_possibleFigure)
            {
                GetComponent<MeshRenderer>().enabled = false;
                _possibleFigure.CanBeBeaten = true;
                _figureInitialMaterial = _possibleFigure.GetComponent<MeshRenderer>().material;
                Material movePositionMaterial = Resources.Load(HighlightMaterialPath) as Material;
                _possibleFigure.ChangeMaterial(movePositionMaterial);
            }
        }

        private void OnDestroy()
        {
            if (!_possibleFigure) return;
            
            _possibleFigure.CanBeBeaten = false;
            _possibleFigure.ChangeMaterial(_figureInitialMaterial);
        }
    }
}