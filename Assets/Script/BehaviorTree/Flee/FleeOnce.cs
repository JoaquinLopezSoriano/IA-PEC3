using BBUnity.Actions;
using Pada1.BBCore;
using Pada1.BBCore.Tasks;

namespace Script.BehaviorTree.Flee
{
    
    [Action("MyActions/Flee")]
    [Help("Triggers when is near the target.")]
    public class FleeOnce : GOAction {
    
        // Event raised when sun rises or sets.
        public static event System.EventHandler OnFlee;
    
        // Main class method, invoked by the execution engine.
        public override TaskStatus OnUpdate()
        {
            // Trigger the flee
            if (OnFlee != null)
                OnFlee(this, System.EventArgs.Empty);
            return TaskStatus.COMPLETED;
        }
    }
}
