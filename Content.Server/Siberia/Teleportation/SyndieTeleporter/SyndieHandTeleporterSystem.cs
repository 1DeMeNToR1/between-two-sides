using Content.Server.Administration.Logs;
using Content.Server.Popups;
using Content.Shared.DoAfter;
using Content.Shared.Database;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Content.Shared.Teleportation.Components;
using Content.Shared.Teleportation.Components.Syndie;
using Robust.Server.Audio;
using Robust.Server.GameObjects;

namespace Content.Server.Teleportation.SyndieTeleporter;

/// <summary>
/// This handles creating portals from a hand teleporter.
/// </summary>
public sealed class SyndieHandTeleporterSystem : EntitySystem
{
    [Dependency] private readonly IAdminLogManager _adminLogger = default!;
    [Dependency] private readonly AudioSystem _audio = default!;
    [Dependency] private readonly SharedDoAfterSystem _doafter = default!;
    [Dependency] private readonly PopupSystem _popup = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<SyndieHandTeleporterComponent, UseInHandEvent>(OnUseInHand);
        SubscribeLocalEvent<SyndieHandTeleporterComponent, TeleporterDoAfterEvent>(OnDoAfter);
    }

    private void OnDoAfter(EntityUid uid, SyndieHandTeleporterComponent component, DoAfterEvent args)
    {
        if (args.Cancelled || args.Handled)
            return;

        HandlePortalUpdating(uid, component, args.Args.User);

        args.Handled = true;
    }

    private void OnUseInHand(EntityUid uid, SyndieHandTeleporterComponent component, UseInHandEvent args)
    {
        if (Deleted(component.FirstPortal))
            component.FirstPortal = null;


        if (component.FirstPortal != null)
        {
            // handle removing portals immediately as opposed to a doafter
            HandlePortalUpdating(uid, component, args.User);
        }
        else
        {
            var xform = Transform(args.User);
            if (xform.ParentUid != xform.GridUid)
                return;

            var doafterArgs = new DoAfterArgs(EntityManager, args.User, component.PortalCreationDelay, new TeleporterDoAfterEvent(), uid, used: uid)
            {
                BreakOnDamage = true,
                BreakOnMove = true,
                MovementThreshold = 0.5f,
            };

            _doafter.TryStartDoAfter(doafterArgs);
        }
    }

    /// <summary>
    ///     Creates or removes a portal given the state of the hand teleporter.
    /// </summary>
    private void HandlePortalUpdating(EntityUid uid, SyndieHandTeleporterComponent component, EntityUid user)
    {
        if (Deleted(user))
            return;

        var xform = Transform(user);


        if (Deleted(component.FirstPortal))
        {
            // don't portal
            if (xform.ParentUid != xform.GridUid)
                return;

            var timeout = EnsureComp<PortalTimeoutComponent>(user);
            timeout.EnteredPortal = null;
            component.FirstPortal = Spawn(component.FirstPortalPrototype, Transform(user).Coordinates);
            _adminLogger.Add(LogType.EntitySpawn, LogImpact.Low, $"{ToPrettyString(user):player} opened {ToPrettyString(component.FirstPortal.Value)} at {Transform(component.FirstPortal.Value).Coordinates} using {ToPrettyString(uid)}");
            _audio.PlayPvs(component.NewPortalSound, uid);
        }
        else
        {

            FizzlePortals(uid, component, user, false);
        }
    }

    private void FizzlePortals(EntityUid uid, SyndieHandTeleporterComponent component, EntityUid user, bool instability)
    {
        // Logging
        var portalStrings = "";
        portalStrings += ToPrettyString(component.FirstPortal);
        if (portalStrings != "")
            _adminLogger.Add(LogType.EntityDelete, LogImpact.Low, $"{ToPrettyString(user):player} closed {portalStrings} with {ToPrettyString(uid)}");


        if (!Deleted(component.FirstPortal))
            QueueDel(component.FirstPortal.Value);

        component.FirstPortal = null;
        _audio.PlayPvs(component.ClearPortalsSound, uid);

        if (instability)
            _popup.PopupEntity(Loc.GetString("handheld-teleporter-instability-fizzle"), uid, user, PopupType.MediumCaution);
    }
}
