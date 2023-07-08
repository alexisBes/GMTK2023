
public class State
{
    // new tile
    public static int SPAWN_WATER = 0x01;
    public static int SPAWN_SAND= 0x02;
    public static int SPAWN_LAND= 0x03;
    public static int SPAWN_TOWN= 0x04;

    // action
    public static int SPAWN_TEMPEST  = 0x01;
    
    // MIX
    public static int MIX_WITH_SAND  = 0x02;
    public static int MIX_WITH_WATER  = 0x04;


    private State()
    { }
    public State getInsance { get; private set; }
    public static int state;

}
