using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HealthComponent : MonoBehaviour
{
    public float MaxHealthPoints = 10;
    public float HealthPoints = 10;
    
    private GameObject LastDamageDealer;
    //public ActionToDo dead;
    public Slider Slider;
    public UnityEvent DamageTakenEvent;
    public UnityEvent OnDead;

    private void Start()
    {
        //dead = GetComponent<ActionToDo>();
        HealthPoints = MaxHealthPoints;
    }

    private void Update()
    {
        if (HealthPoints <= 0)
        {
            OnDead.Invoke();
            //dead.Call();
            //do something
        }
    }

    public void TakeDamage(float damageAmount)
    {
        HealthPoints -= damageAmount;

        if (DamageTakenEvent != null)
        {
            DamageTakenEvent.Invoke();    
        }
        
        //print(HealthPoints);
        if (Slider)
        {
            Slider.value = GetPercentige();
        }
    }
    
    public void TakeDamage(float damageAmount, GameObject damageDealer)
    {
        HealthPoints -= damageAmount;
        LastDamageDealer = damageDealer;

        if (DamageTakenEvent != null)
        {
            DamageTakenEvent.Invoke();
        }
        
        //print(HealthPoints);
        if (Slider)
        {
            Slider.value = GetPercentige();
        }
    }

    public float GetPercentige()
    {
        return HealthPoints / MaxHealthPoints;
    }
}
