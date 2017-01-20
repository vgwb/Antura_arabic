using ModularFramework.Modules;

/// <summary>
/// Serializable options not related to a specific player profile.
/// </summary>
// refactor: what namespace to use for this?
public class GlobalOptions : ModularFramework.Modules.GlobalOptions
{
    public int LastActivePlayerId;
}

