using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnityGameLib.NPlayerController.NControllerPC;
public class NewBehaviourScript : ControllerPC
{ 
    [SerializeField] Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Transform pos1 = GetKeyPositionMoveUp(gameObject, 1.0f);
                  pos1 = GetKeyPositionMoveDown(gameObject, 1.0f);
                  pos1 = GetKeyPositionMoveRight(gameObject, 1.0f);
                  pos1 = GetKeyPositionMoveLeft(gameObject, 1.0f);

        rb.AddForce(pos1.position);
    }
}
