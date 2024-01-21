using UnityEngine;

namespace Script.Citizen.State.Movement
{
    /*
     * Walking state, the movement speed has to be between 1 and 3 included
     *
     * Transition to
     *   Walking -> Running
     *   Walking -> Idle
     */
    public class WalkingMovementState : ICitizenMovementState
    {
        // FSM reference
        private readonly FsmCitizen _fsm;
        public WalkingMovementState (FsmCitizen fsmCitizen)
        {
            _fsm = fsmCitizen;
        }

        //Update the movement state
        public void UpdateMovementState()
        {
            _fsm.IncreaseMovementSpeed();   // update the movement speed
            // check the transition to the other states according to the the movement speed value
            if (_fsm.NavMeshAgent.speed  < _fsm.Walk)   // if the the movement speed value is less than 1
            {                                           // transition to idle state
                ToIdleState();
            }
            else if (_fsm.NavMeshAgent.speed  > _fsm.Sprint)  // if the the movement speed value is greater than 3
            {                                                 // transition to running state
                ToRunningState();
            }
        }

        // Transition to idle state
        public void ToIdleState()
        {
            _fsm.CurrentMovementState = _fsm.IdleMovementState;
        }

        public void ToWalkingState()
        {
            Debug.Log ("Can't switch to its own state");
        }

        // Transition to running state
        public void ToRunningState()
        {
            _fsm.CurrentMovementState = _fsm.RunningMovementState;
        }
    }
}