using UnityEngine;

public class PhysicsObjects : MonoBehaviour
{
    private Rigidbody2D objectRigidbody2D;

    // Start is called before the first frame update
    private void Start()
    {
        objectRigidbody2D = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float velocityVsUp = Vector2.Dot(transform.up, objectRigidbody2D.velocity);
        if (velocityVsUp > 0)
        {
            objectRigidbody2D.drag = Mathf.Lerp(objectRigidbody2D.drag, 3.0f, Time.fixedDeltaTime * 3);
        }
        else
        {
            objectRigidbody2D.drag = 0;
        }
    }
}
