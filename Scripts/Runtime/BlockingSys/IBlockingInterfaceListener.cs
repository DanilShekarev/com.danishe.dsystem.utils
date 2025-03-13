using System.Collections.Generic;
using DSystem;
using UnityEngine;

namespace DSystemUtils.BlockingSys
{
    [Listener]
    public interface IBlockingInterfaceListener
    {
        public void OnQueryAcceptingList(ref IEnumerable<Transform> acceptingObjects) {}

        public void OnQueryBlockingList(ref IEnumerable<Transform> blockingObjects) {}
    }
}