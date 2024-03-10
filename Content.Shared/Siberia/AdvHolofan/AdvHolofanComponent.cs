using Content.Shared.DoAfter;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Siberia.AdvHolofan;

[RegisterComponent, NetworkedComponent]
public sealed partial class AdvHolofanComponent : Component
{
    [ViewVariables, DataField("firstHolofan")]
    public EntityUid? FirstHolofan = null;

    [ViewVariables, DataField("secondHolofan")]
    public EntityUid? SecondHolofan = null;

    [ViewVariables, DataField("thirdHolofan")]
    public EntityUid? ThirdHolofan = null;

    [ViewVariables, DataField("fourthHolofan")]
    public EntityUid? FourthHolofan = null;

    [ViewVariables, DataField("fifthHolofan")]
    public EntityUid? FifthHolofan = null;

    [DataField("firstHolofanPrototype", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string FirstHolofanPrototype = "AdvancedAtmosHoloFan";

    [DataField("secondHolofanPrototype", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string SecondHolofanPrototype = "AdvancedAtmosHoloFan";

    [DataField("thirdHolofanPrototype", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string ThirdHolofanPrototype = "AdvancedAtmosHoloFan";

    [DataField("fourthHolofanPrototype", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string FourthHolofanPrototype = "AdvancedAtmosHoloFan";

    [DataField("fifthHolofanPrototype", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string FifthHolofanPrototype = "AdvancedAtmosHoloFan";

    [DataField("newHolofanSound")] public SoundSpecifier NewHolofanSound =
        new SoundPathSpecifier("/Audio/Machines/anomaly_sync_connect.ogg")
        {
            Params = AudioParams.Default.WithVolume(-2f)
        };

    [DataField("clearHolofansSound")]
    public SoundSpecifier ClearHolofansSound = new SoundPathSpecifier("/Audio/Magic/Cults/ClockCult/steam_whoosh.ogg");

    [DataField("holofanCreationDelay")]
    public float HolofanCreationDelay = 1.0f;
}

[Serializable, NetSerializable]
public sealed partial class AdvHolofanDoAfterEvent : SimpleDoAfterEvent
{
}
