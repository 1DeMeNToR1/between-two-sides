using Content.Shared.Examine;
using Content.Shared.Interaction;
using Robust.Server.Audio;
using Content.Shared.Siberia.AdvHolofan;
using Content.Shared.Storage;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.Interaction.Events;

namespace Content.Server.Siberia.AdvHolofan;

public sealed class AdvHolofanSystem : EntitySystem
{
    [Dependency] private readonly AudioSystem _audio = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<AdvHolofanComponent, BeforeRangedInteractEvent>(OnBeforeInteract);
        SubscribeLocalEvent<AdvHolofanComponent, ExaminedEvent>(OnExamine);
        SubscribeLocalEvent<AdvHolofanComponent, UseInHandEvent>(OnUseInHand);
    }

    private void OnExamine(EntityUid uid, AdvHolofanComponent comp, ExaminedEvent args)
    {
        using (args.PushGroup(nameof(AdvHolofanComponent)))
        {
            args.PushMarkup(Loc.GetString("actived-signs", ("count", comp.Signs.Count)));

            if (comp.Signs.Count == 0)
            {
                args.PushMarkup(Loc.GetString("max-holo-signs"));
            }
        }
    }
    private void OnUseInHand(EntityUid uid, AdvHolofanComponent comp, UseInHandEvent args)
    {
        if (comp.Signs.Count > 0)
        {
            foreach (var holo in comp.Signs)
            {
                QueueDel(holo);
            }

            comp.Signs.Clear();
            _audio.PlayPvs(comp.ClearHolofansSound, uid);
        }

        args.Handled = true;
    }

    private void OnBeforeInteract(EntityUid uid, AdvHolofanComponent comp, BeforeRangedInteractEvent args)
    {
        if (args.Handled
            || !args.CanReach // prevent placing out of range
            || HasComp<StorageComponent>(args.Target) // if it's a storage component like a bag, we ignore usage so it can be stored
           )
            return;

        if (comp.Signs.Count == comp.MaxProjectors)
        {
            foreach (var holo in comp.Signs)
            {
                QueueDel(holo);
            }
            comp.Signs.Clear();
            _audio.PlayPvs(comp.ClearHolofansSound, uid);
        }

        var holoUid = EntityManager.SpawnEntity(comp.SignProto, args.ClickLocation.SnapToGrid(EntityManager));
        var xform = Transform(holoUid);
        if (!xform.Anchored)
            _transform.AnchorEntity(holoUid, xform); // anchor to prevent any tempering with (don't know what could even interact with it)

        args.Handled = true;

        _audio.PlayPvs(comp.NewHolofanSound, uid);
        comp.Signs.Add(holoUid);


    }
}
