using System.Collections;
using Misc;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class Figure : MonoBehaviour
    {
        public FigureType Type { get; private set; }
        public void SetType(FigureType type) => Type = type;
        
        public FigureColor Color { get; private set; }
        public void SetColor(FigureColor color) => Color = color;
        
        public BoardPosition Position { get; private set; }
        public void SetPosition(BoardPosition position) => Position = position;

        public bool WasBeaten { get; set; }

        private BoardService _boardService;
        private Outline _outline;

        [Inject]
        private void Construct(BoardService boardService)
        {
            _boardService = boardService;
            _outline = GetComponent<Outline>();
        }

        private void Start()
        {
            Material mat = Resources
                .Load(Color == FigureColor.White ? "Materials/M_White" : "Materials/M_Black") as Material;
            GetComponent<MeshRenderer>().material = mat;
            
            _boardService.SetStartFigurePosition(Position, this);
        }

        private void OnMouseEnter()
        {
            if (!CanChooseOrHoverFigure()) return;
            _outline.enabled = true;
            _outline.OutlineColor = UnityEngine.Color.blue;
        }

        private void OnMouseExit()
        {
            if (!CanChooseOrHoverFigure()) return;
            _outline.enabled = false;
        }

        private void OnMouseUp()
        {
            if (!CanChooseOrHoverFigure()) return;
            _boardService.SetActiveFigure(this);
        }
        
        private bool CanChooseOrHoverFigure()
        {
            return this != _boardService.ActiveFigure && !_boardService.IsFigureMoving && Color == _boardService.ActivePlayer && !WasBeaten;
        }

        public void Move(Vector3 newPosition)
        {
            _boardService.FiguresPosition[Position.y, Position.x] = null;
            Position = BoardPosition.FromBoardCoord(newPosition);
            Figure figureOnNewPosition = _boardService.FiguresPosition[Position.y, Position.x];
            if (figureOnNewPosition != null)
            {
                _boardService.BeatFigure(figureOnNewPosition);
            }
            _boardService.FiguresPosition[Position.y, Position.x] = this;
            
            _boardService.SetActiveFigure(null);
            _boardService.IsFigureMoving = true;
            StartCoroutine(MoveFigure(newPosition));
        }

        private IEnumerator MoveFigure(Vector3 newPosition)
        {
            float t = 0f;
            Vector3 distance = newPosition - transform.position;
            Vector3 firstBezierPoint = transform.position;
            Vector3 secondBezierPoint = transform.position + distance / 4;
            secondBezierPoint.y = 1.5f;
            Vector3 thirdBezierPoint = newPosition;

            while (t < 1f)
            {
                Vector3 a = Vector3.Lerp(firstBezierPoint, secondBezierPoint, t);
                Vector3 b = Vector3.Lerp(secondBezierPoint, thirdBezierPoint, t);
                transform.position = Vector3.Lerp(a, b, t);
                t += Time.deltaTime;
                yield return null;
            }
            
            _boardService.EndFigureMove();
        }
    }
}