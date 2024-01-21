using Script.Citizen;
using Script.Game;

namespace Script.BehaviorTree.Flee
{
    /*
     * Handle the behavior of the fled citizen
     * It inherit from FsmCitizen
     * Has the same behavior without the Destination State
     */
    public class FsmFled: FsmCitizen
    {
        // Start is called before the first frame update
        private new void Start()
        {
            base.Start();
            CurrentDestinationState = null;
            FleeOnce.OnFlee += Flee;            // it trigger when is near a monster
        }

        // Update is called once per frame
        private new void Update()
        {
            // set the speed at the animator controller
            Animator.SetFloat(Constant.Animation.SPEED, NavMeshAgent.speed);
            CurrentMovementState.UpdateMovementState();
        }

        /*
         * Triggers the flee animation at the animation controller and doubles them speed to fred quicker
         */
        private void Flee(object sender, System.EventArgs e)
        {
            Animator.SetTrigger(Constant.Animation.FLEE);
            NavMeshAgent.speed *= 2;
        }
    }
}