using Misc;
using UnityEngine;

namespace Factories
{
    public interface IFigureFactory
    {
        public void Load();
        public void Create(FigureType type, FigureColor color, BoardPosition position, Vector3 at, Quaternion rotation, Transform parent);
    }
}