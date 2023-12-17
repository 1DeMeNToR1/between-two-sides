using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Configuration;

namespace Content.Shared.Siberia;

/// <summary>
///     Модуль Siberia консольных переменных
/// </summary>
[CVarDefs]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed class SCCVars
{
    /**
     * Sponsors
     */

    /// <summary>
    ///     URL-адрес API сервера спонсоров.
    /// </summary>
    public static readonly CVarDef<string> SponsorsApiUrl =
        CVarDef.Create("sponsor.api_url", "", CVar.SERVERONLY);

    /*
     * Discord Auth
     */

    /// <summary>
    ///     Включена привязка Discord. Показывает кнопку-ссылку и модальное окно.
    /// </summary>
    public static readonly CVarDef<bool> DiscordAuthEnabled =
        CVarDef.Create("discord_auth.enabled", false, CVar.SERVERONLY);

    /// <summary>
    ///     URL-адрес API сервера аутентификации Discord
    /// </summary>
    public static readonly CVarDef<string> DiscordAuthApiUrl =
        CVarDef.Create("discord_auth.api_url", "", CVar.SERVERONLY);

    /// <summary>
    ///     Секретный ключ API сервера авторизации Discord
    /// </summary>
    public static readonly CVarDef<string> DiscordAuthApiKey =
        CVarDef.Create("discord_auth.api_key", "", CVar.SERVERONLY | CVar.CONFIDENTIAL);
}
