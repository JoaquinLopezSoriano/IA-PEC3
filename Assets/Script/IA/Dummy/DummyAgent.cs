using Script.Game;
using Script.Input;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Script.IA.Dummy
{
    /*
     * Handles the behavior of the Dummy and sets the implementation of the agent
     */
    public class DummyAgent : Agent
    {
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
        
        [Tooltip("The max time until reach the destination point")] [SerializeField] [Range(1, 10)]
        protected float waitingTime =5f;
        
        [Tooltip("The max distance for detecting other scene elements")] [SerializeField] [Range(5, 20)]
        protected float detectionRadius =10f;

        [Tooltip("The area to search the acceptable destination points")]
        [SerializeField] private Constant.NavMesh.AreaMask areaMaskName = Constant.NavMesh.AreaMask.Walkable;

        
        //properties
        // the input key manager
        private AgentInputHandler _agentInputHandler;
        // animator
        private Animator _animator;
        private CharacterController _controller;

        //variables
        private float _speed;
        private float _movementSpeed;       // maximum speed
        private float _accelerationSpeed;   // maximum acceleration
        private float _animationBlend;      // speed value of the animation
        private Vector3 _destinationPoint;  // point to go to
        private const float RotationVelocity = 360; // maximum rotation angle
        private int _areaMask;              // walkable layer area number
        private int _maxSpaceSize;          // maximum elements to save while training the IA
        private float _reachTime;           // maximum time for reaching the point
        
        
        // Start is called before the first frame update
        private void Awake()
        {
            // sets an instance of the components
            _agentInputHandler = GetComponent<AgentInputHandler>();
            _animator = GetComponent<Animator>();
            _controller = GetComponent<CharacterController>();
            _controller.detectCollisions = true;
            // get maximum character speed between the walk and the sprint value
            _movementSpeed = Random.Range(walk, sprint);
            // get maximum character acceleration between half the acceleration and the acceleration value
            _accelerationSpeed = Random.Range(maxAcceleration / 2, maxAcceleration);
            // get the mask to get the acceptable points
            _areaMask = 1 << NavMesh.GetAreaFromName(areaMaskName.ToString());
            // the maximum size of the vector observation
            _maxSpaceSize = GetComponent<BehaviorParameters>().BrainParameters.VectorObservationSize;

        }
    
        /*
         * Reset the agent position
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
            // set the beginning reach time
            _reachTime = Time.time;

        }

        /*
         * The information we collect for the training of the IA
         */
        public override void CollectObservations(VectorSensor sensor)
        {
            // Destination and Agent positions
            sensor.AddObservation(_destinationPoint);
            sensor.AddObservation(transform.localPosition);

            // Agent velocity
            sensor.AddObservation(_controller.velocity.x);
            sensor.AddObservation(_controller.velocity.z);
            
            // other object to avoid: another citizen or a non walkable area
            var colliders = Physics.OverlapSphere(transform.position, detectionRadius);
            // get the maximum elements to save at the vector observation
            int max = _maxSpaceSize-4;
            if (max > colliders.Length)
            {
                max = colliders.Length;
            }

            for (int i = 0; i < max; i++)
            {
                Collider colliderElement = colliders[i];
                switch (colliderElement.transform.tag)
                {
                    case Constant.Tag.NO_WALKABLE: // save only the position for still objects like trees or stones 
                        sensor.AddObservation(colliderElement.transform.position);
                        break;
                    case Constant.Tag.CITIZEN:   // save the position and velocity for moving objects, the citizens
                        sensor.AddObservation(colliderElement.transform.position);
                        sensor.AddObservation(colliderElement.GetComponent<NavMeshAgent>().velocity.x);
                        sensor.AddObservation(colliderElement.GetComponent<NavMeshAgent>().velocity.z);
                        break;
                }
            }
   
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
            Movement(controlSignal);    // move and animate the dummy

            if (CheckDistanceToDestination())   // if the dummy reaches the point
            {
                SetReward(1.0f);    // reach the goal and get a positive reward
                EndEpisode();       // start again
            }

            if ((waitingTime + _reachTime) < Time.time) // if the dummy is stuck and no reached the point in a period of time
            {
                EndEpisode();   // not reached the goal, so no reward
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
         * Triggers when the object hits another citizen, bounds or a non walkable area
         * Set the rewards according to the object hitted
         */
        public void OnControllerColliderHit(ControllerColliderHit hit)
        {
            switch (hit.transform.tag)  // the dummy walks over or hit an avoidable object: no walkable area, a citizen 
            {                           // or walks away from the park
                // Rewards
                case Constant.Tag.NO_WALKABLE:
                case Constant.Tag.CITIZEN:
                case Constant.Tag.VOID:
                    EndEpisode();           // not reached the goal, so no reward
                    break;
            }
        }


        //movement
        private void Movement(Vector3 move)
        {
            if (move.Equals(Vector3.zero))  // the dummy is still, reset the speed
            {
                _animationBlend = 0;
                _speed = 0;
            }
            else
            {   // is moving
                Vector3 inputDirection = move.normalized;
                _speed = Mathf.Lerp(_speed, _movementSpeed,
                    _accelerationSpeed * Time.fixedTime);
                // move and rotate the dummy in the movement direction
                Quaternion toRotation = Quaternion.LookRotation(inputDirection, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, RotationVelocity * Time.deltaTime); 
                _controller.Move(inputDirection * (_speed * Time.deltaTime));
                transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z); // reset the vertical position


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
        private bool CheckDistanceToDestination()
        {
            return detectionDistance >= Vector3.Distance(transform.position, _destinationPoint);                   
        }
        
    }
}
