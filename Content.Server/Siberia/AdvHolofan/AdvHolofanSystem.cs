using Content.Shared.Examine;
using Content.Shared.Interaction;
using Robust.Server.Audio;
using Content.Shared.Siberia.AdvHolofan;
using Content.Shared.Storage;
using Content.Shared.Coordinates.Helpers;

namespace Content.Server.Siberia.AdvHolofan;

public sealed class AdvHolofanSystem : EntitySystem
{
    [Dependency] private readonly AudioSystem _audio = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;

    private readonly List<EntityUid> Signs = new();

    public override void Initialize()
    {
        SubscribeLocalEvent<AdvHolofanComponent, BeforeRangedInteractEvent>(OnBeforeInteract);
        SubscribeLocalEvent<AdvHolofanComponent, ExaminedEvent>(OnExamine);
    }

    private void OnExamine(EntityUid uid, AdvHolofanComponent comp, ExaminedEvent args)
    {
        using (args.PushGroup(nameof(AdvHolofanComponent)))
        {
            args.PushMarkup(Loc.GetString("actived-signs", ("count", Signs.Count)));

            if (Signs.Count == 0)
            {
                args.PushMarkup(Loc.GetString("max-holo-signs"));
            }
        }
    }


    private void OnBeforeInteract(EntityUid uid, AdvHolofanComponent component, BeforeRangedInteractEvent args)
    {
        if (args.Handled
            || !args.CanReach // prevent placing out of range
            || HasComp<StorageComponent>(args.Target) // if it's a storage component like a bag, we ignore usage so it can be stored
           )
            return;

        if (Signs.Count == component.MaxProjectors)
        {
            foreach (var holo in Signs)
            {
                QueueDel(holo);
            }
            Signs.Clear();
            _audio.PlayPvs(component.ClearHolofansSound, uid);
        }
        else
        {
            var holoUid = EntityManager.SpawnEntity(component.SignProto, args.ClickLocation.SnapToGrid(EntityManager));
            var xform = Transform(holoUid);
            if (!xform.Anchored)
                _transform.AnchorEntity(holoUid, xform); // anchor to prevent any tempering with (don't know what could even interact with it)

            args.Handled = true;

            _audio.PlayPvs(component.NewHolofanSound, uid);
            Signs.Add(holoUid);

        }
    }
}
