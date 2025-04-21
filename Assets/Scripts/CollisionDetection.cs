using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    public string fingerName;
    private HandCollisionManager manager;
    private float exitCooldown = 0.2f; // time to wait before confirming exit
    private float exitTimer = 0f;
    private bool isTouching = false;

    void Start() {
        manager = FindFirstObjectByType<HandCollisionManager>();
    }

    void Update() {
        if (!isTouching && exitTimer > 0f) {
            exitTimer -= Time.deltaTime;
            if (exitTimer <= 0f) 
                manager?.ReportCollision(fingerName, isTouching);
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (!isTouching) {
            isTouching = true;
            manager?.ReportCollision(fingerName, isTouching);
        }

        exitTimer = 0f;
    }

    void OnCollisionExit(Collision other) {
        isTouching = false;
        exitTimer = exitCooldown;
    }
}
