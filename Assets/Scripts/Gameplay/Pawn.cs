using System;
using Misc;
using UnityEngine;

namespace Gameplay
{
    public class Pawn : Figure
    {
        public bool CanDoubleMove { get; private set; } = true;
        // En Passant - it's hit move, when opposite pawn has double moved, En Passant allows beat pawn would pawn has single moved
        // En Passant is being used only from pawn to pawn
        public bool CanBeBeatenByEnPassant { get; set; } = false;

        public override void Move(Vector3 newPosition)
        {
            CanDoubleMove = false;
            CheckForEnPassantMove(newPosition);
            base.Move(newPosition);
        }

        private void CheckForEnPassantMove(Vector3 newPosition)
        {
            BoardPosition newBoardPosition = BoardPosition.FromBoardCoord(newPosition);

            BoardPosition rightBoardPosition = Position + transform.right;
            BoardPosition forwardBoardPosition = Position + transform.forward;
            Pawn rightPawn = !BoardPosition.IsOutOfBoard(rightBoardPosition)
                ? _boardService.FiguresPosition[rightBoardPosition.y, rightBoardPosition.x] as Pawn
                : null;

            bool isRightEnPassant = rightPawn
                                    && rightPawn.Color != Color
                                    && rightPawn.CanBeBeatenByEnPassant
                                    && newBoardPosition.y == forwardBoardPosition.y && newBoardPosition.x == rightBoardPosition.x;
            if (isRightEnPassant)
                _boardService.BeatFigure(rightPawn);

            BoardPosition leftBoardPosition = Position + (transform.right * -1);
            Pawn leftPawn = !BoardPosition.IsOutOfBoard(leftBoardPosition)
                ? _boardService.FiguresPosition[leftBoardPosition.y, leftBoardPosition.x] as Pawn
                : null;

            bool isLeftEnPassant = leftPawn
                                   && leftPawn.Color != Color
                                   && leftPawn.CanBeBeatenByEnPassant
                                   && newBoardPosition.y == forwardBoardPosition.y && newBoardPosition.x == leftBoardPosition.x;
            if (isLeftEnPassant)
                _boardService.BeatFigure(leftPawn);


            if (Math.Abs(Position.y - newBoardPosition.y) == 2)
                CanBeBeatenByEnPassant = true;
        }
    }
}