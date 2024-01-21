using Script.Game;
using Script.Input;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;

/*
 * https://www.youtube.com/watch?v=Dj-BsYtANE0
 * https://www.youtube.com/watch?v=RANRz9oyzko
 * https://www.youtube.com/watch?v=zPFU30tbyKs&list=PLzDRvYVwl53vehwiN_odYJkPBzcqFw110
 * https://github.com/Unity-Technologies/ml-agents/blob/release_10/docs/Training-ML-Agents.md
 * https://github.com/Unity-Technologies/ml-agents/blob/develop/docs/Installation.md
 * https://stackoverflow.com/questions/70986821/error-could-not-find-a-version-that-satisfies-the-requirement-torch-from-versi
 * https://stackoverflow.com/questions/68357090/getting-an-error-saying-could-not-build-wheels-for-numpy-which-use-pep-517-and
 * https://stackoverflow.com/questions/77526956/could-not-build-wheels-for-numpy-when-installing-mlagents
 * https://stackoverflow.com/questions/64038673/could-not-build-wheels-for-which-use-pep-517-and-cannot-be-installed-directly
 */

namespace Script.IA.RollingAgent
{
    /*
     * Handles the behavior of the RollerAgent and sets the implementation of the agent
     */
    public class RollerAgent : Agent
    {
        [Header("Movement")]
        // the objective of the agent, the gameObject to reach
        [SerializeField] private Transform target;
        // the force applied to move it
        [SerializeField] [Range(1f, 20f)] private float forceMultiplier = 10;
        // how near to the target is has to be to reach it
        // [SerializeField] [Range(0.5f, 2f)] private float minDistanceTarget = 1.5f;
        
        //properties
        // the rigid body
        private Rigidbody _rBody;
        // the input key manager
        private AgentInputHandler _agentInputHandler;
    
        // Start is called before the first frame update
        void Start()
        {
            // sets an instance of the components
            _agentInputHandler = GetComponent<AgentInputHandler>();
            _rBody = GetComponent<Rigidbody>();
        }
    
        /*
         * When the agent reaches the target or falls ->
         * Reset the agent position and the target properties
         */
        public override void OnEpisodeBegin()
        {
            // If the Agent fell, zero its momentum
            if (transform.localPosition.y < 0)
            {
                _rBody.angularVelocity = Vector3.zero;
                _rBody.velocity = Vector3.zero;
                transform.localPosition = new Vector3( 0, 0.5f, 0);
            }

            // Move the target to a new spot
            target.localPosition = new Vector3(Random.value * 8 - 4,
                0.5f,
                Random.value * 8 - 4);
        }

        /*
         * the information we collect for the training of the IA
         */
        public override void CollectObservations(VectorSensor sensor)
        {
            // Target and Agent positions
            sensor.AddObservation(target.localPosition);
            sensor.AddObservation(transform.localPosition);

            // Agent velocity
            sensor.AddObservation(_rBody.velocity.x);
            sensor.AddObservation(_rBody.velocity.z);

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
            _rBody.AddForce(controlSignal * forceMultiplier);

            // Rewards
            // float distanceToTarget = Vector3.Distance(this.transform.localPosition, target.localPosition);
            // // Reached target ++ positive reward
            // if (distanceToTarget < minDistanceTarget)
            // {
            //     SetReward(1.0f);
            //     EndEpisode();
            // }  
            // // Fell off platform -- negative reward
            // if (transform.localPosition.y < 0)
            // {
            //     EndEpisode();
            // }
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
            Debug.Log(_rBody.velocity);

        }

        /*
         * Triggers when the object hits another one
         * Set the rewards according to the object hitted
         */
        private void OnCollisionEnter(Collision collision)
        {
            switch (collision.transform.tag)
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
    }
}
