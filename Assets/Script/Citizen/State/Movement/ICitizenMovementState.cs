namespace Script.Citizen.State.Movement
{
    /*
     * Movement state interface
     */
    public interface ICitizenMovementState
    {
        public void UpdateMovementState();  //Update the movement state
        public void ToIdleState();      // Transition to idle state
        public void ToWalkingState();   // Transition to walking state
        public void ToRunningState();   // Transition to running state
    }
}