public class State
{
    // RAZ
    public const int EMPTY = 0;
    // new tile
    public const int SPAWN_WATER = 1;
    public const int SPAWN_SAND  = 2;
    public const int SPAWN_LAND  = 3;
    public const int SPAWN_TOWN  = 4;

    // action
    public const int SPAWN_TEMPEST  = 5;
    
    public static Tile storm_start_tile;
    public static int action_to_perform;
    
    private State()
    { }
    public State getInsance { get; private set; }
}
