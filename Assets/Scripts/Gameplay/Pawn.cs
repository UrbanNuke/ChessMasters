using UnityEngine;

namespace Gameplay
{
    public class Pawn : Figure
    {
        public bool CanDoubleMove { get; private set; } = true;

        public override void Move(Vector3 newPosition)
        {
            CanDoubleMove = false;
            base.Move(newPosition);
        }
    }
}