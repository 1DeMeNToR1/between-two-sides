using Content.Client.Items;
using Content.Shared.Siberia.AdvHolofan.Components;
using Content.Client.Siberia.AdvHolofan.UI;
using Robust.Client.Audio;
using JetBrains.Annotations;

namespace Content.Client.Siberia.AdvHolofan.System;
[UsedImplicitly]
public sealed class AdvHolofanSystem : SharedAdvHolofanSystem
{
    public override void Initialize()
    {
        base.Initialize();

        Subs.ItemStatus<AdvHolofanComponent>(ent => new AdvHolofanStatusControl(ent));
    }

}
