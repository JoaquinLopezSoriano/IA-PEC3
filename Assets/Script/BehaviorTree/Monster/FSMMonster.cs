using Script.Citizen;
using Script.Game;

namespace Script.BehaviorTree.Monster
{
    /*
    * Handle the behavior of the fled citizen
    * It inherit from FsmCitizen
    * Has the same behavior without the Destination State
    */
    public class FsmMonster : FsmCitizen
    {

        // Start is called before the first frame update
        private new void Start()
        {
            base.Start();
            CurrentDestinationState = null;
            AttackOnce.OnAttack += Attack;   // it trigger when is near a fled citizen
        }

        // Update is called once per frame
        private new void Update()
        {
            // set the speed at the animator controller
            Animator.SetFloat(Constant.Animation.SPEED, NavMeshAgent.speed);
            CurrentMovementState.UpdateMovementState();
        }

        /*
         * Triggers the attack animation at the animation controller
         */
        private void Attack(object sender, System.EventArgs e)
        {
            Animator.SetTrigger(Constant.Animation.ATTACK);
        }

    }
}
