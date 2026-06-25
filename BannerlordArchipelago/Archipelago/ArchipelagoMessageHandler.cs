using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using System.Collections.Concurrent;
using TaleWorlds.Core;
using TaleWorlds.Library;

public class ArchipelagoMessageHandler
{
    private readonly ArchipelagoSession _session;
    private readonly ConcurrentQueue<InformationMessage> _pending = new ConcurrentQueue<InformationMessage>();

    public ArchipelagoMessageHandler(ArchipelagoSession session)
    {
        _session = session;
        _session.MessageLog.OnMessageReceived += OnMessageReceived;
    }

    private void OnMessageReceived(LogMessage message)
    {
        foreach (var part in message.Parts)
        {
            var apColor = part.Color;
            var bColor = new Color(apColor.R / 255f, apColor.G / 255f, apColor.B / 255f);
            _pending.Enqueue(new InformationMessage(part.Text, bColor));
        }
    }


    public void Flush()
    {
        while (_pending.TryDequeue(out var msg))
            InformationManager.DisplayMessage(msg);
    }

    public void Dispose()
    {
        if (_session != null)
            _session.MessageLog.OnMessageReceived -= OnMessageReceived;
    }
}