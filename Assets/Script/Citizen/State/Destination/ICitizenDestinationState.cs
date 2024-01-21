namespace Script.Citizen.State.Destination
{
    /*
     * Destination state interface
     */
    public interface ICitizenDestinationState
    {
        public void UpdateDestinationState();   //Update the destination state
        public void ToFindDestinationState();   // Transition to Find state
        public void ToTargetDestinationState();  // Transition to Target state
    }
}