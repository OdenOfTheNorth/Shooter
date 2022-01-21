using UnityEngine;
using UnityEngine.Events;

public class Bomb : MonoBehaviour
{
    public GameObject parent;
    public UnityEvent Explode;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject != parent)
        {
            //Debug.Break();
            print(other.transform.name);
            Explode.Invoke();
        }
    }
}
