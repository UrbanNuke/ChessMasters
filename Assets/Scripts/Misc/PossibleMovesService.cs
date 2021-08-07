using System;
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
            List<Vector2Int> result = new List<Vector2Int>(4);
            Vector3 forwardRelVec = activeFigure.transform.forward;
            Vector3 rightRelVec = activeFigure.transform.right;

            Vector2Int forward = activeFigure.Position + Vector3ToVector2Int(forwardRelVec);
            Figure towardsFigure = !IsOutOfBoard(forward)
                ? _boardService.FiguresPosition[forward.x, forward.y]
                : null;
            if (towardsFigure == null && !IsOutOfBoardOrTowardsFriendFigure(forward))
                result.Add(forward);

            Vector2Int forwardTwice = activeFigure.Position + Vector3ToVector2Int(forwardRelVec * 2);
            if (towardsFigure == null && !IsOutOfBoardOrTowardsFriendFigure(forwardTwice))
                result.Add(forwardTwice);

            Vector2Int forwardRight = activeFigure.Position + Vector3ToVector2Int(forwardRelVec + rightRelVec);
            Figure forwardRightFigure = !IsOutOfBoardOrTowardsFriendFigure(forwardRight)
                ? _boardService.FiguresPosition[forwardRight.x, forwardRight.y]
                : null;
            if (forwardRightFigure != null && forwardRightFigure.Color != _boardService.ActiveFigure.Color)
                result.Add(forwardRight);

            Vector2Int forwardLeft = activeFigure.Position + Vector3ToVector2Int(forwardRelVec + rightRelVec * -1);
            Figure forwardLeftFigure = !IsOutOfBoardOrTowardsFriendFigure(forwardLeft)
                ? _boardService.FiguresPosition[forwardLeft.x, forwardLeft.y]
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

            IEnumerable<Vector2Int> result = GetPossibleMovesByDirections(activeFigure, directions);

            return result.Select(FigurePositionToBoardCoord).ToList();
        }

        private IEnumerable<Vector3> GetHorsePossibleMoves(Figure activeFigure)
        {
            List<Vector2Int> result = new List<Vector2Int>(8);
            Vector3 forwardRelVec = activeFigure.transform.forward;
            Vector3 rightRelVec = activeFigure.transform.right;

            Vector2Int forwardRight = activeFigure.Position + Vector3ToVector2Int(forwardRelVec * 2 + rightRelVec);
            Vector2Int forwardLeft = activeFigure.Position + Vector3ToVector2Int(forwardRelVec * 2 + rightRelVec * -1);
            Vector2Int backRight = activeFigure.Position + Vector3ToVector2Int(forwardRelVec * -2 + rightRelVec);
            Vector2Int backLeft = activeFigure.Position + Vector3ToVector2Int(forwardRelVec * -2 + rightRelVec * -1);
            Vector2Int rightBottom = activeFigure.Position + Vector3ToVector2Int(rightRelVec * 2 + forwardRelVec * -1);
            Vector2Int rightTop = activeFigure.Position + Vector3ToVector2Int(rightRelVec * 2 + forwardRelVec);
            Vector2Int leftBottom = activeFigure.Position + Vector3ToVector2Int(rightRelVec * -2 + forwardRelVec * -1);
            Vector2Int leftTop = activeFigure.Position + Vector3ToVector2Int(rightRelVec * -2 + forwardRelVec);
            result.AddRange(new List<Vector2Int>
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

            IEnumerable<Vector2Int> result = GetPossibleMovesByDirections(activeFigure, directions);
    
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

            IEnumerable<Vector2Int> result = GetPossibleMovesByDirections(activeFigure, directions);

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

            IEnumerable<Vector2Int> result = GetPossibleMovesByDirections(activeFigure, directions, 1);

            return result.Select(FigurePositionToBoardCoord).ToList();
        }

        private IEnumerable<Vector2Int> GetPossibleMovesByDirections(Figure activeFigure, IEnumerable<Vector2Int> directions, int howFar = 7)
        {
            List<Vector2Int> result = new List<Vector2Int>(30);
            foreach (Vector2Int direction in directions)
            {
                for (int i = 1; i <= howFar; ++i)
                {
                    Vector2Int move = activeFigure.Position + direction * i;
                    if (IsOutOfBoardOrTowardsFriendFigure(move))
                        break;

                    Figure forwardFigure = _boardService.FiguresPosition[move.x, move.y];
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

        private bool IsOutOfBoard(Vector2Int pos) => pos.x > BoardService.Border || pos.x < 0 || pos.y > BoardService.Border || pos.y < 0;

        private bool IsOutOfBoardOrTowardsFriendFigure(Vector2Int pos)
        {
            if (IsOutOfBoard(pos))
                return true;

            Figure figure = _boardService.FiguresPosition[pos.x, pos.y];
            return figure && figure.Color == _boardService.ActiveFigure.Color;
        }

        private Vector3 FigurePositionToBoardCoord(Vector2Int pos)
        {
            Vector3 result = new Vector3(pos.x, 0, pos.y) * BoardService.FieldOffset;
            result.y = PossibleMoveYPosition;
            return result;
        }

        private Vector2Int Vector3ToVector2Int(Vector3 source)
        {
            return new Vector2Int((int)Math.Round(source.x, 0), (int)Math.Round(source.z, 0));
        }
    }
}