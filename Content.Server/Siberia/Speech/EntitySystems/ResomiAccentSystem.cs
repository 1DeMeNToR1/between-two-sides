using System.Text.RegularExpressions;
using Content.Server.Siberia.Speech.Components;
using Content.Server.Speech;
using Content.Shared.Speech;

namespace Content.Server.Siberia.Speech.EntitySystems;

public sealed class ResomiAccentSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ResomiAccentComponent, AccentGetEvent>(OnAccent);
    }

    private void OnAccent(EntityUid uid, ResomiAccentComponent component, AccentGetEvent args)
    {
        var message = args.Message;

        // ekssit
        message = Regex.Replace(message, @"(\w)x", "$1kss");
        // ecks
        message = Regex.Replace(message, @"\bx([\-|r|R]|\b)", "ecks$1");
        // eckS
        message = Regex.Replace(message, @"\bX([\-|r|R]|\b)", "ECKS$1");

        // цоканье
        message = Regex.Replace(message, "ч+", "ц");
        message = Regex.Replace(message, "Ч+", "Ц");
        // соканье
        message = Regex.Replace(message, "Ш+", "С");
        message = Regex.Replace(message, "ш+", "с");

        message = Regex.Replace(message, "З+", "СЬ");
        message = Regex.Replace(message, "з+", "сь");

        args.Message = message;
    }

}
