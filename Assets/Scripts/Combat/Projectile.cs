using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Projectile : MonoBehaviour
{
    private int damage;
    private float speed;
    private float range;

    private Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    public void SetValues(int damage, float speed, float range)
    {
        this.damage = damage;
        this.speed = speed;
        this.range = range;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        if (Vector3.Distance(startPos, transform.position) > range)
            Destroy(gameObject);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Projectile")
            return;

        CombatAI combatAI = other.gameObject.GetComponent<CombatAI>();
        if (combatAI != null)
        {
            combatAI.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
