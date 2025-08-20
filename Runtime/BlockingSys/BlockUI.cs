using DSystem;
using UnityEngine;

namespace DSystemUtils.BlockingSys
{
    public class BlockUI : DBehaviour, IBlockingInterfaceListener
    {
        [SerializeField] private int index = -1;
        [SerializeField] private Transform target;
        
        [Inject] private BlockingSystem _blockingSystem;
        
        private Transform NewTarget
        {
            get
            {
                if (_newTempTarget == null)
                    return target.transform;
                
                return _newTempTarget;
            }
            set
            {
                _newTempTarget = value;
            }
        }
        private Transform _newTempTarget;

        public void AddBlock(Transform tr)
        {
            _blockingSystem.AddBlockingObject(GetTr(tr));
        }

        public void RemoveBlock(Transform tr)
        {
            _blockingSystem.RemoveBlockingObject(GetTr(tr));
        }

        public void AddAccept(Transform tr)
        {
            _blockingSystem.AddAcceptedObject(GetTr(tr));
        }

        public void RemoveAccept(Transform tr)
        {
            _blockingSystem.RemoveAcceptedObject(GetTr(tr));
        }
        
        
        public void AddBlock()
        {
            _blockingSystem.AddBlockingObject(GetTr(NewTarget));
        }

        public void RemoveBlock()
        {
            _blockingSystem.RemoveBlockingObject(GetTr(NewTarget));
        }

        public void AddAccept()
        {
            _blockingSystem.AddAcceptedObject(GetTr(NewTarget));
        }

        public void RemoveAccept()
        {
            _blockingSystem.RemoveAcceptedObject(GetTr(NewTarget));
        }
        
        public void SelectChild(int index)
        {
            NewTarget = NewTarget.GetChild(index);
        }

        private Transform GetTr(Transform tr)
        {
            if (index == -1)
                return tr;
            if (tr.childCount <= index)
                return null;
            return tr.GetChild(index);
        }
    }
}