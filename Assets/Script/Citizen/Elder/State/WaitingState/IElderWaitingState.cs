using Script.Citizen.State.Movement;

namespace Script.Citizen.Elder.State.WaitingState
{
    /*
     * Waiting state interface, inherits from  the movement state interface
     */
    public interface IElderWaitingState : ICitizenMovementState
    {
        public void ToDelayWaitingState();  // Transition to delay state
    }
}