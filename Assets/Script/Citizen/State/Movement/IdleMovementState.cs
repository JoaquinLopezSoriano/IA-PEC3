using UnityEngine;

namespace Script.Citizen.State.Movement
{
    /*
     * Idle state, the movement speed has to be less than 1
     *
     * Transition to
     *   Idle -> Walking
     */
    public class IdleMovementState : ICitizenMovementState
    {
        // FSM reference
        private readonly FsmCitizen _fsm;
        public IdleMovementState (FsmCitizen fsmCitizen)
        {
            _fsm = fsmCitizen;
        }
        
        //Update the movement state
        public void UpdateMovementState()
        {
            _fsm.IncreaseMovementSpeed();   // update the movement speed
            // check the transition to the other states according to the the movement speed value
            if (_fsm.NavMeshAgent.speed >= _fsm.Walk)   // if the the movement speed value is greater or equal to 1
            {                                           // transition to walking state
                ToWalkingState();
            }
        }

        public void ToIdleState()
        {
            Debug.Log ("Can't switch to its own state");
        }

        // Transition to walking state
        public void ToWalkingState()
        {
            _fsm.CurrentMovementState = _fsm.WalkingMovementState;
        }

        public void ToRunningState()
        {
            Debug.Log ("Not reachable state");
        }
    }
}