using Unity.VisualScripting;
using UnityEngine;

public class FallingPlatform : MonoBehaviour,canHammer
{
    
    public Rigidbody2D rb;
    private bool qwerty= false; //no supe como nombrar este
    public GameObject boss;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if (rb.linearVelocityY<=-5)
        {
            rb.linearVelocityY=-5;
        }
    }
    public void Interact()
    {
        if(qwerty)
        {
            //boss.GetComponent<BossController>().hurt();
            //que le baja la "vida" al jefe, luego miro como lo hago
            print("golpe");
            return;
        }
        if(rb.bodyType == RigidbodyType2D.Dynamic)
        {
            print("estatico");
            rb.bodyType = RigidbodyType2D.Static;
        }
        else
        {
            print("dinamico");
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Debug.Log("Player kill");
            //collision.gameObject.GetComponent<PlayerController>().KillPlayer(); //no se nombres de cosas asi que lo puse asi 
        }
        // else if(collision.CompareTag("Button"))
        // {
        //     Debug.Log("Button Press");
        //     //collision.gameObject.GetComponent<ButtonController>().interact();
        // }
        else if(collision.CompareTag("Boss"))
        {
            qwerty=true;
        }
    }

    bool canHammer.CanInteract()
    {
        return true;
    }
}