
public class State
{
    // new tile
    public static int SPAWN_WATER = 1;
    public static int SPAWN_SAND  = 2;
    public static int SPAWN_LAND  = 3;
    public static int SPAWN_TOWN  = 4;

    // action
    public static int SPAWN_TEMPEST  = 5;

    public static Tile originTile;
    private State()
    { }
    public State getInsance { get; private set; }
    public static int state;

}
