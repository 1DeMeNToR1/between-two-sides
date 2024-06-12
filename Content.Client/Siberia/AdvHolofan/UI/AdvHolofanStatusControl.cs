using Content.Client.Message;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Content.Client.Stylesheets;
using Robust.Shared.Timing;
using Content.Client.Siberia.AdvHolofan.System;
using Content.Shared.Siberia.AdvHolofan.Components;

namespace Content.Client.Siberia.AdvHolofan.UI;

public sealed class AdvHolofanStatusControl : Control
{
    private readonly AdvHolofanComponent _parent;
    private readonly RichTextLabel _label;
    private float _updateDif;
    private readonly IEntityManager _entMan;
    private readonly SharedAdvHolofanSystem _advholo;

    public AdvHolofanStatusControl(AdvHolofanComponent parent)
    {
        _parent = parent;
        _label = new RichTextLabel { StyleClasses = { StyleNano.StyleClassDigitsItemStatus } };
        _entMan = IoCManager.Resolve<IEntityManager>();
        _advholo = _entMan.System<AdvHolofanSystem>();
        AddChild(_label);
        UpdateStatus();
    }
    protected override void FrameUpdate(FrameEventArgs args)
    {
        base.FrameUpdate(args);

        if (!_parent.UiUpdateNeeded)
        {
            return;
        }

        _parent.UiUpdateNeeded = false;
        UpdateStatus();
    }
    private void UpdateStatus()
    {
        _label.SetMarkup($"{_parent.Signs.Count} / {_parent.MaxProjectors}");
    }
}
