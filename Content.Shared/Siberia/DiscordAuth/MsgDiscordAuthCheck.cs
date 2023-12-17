using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Siberia.DiscordAuth;

/// <summary>
/// Клиент отправляет это событие, чтобы заставить сервер проверить состояние верификации игрока в Discord.
/// </summary>
public sealed class MsgDiscordAuthCheck : NetMessage
{
    public override MsgGroups MsgGroup => MsgGroups.Command;

    public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
    {
    }

    public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
    {
    }
}
