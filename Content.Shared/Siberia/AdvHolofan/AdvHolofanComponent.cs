using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Siberia.AdvHolofan;

[RegisterComponent, NetworkedComponent]
public sealed partial class AdvHolofanComponent : Component
{
    [DataField("newHolofanSound")] public SoundSpecifier NewHolofanSound =
        new SoundPathSpecifier("/Audio/Machines/anomaly_sync_connect.ogg")
        {
            Params = AudioParams.Default.WithVolume(-2f)
        };

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("signProto", customTypeSerializer:typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string SignProto = "HolosignWetFloor";

    public List<EntityUid> Signs = new();

    [DataField(required: true)]
    public int MaxProjectors = default!;

    [DataField("clearHolofansSound")]
    public SoundSpecifier ClearHolofansSound = new SoundPathSpecifier("/Audio/Magic/Cults/ClockCult/steam_whoosh.ogg");
}
