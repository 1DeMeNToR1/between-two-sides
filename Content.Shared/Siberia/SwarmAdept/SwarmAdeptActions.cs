using Content.Shared.Actions;

namespace Content.Shared.Siberia.SwarmAdept;

public sealed partial class SwarmAdeptRaiseArmyActionEvent : InstantActionEvent
{

}

public sealed partial class SwarmAdeptDomainActionEvent : InstantActionEvent
{

}

public sealed partial class SwarmAdeptOrderActionEvent : InstantActionEvent
{
    /// <summary>
    /// The type of order being given
    /// </summary>
    [DataField("type")]
    public SwarmAdeptOrderType Type;
}
