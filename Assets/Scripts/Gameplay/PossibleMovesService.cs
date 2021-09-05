using System.Collections.Generic;
using System.Linq;
using Misc;
using UnityEngine;

namespace Gameplay
{
    public class PossibleMovesService
    {
        private readonly BoardService _boardService;
        private const float PossibleMoveYPosition = 0.5004f;
        
        public PossibleMovesService(BoardService boardService)
        {
            _boardService = boardService;
        }

        public IEnumerable<Vector3> Get(Figure activeFigure)
        {
            IEnumerable<BoardPosition> baseBoardPositions = GetBoardPositions(activeFigure);
            return GetPossibleMovesWithoutCheckmate(activeFigure, baseBoardPositions).Select(FigurePositionToBoardCoord).ToList();
        }

        private IEnumerable<BoardPosition> GetBoardPositions(Figure figure)
        {
            return figure.Type switch
            {
                FigureType.Pawn => GetPawnPossibleMoves((Pawn)figure),
                FigureType.Tower => GetTowerPossibleMoves(figure),
                FigureType.Horse => GetHorsePossibleMoves(figure),
                FigureType.Bishop => GetBishopPossibleMoves(figure),
                FigureType.Queen => GetQueenPossibleMoves(figure),
                FigureType.King => GetKingPossibleMoves(figure),
                _ => null
            };
        }

        // TODO add extra strike move when enemy pawn jump 2 field
        private IEnumerable<BoardPosition> GetPawnPossibleMoves(Pawn activeFigure)
        {
            List<BoardPosition> result = new List<BoardPosition>(4);
            Vector3 forwardRelVec = activeFigure.transform.forward;
            Vector3 rightRelVec = activeFigure.transform.right;

            BoardPosition forward = activeFigure.Position + forwardRelVec;
            Figure towardsFigure = !IsOutOfBoard(forward)
                ? _boardService.FiguresPosition[forward.y, forward.x]
                : null;
            if (towardsFigure == null && !IsOutOfBoardOrTowardsFriendFigure(forward, activeFigure))
                result.Add(forward);

            if (activeFigure.CanDoubleMove)
            {
                BoardPosition forwardTwice = activeFigure.Position + forwardRelVec * 2;
                Figure doubleTowardsFigure = !IsOutOfBoard(forwardTwice)
                    ? _boardService.FiguresPosition[forwardTwice.y, forwardTwice.x]
                    : null;
                if (towardsFigure == null && doubleTowardsFigure == null && !IsOutOfBoard(forwardTwice))
                    result.Add(forwardTwice);
            }

            BoardPosition forwardRight = activeFigure.Position + (forwardRelVec + rightRelVec);
            Figure forwardRightFigure = !IsOutOfBoardOrTowardsFriendFigure(forwardRight, activeFigure)
                ? _boardService.FiguresPosition[forwardRight.y, forwardRight.x]
                : null;
            if (forwardRightFigure != null && forwardRightFigure.Color != activeFigure.Color)
                result.Add(forwardRight);

            BoardPosition forwardLeft = activeFigure.Position + (forwardRelVec + rightRelVec * -1);
            Figure forwardLeftFigure = !IsOutOfBoardOrTowardsFriendFigure(forwardLeft, activeFigure)
                ? _boardService.FiguresPosition[forwardLeft.y, forwardLeft.x]
                : null;
            if (forwardLeftFigure != null && forwardLeftFigure.Color != activeFigure.Color)
                result.Add(forwardLeft);

            return result;
        }

        private IEnumerable<BoardPosition> GetTowerPossibleMoves(Figure activeFigure)
        {
            Vector2Int[] directions = new Vector2Int[]
            {
                Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
            };

            IEnumerable<BoardPosition> result = GetPossibleMovesByDirections(activeFigure, directions);

            return result;
        }

        private IEnumerable<BoardPosition> GetHorsePossibleMoves(Figure activeFigure)
        {
            List<BoardPosition> result = new List<BoardPosition>(8);
            Vector3 forwardRelVec = activeFigure.transform.forward;
            Vector3 rightRelVec = activeFigure.transform.right;

            BoardPosition forwardRight = activeFigure.Position + (forwardRelVec * 2 + rightRelVec);
            BoardPosition forwardLeft = activeFigure.Position + (forwardRelVec * 2 + rightRelVec * -1);
            BoardPosition backRight = activeFigure.Position + (forwardRelVec * -2 + rightRelVec);
            BoardPosition backLeft = activeFigure.Position + (forwardRelVec * -2 + rightRelVec * -1);
            BoardPosition rightBottom = activeFigure.Position + (rightRelVec * 2 + forwardRelVec * -1);
            BoardPosition rightTop = activeFigure.Position + (rightRelVec * 2 + forwardRelVec);
            BoardPosition leftBottom = activeFigure.Position + (rightRelVec * -2 + forwardRelVec * -1);
            BoardPosition leftTop = activeFigure.Position + (rightRelVec * -2 + forwardRelVec);
            result.AddRange(new List<BoardPosition>
            {
                forwardRight, forwardLeft, backRight, backLeft, rightBottom, rightTop, leftBottom, leftTop
            });

            return result.Where(move => !IsOutOfBoardOrTowardsFriendFigure(move, activeFigure)).ToList();
        }

