 using UnityEngine;

public class Fighter : MonoBehaviour
{
    
    public float hitpoint = 0;

    protected float immuneTime = 0.1f;
    protected float lastImmune;
    protected Vector2 pushDirection;

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
            
            GetComponent<Rigidbody2D>().velocity += ((pushDirection+new Vector2(0,0.3f))*hitpoint/3);
        }

    }
}