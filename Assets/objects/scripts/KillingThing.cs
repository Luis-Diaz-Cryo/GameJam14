using Unity.Collections;
using UnityEngine;

public class KillingThing : MonoBehaviour,canHammer
{
      
    public Rigidbody2D rb;

    bool canHammer.CanInteract()
    {
        return false;
    }

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }
    public void Interact()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Debug.Log("Player kill");
            //collision.gameObject.GetComponent<PlayerController>().KillPlayer(); //no se nombres de cosas asi que lo puse asi 
        }
        if(collision.CompareTag("presser"))
        {
            //Debug.Log("plataforma devuelta");
            Vector3 x = collision.gameObject.transform.position;
            collision.gameObject.transform.position = new Vector3(x.x,13f,0f);
        }
        
    }
}