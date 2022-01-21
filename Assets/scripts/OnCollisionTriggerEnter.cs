using UnityEngine;
using UnityEngine.Events;

public class OnCollisionTriggerEnter : MonoBehaviour
{
    public UnityEvent eventAction;
    private Collider _collider;
    //public bool OnTrigger = false;
    //public ActionToDo action;

    private void Start()
    {
        _collider = GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (_collider.isTrigger)
        {
            return;
        }
        
        eventAction.Invoke();    
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_collider.isTrigger)
        {
            return;
        }
        
        eventAction.Invoke();     
    }
}
