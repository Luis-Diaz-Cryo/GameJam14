using UnityEngine;

public class FallingDebree : MonoBehaviour,canHammer
{
      
    public Rigidbody2D rb;

    bool canHammer.CanInteract()
    {
        throw new System.NotImplementedException();
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
    }
}