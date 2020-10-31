using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAttack : MonoBehaviour
{

    // Variable Initizaliation
    public GameObject attackTargeter;
    public GameObject arrowPrefab;
    public GameObject fireBoltPrefab;
    public GameObject lightningBoltPrefab;
    public int maxArrowSpeed = 5;
    public int arrowSpeedMultiplier = 1;
    public int fireBoltSpeed = 5;
    public int lightningBoltSpeed = 5;

    // Private Variables
    private bool canFire = true;
    private float arrowAttackCooldownTimer = .5f;
    private float magicAttackCooldownTimer = 1f;
    private bool isMagic = false;
    private int magicSpeedMultiplier = 1;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && canFire)
        {

            createGameObject(arrowPrefab, maxArrowSpeed, arrowSpeedMultiplier, isMagic);
            StartCoroutine(attackCooldown(arrowAttackCooldownTimer));
        }
        if (Input.GetKeyDown(KeyCode.Alpha1) && canFire)
        {
            isMagic = true;
            createGameObject(fireBoltPrefab, fireBoltSpeed, 1, isMagic);
            StartCoroutine(attackCooldown(magicAttackCooldownTimer));
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && canFire)
        {
            Instantiate(lightningBoltPrefab);
        }
    }

    private void createGameObject(GameObject prefab, int maxSpeed, int speedMultiplier, bool isMagicAttack)
	{
        // Creates a new game object
        Vector2 start = transform.position;
        GameObject obj = (GameObject)Instantiate(prefab, start, transform.rotation);

        // Rotates the projectile to be the correct orientation
        obj.transform.right = attackTargeter.GetComponent<Transform>().position - obj.transform.position;

        Vector3 newVelocity = new Vector3(1000,1000,1000);

        // Checks the projectile new velocity
        newVelocity = attackTargeter.GetComponent<Transform>().position - obj.GetComponent<Transform>().position;

        // Makes sure the projectile isn't going faster than max arrow speed
        float speed = newVelocity.magnitude;
        if (speed > maxSpeed || isMagic)
        {
            newVelocity = newVelocity.normalized * maxSpeed;
        }

        // Gives the projectile the velocity
        obj.GetComponent<Rigidbody2D>().velocity = (newVelocity) * speedMultiplier;
    }

    // Puts a limit on firing
    IEnumerator attackCooldown(float seconds)
    {
        canFire = false;
        yield return new WaitForSeconds(seconds);
        canFire = true;
    }
}
