using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.MessageLog.Parts;
using Archipelago.MultiClient.Net.Packets;
using BannerlordArchipelago.Archipelago;
using BannerlordArchipelago.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TaleWorlds.Library;
using TaleWorlds.Localization;

public class ArchipelagoClient
{
    public const string APVersion = "0.6.7";
    private const string Game = "Mount And Blade Bannerlord";

    public static bool Authenticated;
    private bool attemptingConnection;

    public static ArchipelagoData ServerData = new ArchipelagoData();
    //private DeathLinkHandler DeathLinkHandler;
    private ArchipelagoSession session;

    /// <summary>
    /// call to connect to an Archipelago session. Connection info should already be set up on ServerData
    /// </summary>
    /// <returns></returns>
    public void Connect()
    {
        if (Authenticated || attemptingConnection) return;
        attemptingConnection = true; 

        try
        {
            session = ArchipelagoSessionFactory.CreateSession(ServerData.Uri);
            SetupSession();
        }
        catch (Exception e)
        {
            InformationManager.DisplayMessage(new InformationMessage($"AP Connection error: {e.Message}"));
            attemptingConnection = false;
            return;
        }

        TryConnect();
    }
    /// <summary>
    /// add handlers for Archipelago events
    /// </summary>
    private void SetupSession()
    {
        //ReceivedItemsTracker.Reset();
        session.Items.ItemReceived += OnItemReceived;
        session.Socket.ErrorReceived += OnSessionErrorReceived;
        session.Socket.SocketClosed += OnSessionSocketClosed;
        session.MessageLog.OnMessageReceived += OnMessageReceived;
    }

    /// <summary>
    /// attempt to connect to the server with our connection info
    /// </summary>
    private void TryConnect()
    {
        try
        {
            // it's safe to thread this function call but unity notoriously hates threading so do not use excessively
            ThreadPool.QueueUserWorkItem(
                _ => HandleConnectResult(
                    session.TryConnectAndLogin(
                        Game,
                        ServerData.SlotName,
                        ItemsHandlingFlags.AllItems, 
                        new Version(APVersion),
                        password: ServerData.Password,
                        requestSlotData: true 
                    )));
        }
        catch (Exception e)
        {
            //Plugin.BepinLogger.LogError(e);
            HandleConnectResult(new LoginFailure(e.ToString()));
            attemptingConnection = false;
        }
    }

    /// <summary>
    /// handle the connection result and do things
    /// </summary>
    /// <param name="result"></param>
    private void HandleConnectResult(LoginResult result)
    {
        string outText;
        if (result.Successful)
        {
            var success = (LoginSuccessful)result;

            ServerData.SetupSession(success.SlotData, session.RoomState.Seed);
            Authenticated = true;

            session.Locations.CompleteLocationChecksAsync(ServerData.CheckedLocations.ToArray());
            outText = $"Successfully connected to {ServerData.Uri} as {ServerData.SlotName}!";
        }
        else
        {
            var failure = (LoginFailure)result;
            outText = $"Failed to connect to {ServerData.Uri} as {ServerData.SlotName}.";
            outText = failure.Errors.Aggregate(outText, (current, error) => current + $"\n    {error}");

            //Plugin.BepinLogger.LogError(outText);

            Authenticated = false;
            Disconnect();
        }

        //ArchipelagoConsole.LogMessage(outText);
        attemptingConnection = false;
    }

    /// <summary>
    /// something went wrong, or we need to properly disconnect from the server. cleanup and re null our session
    /// </summary>
    public void Disconnect()
    {
       // Plugin.BepinLogger.LogDebug("disconnecting from server...");
        session?.Socket.DisconnectAsync();
        session = null;
        Authenticated = false;
    }

    public void SendMessage(string message)
    {
        session.Socket.SendPacketAsync(new SayPacket { Text = message });
    }

    /// <summary>
    /// we received an item so reward it here
    /// </summary>
    /// <param name="helper">item helper which we can grab our item from</param>
    private void OnItemReceived(ReceivedItemsHelper helper)
    {
        if (helper.Index <= ServerData.Index) return;
        var receivedItem = helper.DequeueItem();
        ServerData.Index++;

        // We always enqueue so our list is up to date. We try and process if the game is ready to accept items.
        ReceivedItemsTracker.EnqueuePending(receivedItem.ItemName, helper.Index);
        ReceivedItemsTracker.OnItemReceived();
    }

    public void SendLocationCheck(string locationName)
    {
        try
        {
            var locationIds = new List<long>();

            if (DataDicts.GameNameToAPLocation.TryGetValue(locationName, out var primaryLocation))
            {
                var locationId = session.Locations.GetLocationIdFromName(Game, primaryLocation);
                locationIds.Add(locationId);
            }
            else
            {
                var locationId = session.Locations.GetLocationIdFromName(Game, locationName);
                locationIds.Add(locationId);
            }

            if (locationIds.Count > 0)
            {
                session.Locations.CompleteLocationChecksAsync(locationIds.ToArray());
            }
        }
        catch (Exception e)
        {
            InformationManager.DisplayMessage(new InformationMessage(
                $"[AP Debug] SendLocationCheck error: {e.Message}\n{e.StackTrace}",
                Colors.Red
            ));
        }
    }


    /// <summary>
    /// something went wrong with our socket connection
    /// </summary>
    /// <param name="e">thrown exception from our socket</param>
    /// <param name="message">message received from the server</param>
    private void OnSessionErrorReceived(Exception e, string message)
    {
        //Plugin.BepinLogger.LogError(e);
       // ArchipelagoConsole.LogMessage(message);
    }

    /// <summary>
    /// something went wrong closing our connection. disconnect and clean up
    /// </summary>
    /// <param name="reason"></param>
    private void OnSessionSocketClosed(string reason)
    {
        Disconnect();
    }
    public void SendGoalAchieved()
    {
        if (!Authenticated || session == null) return;

        var statusUpdate = new StatusUpdatePacket { Status = ArchipelagoClientState.ClientGoal };
        session.Socket.SendPacketAsync(statusUpdate);
    }

    private static ConcurrentQueue<InformationMessage> _pending = new ConcurrentQueue<InformationMessage>();
    public static void OnMessageReceived(LogMessage message)
    {

        _pending.Enqueue(new InformationMessage(message.ToString(),Colors.Magenta));
    }
    public void Flush()
    {
        while (_pending.TryDequeue(out var msg))
            InformationManager.DisplayMessage(msg);

    }
}