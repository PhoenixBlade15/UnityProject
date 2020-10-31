using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	// Public variables for the projectile
    public bool isPlayerProjectile = true;
    public int damage = 10;

	// Private variables for the projectile
    private BoxCollider2D thisCollider;
    private SpriteRenderer thisRenderer;
	private float lifeTime = 10;
	private float liftTimeAfterHit = 2;

	private void Start()
	{
		// Sets the collider and renderer for later use
        thisCollider = this.GetComponent<BoxCollider2D>();
        thisRenderer = this.GetComponent<SpriteRenderer>();

		// Delete the projectile after certain amount of time
		StartCoroutine(DestroyThisProjectile(lifeTime));
	}

	// When the arrow touches something
	private void OnTriggerEnter2D(Collider2D collision)
	{
		string tagOfCollision = collision.gameObject.tag;
		int layerOfCollision = collision.gameObject.layer;

		// Makes sure a projectile isn't colliding with a projectile
		if (gameObject.layer != layerOfCollision)
		{

			// Checks if the tag is player or not, and if tag is null output an error
			if (tagOfCollision == "Player" && !isPlayerProjectile)
			{
				// Damage the player


			}
			else if (tagOfCollision != "Player" && isPlayerProjectile)
			{

				// If an enemy is hurt hurt them
				if (tagOfCollision == "Enemy")
				{
					// Damage Enemy
				}

				// Turn off the renderer and collider so they can't be used anymore
				thisCollider.enabled = false;
				thisRenderer.enabled = false;

				// Destroys after hitting something
				StartCoroutine(DestroyThisProjectile(liftTimeAfterHit));

			}
			else
			{
				Debug.LogError("Error in Projectile.OnCollisionEnter2D.");
			}

		}

	}

	// Destroys the projectile after X amount of seconds
	IEnumerator DestroyThisProjectile(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		Destroy(gameObject);
	}


}
