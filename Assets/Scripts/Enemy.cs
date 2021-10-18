using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class MovementPair
{
    public GameObject movementGO;
    public GameObject waypointParent;
    public float[] sectionTimings;
    public float[] sectioPauses;
}
public class MovementPairCR
{
    public MovementPair movementPair;
    public IEnumerator movementCR;
}
public class Enemy : MonoBehaviour
{
    [SerializeField]
    private MovementPair[] movementPairs;
    private List<MovementPairCR> movementPairCRs = new List<MovementPairCR>();
    [SerializeField]
    private bool moveAtStart = false;
    [SerializeField]
    private float standardDuration = 9999f;
    private void Start()
    {
        if (moveAtStart)
        {
            foreach (MovementPair movementPair in movementPairs)
            {
                StartCoroutine(StartMovement(movementPair, standardDuration));
            }
        }
    }

    private IEnumerator StartMovement(MovementPair targetMovementPair, float duration)
    {
        Rigidbody2D targetRig = targetMovementPair.movementGO.GetComponent<Rigidbody2D>();
        Transform targetTrans = targetMovementPair.movementGO.transform;
        Transform lastAnchor = targetMovementPair.waypointParent.transform.GetChild(0);
        Transform nextAnchor = targetMovementPair.waypointParent.transform.GetChild(1);
        int currentAnchorID = 0;
        float fulltimer = 0;
        while (fulltimer < duration)
        {
            float timer = 0;
            float targetTime = targetMovementPair.sectionTimings[currentAnchorID];
            while (timer < targetTime)
            {
                fulltimer += Time.deltaTime;
                if (fulltimer >= duration)
                {
                    break;
                }
                timer += Time.deltaTime;
                if (timer > targetTime)
                {
                    timer = targetTime;
                }
                targetRig.MovePosition(lastAnchor.position + (nextAnchor.position - lastAnchor.position) * (timer / targetTime));
                yield return new WaitForFixedUpdate();
            }
            currentAnchorID++;
            lastAnchor = nextAnchor;
            if (currentAnchorID > targetMovementPair.waypointParent.transform.childCount - 1)
            {
                currentAnchorID = 0;
                nextAnchor = targetMovementPair.waypointParent.transform.GetChild(0);
            }
            else
            {
                nextAnchor = targetMovementPair.waypointParent.transform.GetChild(currentAnchorID);
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
