using Content.Client.Message;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Content.Client.Stylesheets;
using Content.Shared.Clock;
using Robust.Shared.Timing;


namespace Content.Client.Clock;

public sealed class ClockStatusControl : Control
{
    private readonly RichTextLabel _label;
    private readonly IEntityManager _entMan;
    private readonly Entity<ClockComponent> _parent;
    private readonly SharedClockSystem _clock;
    private TimeSpan _past;

    public ClockStatusControl(Entity<ClockComponent> parent)
    {
        _parent = parent;
        // needs to be redone when upstreaming with Redux
        _label = new RichTextLabel { StyleClasses = { StyleNano.StyleClassDigitsItemStatus } };
        _entMan = IoCManager.Resolve<IEntityManager>();
        _clock = _entMan.System<ClockSystem>();
        _past = _clock.GetClockTime(_parent);
        AddChild(_label);
        UpdateStatus();
    }
    protected override void FrameUpdate(FrameEventArgs args)
    {
        base.FrameUpdate(args);

        if (_past.Minutes >= _clock.GetClockTime(_parent).Minutes)
            return;

        _past = _clock.GetClockTime(_parent);
        UpdateStatus();
    }
    private void UpdateStatus()
    {
        _label.SetMarkup(_clock.GetClockTimeText(_parent));
    }
}
