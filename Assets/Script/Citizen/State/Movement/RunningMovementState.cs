using UnityEngine;

namespace Script.Citizen.State.Movement
{
    /*
     * Running state, the movement speed has to be greater than 3
     *
     * Transition to
     *   Running -> Walking
     */
    public class RunningMovementState : ICitizenMovementState
    {
        // FSM reference
        private readonly FsmCitizen _fsm;
        public RunningMovementState (FsmCitizen fsmCitizen)
        {
            _fsm = fsmCitizen;
        }
        
        //Update the movement state
        public void UpdateMovementState()
        {
            _fsm.IncreaseMovementSpeed();   // update the movement speed
            // check the transition to the other states according to the the movement speed value
            if (_fsm.NavMeshAgent.speed  <= _fsm.Sprint)    // if the the movement speed value is less or equal to 3
            {                                               // transition to walking state
                ToWalkingState();
            }
        }

        public void ToIdleState()
        {
            Debug.Log ("Not reachable state");
        }
        
        // Transition to walking state
        public void ToWalkingState()
        {
            _fsm.CurrentMovementState = _fsm.WalkingMovementState;
        }

        public void ToRunningState()
        {
            Debug.Log ("Can't switch to its own state");
        }
    }
}