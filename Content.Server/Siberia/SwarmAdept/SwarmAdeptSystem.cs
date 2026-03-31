using System.Numerics;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Chat.Systems;
using Content.Server.NPC;
using Content.Server.NPC.HTN;
using Content.Server.NPC.Systems;
using Content.Server.Popups;
using Content.Shared.Atmos;
using Content.Shared.Chat;
using Content.Shared.Dataset;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Content.Shared.Pointing;
using Content.Shared.Siberia.SwarmAdept;
using Robust.Server.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Random;

namespace Content.Server.Siberia.SwarmAdept
{
    /// <inheritdoc/>
    public sealed class SwarmAdeptSystem : SharedSwarmAdeptSystem
    {
        [Dependency] private readonly AtmosphereSystem _atmos = default!;
        [Dependency] private readonly ChatSystem _chat = default!;
        [Dependency] private readonly HTNSystem _htn = default!;
        [Dependency] private readonly HungerSystem _hunger = default!;
        [Dependency] private readonly NPCSystem _npc = default!;
        [Dependency] private readonly PopupSystem _popup = default!;
        [Dependency] private readonly TransformSystem _xform = default!;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<SwarmAdeptComponent, SwarmAdeptRaiseArmyActionEvent>(OnRaiseArmy);
            SubscribeLocalEvent<SwarmAdeptComponent, SwarmAdeptDomainActionEvent>(OnDomain);
            SubscribeLocalEvent<SwarmAdeptComponent, AfterPointedAtEvent>(OnPointedAt);
        }

        private void OnRaiseArmy(EntityUid uid, SwarmAdeptComponent component, SwarmAdeptRaiseArmyActionEvent args)
        {
            if (args.Handled)
                return;

            if (!TryComp<HungerComponent>(uid, out var hunger))
                return;

            if (_hunger.GetHunger(hunger) < component.HungerPerArmyUse)
            {
                _popup.PopupEntity(Loc.GetString("swarm-adept-low-energy"), uid, uid);
                return;
            }
            args.Handled = true;
            _hunger.ModifyHunger(uid, -component.HungerPerArmyUse, hunger);
            var servant = Spawn(component.ArmyMobSpawnId, Transform(uid).Coordinates);
            var comp = EnsureComp<SwarmDroneComponent>(servant);
            comp.King = uid;
            Dirty(servant, comp);

            component.Servants.Add(servant);
            _npc.SetBlackboard(servant, NPCBlackboard.FollowTarget, new EntityCoordinates(uid, Vector2.Zero));
            UpdateServantNpc(servant, component.CurrentOrder);
        }

        private void OnDomain(EntityUid uid, SwarmAdeptComponent component, SwarmAdeptDomainActionEvent args)
        {
            if (args.Handled)
                return;

            if (!TryComp<HungerComponent>(uid, out var hunger))
                return;

            //make sure the hunger doesn't go into the negatives
            if (_hunger.GetHunger(hunger) < component.HungerPerDomainUse)
            {
                _popup.PopupEntity(Loc.GetString("swarm-adept-low-energy"), uid, uid);
                return;
            }
            args.Handled = true;
            _hunger.ModifyHunger(uid, -component.HungerPerDomainUse, hunger);

            _popup.PopupEntity(Loc.GetString("swarm-adept-domain-popup"), uid);
            var tileMix = _atmos.GetTileMixture(uid, excite: true);
            tileMix?.AdjustMoles(Gas.CarbonDioxide, component.MolesCarbonDioxidePerDomain);
        }

        private void OnPointedAt(EntityUid uid, SwarmAdeptComponent component, ref AfterPointedAtEvent args)
        {
            if (component.CurrentOrder != SwarmAdeptOrderType.CheeseEm)
                return;

            foreach (var servant in component.Servants)
            {
                _npc.SetBlackboard(servant, NPCBlackboard.CurrentOrderedTarget, args.Pointed);
            }
        }

        public override void UpdateServantNpc(EntityUid uid, SwarmAdeptOrderType orderType)
        {
            base.UpdateServantNpc(uid, orderType);

            if (!TryComp<HTNComponent>(uid, out var htn))
                return;

            if (htn.Plan != null)
                _htn.ShutdownPlan(htn);

            _npc.SetBlackboard(uid, NPCBlackboard.CurrentOrders, orderType);
            _htn.Replan(htn);
        }

        public override void DoCommandCallout(EntityUid uid, SwarmAdeptComponent component)
        {
            base.DoCommandCallout(uid, component);

            if (!component.OrderCallouts.TryGetValue(component.CurrentOrder, out var datasetId) ||
                !PrototypeManager.TryIndex<DatasetPrototype>(datasetId, out var datasetPrototype))
                return;

            var msg = Random.Pick(datasetPrototype.Values);
            _chat.TrySendInGameICMessage(uid, msg, InGameICChatType.Speak, true);
        }
    }
}
