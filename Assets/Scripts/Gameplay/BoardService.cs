using System;
using System.Collections.Generic;
using System.Linq;
using Misc;
using UnityEngine;

namespace Gameplay
{
    public class BoardService
    {
        public event Action<Figure> OnFigureSelected;
        public event Func<FigureColor, bool> OnFigureWasMoved;
        public event Func<FigureColor, bool> OnPlayerCheck;

        public const int Rows = 8;
        public const int Columns = 8;
        public const float FieldOffset = 0.5f;
        public const int Border = 7;

        public FigureMeta[,] StartFigureData { get; private set; } = new FigureMeta[8, 8];
        public Figure[,] FiguresPosition { get; private set; } = new Figure[8, 8];

        public List<Figure> WhiteFigures { get; private set; } = new List<Figure>(16);
        public Figure WhiteKing { get; private set; }

        public List<Figure> BlackFigures { get; private set; } = new List<Figure>(16);
        public Figure BlackKing { get; private set; }

        public Figure ActiveFigure { get; private set; }
        public bool IsFigureMoving { get; set; }
        public FigureColor ActivePlayer { get; private set; } = FigureColor.White;

        public BoardState BoardState { get; private set; } = BoardState.None;
        
        private readonly BeatenFigures _beatenFigures;
        private readonly HistoryService _historyService;

        public BoardService(BeatenFigures beatenFigures, HistoryService historyService)
        {
            _beatenFigures = beatenFigures;
            _historyService = historyService;
            for (int i = 0; i < Rows; ++i)
            {
                for (int j = 0; j < Columns; ++j)
                {
                    StartFigureData[i, j] = GetStartFigureMetaByPosition(j, i);
                }
            }
        }

        public void SetStartFigurePosition(BoardPosition position, Figure figure)
        {
            FiguresPosition[position.y, position.x] = figure;
            if (figure.Color == FigureColor.White)
            {
                WhiteFigures.Add(figure);
                if (figure.Type == FigureType.King) WhiteKing = figure;
            }
            else
            {
                BlackFigures.Add(figure);
                if (figure.Type == FigureType.King) BlackKing = figure;
            }
        }

        public void SetActiveFigure(Figure figure)
        {
            if (ActiveFigure)
                ActiveFigure.GetComponent<Outline>().enabled = false;

            ActiveFigure = figure;
            if (ActiveFigure != null)
            {
                Outline newFigureOutline = ActiveFigure.GetComponent<Outline>();
                newFigureOutline.enabled = true;
                newFigureOutline.OutlineColor = UnityEngine.Color.green;
            }

            OnFigureSelected?.Invoke(ActiveFigure);
        }

        public void EndFigureMove()
        {
            IsFigureMoving = false;
            ClearPawnsEnPassantStatus();
            ActivePlayer = ActivePlayer == FigureColor.White ? FigureColor.Black : FigureColor.White;
            bool? isCheckState = OnFigureWasMoved?.Invoke(ActivePlayer);
            
            if (!isCheckState.HasValue || !isCheckState.Value) return;
            
            BoardState = BoardState.Check;
            bool? isCheckmateState = OnPlayerCheck?.Invoke(ActivePlayer);
            if (isCheckmateState.HasValue && isCheckmateState.Value)
                BoardState = BoardState.Checkmate;
        }

        public void BeatFigure(Figure beatenFigure)
        {
            beatenFigure.WasBeaten = true;
            beatenFigure.CanBeBeaten = false;
            beatenFigure.GetComponent<MeshCollider>().enabled = false; // so not to fire mouse enter events
            _beatenFigures.PutBeatenFigure(beatenFigure);
        }

        private FigureMeta GetStartFigureMetaByPosition(int x, int y)
        {
            switch (y)
            {
                case 1:
                case 6:
                    return new FigureMeta(FigureType.Pawn, y == 1 ? FigureColor.White : FigureColor.Black);
                case 0:
                case 7:
                    if (x == 0 || x == 7)
                        return new FigureMeta(FigureType.Tower, y == 0 ? FigureColor.White : FigureColor.Black);

                    if (x == 1 || x == 6)
                        return new FigureMeta(FigureType.Horse, y == 0 ? FigureColor.White : FigureColor.Black);

                    if (x == 2 || x == 5)
                        return new FigureMeta(FigureType.Bishop, y == 0 ? FigureColor.White : FigureColor.Black);

                    if (x == 3)
                        return new FigureMeta(FigureType.Queen, y == 0 ? FigureColor.White : FigureColor.Black);

                    if (x == 4)
                        return new FigureMeta(FigureType.King, y == 0 ? FigureColor.White : FigureColor.Black);

                    break;
            }

            return null;
        }

        private void ClearPawnsEnPassantStatus()
        {
            HistoryEl lastMove = _historyService.History[_historyService.History.Count - 1];
            
            List<Figure> oneSideFigures = lastMove.FigureMeta.color == FigureColor.White
                ? WhiteFigures
                : BlackFigures;

            oneSideFigures
                .Where(figure => !figure.WasBeaten && figure.Type == FigureType.Pawn && figure != lastMove.Figure)
                .ToList()
                .ForEach(figure =>
                {
                    if (figure is Pawn pawn)
                        pawn.CanBeBeatenByEnPassant = false;
                });
        }
    }
}