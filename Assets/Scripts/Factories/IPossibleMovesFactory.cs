using UnityEngine;

namespace Factories
{
    public interface IPossibleMovesFactory
    {
        public void Load();
        public void Create(Vector3 at, Quaternion rotation, Transform parent);
    }
}