        private IEnumerable<BoardPosition> GetBishopPossibleMoves(Figure activeFigure)
        {
            Vector2Int[] directions = new Vector2Int[]
            {
                Vector2Int.up + Vector2Int.right, Vector2Int.up + Vector2Int.left, 
                Vector2Int.down + Vector2Int.right, Vector2Int.down + Vector2Int.left,
            };

            IEnumerable<BoardPosition> result = GetPossibleMovesByDirections(activeFigure, directions);

            return result;
        }

        private IEnumerable<BoardPosition> GetQueenPossibleMoves(Figure activeFigure)
        {
            Vector2Int[] directions = new Vector2Int[]
            {
                Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
                Vector2Int.up + Vector2Int.right, Vector2Int.up + Vector2Int.left, 
                Vector2Int.down + Vector2Int.right, Vector2Int.down + Vector2Int.left,
            };

            IEnumerable<BoardPosition> result = GetPossibleMovesByDirections(activeFigure, directions);

            return result;
        }

        private IEnumerable<BoardPosition> GetKingPossibleMoves(Figure activeFigure)
        {
            Vector2Int[] directions = new Vector2Int[]
            {
                Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
                Vector2Int.up + Vector2Int.right, Vector2Int.up + Vector2Int.left, 
                Vector2Int.down + Vector2Int.right, Vector2Int.down + Vector2Int.left,
            };

            IEnumerable<BoardPosition> result = GetPossibleMovesByDirections(activeFigure, directions, 1);

            return result;
        }

        private IEnumerable<BoardPosition> GetPossibleMovesByDirections(Figure activeFigure, IEnumerable<Vector2Int> directions, int howFar = 7)
        {
            List<BoardPosition> result = new List<BoardPosition>(30);
            foreach (Vector2Int direction in directions)
            {
                for (int i = 1; i <= howFar; ++i)
                {
                    BoardPosition move = activeFigure.Position + direction * i;
                    if (IsOutOfBoardOrTowardsFriendFigure(move, activeFigure))
                        break;

                    Figure forwardFigure = _boardService.FiguresPosition[move.y, move.x];
                    if (!forwardFigure)
                    {
                        result.Add(move);
                        continue;
                    }

                    if (forwardFigure.Color == activeFigure.Color) continue;
                    result.Add(move);
                    break;
                }
            }

            return result;
        }

        private bool IsOutOfBoard(BoardPosition pos) => pos.x > BoardService.Border || pos.x < 0 || pos.y > BoardService.Border || pos.y < 0;

        private bool IsOutOfBoardOrTowardsFriendFigure(BoardPosition pos, Figure activeFigure)
        {
            if (IsOutOfBoard(pos))
                return true;

            Figure figure = _boardService.FiguresPosition[pos.y, pos.x];
            return figure && figure.Color == activeFigure.Color;
        }

        private Vector3 FigurePositionToBoardCoord(BoardPosition pos)
        {
            Vector3 result = new Vector3(pos.x, 0, pos.y) * BoardService.FieldOffset;
            result.y = PossibleMoveYPosition;
            return result;
        }

        private IEnumerable<BoardPosition> GetPossibleMovesWithoutCheckmate(Figure activeFigure, IEnumerable<BoardPosition> figuresMoves)
        {
            List<Figure> enemyFigures = activeFigure.Color == FigureColor.White ? _boardService.BlackFigures : _boardService.WhiteFigures;
            Figure activeKing = activeFigure.Color == FigureColor.White ? _boardService.WhiteKing : _boardService.BlackKing;
            
            return figuresMoves.Where(move =>
            {

                _boardService.FiguresPosition[activeFigure.Position.y, activeFigure.Position.x] = null;
                Figure figureOnPossibleMove = _boardService.FiguresPosition[move.y, move.x];
                _boardService.FiguresPosition[move.y, move.x] = activeFigure;
                
                // TODO figure on possible move will be beaten (find out what I meant))
                enemyFigures = enemyFigures.Where(figure => !figure.WasBeaten).ToList();
                bool willKingBeBeaten = enemyFigures.Any(figure =>
                {
                    IEnumerable<BoardPosition> enemyMoves = GetBoardPositions(figure);
                    return enemyMoves.Any(enemyMove =>
                    {
                        return enemyMove.y == activeKing.Position.y && enemyMove.x == activeKing.Position.x;
                    });
                });

                // TODO make figure on possible move unbeaten (find out what I meant))
                _boardService.FiguresPosition[activeFigure.Position.y, activeFigure.Position.x] = activeFigure;
                _boardService.FiguresPosition[move.y, move.x] = figureOnPossibleMove;

                return !willKingBeBeaten;
            });
        }
    }
}











