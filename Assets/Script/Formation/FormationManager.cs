using System.Collections.Generic;
using UnityEngine;

namespace Script.Formation
{
    /*
     * Handles the formation algorithm
     */
    public class FormationManager : MonoBehaviour
    {
        [Header("Slot")]
        [SerializeField] private GameObject[] formationPrefab;// slot prefabs
        [SerializeField] private int[] formationNumber; // number of slots in each row

        public int FormationNumber => formationNumber[0];

        [SerializeField] private GameObject leader;     // leader of the formation
    
        [Header("Settings")]
        [SerializeField] [Range(2,5)] private int neighbourSideDistance = 2;    // distance between slots in a row
        [SerializeField] [Range(1,3)] private int neighbourLineDistance = 2;    // distance between rows in the formation


        private List<GameObject> _formationList;
        public List<GameObject> FormationList => _formationList;

        private void Start()
       {
           _formationList = new List<GameObject>();
           float lineDistance = neighbourLineDistance;          // distance of each row in relation to the leader
            for (int i = 0; i < formationPrefab.Length; i++)    // formation loop
            {
                int fNum = formationNumber[i];                  // number of slots on a row 
                float sideDistance = -neighbourSideDistance * Mathf.RoundToInt(fNum/2); // initial position of the row based on the number of slots
                if (fNum % 2 == 0)                              // correct the starting position if the the number of slots is even
                {
                    sideDistance -= sideDistance / fNum;
                }
                for (int j = 0; j < fNum; j++)                  // row loop
                {
                    Vector3 position = new Vector3(leader.transform.localPosition.x - sideDistance, 0f,
                        leader.transform.localPosition.z + lineDistance);                     // get the position of a follower
                    Vector3 initPosition = leader.transform.TransformPoint(position);           // translate the position to world position   
                    GameObject temp = Instantiate(formationPrefab[i], initPosition, leader.transform.rotation);     // create a follower
                    temp.GetComponent<Slot>().Target = leader;                                  // set the leader
                    temp.GetComponent<Slot>().OffsetPosition = position;                        // set the initial follower's position, so they will always be at the same distance to the leader's position
                    temp.transform.parent = transform;                                          // set the parent of the follower in the screen hierarchy
                    temp.transform.rotation *= Quaternion.AngleAxis( 180, transform.up );  // turn 180º to face the leader
                    _formationList.Add(temp);
                    sideDistance += neighbourSideDistance;                                      // increase the distance from the initial position
                }
                lineDistance += neighbourLineDistance;                                          // increase the distance to the leader of the next row
                
            }
        }
    }
}
