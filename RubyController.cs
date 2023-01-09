using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;
    
    public int maxHealth = 100;
    public float timeInvincible = 2.0f;
    
    public int health { get { return currentHealth; }}
    public int currentHealth;

    public float damageAbsorption = 1.0f;
    
    public bool isInvincible;
    float invincibleTimer;
    
    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    Vector2 lookDirection = new Vector2(1,0);

    public GameObject projectilePrefab;

    [SerializeField]
    float rotationSpeed;

    [SerializeField]
    private string newLevel;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);
    }

    void Armor(int maxHealthAdded, float absorptionDecreased)
    {
        maxHealth += maxHealthAdded;
        damageAbsorption -= absorptionDecreased;
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);
        
        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }
        
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            Launch();
        }

        if (move != Vector2.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, move);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                if (hit.collider != null)
                {
                    NPC character = hit.collider.GetComponent<NPC>();
                    if (character != null)
                    {
                        character.DisplayDialog();
                    }  
                }
            }
        }
    }
    
    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);

        if (currentHealth <= 0) {
            Destroy(gameObject);
            SceneManager.LoadScene(newLevel);
        }
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;
            
            isInvincible = true;
            invincibleTimer = timeInvincible;
        }
        
        if (amount > 0) 
        {
            currentHealth = Mathf.Clamp(currentHealth+amount, 0, maxHealth);
        } else 
        {
            currentHealth = Mathf.Clamp(currentHealth+(int)(amount*damageAbsorption), 0, maxHealth);
        }
        HealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }
}