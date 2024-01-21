using System;
using Script.Citizen;
using Script.Game;
using Script.Input;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Script.IA.Dummy
{
    public class DummyAgent : Agent
    {
        [SerializeField] private Transform target;

        // Fields
        [Header("Movement")]
        [Tooltip("The speed at which the npc walks")] [SerializeField] [Range(0, 1.5f)]
        protected float walk = 1f;
        
        [Tooltip("The speed at which the npc runs")] [SerializeField] [Range(0, 4)]
        protected float sprint = 3f;
        
        [Tooltip("Max speed of movement transitions")] [Range(0, 20)]
        [SerializeField] private float maxAcceleration = 10f;

        [Tooltip("The max distance to move")] [SerializeField] [Range(0, 15)]
        protected float movementDistance = 7f;

        [Tooltip("The max distance for detecting the destination point")] [SerializeField] [Range(0, 2)]
        protected float detectionDistance = 0.5f;
        
        [FormerlySerializedAs("areaMask")] [SerializeField] private Constant.NavMesh.AreaMask areaMaskName = Constant.NavMesh.AreaMask.Walkable;

        
        //properties
        // the rigid body
        private Rigidbody _rBody;
        // the input key manager
        private AgentInputHandler _agentInputHandler;
        private Animator _animator;
        private CharacterController _controller;

        //variables
        private float _speed;
        private float _movementSpeed;       // max speed
        private float _accelerationSpeed;   // max acceleration
        private float _animationBlend;      // speed of the animation
        private Vector3 _destinationPoint;  // point to go to
        private const float RotationVelocity = 360;
        
        //variables
        private int _areaMask;     // walkable layer area number
        
        // Start is called before the first frame update
        private void Awake()
        {
            // sets an instance of the components
            _agentInputHandler = GetComponent<AgentInputHandler>();
            _rBody = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
            _controller = GetComponent<CharacterController>();
            _controller.detectCollisions = true;
            // max speed between the walk and the sprint value
            _movementSpeed = Random.Range(walk, sprint);
            // max acceleration between half the acceleration and the acceleration value
            _accelerationSpeed = Random.Range(maxAcceleration / 2, maxAcceleration); 
            _areaMask = 1 << NavMesh.GetAreaFromName(areaMaskName.ToString());

        }
    
        /*
         * When the agent reaches the target or falls ->
         * Reset the agent position and the target properties
         */
        public override void OnEpisodeBegin()
        {
            // max speed between the walk and the sprint value
            _movementSpeed = Random.Range(walk, sprint);
            // max acceleration between half the acceleration and the acceleration value
            _accelerationSpeed = Random.Range(maxAcceleration / 2, maxAcceleration);
            _animationBlend = 0;
            _speed = 0;
            // loop until find a random point in the walkable area
           
            while (!RandomPoint(transform.position, movementDistance, out _destinationPoint))
            {
            }

            target.position = _destinationPoint;

        }

        /*
         * the information we collect for the training of the IA
         */
        public override void CollectObservations(VectorSensor sensor)
        {
            // Target and Agent positions
            sensor.AddObservation(_destinationPoint);
            sensor.AddObservation(transform.localPosition);

            // Agent velocity
            sensor.AddObservation(_controller.velocity.x);
            sensor.AddObservation(_controller.velocity.z);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        /*
         *  Receives actions and assigns the reward
         */
        public override void OnActionReceived(ActionBuffers actionBuffers)
        {
            // Actions, size = 2
            Vector3 controlSignal = Vector3.zero;
            controlSignal.x = actionBuffers.ContinuousActions[0];
            controlSignal.z = actionBuffers.ContinuousActions[1];
            Movement(controlSignal);

            if (CheckDistanceToDestination())
            {
                SetReward(1.0f);
                EndEpisode();
            }
            else
            {
                // Debug.Log(Vector3.Distance(transform.position, _destinationPoint));
            }
            
        }
    
        // ReSharper disable Unity.PerformanceAnalysis
        /*
         *  Control the agent ant the heuristic behavior
         */
        public override void Heuristic(in ActionBuffers actionsOut)
        {
            Vector2 movement = _agentInputHandler.GetMoveInput();
            var continuousActionsOut = actionsOut.ContinuousActions;
            continuousActionsOut[0] = movement.x;
            continuousActionsOut[1] = movement.y;
        }

        /*
         * Triggers when the object hits another one
         * Set the rewards according to the object hitted
         */
        public void OnControllerColliderHit(ControllerColliderHit hit)
        {
            switch (hit.transform.tag)
            {
                // Rewards
                case Constant.Tag.TARGET:
                    SetReward(1.0f);    // reach the goal and get a positive reward
                    EndEpisode();
                    break;
                case Constant.Tag.VOID: // not reached the goal, so no reward
                    EndEpisode();
                    break;
            }
        }


        //movement
        private void Movement(Vector3 move)
        {
            if (move.Equals(Vector3.zero))
            {
                _animationBlend = 0;
                _speed = 0;
            }
            else
            {
                Vector3 inputDirection = move.normalized;
                _speed = Mathf.Lerp(_speed, _movementSpeed,
                    _accelerationSpeed * Time.fixedTime);
                // move the player
                Quaternion toRotation = Quaternion.LookRotation(inputDirection, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, RotationVelocity * Time.deltaTime); 
                _controller.Move(inputDirection * (_speed * Time.deltaTime));

            }
            
            //animation
            _animationBlend = Mathf.Lerp(_animationBlend, _speed,Time.fixedTime * _accelerationSpeed);
            _animator.SetFloat(Constant.Animation.SPEED, _animationBlend);
          
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
        
        /*
        * Checks if the the citizen position is close enough to the destination point    
        */
        protected bool CheckDistanceToDestination()
        {
            return detectionDistance >= Vector3.Distance(transform.position, _destinationPoint);                   
        }
        
    }
}
