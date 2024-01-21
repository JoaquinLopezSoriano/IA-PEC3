using System;
using System.Collections;
using System.Linq;
using Script.Citizen.State.Destination;
using Script.Game;
using UnityEngine;
using Random = UnityEngine.Random;

// https://stackoverflow.com/questions/30056471/how-to-make-the-script-wait-sleep-in-a-simple-way-in-unity
// https://forum.unity.com/threads/making-an-object-rotate-to-face-another.1211/
namespace Script.Formation
{
    /*
     * Handles the destination behavior of the taichi leader
     * When they reach the destination point they do a taichi movement
     */
    public class TaichiDestinationState : TargetDestinationState
    {
        // taichi id movement list
        private string[] _taichiList;
        private readonly FsmTaichiCitizen _fsm;
        private int _taichiState;
        private int _taichiMovementCounter;
        private readonly FormationManager _formationManager;
        private readonly int _position;
        private readonly float _strength = 0.1f;
        private Quaternion _targetRotation;

        public TaichiDestinationState(FsmTaichiCitizen fsmCitizen, FormationManager formationManager) : base(fsmCitizen)
        {
            _fsm = fsmCitizen;
            _formationManager = formationManager;
            _taichiState = 0;
            _taichiMovementCounter = 0;
            _taichiList = Enum.GetNames(typeof(Constant.Animation.TaichiMovements));
            _position = Mathf.FloorToInt(_formationManager.FormationNumber / 2f);
        }
        
        //Update the destination state
        // checks if the the citizen position is close enough to the destination point 
        public override void UpdateDestinationState()
        {
            
            switch (_taichiState)
            {
                  case 0:
                      if (CheckDistanceToDestination())
                      {   
                          _taichiState = 1;
                          // _fsm.transform.LookAt(_formationManager.FormationList[position].gameObject.transform);   
                          // _fsm.transform.rotation *= Quaternion.AngleAxis( 180, _fsm.transform.up );  // turn 180º to face the leader
                          _taichiList = _taichiList.ToList().OrderBy(x => Random.value).ToArray();
                          _targetRotation = Quaternion.LookRotation (_formationManager.FormationList[_position].gameObject.transform.position - _fsm.transform.position);
                      }
                      break;
                  case 1:
                      string animationName = _taichiList[_taichiMovementCounter];
                      _fsm.Animator.Play(animationName);                             // do the leader the taichi movement
                      foreach (GameObject go in _formationManager.FormationList)     // do every slot follower the taichi movement as well
                      {
                          go.GetComponent<Slot>().Anime(animationName);
                      
                      }
                      _taichiState = 2;
                      // get a random taichi movement
                      _fsm.LaunchCoroutine(TaichiMovement());   // launch a coroutine   
                      break;
                  case 2:       
                      _fsm.transform.rotation = Quaternion.Lerp (_fsm.transform.rotation, _targetRotation, _strength);          
                      break;
                  case 3:
                      _fsm.Animator.Play(Constant.Animation.LOCOMOTION);             // return moving the leader   
                      foreach (GameObject go in _formationManager.FormationList)             // return moving the slot followers 
                      { 
                          go.GetComponent<Slot>().Anime(Constant.Animation.LOCOMOTION);
                      }
                      ToFindDestinationState();       // transition to find state
                      _taichiState = 0;
                      _taichiMovementCounter = 0;
                      _fsm.transform.rotation = Quaternion.Lerp(_fsm.transform.rotation,Quaternion.AngleAxis( 180, _fsm.transform.up ), _strength);  // turn 180º to face the leader

                      break;
            }
        }
      
        private IEnumerator TaichiMovement()
        {
            yield return new WaitForSecondsRealtime(0.5f);  // wait a bit so the transition to taichi could be completed, if not sometimes the conditions doesn't work properly
            yield return new WaitUntil(() => _fsm.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 || _fsm.Animator.IsInTransition(0)); // wait untill the animation has finished
            _taichiMovementCounter++;
            _taichiState = _taichiMovementCounter.Equals(_fsm.TaichiMovements) ? 3 : 1;
        }
    }
}
