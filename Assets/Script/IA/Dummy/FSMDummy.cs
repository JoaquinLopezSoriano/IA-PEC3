using Script.Game;
using Script.Input;
using UnityEngine;
using Random = UnityEngine.Random;

/*
 * https://www.youtube.com/watch?v=BJzYGsMcy8Q
 * https://www.youtube.com/watch?v=UJsaEVPntMg
 */

namespace Script.IA.Dummy
{
    public class FsmDummy : MonoBehaviour
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
   
        // script classes
        private Animator _animator;
        private CharacterController _controller;
        private AgentInputHandler _inputHandler;
    
        //variables
        private float _speed;
        private float _movementSpeed;       // max speed
        private float _accelerationSpeed;   // max acceleration
        private float _animationBlend;      // speed of the animation
        private Vector3 _destinationPoint;  // point to go to
        private const float RotationVelocity = 360;

        private void Awake()
        {
            // set components on the gameObject
            _animator = GetComponent<Animator>();
            _controller = GetComponent<CharacterController>();
            _inputHandler = GetComponent<AgentInputHandler>();
            // max speed between the walk and the sprint value
            _movementSpeed = Random.Range(walk, sprint);
            // max acceleration between half the acceleration and the acceleration value
            _accelerationSpeed = Random.Range(maxAcceleration / 2, maxAcceleration); 
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            Movement();
        }
        
         //movement
        private void Movement()
        {
            // converts move input to a world space vector based on our character's transform orientation
            Vector2 move = _inputHandler.GetMoveInput();
            
            if (move.Equals(Vector2.zero))
            {
                _animationBlend = 0;
                _speed = 0;
            }
            else
            {
                Vector3 inputDirection = new Vector3(move.x, 0.0f, move.y).normalized;
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
    }
}
