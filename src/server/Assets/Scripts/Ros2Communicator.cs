using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;

public class Ros2Communicator : MonoBehaviour
{

    private ROSConnection rosConnection;

    // Start is called before the first frame update
    void Start()
    {
        rosConnection = ROSConnection.GetOrCreateInstance();

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
