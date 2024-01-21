using Script.Game;
using UnityEngine;
using UnityEngine.AI;

namespace Script.Formation
{
    /*
     * Handles a follower in the formation
     */
    public class Slot : MonoBehaviour
    {
        private Animator _animator;         // animation controller
        private NavMeshAgent _navMeshAgent; // navigation mesh agent
        private GameObject _target;         // leader game object
        public GameObject Target
        {
            set => _target = value;
        }
        private Vector3 _offsetPosition;
        public Vector3 OffsetPosition   // the initial position, to keep the same distance from the leader while following them
        {
            set => _offsetPosition = value;
        }


        private void Start()
        {
            _animator = GetComponent<Animator>();
            _navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
            
        }

        private void Update()
        {
            _navMeshAgent.SetDestination(_target.transform.position + _offsetPosition);  // go to the current position of the leader plus an offset, to keep the same distance from the leader while following them
            transform.LookAt(_target.transform);                                              // face the leader
            _animator.SetFloat(Constant.Animation.SPEED, _navMeshAgent.speed);           // animate the movement
        }

        public void Anime(string taichiMovement)
        {
            _animator.Play(taichiMovement);  
        }
    }
}
