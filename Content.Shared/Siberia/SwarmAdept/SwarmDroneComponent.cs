using Content.Shared.Siberia.SwarmAdept;
using Robust.Shared.GameStates;

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
}
