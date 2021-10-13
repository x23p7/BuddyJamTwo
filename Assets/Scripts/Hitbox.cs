using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Hitbox : MonoBehaviour
{
    public float damageAmount = 1f;
    [SerializeField]
    private string destructableTag = "Destructable";
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("hit a collider");
        if (collision.gameObject.CompareTag(destructableTag))
        {
            Debug.Log("hit a destructable");
            Destructable destructable = collision.gameObject.GetComponent<Destructable>();
            destructable.DealDamage(damageAmount);
        }
    }
}
