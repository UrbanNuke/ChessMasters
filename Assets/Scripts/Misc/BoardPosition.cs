using System;
using Gameplay;
using UnityEngine;

namespace Misc
{
    public struct BoardPosition
    {
        public int y;
        public int x;

        public BoardPosition(int yPos, int xPos)
        {
            y = yPos;
            x = xPos;
        }

        public static BoardPosition operator +(BoardPosition pos1, BoardPosition pos2)
        {
            return new BoardPosition(pos1.y + pos2.y, pos1.x + pos2.x);
        }

        public static BoardPosition operator +(BoardPosition bPos, Vector2Int vec)
        {
            return new BoardPosition(bPos.y + vec.y, bPos.x + vec.x);
        }

        public static BoardPosition operator +(BoardPosition bPos, Vector3 vec)
        {
            return new BoardPosition(bPos.y + (int)Math.Round(vec.z, 0), bPos.x + (int)Math.Round(vec.x, 0));
        }

        public static BoardPosition FromBoardCoord(Vector3 vec)
        {
            return new BoardPosition(
                (int)(Math.Round(vec.z / BoardService.FieldOffset)),
                (int)(Math.Round(vec.x / BoardService.FieldOffset))
            );
        }
    }
}