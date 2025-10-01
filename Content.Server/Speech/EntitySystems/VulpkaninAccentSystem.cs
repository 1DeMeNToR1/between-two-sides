using System.Text.RegularExpressions;
using Content.Server.Speech.Components;
using Robust.Shared.Random;
using Content.Shared.Speech;

namespace Content.Server.Speech.EntitySystems;

public sealed class VulpkaninAccentSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<VulpkaninAccentComponent, AccentGetEvent>(OnAccent);
    }

    private void OnAccent(EntityUid uid, VulpkaninAccentComponent component, AccentGetEvent args)
    {
        var message = args.Message;

        // rrr
        message = Regex.Replace(message, "r+", "rrr");
        // Rrr
        message = Regex.Replace(message, "R+", "Rrr");

        // 43-Localization-Start
        // р => ррр
        message = Regex.Replace(
            message,
            "р+",
            _random.Pick(new List<string>() { "рр", "ррр" })
        );
        // Р => Ррр
        message = Regex.Replace(
            message,
            "Р+",
            _random.Pick(new List<string>() { "Рр", "Ррр" })
        );
        // 43-Localization-End
        args.Message = message;
    }
}
