using Content.Server.Administration.Logs;
using Content.Server.Popups;
using Content.Shared.DoAfter;
using Content.Shared.Database;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Content.Shared.Siberia.AdvHolofan.Components;
using Content.Shared.Teleportation.Components;
using Content.Shared.Teleportation.Systems;
using Robust.Server.Audio;
using Robust.Server.GameObjects;

namespace Content.Server.Siberia.AdvHolofan;

/// <summary>
/// This handles creating portals from a hand teleporter.
/// </summary>
public sealed class AdvHolofanSystem : EntitySystem
{
    [Dependency] private readonly IAdminLogManager _adminLogger = default!;
    [Dependency] private readonly LinkedEntitySystem _link = default!;
    [Dependency] private readonly AudioSystem _audio = default!;
    [Dependency] private readonly SharedDoAfterSystem _doafter = default!;
    [Dependency] private readonly PopupSystem _popup = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<AdvHolofanComponent, UseInHandEvent>(OnUseInHand);
        SubscribeLocalEvent<AdvHolofanComponent, HolofanDoAfterEvent>(OnDoAfter);
    }

    private void OnDoAfter(EntityUid uid, AdvHolofanComponent component, DoAfterEvent args)
    {
        if (args.Cancelled || args.Handled)
            return;

        HolofanUpdating(uid, component, args.Args.User);

        args.Handled = true;
    }

    private void OnUseInHand(EntityUid uid, AdvHolofanComponent component, UseInHandEvent args)
    {
        if (Deleted(component.FirstHolofan))
            component.FirstHolofan = null;

        if (Deleted(component.SecondHolofan))
            component.SecondHolofan = null;

        if (Deleted(component.ThirdHolofan))
            component.ThirdHolofan = null;

        if (Deleted(component.FourthHolofan))
            component.FourthHolofan = null;

        if (Deleted(component.FifthHolofan))
            component.FifthHolofan = null;

        if (component.FirstHolofan != null && component.SecondHolofan != null && component.ThirdHolofan != null && component.FourthHolofan != null && component.FifthHolofan != null)
        {
            // handle removing portals immediately as opposed to a doafter
            HolofanUpdating(uid, component, args.User);
        }
        else
        {
            var xform = Transform(args.User);
            if (xform.ParentUid != xform.GridUid)
                return;

            var doafterArgs = new DoAfterArgs(EntityManager, args.User, component.HolofanCreationDelay, new HolofanDoAfterEvent(), uid, used: uid)
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
    private void HolofanUpdating(EntityUid uid, AdvHolofanComponent component, EntityUid user)
    {
        if (Deleted(user))
            return;

        var xform = Transform(user);

        // Create the first portal.
        if (Deleted(component.FirstHolofan) && Deleted(component.SecondHolofan) && Deleted(component.ThirdHolofan) && Deleted(component.FourthHolofan) && Deleted(component.FifthHolofan))
        {
            // don't portal
            if (xform.ParentUid != xform.GridUid)
                return;

            var timeout = EnsureComp<PortalTimeoutComponent>(user);
            timeout.EnteredPortal = null;
            component.FirstHolofan = Spawn(component.FirstHolofanPrototype, Transform(user).Coordinates);
            _adminLogger.Add(LogType.EntitySpawn, LogImpact.Low, $"{ToPrettyString(user):player} created {ToPrettyString(component.FirstHolofan.Value)} at {Transform(component.FirstHolofan.Value).Coordinates} using {ToPrettyString(uid)}");
            _audio.PlayPvs(component.NewHolofanSound, uid);
        }
        else if (Deleted(component.SecondHolofan))
        {
            if (xform.ParentUid != xform.GridUid) // Still, don't portal.
                return;

            if (!component.AllowPortalsOnDifferentGrids && xform.ParentUid != Transform(component.FirstHolofan!.Value).ParentUid)
            {
                // Whoops. Fizzle time. Crime time too because yippee I'm not refactoring this logic right now (I started to, I'm not going to.)
                FizzleHolofans(uid, component, user, true);
                return;
            }

            var timeout = EnsureComp<PortalTimeoutComponent>(user);
            timeout.EnteredPortal = null;
            component.SecondHolofan = Spawn(component.SecondHolofanPrototype, Transform(user).Coordinates);
            _adminLogger.Add(LogType.EntitySpawn, LogImpact.Low, $"{ToPrettyString(user):player} created {ToPrettyString(component.SecondHolofan.Value)} at {Transform(component.SecondHolofan.Value).Coordinates} linked to {ToPrettyString(component.FirstHolofan!.Value)} using {ToPrettyString(uid)}");
            _link.TryLink(component.FirstHolofan!.Value, component.SecondHolofan.Value, true);
            _audio.PlayPvs(component.NewHolofanSound, uid);
        }
        else if (Deleted(component.ThirdHolofan))
        {
            if (xform.ParentUid != xform.GridUid) // Still, don't portal.
                return;

            if (!component.AllowPortalsOnDifferentGrids && xform.ParentUid != Transform(component.FirstHolofan!.Value).ParentUid)
            {
                // Whoops. Fizzle time. Crime time too because yippee I'm not refactoring this logic right now (I started to, I'm not going to.)
                FizzleHolofans(uid, component, user, true);
                return;
            }

            var timeout = EnsureComp<PortalTimeoutComponent>(user);
            timeout.EnteredPortal = null;
            component.ThirdHolofan = Spawn(component.ThirdHolofanPrototype, Transform(user).Coordinates);
            _adminLogger.Add(LogType.EntitySpawn, LogImpact.Low, $"{ToPrettyString(user):player} created {ToPrettyString(component.ThirdHolofan.Value)} at {Transform(component.ThirdHolofan.Value).Coordinates} linked to {ToPrettyString(component.SecondHolofan!.Value)} using {ToPrettyString(uid)}");
            _link.TryLink(component.SecondHolofan!.Value, component.ThirdHolofan.Value, true);
            _audio.PlayPvs(component.NewHolofanSound, uid);
        }
        else if (Deleted(component.FourthHolofan))
        {
            if (xform.ParentUid != xform.GridUid) // Still, don't portal.
                return;

            if (!component.AllowPortalsOnDifferentGrids && xform.ParentUid != Transform(component.FirstHolofan!.Value).ParentUid)
            {
                // Whoops. Fizzle time. Crime time too because yippee I'm not refactoring this logic right now (I started to, I'm not going to.)
                FizzleHolofans(uid, component, user, true);
                return;
            }

            var timeout = EnsureComp<PortalTimeoutComponent>(user);
            timeout.EnteredPortal = null;
            component.FourthHolofan = Spawn(component.FourthHolofanPrototype, Transform(user).Coordinates);
            _adminLogger.Add(LogType.EntitySpawn, LogImpact.Low, $"{ToPrettyString(user):player} created {ToPrettyString(component.FourthHolofan.Value)} at {Transform(component.FourthHolofan.Value).Coordinates} linked to {ToPrettyString(component.ThirdHolofan!.Value)} using {ToPrettyString(uid)}");
            _link.TryLink(component.ThirdHolofan!.Value, component.FourthHolofan.Value, true);
            _audio.PlayPvs(component.NewHolofanSound, uid);
        }
        else if (Deleted(component.FifthHolofan))
        {
            if (xform.ParentUid != xform.GridUid) // Still, don't portal.
                return;

            if (!component.AllowPortalsOnDifferentGrids && xform.ParentUid != Transform(component.FirstHolofan!.Value).ParentUid)
            {
                // Whoops. Fizzle time. Crime time too because yippee I'm not refactoring this logic right now (I started to, I'm not going to.)
                FizzleHolofans(uid, component, user, true);
                return;
            }

            var timeout = EnsureComp<PortalTimeoutComponent>(user);
            timeout.EnteredPortal = null;
            component.FifthHolofan = Spawn(component.FifthHolofanPrototype, Transform(user).Coordinates);
            _adminLogger.Add(LogType.EntitySpawn, LogImpact.Low, $"{ToPrettyString(user):player} created {ToPrettyString(component.FifthHolofan.Value)} at {Transform(component.FifthHolofan.Value).Coordinates} linked to {ToPrettyString(component.FourthHolofan!.Value)} using {ToPrettyString(uid)}");
            _link.TryLink(component.FourthHolofan!.Value, component.FifthHolofan.Value, true);
            _audio.PlayPvs(component.NewHolofanSound, uid);
        }
        else
        {
            FizzleHolofans(uid, component, user, false);
        }
    }

    private void FizzleHolofans(EntityUid uid, AdvHolofanComponent component, EntityUid user, bool instability)
    {
        // Logging
        var holofanStrings = "";
        holofanStrings += ToPrettyString(component.FirstHolofan);
        if (holofanStrings != "")
            holofanStrings += " and ";
        holofanStrings += ToPrettyString(component.SecondHolofan);
        if (holofanStrings != "")
            holofanStrings += " and ";
        holofanStrings += ToPrettyString(component.ThirdHolofan);
        if (holofanStrings != "")
            holofanStrings += " and ";
        holofanStrings += ToPrettyString(component.FourthHolofan);
        if (holofanStrings != "")
            holofanStrings += " and ";
        holofanStrings += ToPrettyString(component.FifthHolofan);
        if (holofanStrings != "")
        _adminLogger.Add(LogType.EntityDelete, LogImpact.Low, $"{ToPrettyString(user):player} closed {holofanStrings} with {ToPrettyString(uid)}");

        // Clear both portals
        if (!Deleted(component.FirstHolofan))
            QueueDel(component.FirstHolofan.Value);
        if (!Deleted(component.SecondHolofan))
            QueueDel(component.SecondHolofan.Value);
        if (!Deleted(component.ThirdHolofan))
            QueueDel(component.ThirdHolofan.Value);
        if (!Deleted(component.FourthHolofan))
            QueueDel(component.FourthHolofan.Value);
        if (!Deleted(component.FifthHolofan))
            QueueDel(component.FifthHolofan.Value);

        component.FirstHolofan = null;
        component.SecondHolofan = null;
        component.ThirdHolofan = null;
        component.FourthHolofan = null;
        component.FifthHolofan = null;
        _audio.PlayPvs(component.ClearHolofansSound, uid);

        if (instability)
            _popup.PopupEntity(Loc.GetString("handheld-teleporter-instability-fizzle"), uid, user, PopupType.MediumCaution);
    }
}
