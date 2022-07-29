using System;

public class Codition
{
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public string StartMessage { get; set; }

    public Action<Pokemon> OnStart { get; set; }
    public Func<Pokemon, bool> OnBeforMove { get; set; }
    public Action<Pokemon> OnAfterTurn { get; set; }
}
