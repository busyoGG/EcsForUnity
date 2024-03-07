
public abstract class Comp
{
    //public static int id { get; set; }
    //public static string name { get; set; }
    public int compId { get; set; }
    public string compName { get; set; }

    public Comp() { Reset(); }
    public abstract void Reset();
}
