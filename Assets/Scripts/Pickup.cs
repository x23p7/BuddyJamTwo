using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pickup : MonoBehaviour
{
    public UnityEvent onPickupEvent;
    public GameObject pickingEntity;
    public void HealPlayer(float amount)
    {
        if (amount == 42)
        {
            pickingEntity.GetComponent<SpriteRenderer>().color = Color.green;
        }
    }
}
