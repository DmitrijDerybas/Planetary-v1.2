using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Moves the missile and attracts it towards the planets and the Sun.
public class Missile : MonoBehaviour
{
    //Owner of the missile.
    public Planet owner;

    public MissileType type;
    public Rigidbody2D rbody;
    public CapsuleCollider2D coll;

    public float moveSpeed = 2f;
    public float damage = 10f;
    public float reloadTime = 2f;

    float lifeTime = 0;
    const float maxLifeTime = 5f;

    //A planet that attracts the missile.
    Planet attractor;
    Vector2 velocity;
    float angle;

    private void OnEnable()
    {
        //Fires the missile with the moveSpeed.
        rbody.AddForce(moveSpeed * transform.right);
    }

    //When the missile hits the planet/sun or runs out of life timer.
    public void MissileExplode()
    {
        owner.controller.effect.CreateEffectInPos(EffectType.MissileExplode, transform.position, Quaternion.identity);

        gameObject.SetActive(false);
        lifeTime = 0;
        rbody.velocity = Vector2.zero;
        owner = null;

        attractor = null;
    }

    void Update()
    {
        if (owner.controller.isPaused)
            return;

        lifeTime += Time.deltaTime;
        if (lifeTime > maxLifeTime)
            MissileExplode();

        //Rotates the missile towards it's velocity.
        velocity = rbody.velocity;
        angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        //If there is a planet nearby - attract the missile to the planet/Sun.
        if(attractor != null && attractor != owner)
        {
            Vector2 dir = attractor.transform.position - transform.position;
            dir.Normalize();

            rbody.AddForce(dir * attractor.rbody.mass, ForceMode2D.Force);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(attractor != null && attractor == collision.transform)
            attractor = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.isTrigger)
        {
            if(collision.tag == "Planet")
                collision.GetComponent<Planet>().Hit(damage);
            MissileExplode();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.isTrigger && collision.tag == "Planet")
            attractor = collision.GetComponent<Planet>();
    }
}
