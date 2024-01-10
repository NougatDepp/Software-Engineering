using UnityEngine;

public class Fighter : MonoBehaviour
{
    
    public float hitpoint = 0;
    public int maxHitpoint = 999;
    public float pushRecoverySpeed = 0.2f;
    
    
    protected float immuneTime = 0.1f;
    protected float lastImmune;

    protected Vector3 pushDirection;

    protected virtual void ReceiveDamage(Damage dmg)
    {
        if (Time.time - lastImmune > immuneTime)
        {
            
            lastImmune = Time.time; 
            hitpoint += dmg.damageAmount; 
            pushDirection = (transform.position - dmg.origin).normalized * dmg.pushForce;

            
            if (hitpoint >= 999)
            {
                hitpoint = 999;
            }
            
            
            
            this.GetComponent<Rigidbody2D>().velocity = (pushDirection*new Vector2(0,1.5f)*hitpoint/10);
            Debug.Log(pushDirection);
        }

    }

    protected virtual void Death()
    {
        
    }
}