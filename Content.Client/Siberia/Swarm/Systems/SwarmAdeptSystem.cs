using Content.Shared.Siberia.SwarmAdept;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.Prototypes;

namespace Content.Client.Siberia.Swarm.Systems;

public sealed class SwarmAdeptSystem : SharedSwarmAdeptSystem
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<SwarmAdeptComponent, GetStatusIconsEvent>(GetSwarmAdeptIcon);
        SubscribeLocalEvent<SwarmDroneComponent, GetStatusIconsEvent>(GetSwarmDroneIcon);
    }
    private void GetSwarmAdeptIcon(Entity<SwarmAdeptComponent> ent, ref GetStatusIconsEvent args)
    {
        var iconPrototype = _prototype.Index(ent.Comp.StatusIcon);
        args.StatusIcons.Add(iconPrototype);
    }

    private void GetSwarmDroneIcon(Entity<SwarmDroneComponent> ent, ref GetStatusIconsEvent args)
    {
        if (HasComp<SwarmAdeptComponent>(ent))
            return;

        var iconPrototype = _prototype.Index(ent.Comp.StatusIcon);
        args.StatusIcons.Add(iconPrototype);
    }

}
