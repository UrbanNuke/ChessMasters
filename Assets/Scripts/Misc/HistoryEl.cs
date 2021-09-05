using Gameplay;

namespace Misc
{
    public struct HistoryEl
    {
        public BoardPosition Position { get; private set; }
        public FigureMeta FigureMeta { get; private set; }
        public Figure Figure { get; private set; }

        public HistoryEl(Figure figure)
        {
            Position = new BoardPosition(figure.Position.y, figure.Position.x);
            FigureMeta = new FigureMeta(figure.Type, figure.Color);
            Figure = figure;
        }
    }
}