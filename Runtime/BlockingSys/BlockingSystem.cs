using System.Collections.Generic;
using System.Linq;
using DSystem;
using JetBrains.Annotations;
using UnityEngine;

namespace DSystemUtils.BlockingSys
{
    [AutoRegistry, UsedImplicitly]
    public class BlockingSystem : DSystemBase, IBlockingInterfaceListener
    {
        private readonly List<Transform> _acceptedObjects = new ();
        private readonly List<Transform> _blockingObjects = new ();

        public void AddAcceptedObject(Transform acceptedObject)
        {
            _acceptedObjects.Add(acceptedObject);
        }

        public void RemoveAcceptedObject(Transform acceptedObject)
        {
            _acceptedObjects.Remove(acceptedObject);
        }
        
        public void AddBlockingObject(Transform blockingObject)
        {
            _blockingObjects.Add(blockingObject);
        }

        public void RemoveBlockingObject(Transform blockingObject)
        {
            _blockingObjects.Remove(blockingObject);
        }

        public void OnQueryAcceptingList(ref IEnumerable<Transform> acceptingObjects)
        {
            acceptingObjects = acceptingObjects.Concat(_acceptedObjects);
        }

        public void OnQueryBlockingList(ref IEnumerable<Transform> blockingObjects)
        {
            blockingObjects = blockingObjects.Concat(_blockingObjects);
        }
    }
}