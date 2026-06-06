using System.Collections;
using UnityEngine;

public class ExplosionController : MonoBehaviour, canHammer
{
    [Header("Config")]
    [SerializeField] private GameObject objectToActivate;
    [SerializeField] private bool canBeHammered = false;

    [Header("This Object Animation")]
    [SerializeField] private string explosionTrigger = "Explosion";

    [Header("Other Object Animation")]
    [SerializeField] private string otherObjectTrigger = "Activate";

    [Header("Other Object Collider Settings")]
    [SerializeField] private float colliderChangeDelay = 0.5f;
    [SerializeField] private Vector2 otherNewColliderSize = new Vector2(1f, 1f);
    [SerializeField] private Vector2 otherNewColliderOffset = Vector2.zero;

    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public bool CanInteract()
    {
        return canBeHammered;
    }

    public void Interact()
    {
        Debug.Log("Explosion triggered!");

        if (anim != null)
        {
            anim.SetTrigger(explosionTrigger);
        }

        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);

            Animator otherAnim = objectToActivate.GetComponent<Animator>();

            if (otherAnim != null)
            {
                otherAnim.SetTrigger(otherObjectTrigger);
            }

            StartCoroutine(ChangeOtherColliderAfterDelay());
        }
    }

    private IEnumerator ChangeOtherColliderAfterDelay()
    {
        yield return new WaitForSeconds(colliderChangeDelay);

        BoxCollider2D otherCollider = objectToActivate.GetComponent<BoxCollider2D>();
        Rigidbody2D otherRigid = objectToActivate.GetComponent<Rigidbody2D>();

        if (otherCollider != null)
        {
            otherCollider.size = otherNewColliderSize;
            otherCollider.offset = otherNewColliderOffset;
            Debug.Log("Other object's collider changed");
        }
        else
        {
            Debug.LogWarning(objectToActivate.name + " does not have a BoxCollider2D.");
        }

        if (otherRigid != null)
        {
            otherRigid.bodyType = RigidbodyType2D.Dynamic;
            Debug.Log("Other object's Rigidbody2D is now Dynamic");
        }
        else
        {
            Debug.LogWarning(objectToActivate.name + " does not have a Rigidbody2D.");
        }
    }
}