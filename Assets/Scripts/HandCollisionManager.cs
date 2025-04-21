using UnityEngine;
using System.IO.Ports;
using System.Collections.Generic;

public class HandCollisionManager : MonoBehaviour
{
    SerialPort dataStream = new SerialPort("COM11", 115200);
    SerialManager serialComm;

    private Dictionary<string,bool> fingerStates= new Dictionary<string, bool>();
    
    void Start()
    {
        fingerStates["Thumb"] = false;
        fingerStates["Pointer"] = false;
        fingerStates["Mid"] = false;
        fingerStates["Ring"] = false;
        fingerStates["Pinky"] = false;    

        serialComm = FindFirstObjectByType<SerialManager>();
    }

    public void ReportCollision(string fingerName, bool isTouching) {
        if (fingerStates.TryGetValue(fingerName, out bool collision)) {
            if (fingerStates[fingerName] != isTouching) {
            fingerStates[fingerName] = isTouching;

            //Debug.Log($"{fingerName} is now {(isTouching ? "TOUCHING" : "NOT touching")}");

            SendToManager(fingerName, isTouching);
            }
        }
        else 
            Debug.LogWarning($"Finger name '{fingerName}' not found in fingerStates dictionary!");
    }

    void SendToManager(string fingerName, bool isTouching)
    {
        Debug.Log($"SERVO:{fingerName} {(isTouching ? 1 : 0)}");
        //Connect to SerialManager
        serialComm.SendToArduino($"SERVO:{fingerName} {(isTouching ? 1 : 0)}\n");

    }
}
