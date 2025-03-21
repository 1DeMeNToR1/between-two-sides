using Content.Server.Humanoid;
using Content.Shared.Administration.Logs;
using Content.Shared.Cloning;
using Content.Shared.Cloning.Events;
using Content.Shared.Database;
using Content.Shared.Humanoid;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Roles.Jobs;
using Robust.Server.Containers;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Content.Server.Cloning
{
    public sealed class CloningSystem : EntitySystem
    {
        [Dependency] private readonly DeviceLinkSystem _signalSystem = default!;
        [Dependency] private readonly IPlayerManager _playerManager = null!;
        [Dependency] private readonly IPrototypeManager _prototype = default!;
        [Dependency] private readonly EuiManager _euiManager = null!;
        [Dependency] private readonly CloningConsoleSystem _cloningConsoleSystem = default!;
        [Dependency] private readonly HumanoidAppearanceSystem _humanoidSystem = default!;
        [Dependency] private readonly ContainerSystem _containerSystem = default!;
        [Dependency] private readonly MobStateSystem _mobStateSystem = default!;
        [Dependency] private readonly PowerReceiverSystem _powerReceiverSystem = default!;
        [Dependency] private readonly IRobustRandom _robustRandom = default!;
        [Dependency] private readonly AtmosphereSystem _atmosphereSystem = default!;
        [Dependency] private readonly TransformSystem _transformSystem = default!;
        [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
        [Dependency] private readonly PuddleSystem _puddleSystem = default!;
        [Dependency] private readonly ChatSystem _chatSystem = default!;
        [Dependency] private readonly SharedAudioSystem _audio = default!;
        [Dependency] private readonly IConfigurationManager _configManager = default!;
        [Dependency] private readonly MaterialStorageSystem _material = default!;
        [Dependency] private readonly PopupSystem _popupSystem = default!;
        [Dependency] private readonly SharedMindSystem _mindSystem = default!;
        [Dependency] private readonly MetaDataSystem _metaSystem = default!;
        [Dependency] private readonly SharedJobSystem _jobs = default!;

        public readonly Dictionary<MindComponent, EntityUid> ClonesWaitingForMind = new();
        public const float EasyModeCloningCost = 0.7f;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<CloningPodComponent, ComponentInit>(OnComponentInit);
            SubscribeLocalEvent<RoundRestartCleanupEvent>(Reset);
            SubscribeLocalEvent<BeingClonedComponent, MindAddedMessage>(HandleMindAdded);
            SubscribeLocalEvent<CloningPodComponent, PortDisconnectedEvent>(OnPortDisconnected);
            SubscribeLocalEvent<CloningPodComponent, AnchorStateChangedEvent>(OnAnchor);
            SubscribeLocalEvent<CloningPodComponent, ExaminedEvent>(OnExamined);
            SubscribeLocalEvent<CloningPodComponent, GotEmaggedEvent>(OnEmagged);
        }

        private void OnComponentInit(EntityUid uid, CloningPodComponent clonePod, ComponentInit args)
        {
            clonePod.BodyContainer = _containerSystem.EnsureContainer<ContainerSlot>(uid, "clonepod-bodyContainer");
            _signalSystem.EnsureSinkPorts(uid, CloningPodComponent.PodPort);
        }

        internal void TransferMindToClone(EntityUid mindId, MindComponent mind)
        {
            if (!_componentFactory.TryGetRegistration(componentName, out var componentRegistration))
            {
                Log.Error($"Tried to use invalid component registration for cloning: {componentName}");
                continue;
            }

            if (EntityManager.TryGetComponent(original, componentRegistration.Type, out var sourceComp)) // Does the original have this component?
            {
                if (HasComp(clone.Value, componentRegistration.Type)) // CopyComp cannot overwrite existing components
                    RemComp(clone.Value, componentRegistration.Type);
                CopyComp(original, clone.Value, sourceComp);
            }
        }

        var cloningEv = new CloningEvent(settings, clone.Value);
        RaiseLocalEvent(original, ref cloningEv); // used for datafields that cannot be directly copied

        // Add equipment first so that SetEntityName also renames the ID card.
        if (settings.CopyEquipment != null)
            CopyEquipment(original, clone.Value, settings.CopyEquipment.Value, settings.Whitelist, settings.Blacklist);

        // Copy storage on the mob itself as well.
        // This is needed for slime storage.
        CopyStorage(original, clone.Value, settings.Whitelist, settings.Blacklist);

        var originalName = Name(original);
        if (TryComp<NameModifierComponent>(original, out var nameModComp)) // if the originals name was modified, use the unmodified name
            originalName = nameModComp.BaseName;

        // This will properly set the BaseName and EntityName for the clone.
        // Adding the component first before renaming will make sure RefreshNameModifers is called.
        // Without this the name would get reverted to Urist.
        // If the clone has no name modifiers, NameModifierComponent will be removed again.
        EnsureComp<NameModifierComponent>(clone.Value);
        _metaData.SetEntityName(clone.Value, originalName);

        _adminLogger.Add(LogType.Chat, LogImpact.Medium, $"The body of {original:player} was cloned as {clone.Value:player}");
        return true;
    }

    /// <summary>
    ///     Copies the equipment the original has to the clone.
    ///     This uses the original prototype of the items, so any changes to components that are done after spawning are lost!
    /// </summary>
    public void CopyEquipment(Entity<InventoryComponent?> original, Entity<InventoryComponent?> clone, SlotFlags slotFlags, EntityWhitelist? whitelist = null, EntityWhitelist? blacklist = null)
    {
        if (!Resolve(original, ref original.Comp) || !Resolve(clone, ref clone.Comp))
            return;

            args.PushMarkup(Loc.GetString("cloning-pod-biomass", ("number", _material.GetMaterialAmount(uid, component.RequiredMaterial))));
        }

        public bool TryCloning(EntityUid uid, EntityUid bodyToClone, Entity<MindComponent> mindEnt, CloningPodComponent? clonePod, float failChanceModifier = 1)
        {
            if (!Resolve(uid, ref clonePod))
                return false;

            if (HasComp<ActiveCloningPodComponent>(uid))
                return false;

            var mind = mindEnt.Comp;
            if (ClonesWaitingForMind.TryGetValue(mind, out var clone))
            {
                if (EntityManager.EntityExists(clone) &&
                    !_mobStateSystem.IsDead(clone) &&
                    TryComp<MindContainerComponent>(clone, out var cloneMindComp) &&
                    (cloneMindComp.Mind == null || cloneMindComp.Mind == mindEnt))
                    return false; // Mind already has clone

                ClonesWaitingForMind.Remove(mind);
            }

            if (mind.OwnedEntity != null && !_mobStateSystem.IsDead(mind.OwnedEntity.Value))
                return false; // Body controlled by mind is not dead

            // Yes, we still need to track down the client because we need to open the Eui
            if (mind.UserId == null || !_playerManager.TryGetSessionById(mind.UserId.Value, out var client))
                return false; // If we can't track down the client, we can't offer transfer. That'd be quite bad.

            if (!TryComp<HumanoidAppearanceComponent>(bodyToClone, out var humanoid))
                return false; // whatever body was to be cloned, was not a humanoid

            if (!_prototype.TryIndex(humanoid.Species, out var speciesPrototype))
                return false;

            if (!TryComp<PhysicsComponent>(bodyToClone, out var physics))
                return false;

            var cloningCost = (int) Math.Round(physics.FixturesMass);

            if (_configManager.GetCVar(CCVars.BiomassEasyMode))
                cloningCost = (int) Math.Round(cloningCost * EasyModeCloningCost);

            // biomass checks
            var biomassAmount = _material.GetMaterialAmount(uid, clonePod.RequiredMaterial);

            if (biomassAmount < cloningCost)
            {
                if (clonePod.ConnectedConsole != null)
                    _chatSystem.TrySendInGameICMessage(clonePod.ConnectedConsole.Value, Loc.GetString("cloning-console-chat-error", ("units", cloningCost)), InGameICChatType.Speak, false);
                return false;
            }

            _material.TryChangeMaterialAmount(uid, clonePod.RequiredMaterial, -cloningCost);
            clonePod.UsedBiomass = cloningCost;
            // end of biomass checks

            // genetic damage checks
            if (TryComp<DamageableComponent>(bodyToClone, out var damageable) &&
                damageable.Damage.DamageDict.TryGetValue("Cellular", out var cellularDmg))
            {
                var chance = Math.Clamp((float) (cellularDmg / 100), 0, 1);
                chance *= failChanceModifier;

                if (cellularDmg > 0 && clonePod.ConnectedConsole != null)
                    _chatSystem.TrySendInGameICMessage(clonePod.ConnectedConsole.Value, Loc.GetString("cloning-console-cellular-warning", ("percent", Math.Round(100 - chance * 100))), InGameICChatType.Speak, false);

                if (_robustRandom.Prob(chance))
                {
                    UpdateStatus(uid, CloningPodStatus.Gore, clonePod);
                    clonePod.FailedClone = true;
                    AddComp<ActiveCloningPodComponent>(uid);
                    return true;
                }
            }
            // end of genetic damage checks

            var mob = Spawn(speciesPrototype.Prototype, _transformSystem.GetMapCoordinates(uid));
            _humanoidSystem.CloneAppearance(bodyToClone, mob);

            var ev = new CloningEvent(bodyToClone, mob);
            RaiseLocalEvent(bodyToClone, ref ev);

            if (!ev.NameHandled)
                _metaSystem.SetEntityName(mob, MetaData(bodyToClone).EntityName);

            var cloneMindReturn = EntityManager.AddComponent<BeingClonedComponent>(mob);
            cloneMindReturn.Mind = mind;
            cloneMindReturn.Parent = uid;
            _containerSystem.Insert(mob, clonePod.BodyContainer);
            ClonesWaitingForMind.Add(mind, mob);
            UpdateStatus(uid, CloningPodStatus.NoMind, clonePod);
            _euiManager.OpenEui(new AcceptCloningEui(mindEnt, mind, this), client);

            AddComp<ActiveCloningPodComponent>(uid);

            // TODO: Ideally, components like this should be components on the mind entity so this isn't necessary.
            // Add on special job components to the mob.
            if (_jobs.MindTryGetJob(mindEnt, out var prototype))
            {
                foreach (var special in prototype.Special)
                {
                    if (special is AddComponentSpecial)
                        special.AfterEquip(mob);
                }
            }

            return true;
        }

        public void UpdateStatus(EntityUid podUid, CloningPodStatus status, CloningPodComponent cloningPod)
        {
            cloningPod.Status = status;
            _appearance.SetData(podUid, CloningPodVisuals.Status, cloningPod.Status);
        }

        public override void Update(float frameTime)
        {
            var query = EntityQueryEnumerator<ActiveCloningPodComponent, CloningPodComponent>();
            while (query.MoveNext(out var uid, out var _, out var cloning))
            {
                if (!_powerReceiverSystem.IsPowered(uid))
                    continue;

                if (cloning.BodyContainer.ContainedEntity == null && !cloning.FailedClone)
                    continue;

                cloning.CloningProgress += frameTime;
                if (cloning.CloningProgress < cloning.CloningTime)
                    continue;

                if (cloning.FailedClone)
                    EndFailedCloning(uid, cloning);
                else
                    Eject(uid, cloning);
            }
        }

        /// <summary>
        /// On emag, spawns a failed clone when cloning process fails which attacks nearby crew.
        /// </summary>
        private void OnEmagged(EntityUid uid, CloningPodComponent clonePod, ref GotEmaggedEvent args)
        {
            if (!this.IsPowered(uid, EntityManager))
                return;

            _audio.PlayPvs(clonePod.SparkSound, uid);
            _popupSystem.PopupEntity(Loc.GetString("cloning-pod-component-upgrade-emag-requirement"), uid);
            args.Handled = true;
        }

        public void Eject(EntityUid uid, CloningPodComponent? clonePod)
        {
            if (!Resolve(uid, ref clonePod))
                return;

        var coords = Transform(target).Coordinates;

        // delete all items in the target storage
        _container.CleanContainer(target.Comp.Container);

        // recursively replace them
        foreach ((var itemUid, var itemLocation) in original.Comp.StoredItems)
        {
            var copy = CopyItem(itemUid, coords, whitelist, blacklist);
            if (copy != null)
                _storage.InsertAt(target, copy.Value, itemLocation, out _, playSound: false);
        }
    }
}
