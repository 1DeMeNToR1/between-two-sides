using Content.Client.Message;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Content.Client.Stylesheets;
using Robust.Shared.Timing;
using Content.Client.Message;
using Content.Client.Siberia.AdvHolofan.System;
using Robust.Client.GameObjects;
using Content.Shared.Siberia.AdvHolofan.Components;

namespace Content.Client.Siberia.AdvHolofan.UI;

public sealed class AdvHolofanStatusControl : Control
{
    private readonly AdvHolofanComponent _parent;
    private readonly RichTextLabel _label;
    private float _updateDif;
    private readonly IEntityManager _entMan;
    private readonly SharedTransformSystem _transform;
    private readonly SharedAdvHolofanSystem _advholo;

    public AdvHolofanStatusControl(AdvHolofanComponent parent)
    {
        _parent = parent;
        _label = new RichTextLabel { StyleClasses = { StyleNano.StyleClassItemStatus } };
        _entMan = IoCManager.Resolve<IEntityManager>();
        _transform = _entMan.System<TransformSystem>();
        _advholo = _entMan.System<AdvHolofanSystem>();
        AddChild(_label);
    }

    protected override void FrameUpdate(FrameEventArgs args)
    {
        base.FrameUpdate(args);

        if (!_parent.UiUpdateNeeded)
        {
            return;
        }

        _parent.UiUpdateNeeded = false;

        _label.SetMarkup(_advholo.GetStr(_parent));

    }

    /*private void UpdateHoloDetails()
    {
        _label.SetMarkup(_advholo.GetStr(_parent.Comp));
        //_label.SetMarkup(Loc.GetString("actived-signs", ("count", _parent.Comp.Signs.Count)));
    }

    private void UpdateHoloDetails()
    {
        var posText = "Error";
        if (_entMan.TryGetComponent(_parent, out TransformComponent? transComp))
        {
            var pos =  _transform.GetMapCoordinates(_parent.Owner, xform: transComp);
            var x = (int) pos.X;
            var y = (int) pos.Y;
            posText = $"({x}, {y})";
        }
        _label.SetMarkup(Loc.GetString("handheld-gps-coordinates-title", ("coordinates", posText)));
    }*/

}
