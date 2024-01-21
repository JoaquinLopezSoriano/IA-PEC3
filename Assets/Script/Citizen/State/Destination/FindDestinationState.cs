using Script.Game;
using UnityEngine;
using UnityEngine.AI;

/*
 * https://docs.unity3d.com/ScriptReference/Random-insideUnitSphere.html
 * https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
 */

namespace Script.Citizen.State.Destination
{
    /*
     * Find state, on the way to the destination point
     *
     * Transition to
     *    Find -> Target
     */
    public class FindDestinationState : ICitizenDestinationState
    {
        // FSM reference
        private readonly FsmCitizen _fsm;

        //variables
        private readonly int _areaMask;     // walkable layer area number
        public FindDestinationState (FsmCitizen fsmCitizen, Constant.NavMesh.AreaMask areaMaskName)
        {
            _fsm = fsmCitizen;
            _areaMask = 1 << NavMesh.GetAreaFromName(areaMaskName.ToString());
        }
        
        //Update the destination state
        // Search for a random point in the walkable area to go to
        public void UpdateDestinationState()
        {
            // loop until find a random point in the walkable area
            Vector3 destinationPoint;   // the random point in the walkable area gotten
            while (!RandomPoint(_fsm.transform.position, _fsm.MovementDistance, out destinationPoint))
            {
            }
            _fsm.DestinationPoint = destinationPoint;           
            _fsm.NavMeshAgent.SetDestination(destinationPoint); // target the destination point
            _fsm.GetMovementSpeed();                            // Obtain a random maximum speed
            ToTargetDestinationState();                         // transition to target state
            
        }

        public void ToFindDestinationState()
        {
            Debug.Log ("Can't switch to its own state");
        }

        // Transition to target state
        public void ToTargetDestinationState()
        {
            _fsm.CurrentDestinationState = _fsm.TargetDestinationState;
        }
        
        // Find a random point in the walkable area
        private bool RandomPoint(Vector3 center, float range, out Vector3 result)
        {
            // get a random 3D point and extends it with a certain range
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            // checks if the random 3D point is in the walkable area
            if (NavMesh.SamplePosition(new Vector3(randomPoint.x, 0.25f, randomPoint.z), out var hit, 0.5f, _areaMask))
            {
                result = hit.position;  // if found, returns the random 3D point
                return true;
            }
            // returns vector zero if not
            result = Vector3.zero;
            return false;
        }
    }
}