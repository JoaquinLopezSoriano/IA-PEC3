using System.Collections;
using Script.Game;
using UnityEngine;

/*
 * https://stackoverflow.com/questions/30056471/how-to-make-the-script-wait-sleep-in-a-simple-way-in-unity
 * https://forum.unity.com/threads/passing-a-function-as-an-argument.90484/
 */

namespace Script.Citizen.Elder.State.WaitingState
{
    /*
     * Waiting state, waits stopped an amount of time until continue moving again
     *
     * Transition from the other movement states
     * Transition to
     *   Delay -> Idle
     */
    public class DelayWaitingState : IElderWaitingState
    {
        // FSM reference
        private readonly FsmElder _fsm;
        public DelayWaitingState (FsmElder fsmElder)
        {
            _fsm = fsmElder;
        }

        public void UpdateMovementState()
        {
            // do nothing updatable
        }

        // Transition to idle state
        public void ToIdleState()
        {
            // return to the locomotion state at the animator controller and start moving again
            _fsm.Animator.SetTrigger(Constant.Animation.LEAVE);
            _fsm.CurrentMovementState = _fsm.IdleMovementState;         
        }

        public void ToWalkingState()
        {
            Debug.Log ("Not reachable state");
        }

        public void ToRunningState()
        {
            Debug.Log ("Not reachable state");
        }

        // Transition to delay state
        public void ToDelayWaitingState()
        { 
            // jump to the waiting state at the animator controller 
            _fsm.Animator.SetTrigger(Constant.Animation.WAIT);
            _fsm.NavMeshAgent.speed = 0;                         // stop the elder citizen
            _fsm.CurrentMovementState =  _fsm.DelayWaitingState;  // update the state to delay state
            _fsm.LaunchCoroutine(DelaySecond(_fsm.MinWaitingTime, _fsm.MaxWaitingTime));   // launch a coroutine
        }

        // Waits a random time between a minimum and maximum value
        private IEnumerator DelaySecond(float minWaitingTime, float maxWaitingTime)
        {
            //yield on a new YieldInstruction that waits for random seconds
            yield return new WaitForSeconds( Random.Range(minWaitingTime, maxWaitingTime));
            _fsm.ResetWaitingTime(); // update the passed by time
            ToIdleState();          // update the state to idle state

        }
    }
}