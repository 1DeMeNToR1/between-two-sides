using Content.Shared.Interaction;
using Content.Shared.Siberia.AdvHolofan.Components;
using Content.Shared.Storage;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.Interaction.Events;
using JetBrains.Annotations;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Timing;

namespace Content.Shared.Siberia.AdvHolofan.Components;
[UsedImplicitly]
public abstract class SharedAdvHolofanSystem : EntitySystem
{
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<AdvHolofanComponent, BeforeRangedInteractEvent>(OnBeforeInteract);
        SubscribeLocalEvent<AdvHolofanComponent, UseInHandEvent>(OnUseInHand);
    }

    public String GetStr(AdvHolofanComponent comp)
    {
        return Loc.GetString("actived-signs", ("count", comp.Signs.Count));
    }
    public virtual void OnBeforeInteract(EntityUid uid, AdvHolofanComponent comp, BeforeRangedInteractEvent args)
    {
        if (!_timing.IsFirstTimePredicted
            || args.Handled
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

        _audio.PlayPvs(comp.NewHolofanSound, uid);
        comp.Signs.Add(holoUid);

        comp.UiUpdateNeeded = true;
        args.Handled = true;
    }
    public virtual void OnUseInHand(EntityUid uid, AdvHolofanComponent comp, UseInHandEvent args)
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

        comp.UiUpdateNeeded = true;
        args.Handled = true;
    }
}
