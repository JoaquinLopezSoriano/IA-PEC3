using Script.Game;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Script.Flocking
{
    /*
     * Handles the behaviour of each element in the swarm
     */
    public class Flock : MonoBehaviour
    {

         private FlockingManager _flockingManager;  // swarm handler
         public FlockingManager FlockingManager
         {
             set => _flockingManager = value;
         }
         private Animator _animator;                // animator

        private float _speed;                       // element current speed
        private Bounds _bounds;                     // element's bounds
        private readonly int _boundsFactor = 2;     // bound's multiplier factor             
        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        // Start is called before the first frame update
        private void Start()
        {
            _speed = Random.Range(_flockingManager.MinSpeed, _flockingManager.MaxSpeed);    // set randomly the initial speed
            // set the element's bounds, its position is the handler swarm position, its bounds are the swarm bounds plus a multiplier factor
            _bounds = new Bounds(_flockingManager.transform.position, _flockingManager.SwarmLimits * _boundsFactor);    
        }

        // Update is called once per frame
        private void Update()
        {
            if (!_bounds.Contains(transform.position))  // if the element's position is not in its bounds, 
            {                                           // redirect its orientation and movement to its bounds
                Vector3 direction = _flockingManager.transform.position - transform.position;
                transform.rotation = Quaternion.Slerp(transform.rotation, 
                    Quaternion.LookRotation(direction), 
                    _flockingManager.RotationSpeed * Time.deltaTime);
            }
            else
            {
                if (_flockingManager.IsRandomValue())   // change the element's speed randomly
                {
                    _speed = Random.Range(_flockingManager.MinSpeed, _flockingManager.MaxSpeed);
                }

                if (_flockingManager.IsRandomValue())   // apply the flocking algorithm randomly
                {
                    ApplyRules();
                }
            }
            transform.Translate(0, 0, _speed * Time.deltaTime);     // move the element
            _animator.SetFloat(Constant.Animation.SPEED, _speed);   // animate the element
        }

        // flocking algorithm to the orientation on a swarm element
        //  orientation = group orientation + avoid orientation + group position
        private void ApplyRules()
        {
            GameObject[] gameObjects = _flockingManager.Swarm;

            Vector3 center = Vector3.zero;      // relative position of the swarm elements
            Vector3 avoid = Vector3.zero;       // position to avoid the neighbours swarm elements
            float groupSpeed = 0.01f;           // relative speed of the swarm elements
            int groupSize = 0;                  // swarm elements near it

            foreach (GameObject go in gameObjects)  // iterate through all the swarm elements to get the relative position and speed
            {
                if (go != gameObject)               // not take into account for the calculus itself
                {
                    float neighbourDistance = Vector3.Distance(go.transform.position, transform.position);  // get the distance to the element
                    if (neighbourDistance <= _flockingManager.MaxNeighbourDistance)                             // check if the element is too far
                    {
                        center += go.transform.position;                                                        // take the element's position into account to move along with it
                        groupSize++;                                                                            // increase the counter        

                        if (neighbourDistance < _flockingManager.MinNeighbourDistance)                          // check f the element is too near
                        {
                            avoid += (transform.position - go.transform.position);                              // take the element's position into account to avoid it    
                        }

                        groupSpeed += go.GetComponent<Flock>()._speed;                                          // take the element's speed into account to move along with it
                    }
                }
            }

            if (groupSize > 0)              // it the are swarm elements near it, calculate its new orientation
            {
                // get the related position taking into account its near elements, the goal position and its position
                center = center / groupSize + (_flockingManager.GoalPosition - transform.position); 
                _speed = groupSpeed / groupSize;            // get the related speed taking into account its near elements
                if (_speed > _flockingManager.MaxSpeed)     // checks if the speed exceeds its maximum speed
                {
                    _speed = _flockingManager.MaxSpeed;     // set its speed to its maximum value
                }

                Vector3 direction = (center - avoid) - transform.position;  // get the related direction taking into account its near elements
                if (direction != Vector3.zero)
                {                                                           // update its direction
                    transform.rotation = Quaternion.Slerp(transform.rotation, 
                        Quaternion.LookRotation(direction), 
                        _flockingManager.RotationSpeed * Time.deltaTime);
                }
            }

        }
    }
}
