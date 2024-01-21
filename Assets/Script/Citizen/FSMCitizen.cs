using System.Collections;
using Script.Citizen.State.Destination;
using Script.Citizen.State.Movement;
using Script.Game;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;


namespace Script.Citizen
{
    /*
     * Citizen movement state enumeration
     */
    internal enum CitizenMovementState
    {
        IdleState,
        WalkingState,
        RunningState
    }
    /*
     * Handles the behaviour of a citizen using the FSM pattern
     * The citizen wander through the walkable areas of the scene
     */
    public class FsmCitizen : MonoBehaviour
    {
        // Fields
        [Header("Movement")] 
        [Tooltip("The speed at which the npc walks")] [SerializeField] [Range(0, 1.5f)]
        protected float walk = 1f;
        public float Walk => walk;
        
        [Tooltip("The speed at which the npc runs")] [SerializeField] [Range(0, 4)]
        protected float sprint = 3f;
        public float Sprint => sprint;
        
        [Tooltip("Max speed of movement transitions")] [Range(0, 20)]
        [SerializeField] private float maxAcceleration = 10f;

        [Tooltip("The max distance to move")] [SerializeField] [Range(0, 15)]
        protected float movementDistance = 7f;
        public float MovementDistance => movementDistance;

        [Tooltip("The max distance for detecting the destination point")] [SerializeField] [Range(0, 2)]
        protected float detectionDistance = 1f;
        public float DetectionDistance => detectionDistance;

        [SerializeField] private CitizenMovementState citizenMovementState = CitizenMovementState.IdleState;
        [FormerlySerializedAs("areaMask")] [SerializeField] private Constant.NavMesh.AreaMask areaMaskName = Constant.NavMesh.AreaMask.Walkable;


        // script classes
        private NavMeshAgent _navMeshAgent;
        public NavMeshAgent NavMeshAgent => _navMeshAgent;
        private Animator _animator;
        public Animator Animator => _animator;

        // private CapsuleCollider _capsuleCollider;

        //variables
        private float _movementSpeed;       // max speed
        private Vector3 _destinationPoint;  // point to go to
        public Vector3 DestinationPoint
        {
            get => _destinationPoint;
            set => _destinationPoint = value;
        }
        
        // Citizen states
        // Movement states
        private ICitizenMovementState _currentMovementState;
        public ICitizenMovementState CurrentMovementState
        {
            get => _currentMovementState;
            set => _currentMovementState = value;
        }
        private IdleMovementState _idleMovementState;
        public IdleMovementState IdleMovementState => _idleMovementState;

        private WalkingMovementState _walkingMovementState;
        public WalkingMovementState WalkingMovementState => _walkingMovementState;

        private RunningMovementState _runningMovementState;
        public RunningMovementState RunningMovementState => _runningMovementState;
        
        // Destination states
         private ICitizenDestinationState _currentDestinationState;
        public ICitizenDestinationState CurrentDestinationState
        {
            set => _currentDestinationState = value;
        }
        private FindDestinationState _findDestinationState;
        public FindDestinationState FindDestinationState => _findDestinationState;

        protected TargetDestinationState _targetDestinationState;
        public TargetDestinationState TargetDestinationState => _targetDestinationState;

        // Initializes values
        protected void Awake()
        {
            _idleMovementState = new IdleMovementState(this);
            _walkingMovementState = new WalkingMovementState(this);
            _runningMovementState = new RunningMovementState(this);
            _findDestinationState = new FindDestinationState(this, areaMaskName);
            _targetDestinationState = new TargetDestinationState(this);
            _navMeshAgent = GetComponent<NavMeshAgent>();
            // _capsuleCollider = GetComponent<CapsuleCollider>();
            _animator = GetComponent<Animator>();
            // max speed between the walk and the sprint value
            _movementSpeed = Random.Range(walk, sprint);
            // max aceleration between half the acceleration and the acceleration value
            _navMeshAgent.acceleration = Random.Range(maxAcceleration / 2, maxAcceleration); 

        }
        
        // Start is called before the first frame update
        protected void Start()
        {
            // set the initial movement state and its speed
            switch (citizenMovementState)
            {
                case CitizenMovementState.IdleState:
                    _navMeshAgent.speed = 0;
                    _currentMovementState = _idleMovementState;
                    break;
                case CitizenMovementState.WalkingState:
                    _navMeshAgent.speed = walk;
                    _currentMovementState = _walkingMovementState;
                    break;
                case CitizenMovementState.RunningState:
                    _navMeshAgent.speed = sprint;
                    _currentMovementState = _runningMovementState;
                    break;
            }
            // set the initial destination state
            CurrentDestinationState = _findDestinationState;
        }

        // Update is called once per frame
        protected void Update()
        {
            // set the speed at the animator controller
            _animator.SetFloat(Constant.Animation.SPEED, _navMeshAgent.speed);
            _currentMovementState.UpdateMovementState();
            _currentDestinationState.UpdateDestinationState();
        }

        // Obtain a maximum speed
        public void GetMovementSpeed()
        {
            _movementSpeed = Random.Range(walk, sprint);
        }

        // increase the citizen's speed up to the movementSpeed value
        public void IncreaseMovementSpeed()
        {
            _navMeshAgent.speed = Mathf.Lerp(_navMeshAgent.speed, _movementSpeed,
                _navMeshAgent.acceleration * Time.fixedTime);
        }
        
            
        // Launch a coroutine 
        public void LaunchCoroutine(IEnumerator functionDelegate)
        {
            StartCoroutine(functionDelegate);
        }
    }
}
