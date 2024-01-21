using System;
using BBUnity.Conditions;
using Pada1.BBCore;
using UnityEngine;

namespace Script.BehaviorTree
{
    [Condition("MyConditions/IsTargetNear")]
    [Help("Checks whether the object is near another with a specific tag.")]
    public class IsTargetNear : GOCondition
    {
        ///<value>Input maximum distance Parameter to consider that the target is close.</value>
        [InParam("closeDistance")]
        [Help("The maximum distance to consider that the target is close")]
        public float closeDistance;
        
        [InParam("targetName")]
        [Help("The target name of the target")]
        public String targetName;
        
        ///<value>Input Target Parameter to to check the distance.</value>
        [OutParam("target")]
        [Help("The target that is close")]
        public GameObject target;
        
        public override bool Check()
        {
            // loop for the game objects with a specific target
            foreach (GameObject go in GameObject.FindGameObjectsWithTag(targetName))
             {
                 if ((gameObject.transform.position - go.transform.position).sqrMagnitude <
                     closeDistance * closeDistance)     // check if it is close
                 {
                        target = go;    // set the target
                        return true;
                 }
                
             }
             return false;
        }
    }
}
