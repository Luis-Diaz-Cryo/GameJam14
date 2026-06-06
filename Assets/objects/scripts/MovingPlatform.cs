using UnityEngine;

public class MovingPlatform : MonoBehaviour, canHammer
{
    [Header("Movement")]
    [SerializeField] private float moveDistance = 3f;
    [SerializeField] private float moveSpeed = 2f;

    private Vector3 startPosition;
    private Vector3 topPosition;
    private bool isActivated = false;

    private void Start()
    {
        startPosition = transform.position;
        topPosition = startPosition + Vector3.up * moveDistance;
    }

    private void Update()
    {
        if (!isActivated) return;

        float movement = Mathf.PingPong(Time.time * moveSpeed, 1f);

        transform.position = Vector3.Lerp(startPosition, topPosition, movement);
    }

    public void Interact()
    {
        isActivated = true;
        Debug.Log("Platform activated");
    }

    public bool CanInteract()
    {
        return !isActivated;
    }
}