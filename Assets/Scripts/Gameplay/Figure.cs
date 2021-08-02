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
        public Vector2 Position { get; private set; }
        public void SetPosition(Vector2 position) => Position = position;

        private BoardService _boardService;
        
        [Inject]
        private void Construct(BoardService boardService)
        {
            _boardService = boardService;
        }

        private void Start()
        {
            Material mat = Resources
                .Load(Color == FigureColor.White ? "Materials/M_White" : "Materials/M_Black") as Material;
            GetComponent<MeshRenderer>().material = mat;
            
            _boardService.SetStartFigurePosition(Position, this);
        }
    }
}