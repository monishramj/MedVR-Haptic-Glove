using UnityEngine;
using System;

public class FingerBending : MonoBehaviour
{
    [Header("Finger Transforms")]
    public Transform pointerBottom;
    public Transform middleBottom;
    public Transform ringBottom;
    public Transform pinkyBottom;
    public Transform thumbBottom;

    [Header("Curl Amounts")]
    [Range(0f, 68f)]
    public float pointerCurl = 0f;
    [Range(0f, 68f)]
    public float middleCurl = 0f;
    [Range(0f, 68f)]
    public float ringCurl = 0f;
    [Range(0f, 68f)]
    public float pinkyCurl = 0f;
    [Range(0f, 68f)]
    public float thumbCurl = 0f;

    // Arrays to store finger bones and original rotations
    private Transform[][] allFingers = new Transform[5][];
    private Quaternion[][] allOriginalRotations = new Quaternion[5][];

    void Start()
    {
        // Initialize arrays for each finger
        for (int i = 0; i < 5; i++)
        {
            allFingers[i] = new Transform[3];
            allOriginalRotations[i] = new Quaternion[3];
        }

        // Set up each finger if assigned
        if (pointerBottom != null) SetupFinger(pointerBottom, 0);
        if (middleBottom != null) SetupFinger(middleBottom, 1);
        if (ringBottom != null) SetupFinger(ringBottom, 2);
        if (pinkyBottom != null) SetupFinger(pinkyBottom, 3);
        if (thumbBottom != null) SetupFinger(thumbBottom, 4);
    }

    void SetupFinger(Transform bottomBone, int fingerIndex)
    {
        // Assign bottom bone
        allFingers[fingerIndex][0] = bottomBone;
        allOriginalRotations[fingerIndex][0] = bottomBone.localRotation;
        
        // Find mid and top bones based on naming convention
        string baseName = bottomBone.name;
        
        // Find mid bone with proper error handling
        Transform midBone = null;
        for (int i = 0; i < bottomBone.childCount; i++) 
        {
            Transform child = bottomBone.GetChild(i);
            if (child.name.Contains("MidBone")) 
            {
                midBone = child;
                break;
            }
        }
        
        // Store mid bone if found
        if (midBone != null) 
        {
            allFingers[fingerIndex][1] = midBone;
            allOriginalRotations[fingerIndex][1] = midBone.localRotation;
            
            // Find top bone with proper error handling
            Transform topBone = null;
            for (int i = 0; i < midBone.childCount; i++) 
            {
                Transform child = midBone.GetChild(i);
                if (child.name.Contains("TopBone")) 
                {
                    topBone = child;
                    break;
                }
            }
            
            // Store top bone if found
            if (topBone != null) 
            {
                allFingers[fingerIndex][2] = topBone;
                allOriginalRotations[fingerIndex][2] = topBone.localRotation;
            }
            else 
            {
                Debug.LogWarning("TopBone not found for finger: " + baseName);
            }
        }
        else 
        {
            Debug.LogWarning("MidBone not found for finger: " + baseName);
        }
    }

    void Update()
    {
        // Apply curl to each finger
        ApplyCurl(0, pointerCurl, false); // Pointer - X axis
        ApplyCurl(1, middleCurl, false);  // Middle - X axis
        ApplyCurl(2, ringCurl, false);    // Ring - X axis
        ApplyCurl(3, pinkyCurl, false);   // Pinky - X axis
        ApplyCurl(4, thumbCurl, true);    // Thumb - Z axis
    }

    void ApplyCurl(int fingerIndex, float curlAmount, bool useZAxis)
    {
        // Iterate through each bone in the finger
        for (int i = 0; i < 3; i++)
        {
            // Skip if the bone reference is null
            if (allFingers[fingerIndex][i] == null) continue;
            
            // Get original rotation values
            Vector3 originalEuler = allOriginalRotations[fingerIndex][i].eulerAngles;
            
            if (useZAxis)
            {
                // Apply curl on Z-axis (for thumb)
                if (i != 2) {
                    allFingers[fingerIndex][i].localRotation = Quaternion.Euler(
                    originalEuler.x,
                    originalEuler.y + (curlAmount/4),
                    originalEuler.z + (curlAmount/2));
                }

                else {
                    allFingers[fingerIndex][i].localRotation = Quaternion.Euler(
                    originalEuler.x,
                    originalEuler.y - (curlAmount/4),
                    originalEuler.z + (curlAmount/2));
                }
            }
            else
            {
                // Apply curl on X-axis (for other fingers)
                allFingers[fingerIndex][i].localRotation = Quaternion.Euler(
                    originalEuler.x + curlAmount,
                    originalEuler.y,
                    originalEuler.z);
            }
        }
    }
}