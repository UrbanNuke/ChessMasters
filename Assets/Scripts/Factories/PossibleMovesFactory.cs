using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Factories
{
    public class PossibleMovesFactory : IPossibleMovesFactory
    {
        private readonly IInstantiator _instantiator;
        private Object _highlightMovePrefab;

        private PossibleMovesFactory(IInstantiator instantiator)
        {
            _instantiator = instantiator;
        }
        
        public void Load()
        {
            _highlightMovePrefab = Resources.Load("Objects/I_PossibleMove");
        }

        public void Create(Vector3 at, Quaternion rotation, Transform parent)
        {
            _instantiator.InstantiatePrefab(_highlightMovePrefab, at, rotation, parent);
        }
    }
}