//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using MLAPI;

//[RequireComponent(typeof(MovementMotor))]
//public class AiMovementInputs : NetworkBehaviour
//{
//    MovementMotor motor;

//    // Start is called before the first frame update
//    void Start()
//    {
//        motor = GetComponent<MovementMotor>();
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (GameObject.FindGameObjectWithTag("player"))
//		{
//            GameObject player = GameObject.FindGameObjectWithTag("player");
//            //get difference between player and target
//            //Vector3 targetDirection = (player.transform.position - transform.position).normalized;
//            //float angleToTarget = Vector3.SignedAngle(targetDirection, transform.forward, Vector3.up);
//            ////Debug.Log(angleToTarget);
//            ////
//            //Debug.DrawLine(transform.position, transform.position + new Vector3(targetDirection.x, targetDirection.y, targetDirection.z), Color.red, 15f);
//            ////transform.forward = player.transform.position - transform.position;
//            //Vector3 rotationFromTarget = transform.forward - (player.transform.position - transform.position);
//            //Debug.Log("Rotation from target " + rotationFromTarget);
//            //Debug.Log(transform.forward);

//            Vector3.Angle(player.transform.forward, player.transform.position - transform.position);

//            //motor.Rotate();
//		}
//    }
//}
