using Content.Client.Items;
using Content.Shared.Siberia.AdvHolofan.Components;
using Content.Client.Siberia.AdvHolofan.UI;
using Robust.Client.Audio;
using Content.Shared.Storage;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Siberia.AdvHolofan.Components;
using JetBrains.Annotations;


namespace Content.Client.Siberia.AdvHolofan.System;
[UsedImplicitly]
public sealed class AdvHolofanSystem : SharedAdvHolofanSystem
{
    [Dependency] private readonly AudioSystem _audio = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;

    public override void Initialize()
    {
        base.Initialize();

        Subs.ItemStatus<AdvHolofanComponent>(ent => new AdvHolofanStatusControl(ent));
    }

}
