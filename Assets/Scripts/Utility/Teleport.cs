using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Transform tpTarget;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            other.attachedRigidbody.position = tpTarget.position;
        }
    }
}
