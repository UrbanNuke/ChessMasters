﻿using System;
using Misc;

namespace Gameplay
{
    public class BoardService
    {
        private readonly BeatenFigures _beatenFigures;
        public event Action<Figure> OnFigureSelected;
        
        public const int Rows = 8;
        public const int Columns = 8;
        public const float FieldOffset = 0.5f;
        public const int Border = 7;
        
        public FigureMeta[,] StartFigureData { get; private set; } = new FigureMeta[8,8];

        public Figure[,] FiguresPosition { get; private set; } = new Figure[8,8];
        public void SetStartFigurePosition(BoardPosition position, Figure figure) => FiguresPosition[position.y, position.x] = figure;

        public Figure ActiveFigure { get; private set; }
        public bool IsFigureMoving { get; set; }

        public FigureColor ActivePlayer { get; private set; } = FigureColor.White;

        public BoardService(BeatenFigures beatenFigures)
        {
            _beatenFigures = beatenFigures;
            for (int i = 0; i < Rows; ++i)
            {
                for (int j = 0; j < Columns; ++j)
                {
                    StartFigureData[i, j] = GetStartFigureMetaByPosition(j, i);
                }
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
            ActivePlayer = ActivePlayer == FigureColor.White ? FigureColor.Black : FigureColor.White;
        }

        public void BeatFigure(Figure beatenFigure)
        {
            beatenFigure.WasBeaten = true;
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
    }
}