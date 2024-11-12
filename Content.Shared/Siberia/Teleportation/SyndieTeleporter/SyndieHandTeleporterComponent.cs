using Content.Shared.DoAfter;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Teleportation.Components.Syndie;

/// <summary>
///     Creates portals. Only one portal can be created and used.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class SyndieHandTeleporterComponent : Component
{
    [ViewVariables, DataField("firstPortal")]
    public EntityUid? FirstPortal = null;

    /// <summary>
    ///     Portals can't be placed on different grids?
    /// </summary>
    [DataField]
    public bool AllowPortalsOnDifferentGrids;

    [DataField("firstPortalPrototype", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string FirstPortalPrototype = "SyndiePortal";

    [DataField("newPortalSound")]
    public SoundSpecifier NewPortalSound =
        new SoundPathSpecifier("/Audio/Machines/high_tech_confirm.ogg")
        {
            Params = AudioParams.Default.WithVolume(-2f)
        };

    [DataField("clearPortalsSound")]
    public SoundSpecifier ClearPortalsSound = new SoundPathSpecifier("/Audio/Machines/button.ogg");

    /// <summary>
    ///     Delay for creating the portals in seconds.
    /// </summary>
    [DataField("portalCreationDelay")]
    public float PortalCreationDelay = 1.0f;
}
