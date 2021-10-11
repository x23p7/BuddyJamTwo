using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera targetCam;
    [Range(0, 100f)]
    public float bottomLimit = 5;
    [Range(0, 100f)]
    public float leftLimit = 5;
    [Range(0, 100f)]
    public float topLimit = 5;
    [Range(0, 100f)]
    public float rightLimit = 5;
    public GameObject target;
    [Range(0, 5f), SerializeField]
    private float followLerpSpeed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        if (!targetCam)
        {
            targetCam = Camera.main;
        }
        if (!targetCam)
        {
            Debug.LogError("No camera found by cameracontroller");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!target)
        {
            return;
        }
        Vector3 currentTargetScreenPos = targetCam.WorldToScreenPoint(target.transform.position);
        float currentScreenWidth = Screen.width;
        float currentScreenHeight = Screen.height;
        bool aboveTopLimit = currentTargetScreenPos.y > currentScreenHeight * ((100f - topLimit) / 100f);
        bool belowBottomLimit = currentTargetScreenPos.y < currentScreenHeight * (bottomLimit / 100f);
        bool tooFarLeft = currentTargetScreenPos.x < currentScreenWidth * (leftLimit / 100f);
        bool tooFarRight = currentTargetScreenPos.x > currentScreenWidth * ((100 - rightLimit) / 100f);
        Debug.Log($"CurrentPos = {currentTargetScreenPos} bottom = {belowBottomLimit} top = {aboveTopLimit} left = {tooFarLeft} right = {tooFarRight}");
        Debug.Log($"Height = {currentScreenHeight} Width = {currentScreenWidth} bottomL = {currentScreenHeight * (bottomLimit / 100f)} topL = {currentScreenWidth * ((100 - topLimit) / 100f)} leftL = {currentScreenWidth * (leftLimit / 100f)} right = {currentScreenWidth * ((100 - rightLimit) / 100f)}");
        if (tooFarLeft || tooFarRight)
        {
            Vector3 targetPosition2D = Vector3.Scale(target.transform.position,Vector3.right) + Vector3.forward * targetCam.transform.position.z;
            targetCam.transform.position = Vector3.Lerp(targetCam.transform.position, targetPosition2D, followLerpSpeed*Time.fixedDeltaTime);
        }
        if (belowBottomLimit || aboveTopLimit)
        {
            Vector3 targetPosition2D = Vector3.Scale(target.transform.position, Vector3.up) + Vector3.forward * targetCam.transform.position.z;
            targetCam.transform.position = Vector3.Lerp(targetCam.transform.position, targetPosition2D, followLerpSpeed * Time.fixedDeltaTime);
        }
    }
}
