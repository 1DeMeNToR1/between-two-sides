using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Siberia.DiscordAuth;

/// <summary>
/// Сервер отправляет это событие клиенту при подключении, если требуется авторизация Discord'а
/// </summary>
public sealed class MsgDiscordAuthRequired : NetMessage
{
    public override MsgGroups MsgGroup => MsgGroups.Command;

    public string AuthUrl = string.Empty;

    public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
    {
        AuthUrl = buffer.ReadString();
    }

    public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
    {
        buffer.Write(AuthUrl);
    }
}
