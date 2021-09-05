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

        public bool CanBeBeaten { get; set; }
        public bool WasBeaten { get; set; }

        protected BoardService _boardService;
        private HistoryService _historyService;
        private Outline _outline;

        [Inject]
        private void Construct(BoardService boardService, HistoryService historyService)
        {
            _boardService = boardService;
            _historyService = historyService;
            _outline = GetComponent<Outline>();
        }

        private void Start()
        {
            Material mat = Resources.Load(Color == FigureColor.White ? "Materials/M_White" : "Materials/M_Black") as Material;
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
            if (CanBeBeaten)
            {
                _boardService.ActiveFigure.Move(transform.position);
                return;
            }
            
            if (!CanChooseOrHoverFigure()) return;
            _boardService.SetActiveFigure(this);
        }

        public virtual void Move(Vector3 newPosition)
        {
            _boardService.FiguresPosition[Position.y, Position.x] = null;
            Position = BoardPosition.FromBoardCoord(newPosition);
            Figure figureOnNewPosition = _boardService.FiguresPosition[Position.y, Position.x];
            if (figureOnNewPosition != null)
            {
                _boardService.BeatFigure(figureOnNewPosition);
            }
            _boardService.FiguresPosition[Position.y, Position.x] = this;
            
            _historyService.Add(new HistoryEl(this));
            
            _boardService.SetActiveFigure(null);
            _boardService.IsFigureMoving = true;
            StartCoroutine(MoveFigure(newPosition));
        }

        public void ChangeMaterial(Material material) => GetComponent<MeshRenderer>().material = material;

        private bool CanChooseOrHoverFigure()
        {
            return this != _boardService.ActiveFigure && !_boardService.IsFigureMoving && Color == _boardService.ActivePlayer && !WasBeaten;
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