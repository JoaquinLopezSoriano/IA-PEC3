using Script.Citizen.Elder.State.WaitingState;
using Script.Game;
using UnityEngine;

namespace Script.Citizen.Elder
{
    /*
     * Handles the behaviour an elder citizen using the FSM pattern. Inherits from  the citizen handler
     * The elder citizen stops near a bench while wander through the walkable areas of the scene
     */
    public class FsmElder : FsmCitizen
    {
        [Header("Waiting")]
        [Tooltip("The minimum waiting time")] [SerializeField] [Range(0, 5)]
        protected float minWaitingTime = 0.5f;
        public float MinWaitingTime => minWaitingTime;
        
        [Tooltip("The maximum waiting time")] [SerializeField] [Range(0, 5)]
        protected float maxWaitingTime = 2f;
        public float MaxWaitingTime => maxWaitingTime;

        [Tooltip("The maximum passed by time until waiting again")] [SerializeField] [Range(1, 15)]
        protected float waitingPreventionTime = 5f;

        // [Tooltip("The capsule collider radius")] [SerializeField] [Range(1, 2)]
        // protected float colliderRadius = 1.5f;
        
        //variables
        private float _lastTimeWaited; // the amount of time passed by until waiting again near a bench
        // Citizen states
        // Movement states
        private DelayWaitingState _delayWaitingState;
        public DelayWaitingState DelayWaitingState => _delayWaitingState;


        // Initializes values
        private new void Awake()
        {
            base.Awake();
            _delayWaitingState = new DelayWaitingState(this);
            // _capsuleCollider.radius = colliderRadius;
        }

        // Start is called before the first frame update
        private new void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        private new void Update()
        {
            base.Update();

        }

        // checks if the elder citizen is near a bench
        private void OnTriggerEnter(Collider other)
        {
            if (Time.time >= _lastTimeWaited && other.tag.Equals(Constant.Tag.BENCH))
            {
                _delayWaitingState.ToDelayWaitingState();
            }
        }

        // Update the passed by time until waiting again near a bench
        public void ResetWaitingTime()
        {
            _lastTimeWaited = Time.time + waitingPreventionTime;
        }

        
    }
}
