using UnityEngine;

public class ExplosionController : MonoBehaviour, canHammer
{

    [Header("Config")]
    [SerializeField] private GameObject objectToActivate;
    [SerializeField] public bool canHammer = false;

    public bool CanInteract()
    {
        return canHammer;
    }

    public void Interact()
    {
        objectToActivate.SetActive(true); // Aqui se puede cambiar por un "instanciate" de una grita (O no, depende de lo que se quiera hacer)
        Debug.Log("Explosion triggered!");
        Destroy(gameObject);
    }
}