using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EzrealAbilityQProjectile : MonoBehaviour
{
    public float speed = 10f; // The speed at which the projectile moves
    public float maxDistance = 100f; // Maximum distance the projectile can travel before being destroyed
    public LayerMask hitLayers; // Determines which layers the projectile can hit
    public float damage; // The amount of damage the projectile deals

    private Vector3 startPosition; // The starting position of the projectile

    void Start()
    {
        startPosition = transform.position; // Store the starting position of the projectile

        // Ensure the projectile has a Rigidbody component
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Make Rigidbody kinematic for precise movement control
        }
    }

    void Update()
    {
        MoveProjectile(); // Move the projectile forward
        CheckDistanceTravelled(); // Check if the projectile has reached its max distance
    }

    // Moves the projectile forward based on its speed
    private void MoveProjectile()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    // Checks if the projectile has traveled its maximum distance and destroys it if so
    private void CheckDistanceTravelled()
    {
        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            Destroy(gameObject); // Destroy the projectile
        }
    }

    // Handles collision with other objects using triggers
    void OnTriggerEnter(Collider other)
    {
        // Check if the collided object is in the specified hitLayers
        if (((1 << other.gameObject.layer) & hitLayers) != 0)
        {
            // Attempt to get the Stats component from the collided object and apply damage
            Stats targetStats = other.gameObject.GetComponent<Stats>();
            if (targetStats != null)
            {
                targetStats.TakeDamage(other.gameObject, damage); // Apply damage
            }

            Destroy(gameObject); // Destroy the projectile upon hitting a valid target
        }
        if (((1 << other.gameObject.layer) & hitLayers) != 0)
        {
            // Attempt to get the Stats component from the collided object and apply damage
            ObjectiveStats targetStats2 = other.gameObject.GetComponent<ObjectiveStats>();
            if (targetStats2 != null)
            {
                targetStats2.TakeDamage(damage); // Apply damage
            }

            Destroy(gameObject); // Destroy the projectile upon hitting a valid target
        }
    }
}
