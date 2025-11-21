using DSystem;
using UnityEngine;

namespace DSystemUtils.BlockingSys
{
    [Listener]
    public interface IBlockingSystemChecker
    {
        void CheckObject(Transform transform, ref InputBehaviourState state);
    }
}