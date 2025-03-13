using System.Collections.Generic;
using System.Linq;
using DSystem;
using DSystemUtils.Utils;
using UnityEngine;

namespace DSystemUtils.BlockingSys
{
    public class DInputModule : CustomInputModule
    {
        private readonly DAction<IBlockingInterfaceListener> _blockingInterfaceAction;

        private bool _block;
        
        public DInputModule()
        {
            if (MainInjector.Instance == null)
                return;

            _blockingInterfaceAction = MainInjector.Instance.GetDAction<IBlockingInterfaceListener>();
        }

        protected override void OnObjectChanged(GameObject newObject)
        {
            if (_blockingInterfaceAction == null)
                return;
            
            if (newObject == null)
                return;

            _block = Check(newObject);
        }

        private bool Check(GameObject newObject)
        {
            var acceptedObjects = Enumerable.Empty<Transform>();
            var blockingObjects = Enumerable.Empty<Transform>();
            _blockingInterfaceAction.Invoke(l => l.OnQueryAcceptingList(ref acceptedObjects));
            _blockingInterfaceAction.Invoke(l => l.OnQueryBlockingList(ref blockingObjects));
            
            var tr = newObject.transform;
            bool blockingExist = blockingObjects.Count() != 0;
            bool exceptedExist = acceptedObjects.Count() != 0;
            if (!exceptedExist && !blockingExist)
                return false;
            if (blockingExist && CheckParent(tr, blockingObjects))
                return true;
            if (exceptedExist && CheckParent(tr, acceptedObjects))
                return false;
            return true;
        }

        private bool CheckParent(Transform tr, IEnumerable<Transform> blockingObjects)
        {
            while (tr != null)
            {
                if (blockingObjects.Contains(tr))
                    return true;
                tr = tr.parent;
            }
            return false;
        }

        protected override bool OnCheckBlock()
        {
            bool blocked = base.OnCheckBlock();
            blocked |= _block;
            return blocked;
        }
    }
}