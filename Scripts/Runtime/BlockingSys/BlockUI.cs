using DSystem;
using UnityEngine;

namespace DSystemUtils.BlockingSys
{
    public class BlockUI : DBehaviour, IBlockingInterfaceListener
    {
        [SerializeField] private int index = -1;
        
        [Inject] private BlockingSystem _blockingSystem;

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