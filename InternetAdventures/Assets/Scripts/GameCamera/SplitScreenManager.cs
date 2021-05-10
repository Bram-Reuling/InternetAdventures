// using System;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
//
// namespace GameCamera
// {
//     public class SplitScreenManager : MonoBehaviour
//     {
//         [SerializeField] private CameraRig mainCameraRig;
//         [SerializeField] private List<CameraRig> playerCameraRigs;
//         [SerializeField] private float activateSplitScreenRange;
//
//         private void Update()
//         {
//             // if the players are within the activateSplitScreenRange, enable the mainCameraRig
//             // else enable all the playerCameraRigs
//
//             float highestDistanceBetweenPlayers = 0;
//             
//             List<Vector3> playerPosition = mainCameraRig.Targets.Select(target => target.transform.position).ToList();
//
//             foreach (Vector3 p1 in playerPosition)
//             {
//                 foreach (Vector3 p2 in playerPosition)
//                 {
//                     float distance = Vector3.Distance(p1, p2);
//
//                     if (distance > highestDistanceBetweenPlayers)
//                     {
//                         highestDistanceBetweenPlayers = distance;
//                     }
//                 }
//             }
//
//             if (highestDistanceBetweenPlayers >= activateSplitScreenRange)
//             {
//                 mainCameraRig.gameObject.SetActive(false);
//
//                 foreach (CameraRig rig in playerCameraRigs)
//                 {
//                     rig.gameObject.SetActive(true);
//                 }
//                 
//                 Debug.Log("Activating player rigs");
//             }
//             else
//             {
//                 mainCameraRig.gameObject.SetActive(true);
//
//                 foreach (CameraRig rig in playerCameraRigs)
//                 {
//                     rig.gameObject.SetActive(false);
//                 }
//                 
//                 Debug.Log("Activating main rig");
//             }
//         }
//     }
// }
