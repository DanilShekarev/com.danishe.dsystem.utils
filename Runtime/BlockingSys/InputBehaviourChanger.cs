using UnityEngine;

namespace DSystemUtils.BlockingSys
{
    public class InputBehaviourChanger : MonoBehaviour
    {
        public InputBehaviourState State => state;

        [SerializeField] private InputBehaviourState state;

        public void ChangeState(InputBehaviourState st)
        {
            state = st;
        }
    }
    
    public enum InputBehaviourState
    {
        None, ForceInteractable, ForceBlock
    }
}