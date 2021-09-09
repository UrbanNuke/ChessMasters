using System.Linq;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Misc
{
    public class DebugService : MonoBehaviour
    {
        [SerializeField]
        private GameObject outputObject;
        private Text _output;
        
        private BoardService _boardService;
        private HistoryService _historyService;
        
        
        [Inject]
        public void Construct(BoardService boardService, HistoryService historyService)
        {
            _boardService = boardService;
            _historyService = historyService;
            _output = outputObject.GetComponent<Text>();
        }

        private void LateUpdate()
        {
            BuildOutputText();
        }

        private void BuildOutputText()
        {
            _output.text = $"Board state: {_boardService.BoardState.ToString()}\n";
            string activeFigure = _boardService.ActiveFigure ? _boardService.ActiveFigure.Type.ToString() : "None";
            _output.text += $"Active figure: {activeFigure}\n";
            string lastMoveString = "None";
            if (_historyService.History.Count > 0)
            {
                HistoryEl lastMove = _historyService.History[_historyService.History.Count - 1];
                lastMoveString = $"{lastMove.FigureMeta.color.ToString()} {lastMove.FigureMeta.type.ToString()} " +
                                 $"{lastMove.Position.y}:{lastMove.Position.x}";
            }
            _output.text += $"Last move: {lastMoveString}\n";
            _output.text += $"White figures remaining: {_boardService.WhiteFigures.Where(fig => !fig.WasBeaten).ToList().Count}\n";
            _output.text += $"Black figures remaining: {_boardService.BlackFigures.Where(fig => !fig.WasBeaten).ToList().Count}\n";
        }
    }
}