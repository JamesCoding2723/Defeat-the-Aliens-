using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    public float speed;

    Rigidbody2D rigidbody2D;

    public Transform target;
    Vector2 moveDirection;

    public int health;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        target = GameObject.Find("Chad").transform;
        health = 25;
    }

    public void Hit(int damage)
    {
        health -= damage;
    }

    void Update()
    {
        if (target)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rigidbody2D.rotation = angle + 90;
            moveDirection = direction;
        }

        if (health <= 0) 
        {
            Destroy(gameObject);
        }
    }
    
    void FixedUpdate()
    {
        if (target) 
        {
            rigidbody2D.velocity = new Vector2(moveDirection.x, moveDirection.y) * speed;
        }

        if (health <= 0) 
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        RubyController player = other.gameObject.GetComponent<RubyController >();

        if (player != null)
        {
            player.ChangeHealth(-20);
        }
    }
}
