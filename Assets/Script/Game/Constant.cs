namespace Script.Game
{
    /*
     * Constant parameters
     */
    public static class Constant
    {
        // NavMesh name layers
        public struct NavMesh
        {
            // Walkable name navMesh layer
            public const string CITIZEN_WALK = "Walkable";
            public const string TAICHI_WALK = "Taichi";
            public enum AreaMask
            {
                Walkable,
                Taichi
            }

        }

        // Animation parameters
        public struct Animation
        {
            public static string ATTACK = "Attack";
            public static string FLEE = "Flee";
            public const string SPEED = "Speed";
            public const string WAIT = "Wait";
            public const string LEAVE = "Leave";
            public const string TAICHI = "Taichi";
            public const int MAX_TAICHI = 8;
            public enum TaichiMovements
            {
                Kicking,
                Double_Kicking,
                Knee_Kick_Lead,
                Fight_Idle,
                Punching_Left,
                Punching_Right,
                Quad_Punch,
                Elbow_Punching
            }
            public const string LOCOMOTION = "Locomotion";
        }
        
        // Collider Tags
        public struct Collider
        {
            public const string BENCH = "Bench";
        }
        
        public struct Tag
        {
            public const string TARGET = "Target";
            public const string VOID = "Void";
        }
   

    }
}
