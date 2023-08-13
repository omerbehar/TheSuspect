using UnityEngine;
using UnityEngine.Events;

namespace DefaultNamespace
{
    public class EventManager : MonoBehaviour
    {
        // Create a static UnityEvent
        public static UnityEvent AssignmentCompleted = new UnityEvent();
        
    }
}