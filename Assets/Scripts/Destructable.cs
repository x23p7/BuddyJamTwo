using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
    [SerializeField]
    private float health = 3;

    public void DealDamage(float amount)
    {
        this.health -= amount;
        if (this.health <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
