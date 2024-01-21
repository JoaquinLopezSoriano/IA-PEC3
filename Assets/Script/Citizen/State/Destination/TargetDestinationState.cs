using UnityEngine;

namespace Script.Citizen.State.Destination
{
    /*
     * Target state, on the way to the destination point
     *
     * Transition to
     *   Target -> Find
     */
    public class TargetDestinationState : ICitizenDestinationState
    {
        // FSM reference
        private readonly FsmCitizen _fsm;
        public TargetDestinationState (FsmCitizen fsmCitizen)
        {
            _fsm = fsmCitizen;
        }
        
        //Update the destination state
        // checks if the the citizen position is close enough to the destination point 
        public virtual void UpdateDestinationState()
        {
            // checks the transition to the other state when reached the destination point
            if(CheckDistanceToDestination())
            {
                ToFindDestinationState();       // transition to find state
            }
        }

        /*
         * Checks if the the citizen position is close enough to the destination point    
         */
        protected bool CheckDistanceToDestination()
        {
            return _fsm.DetectionDistance >= Vector3.Distance(_fsm.transform.position, _fsm.DestinationPoint);                   
        }
        
        // transition to find state
        public virtual void ToFindDestinationState()
        {
            _fsm.CurrentDestinationState = _fsm.FindDestinationState;
        }

        public void ToTargetDestinationState()
        {
            Debug.Log ("Can't switch to its own state");
        }
    }
}