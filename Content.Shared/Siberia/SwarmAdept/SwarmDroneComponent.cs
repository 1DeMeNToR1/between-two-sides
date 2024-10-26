using Content.Shared.Siberia.SwarmAdept;
using Content.Shared.StatusIcon;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Siberia.SwarmAdept;

[RegisterComponent, NetworkedComponent, Access(typeof(SharedSwarmAdeptSystem))]
[AutoGenerateComponentState]
public sealed partial class SwarmDroneComponent : Component
{
    /// <summary>
    /// The king this rat belongs to.
    /// </summary>
    [DataField("king")]
    [AutoNetworkedField]
    public EntityUid? King;

    [DataField("swarmStatusIcon")]
    public ProtoId<FactionIconPrototype> StatusIcon { get; set; } = "SwarmFaction";
}
