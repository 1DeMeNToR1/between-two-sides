using Content.Server.Administration.Logs;
using Content.Shared.DoAfter;
using Content.Shared.Database;
using Content.Shared.Interaction.Events;
using Content.Shared.Teleportation.Systems;
using Robust.Server.Audio;
using Content.Shared.Siberia.AdvHolofan;

namespace Content.Server.Siberia.AdvHolofan;

public sealed class AdvHolofanSystem : EntitySystem
{
    [Dependency] private readonly IAdminLogManager _adminLogger = default!;
    [Dependency] private readonly LinkedEntitySystem _link = default!;
    [Dependency] private readonly AudioSystem _audio = default!;
    [Dependency] private readonly SharedDoAfterSystem _doafter = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<AdvHolofanComponent, UseInHandEvent>(OnUseInHand);
        SubscribeLocalEvent<AdvHolofanComponent, AdvHolofanDoAfterEvent>(OnDoAfter);
    }

    private void OnDoAfter(EntityUid uid, AdvHolofanComponent component, DoAfterEvent args)
    {
        if (args.Cancelled || args.Handled)
            return;

        HandleHolofanUpdating(uid, component, args.Args.User);

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
            HandleHolofanUpdating(uid, component, args.User);
        }
        else
        {
            var xform = Transform(args.User);
            if (xform.ParentUid != xform.GridUid)
                return;

            var doafterArgs = new DoAfterArgs(EntityManager, args.User, component.HolofanCreationDelay, new AdvHolofanDoAfterEvent(), uid, used: uid)
            {
                BreakOnDamage = true,
                BreakOnUserMove = true,
                MovementThreshold = 0.5f,
            };

            _doafter.TryStartDoAfter(doafterArgs);
        }
    }

    private void HandleHolofanUpdating(EntityUid uid, AdvHolofanComponent component, EntityUid user)
    {
        if (Deleted(user))
            return;

        var xform = Transform(user);

        if (Deleted(component.FirstHolofan) && Deleted(component.SecondHolofan) && Deleted(component.ThirdHolofan) && Deleted(component.FourthHolofan) && Deleted(component.FifthHolofan))
        {

            if (xform.ParentUid != xform.GridUid)
                return;

            component.FirstHolofan = Spawn(component.FirstHolofanPrototype, Transform(user).Coordinates);
            _adminLogger.Add(LogType.EntitySpawn, LogImpact.Low, $"{ToPrettyString(user):player} opened {ToPrettyString(component.FirstHolofan.Value)} at {Transform(component.FirstHolofan.Value).Coordinates} using {ToPrettyString(uid)}");
            _audio.PlayPvs(component.NewHolofanSound, uid);
        }
        else if (Deleted(component.SecondHolofan))
        {
            component.SecondHolofan = Spawn(component.SecondHolofanPrototype, Transform(user).Coordinates);
            _adminLogger.Add(LogType.EntitySpawn, LogImpact.Low, $"{ToPrettyString(user):player} opened {ToPrettyString(component.SecondHolofan.Value)} at {Transform(component.SecondHolofan.Value).Coordinates} linked to {ToPrettyString(component.FirstHolofan!.Value)} using {ToPrettyString(uid)}");
            _link.TryLink(component.FirstHolofan!.Value, component.SecondHolofan.Value, true);
            _audio.PlayPvs(component.NewHolofanSound, uid);
        }
        else if (Deleted(component.ThirdHolofan))
        {
            component.ThirdHolofan = Spawn(component.ThirdHolofanPrototype, Transform(user).Coordinates);
            _adminLogger.Add(LogType.EntitySpawn, LogImpact.Low, $"{ToPrettyString(user):player} opened {ToPrettyString(component.ThirdHolofan.Value)} at {Transform(component.ThirdHolofan.Value).Coordinates} linked to {ToPrettyString(component.SecondHolofan!.Value)} using {ToPrettyString(uid)}");
            _link.TryLink(component.SecondHolofan!.Value, component.ThirdHolofan.Value, true);
            _audio.PlayPvs(component.NewHolofanSound, uid);
        }
        else if (Deleted(component.FourthHolofan))
        {
            component.FourthHolofan = Spawn(component.FourthHolofanPrototype, Transform(user).Coordinates);
            _adminLogger.Add(LogType.EntitySpawn, LogImpact.Low, $"{ToPrettyString(user):player} opened {ToPrettyString(component.FourthHolofan.Value)} at {Transform(component.FourthHolofan.Value).Coordinates} linked to {ToPrettyString(component.ThirdHolofan!.Value)} using {ToPrettyString(uid)}");
            _link.TryLink(component.ThirdHolofan!.Value, component.FourthHolofan.Value, true);
            _audio.PlayPvs(component.NewHolofanSound, uid);
        }
        else if (Deleted(component.FifthHolofan))
        {
            component.FifthHolofan = Spawn(component.FifthHolofanPrototype, Transform(user).Coordinates);
            _adminLogger.Add(LogType.EntitySpawn, LogImpact.Low, $"{ToPrettyString(user):player} opened {ToPrettyString(component.FifthHolofan.Value)} at {Transform(component.FifthHolofan.Value).Coordinates} linked to {ToPrettyString(component.FourthHolofan!.Value)} using {ToPrettyString(uid)}");
            _link.TryLink(component.FourthHolofan!.Value, component.FifthHolofan.Value, true);
            _audio.PlayPvs(component.NewHolofanSound, uid);
        }
        else
        {
            QueueDel(component.FirstHolofan!.Value);
            QueueDel(component.SecondHolofan!.Value);
            QueueDel(component.ThirdHolofan!.Value);
            QueueDel(component.FourthHolofan!.Value);
            QueueDel(component.FifthHolofan!.Value);

            component.FirstHolofan = null;
            component.SecondHolofan = null;
            component.ThirdHolofan = null;
            component.FourthHolofan = null;
            component.FifthHolofan = null;
            _audio.PlayPvs(component.ClearHolofansSound, uid);
        }
    }
}
