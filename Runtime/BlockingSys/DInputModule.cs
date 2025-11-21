using System.Collections.Generic;
using System.Linq;
using DSystem;
using DSystemUtils.Utils;
using UnityEngine;

namespace DSystemUtils.BlockingSys
{
    public class DInputModule : CustomInputModule
    {
        [SerializeField] private Transform[] alwaysAcceptingObjects;
        [SerializeField] private string[] alwaysAcceptingObjectNames =
        {
            "ConsentForm(Clone)", "Interstitial(Clone)"
        };
        
        private readonly DAction<IBlockingInterfaceListener> _blockingInterfaceAction;
        private readonly DAction<IBlockingSystemChecker> _blockingCheckers;

        private bool _block;
        
        public DInputModule()
        {
            if (DEventSystem.Instance == null)
                return;

            _blockingInterfaceAction = DEventSystem.Instance.GetDAction<IBlockingInterfaceListener>();
            _blockingCheckers = DEventSystem.Instance.GetDAction<IBlockingSystemChecker>();
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
            
            var beh = CheckBehaviour(tr);
            if (beh == InputBehaviourState.ForceInteractable)
                return false;
            if (beh == InputBehaviourState.ForceBlock)
                return true;
            
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
        
        private InputBehaviourState CheckBehaviour(Transform tr)
        {
            var lastBeh = InputBehaviourState.None;
            while (tr != null)
            {
                if (alwaysAcceptingObjects.Contains(tr))
                    return InputBehaviourState.ForceInteractable;
                if (alwaysAcceptingObjectNames.Contains(tr.gameObject.name))
                    return InputBehaviourState.ForceInteractable;
                if (tr.gameObject.TryGetComponent(out InputBehaviourChanger changer))
                    if (changer.enabled && changer.State != InputBehaviourState.None)
                        lastBeh = changer.State;
                InputBehaviourState checkState = InputBehaviourState.None;
                _blockingCheckers.Invoke(l => l.CheckObject(tr, ref checkState));
                if (checkState != InputBehaviourState.None)
                    lastBeh = checkState;
                
                tr = tr.parent;
            }
            return lastBeh;
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