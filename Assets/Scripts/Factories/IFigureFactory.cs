using Misc;
using UnityEngine;

namespace Factories
{
    public interface IFigureFactory
    {
        public void Load();
        public void Create(FigureType type, FigureColor color, Vector2 position, Vector3 at, Quaternion rotation, Transform parent);
    }
}