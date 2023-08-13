using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static UnityEvent AssignmentCompleted = new UnityEvent();
    public static UnityEvent TimerFinished = new UnityEvent();
        
}