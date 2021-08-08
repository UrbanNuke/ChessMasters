using System.Collections.Generic;
using System.Linq;
using Gameplay;
using UnityEngine;

namespace Misc
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
            return activeFigure.Type switch
            {
                FigureType.Pawn => GetPawnPossibleMoves(activeFigure),
                FigureType.Tower => GetTowerPossibleMoves(activeFigure),
                FigureType.Horse => GetHorsePossibleMoves(activeFigure),
                FigureType.Bishop => GetBishopPossibleMoves(activeFigure),
                FigureType.Queen => GetQueenPossibleMoves(activeFigure),
                FigureType.King => GetKingPossibleMoves(activeFigure),
                _ => null
            };
        }

        // TODO add extra strike move when enemy pawn jump 2 field

        private IEnumerable<Vector3> GetPawnPossibleMoves(Figure activeFigure)
        {
            List<BoardPosition> result = new List<BoardPosition>(4);
            Vector3 forwardRelVec = activeFigure.transform.forward;
            Vector3 rightRelVec = activeFigure.transform.right;

            BoardPosition forward = activeFigure.Position + forwardRelVec;
            Figure towardsFigure = !IsOutOfBoard(forward)
                ? _boardService.FiguresPosition[forward.y, forward.x]
                : null;
            if (towardsFigure == null && !IsOutOfBoardOrTowardsFriendFigure(forward))
                result.Add(forward);

            BoardPosition forwardTwice = activeFigure.Position + forwardRelVec * 2;
            if (towardsFigure == null && !IsOutOfBoardOrTowardsFriendFigure(forwardTwice))
                result.Add(forwardTwice);

            BoardPosition forwardRight = activeFigure.Position + (forwardRelVec + rightRelVec);
            Figure forwardRightFigure = !IsOutOfBoardOrTowardsFriendFigure(forwardRight)
                ? _boardService.FiguresPosition[forwardRight.y, forwardRight.x]
                : null;
            if (forwardRightFigure != null && forwardRightFigure.Color != _boardService.ActiveFigure.Color)
                result.Add(forwardRight);

            BoardPosition forwardLeft = activeFigure.Position + (forwardRelVec + rightRelVec * -1);
            Figure forwardLeftFigure = !IsOutOfBoardOrTowardsFriendFigure(forwardLeft)
                ? _boardService.FiguresPosition[forwardLeft.y, forwardLeft.x]
                : null;
            if (forwardLeftFigure != null && forwardLeftFigure.Color != _boardService.ActiveFigure.Color)
                result.Add(forwardLeft);

            return result.Select(FigurePositionToBoardCoord).ToList();
        }

        private IEnumerable<Vector3> GetTowerPossibleMoves(Figure activeFigure)
        {
            Vector2Int[] directions = new Vector2Int[]
            {
                Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
            };

            IEnumerable<BoardPosition> result = GetPossibleMovesByDirections(activeFigure, directions);

            return result.Select(FigurePositionToBoardCoord).ToList();
        }

        private IEnumerable<Vector3> GetHorsePossibleMoves(Figure activeFigure)
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

            result = result.Where(move => !IsOutOfBoardOrTowardsFriendFigure(move)).ToList();
            return result.Select(FigurePositionToBoardCoord).ToList();
        }

        private IEnumerable<Vector3> GetBishopPossibleMoves(Figure activeFigure)
        {
            Vector2Int[] directions = new Vector2Int[]
            {
                Vector2Int.up + Vector2Int.right, Vector2Int.up + Vector2Int.left, 
                Vector2Int.down + Vector2Int.right, Vector2Int.down + Vector2Int.left,
            };

            IEnumerable<BoardPosition> result = GetPossibleMovesByDirections(activeFigure, directions);
    
            return result.Select(FigurePositionToBoardCoord).ToList();
        }

        private IEnumerable<Vector3> GetQueenPossibleMoves(Figure activeFigure)
        {
            Vector2Int[] directions = new Vector2Int[]
            {
                Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
                Vector2Int.up + Vector2Int.right, Vector2Int.up + Vector2Int.left, 
                Vector2Int.down + Vector2Int.right, Vector2Int.down + Vector2Int.left,
            };

            IEnumerable<BoardPosition> result = GetPossibleMovesByDirections(activeFigure, directions);

            return result.Select(FigurePositionToBoardCoord).ToList();
        }

        private IEnumerable<Vector3> GetKingPossibleMoves(Figure activeFigure)
        {
            Vector2Int[] directions = new Vector2Int[]
            {
                Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
                Vector2Int.up + Vector2Int.right, Vector2Int.up + Vector2Int.left, 
                Vector2Int.down + Vector2Int.right, Vector2Int.down + Vector2Int.left,
            };

            IEnumerable<BoardPosition> result = GetPossibleMovesByDirections(activeFigure, directions, 1);

            return result.Select(FigurePositionToBoardCoord).ToList();
        }

        private IEnumerable<BoardPosition> GetPossibleMovesByDirections(Figure activeFigure, IEnumerable<Vector2Int> directions, int howFar = 7)
        {
            List<BoardPosition> result = new List<BoardPosition>(30);
            foreach (Vector2Int direction in directions)
            {
                for (int i = 1; i <= howFar; ++i)
                {
                    BoardPosition move = activeFigure.Position + direction * i;
                    if (IsOutOfBoardOrTowardsFriendFigure(move))
                        break;

                    Figure forwardFigure = _boardService.FiguresPosition[move.y, move.x];
                    if (!forwardFigure)
                    {
                        result.Add(move);
                        continue;
                    }

                    if (forwardFigure.Color == _boardService.ActiveFigure.Color) continue;
                    result.Add(move);
                    break;
                }
            }

            return result;
        }

        private bool IsOutOfBoard(BoardPosition pos) => pos.x > BoardService.Border || pos.x < 0 || pos.y > BoardService.Border || pos.y < 0;

        private bool IsOutOfBoardOrTowardsFriendFigure(BoardPosition pos)
        {
            if (IsOutOfBoard(pos))
                return true;

            Figure figure = _boardService.FiguresPosition[pos.y, pos.x];
            return figure && figure.Color == _boardService.ActiveFigure.Color;
        }

        private Vector3 FigurePositionToBoardCoord(BoardPosition pos)
        {
            Vector3 result = new Vector3(pos.x, 0, pos.y) * BoardService.FieldOffset;
            result.y = PossibleMoveYPosition;
            return result;
        }
    }
}