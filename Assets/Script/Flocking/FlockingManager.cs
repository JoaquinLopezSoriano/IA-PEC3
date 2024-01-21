using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Script.Flocking
{
/* 
 * Handles the behaviour of a swarm
 * The swarm bounds are set at the _swarmLimits variable and defined with:
 *      height -> as parameter
 *      width and depth -> the nav mesh obstacle 3D geometry  
 */
    public class FlockingManager : MonoBehaviour
    {
        [Header("Settings")] 
        [SerializeField] private GameObject beePrefab;  // the model use in the swarm
        [SerializeField][Range(0, 50)] private int swarmNumber = 20;        // total number of elements in the swarm
        [SerializeField][Range(0.5f, 3)] private float swarmHeight= 1.5f;   // height of the swarm
        
        [Header("Movement")] 
        [SerializeField][Range(0, 4)] private float minSpeed= 0.5f;     // minimum speed of the elements in the swarm
        public float MinSpeed => minSpeed;
    
        [SerializeField][Range(1, 5)] private float maxSpeed = 3;   // maximum speed of the elements in the swarm
        public float MaxSpeed => maxSpeed;
    
        [SerializeField][Range(0, 2)] private float minNeighbourDistance = 1;   // minimum distance between the elements in the swarm
        public float MinNeighbourDistance => minNeighbourDistance;
        
        [SerializeField][Range(1, 10)] private float maxNeighbourDistance = 10;  // maximum distance between the elements in the swarm
        public float MaxNeighbourDistance => maxNeighbourDistance;
        
        [SerializeField][Range(1, 5)] private float rotationSpeed = 2;  // rotation speed of the elements in the swarm
        public float RotationSpeed => rotationSpeed;
        
        [Header("Randomness")] 
        [SerializeField][Range(0.1f, 100)] private float randomRatio = 10;  // ratio of an number included in a gap
        [SerializeField][Range(50, 500)] private float maxRatio = 100;  // maximum value of the random gap 
        
        
        // private variables
        private GameObject[] _swarm;    // list of the elements in the swarm
        public GameObject[] Swarm => _swarm;
        
        private Vector3 _swarmLimits;   // bounds of the swarm
        public Vector3 SwarmLimits => _swarmLimits;
        
        private Vector3 _goalPosition = Vector3.zero;   // position to reach of the elements in the swarm
        public Vector3 GoalPosition => _goalPosition;

        private NavMeshObstacle _navMeshObstacle;   // obstacle to avoid on the navigation system, bounds set with a 3D geometry

        // Initializes values
        private void Awake()
        {
            _navMeshObstacle = GetComponent<NavMeshObstacle>(); // get the NavMeshObstacle component
            if (swarmHeight > _navMeshObstacle.height)          // checks if the height of the swarm is in bounds, is less than the NavMeshObstacle height 3D geometry
            {
                swarmHeight = _navMeshObstacle.height;          // if the swarmHeight value is greater, set to the 3D geometry height
            }
            _swarmLimits = new Vector3(_navMeshObstacle.radius, swarmHeight, _navMeshObstacle.radius);  // set the swarm bounds related to the NavMeshObstacle 3D geometry
        }

        // Start is called before the first frame update
        private void Start()
        {
            _swarm = new GameObject[swarmNumber];           // creates an array with a lenght of the swarmNumber
            for (int i = 0; i < _swarm.Length; i++)         // creates an element of the swarm in a random position inside the swarm bounds
            {
                Vector3 position = transform.position +
                                   new Vector3(Random.Range(-_swarmLimits.x, _swarmLimits.x),
                                       Random.Range(-_swarmLimits.y, _swarmLimits.y),
                                       Random.Range(-_swarmLimits.z, _swarmLimits.z));
                 GameObject bee = Instantiate(beePrefab, position, Quaternion.identity);
                 bee.GetComponent<Flock>().FlockingManager = this;  
                 bee.transform.parent = transform;          // set the swarm handler the parent of the element
                 _swarm[i] = bee;
            }

            _goalPosition = transform.position;
        }

        // Update is called once per frame
        private void Update()
        {
            if (IsRandomValue())                        // set a random position inside the swarm bounds
            {                                           // to reach to all the elements in the swarm
                _goalPosition = transform.position +
                               new Vector3(Random.Range(-_swarmLimits.x, _swarmLimits.x),
                                   Random.Range(-_swarmLimits.y, _swarmLimits.y),
                                   Random.Range(-_swarmLimits.z, _swarmLimits.z));
            }
        }

        // checks if a random value is included in the gap 
        public bool IsRandomValue()
        {
            return Random.Range(0, maxRatio) < randomRatio;
        }
    }
}
