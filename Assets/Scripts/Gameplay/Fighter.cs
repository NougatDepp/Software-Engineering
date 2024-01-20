using UnityEngine;

public class Fighter : MonoBehaviour
{
    
    public float hitpoint = 0;
    public int maxHitpoint = 999;
    public float pushRecoverySpeed = 0.2f;
    
    
    protected float immuneTime = 0.1f;
    protected float lastImmune;

    [SerializeField]
    protected Vector2 pushDirection;

    protected virtual void ReceiveDamage(Damage dmg)
    {
        if (Time.time - lastImmune > immuneTime)
        {
            
            lastImmune = Time.time; 
            hitpoint += dmg.damageAmount; 
            pushDirection = (transform.position - dmg.origin).normalized * dmg.pushForce;

            Debug.Log(transform.position - dmg.origin);
            
            if (hitpoint >= 999)
            {
                hitpoint = 999;
            }
            
            GetComponent<Rigidbody2D>().velocity += ((pushDirection+new Vector2(0,0.3f))*hitpoint/3);
        }

    }

    protected virtual void Death()
    {
        
    }
}