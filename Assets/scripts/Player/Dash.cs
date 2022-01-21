using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Dash : Ability
{
    public bool canDash = true;
    public float DashForce = 100f;
    public float DashDuration = 1.2f;
    public bool isDashing = false;

    public Slider Slider;
    private Rigidbody rigidbody;
    private Vector3 savedVelocity;
    private Transform trans;

    private float lastDashTime = 0;
    public float DashCooldown = 5;
    public float currentColdown = 0;

    public float CurrentPercentige()
    {
        return currentColdown / DashCooldown;
    }
    
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        trans = Camera.main.transform;
    }

    private void Start()
    {
        lastDashTime = Time.time;
    }

    public void Update()
    {
        currentColdown += Time.deltaTime;

        Slider.value = CurrentPercentige();
        
        if (abilityInput && canDash)
        {
            float time = Time.time;
          
            if (time - lastDashTime > DashCooldown)
            {
                lastDashTime = time;
                StartCoroutine(Cast());
            }
        }
    }

    public IEnumerator Cast()
    {
        //print("start Dash");
        isDashing = true;
        gameObject.GetComponent<PlayerMovement>().ResetDownwardsVel();
        savedVelocity = rigidbody.velocity;
        
        rigidbody.AddForce(trans.forward * DashForce, ForceMode.Impulse);

        yield return new WaitForSeconds(DashDuration);
        isDashing = false;
        rigidbody.velocity = savedVelocity;
        currentColdown  = 0;
        //yield return null;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!isDashing)
        {
            return;
        }
        
        HealthComponent health = other.gameObject.GetComponent<HealthComponent>();

        if (health)
        {
            health.TakeDamage(100);
        }
    }
}
