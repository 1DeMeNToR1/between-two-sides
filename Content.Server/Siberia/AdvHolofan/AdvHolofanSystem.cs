using Robust.Server.Audio;
using Content.Shared.Siberia.AdvHolofan.Components;
using Content.Shared.Storage;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;

namespace Content.Server.Siberia.AdvHolofan;


public sealed partial class AdvHolofanSystem : SharedAdvHolofanSystem
{
    [Dependency] private readonly AudioSystem _audio = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    public override void Initialize()
    {
        base.Initialize();

        //SubscribeLocalEvent<AdvHolofanComponent, BeforeRangedInteractEvent>(OnBeforeInteract);
        //SubscribeLocalEvent<AdvHolofanComponent, UseInHandEvent>(OnUseInHand);
    }
    /*private void OnBeforeInteract(EntityUid uid, AdvHolofanComponent comp, BeforeRangedInteractEvent args)
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
    public override void OnUseInHand(EntityUid uid, AdvHolofanComponent comp, UseInHandEvent args)
    {
        base.OnUseInHand(uid, comp, args);
    }*/
}

