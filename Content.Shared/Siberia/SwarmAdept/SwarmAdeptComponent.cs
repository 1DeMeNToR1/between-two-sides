using Content.Shared.Siberia.SwarmAdept;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Siberia.SwarmAdept;

[RegisterComponent, NetworkedComponent, Access(typeof(SharedSwarmAdeptSystem))]
[AutoGenerateComponentState]
public sealed partial class SwarmAdeptComponent : Component
{
    [DataField("actionRaiseArmy", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string ActionRaiseArmy = "ActionSwarmAdeptRaiseArmy";

    /// <summary>
    ///     The action for the Raise Army ability
    /// </summary>
    [DataField("actionRaiseArmyEntity")]
    public EntityUid? ActionRaiseArmyEntity;

    /// <summary>
    ///     The amount of hunger one use of Raise Army consumes
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField("hungerPerArmyUse", required: true)]
    public float HungerPerArmyUse = 25f;

    /// <summary>
    ///     The entity prototype of the mob that Raise Army summons
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField("armyMobSpawnId", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string ArmyMobSpawnId = "MobRatServant";

    [DataField("actionDomain", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string ActionDomain = "ActionSwarmAdeptDomain";

    /// <summary>
    ///     The action for the Domain ability
    /// </summary>
    [DataField("actionDomainEntity")]
    public EntityUid? ActionDomainEntity;

    /// <summary>
    ///     The amount of hunger one use of Domain consumes
    /// </summary>
    [DataField("hungerPerDomainUse", required: true), ViewVariables(VVAccess.ReadWrite)]
    public float HungerPerDomainUse = 50f;

    /// <summary>
    ///     How many moles of ammonia are released after one us of Domain
    /// </summary>
    [DataField("molesCarbonDioxidePerDomain"), ViewVariables(VVAccess.ReadWrite)]
    public float MolesCarbonDioxidePerDomain = 200f;

    /// <summary>
    /// The current order that the Rat King assigned.
    /// </summary>
    [DataField("currentOrders"), ViewVariables(VVAccess.ReadWrite)]
    [AutoNetworkedField]
    public SwarmAdeptOrderType CurrentOrder = SwarmAdeptOrderType.Loose;

    /// <summary>
    /// The servants that the rat king is currently controlling
    /// </summary>
    [DataField("servants")]
    public HashSet<EntityUid> Servants = new();

    [DataField("actionOrderStay", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string ActionOrderStay = "ActionSwarmAdeptOrderStay";

    [DataField("actionOrderStayEntity")]
    public EntityUid? ActionOrderStayEntity;

    [DataField("actionOrderFollow", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string ActionOrderFollow = "ActionSwarmAdeptOrderFollow";

    [DataField("actionOrderFollowEntity")]
    public EntityUid? ActionOrderFollowEntity;

    [DataField("actionOrderCheeseEm", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string ActionOrderCheeseEm = "ActionSwarmAdeptOrderCheeseEm";

    [DataField("actionOrderCheeseEmEntity")]
    public EntityUid? ActionOrderCheeseEmEntity;

    [DataField("actionOrderLoose", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string ActionOrderLoose = "ActionSwarmAdeptOrderLoose";

    [DataField("actionOrderLooseEntity")]
    public EntityUid? ActionOrderLooseEntity;

    /// <summary>
    /// A dictionary with an order type to the corresponding callout dataset.
    /// </summary>
    [DataField("orderCallouts")]
    public Dictionary<SwarmAdeptOrderType, string> OrderCallouts = new()
    {
        { SwarmAdeptOrderType.Stay, "SwarmAdeptCommandStay" },
        { SwarmAdeptOrderType.Follow, "SwarmAdeptCommandFollow" },
        { SwarmAdeptOrderType.CheeseEm, "SwarmAdeptCommandCheeseEm" },
        { SwarmAdeptOrderType.Loose, "SwarmAdeptCommandLoose" }
    };
}

[Serializable, NetSerializable]
public enum SwarmAdeptOrderType : byte
{
    Stay,
    Follow,
    CheeseEm,
    Loose
}
