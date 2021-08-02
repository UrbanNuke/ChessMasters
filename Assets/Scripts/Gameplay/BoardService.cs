using Misc;
using UnityEngine;

namespace Gameplay
{
    public class BoardService
    {
        public FigureMeta[,] StartFigureData { get; private set; } = new FigureMeta[8,8];
        public const int Rows = 8;
        public const int Columns = 8;
        public const float FieldOffset = 0.5f;

        public Figure[,] FiguresPosition { get; private set; } = new Figure[8,8];
        public void SetStartFigurePosition(Vector2 position, Figure figure) => FiguresPosition[(int)position.x, (int)position.y] = figure;
        
        public BoardService()
        {
            for (int i = 0; i < Rows; ++i)
            {
                for (int j = 0; j < Columns; ++j)
                {
                    StartFigureData[i, j] = GetStartFigureMetaByPosition(j, i);
                }
            }
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