using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

using log4net;

using Plus.Communication.Packets.Incoming;
using Plus.HabboHotel.GameClients;

using Plus.Communication.Packets.Incoming.Catalog;
using Plus.Communication.Packets.Incoming.Handshake;
using Plus.Communication.Packets.Incoming.Navigator;
using Plus.Communication.Packets.Incoming.Quests;
using Plus.Communication.Packets.Incoming.Rooms.Avatar;
using Plus.Communication.Packets.Incoming.Rooms.Chat;
using Plus.Communication.Packets.Incoming.Rooms.Connection;
using Plus.Communication.Packets.Incoming.Rooms.Engine;
using Plus.Communication.Packets.Incoming.Rooms.Action;
using Plus.Communication.Packets.Incoming.Users;
using Plus.Communication.Packets.Incoming.Inventory.AvatarEffects;
using Plus.Communication.Packets.Incoming.Inventory.Purse;
using Plus.Communication.Packets.Incoming.Sound;
using Plus.Communication.Packets.Incoming.Misc;
using Plus.Communication.Packets.Incoming.Inventory.Badges;
using Plus.Communication.Packets.Incoming.Avatar;
using Plus.Communication.Packets.Incoming.Inventory.Achievements;
using Plus.Communication.Packets.Incoming.Inventory.Bots;
using Plus.Communication.Packets.Incoming.Inventory.Pets;
using Plus.Communication.Packets.Incoming.LandingView;
using Plus.Communication.Packets.Incoming.Messenger;
using Plus.Communication.Packets.Incoming.Groups;

using Plus.Communication.Packets.Incoming.Rooms.Settings;
using Plus.Communication.Packets.Incoming.Rooms.AI.Pets;
using Plus.Communication.Packets.Incoming.Rooms.AI.Bots;
using Plus.Communication.Packets.Incoming.Rooms.AI.Pets.Horse;
using Plus.Communication.Packets.Incoming.Rooms.Furni;
using Plus.Communication.Packets.Incoming.Rooms.Furni.RentableSpaces;
using Plus.Communication.Packets.Incoming.Rooms.Furni.YouTubeTelevisions;
using Plus.Communication.Packets.Incoming.Help;
using Plus.Communication.Packets.Incoming.Rooms.FloorPlan;
using Plus.Communication.Packets.Incoming.Rooms.Furni.Wired;
using Plus.Communication.Packets.Incoming.Moderation;
using Plus.Communication.Packets.Incoming.Inventory.Furni;
using Plus.Communication.Packets.Incoming.Rooms.Furni.Stickys;
using Plus.Communication.Packets.Incoming.Rooms.Furni.Moodlight;
using Plus.Communication.Packets.Incoming.Inventory.Trading;
using Plus.Communication.Packets.Incoming.GameCenter;
using Plus.Communication.Packets.Incoming.Marketplace;
using Plus.Communication.Packets.Incoming.Rooms.Furni.LoveLocks;
using Plus.Communication.Packets.Incoming.Talents;

namespace Plus.Communication.Packets
{
    public sealed class PacketManager
    {
        private static readonly ILog Log = LogManager.GetLogger("Plus.Communication.Packets");

        /// <summary>
        ///     Testing the Task code
        /// </summary>
        private readonly bool _ignoreTasks = true;

        /// <summary>
        ///     The maximum time a task can run for before it is considered dead
        ///     (can be used for debugging any locking issues with certain areas of code)
        /// </summary>
        private readonly int _maximumRunTimeInSec = 300; // 5 minutes

        /// <summary>
        ///     Should the handler throw errors or log and continue.
        /// </summary>
        private readonly bool _throwUserErrors = false;

        /// <summary>
        ///     The task factory which is used for running Asynchronous tasks, in this case we use it to execute packets.
        /// </summary>
        private readonly TaskFactory _eventDispatcher;

        private readonly Dictionary<int, IPacketEvent> _incomingPackets;

        /// <summary>
        ///     Currently running tasks to keep track of what the current load is
        /// </summary>
        private readonly ConcurrentDictionary<int, Task> _runningTasks;

        public PacketManager(IEnumerable<IPacketEvent> packets)
        {
            _incomingPackets = packets.ToDictionary(x => x.Header, x => x);
            _incomingPackets.Add(ClientPacketHeader.SaveWiredEffectConfigMessageEvent, new SaveWiredConfigEvent());
            _incomingPackets.Add(ClientPacketHeader.SaveWiredConditionConfigMessageEvent, new SaveWiredConfigEvent());

            _eventDispatcher = new TaskFactory(TaskCreationOptions.PreferFairness, TaskContinuationOptions.None);
            _runningTasks = new ConcurrentDictionary<int, Task>();
        }

        public void TryExecutePacket(GameClient session, ClientPacket packet)
        {
            if (session == null)
                return;

            if (!_incomingPackets.TryGetValue(packet.Id, out IPacketEvent pak))
            {
                if (System.Diagnostics.Debugger.IsAttached)
                    Log.Debug("Unhandled Packet: " + packet);
                return;
            }

            if (System.Diagnostics.Debugger.IsAttached)
            {
                Log.Debug("Handled Packet: [" + packet.Id + "] " + pak.GetType().Name);
            }

            if (!_ignoreTasks)
                ExecutePacketAsync(session, packet, pak);
            else
                pak.Parse(session, packet);
        }

        private void ExecutePacketAsync(GameClient session, ClientPacket packet, IPacketEvent pak)
        {
            CancellationTokenSource cancelSource = new CancellationTokenSource();
            CancellationToken token = cancelSource.Token;

            Task t = _eventDispatcher.StartNew(() =>
            {
                pak.Parse(session, packet);
                token.ThrowIfCancellationRequested();
            }, token);

            _runningTasks.TryAdd(t.Id, t);

            try
            {
                if (!t.Wait(_maximumRunTimeInSec * 1000, token))
                {
                    cancelSource.Cancel();
                }
            }
            catch (AggregateException ex)
            {
                foreach (Exception e in ex.Flatten().InnerExceptions)
                {
                    if (_throwUserErrors)
                    {
                        throw e;
                    }
                    else
                    {
                        //log.Fatal("Unhandled Error: " + e.Message + " - " + e.StackTrace);
                        session.Disconnect();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                session.Disconnect();
            }
            finally
            {
                _runningTasks.TryRemove(t.Id, out Task _);

                cancelSource.Dispose();

                //log.Debug("Event took " + (DateTime.Now - Start).Milliseconds + "ms to complete.");
            }
        }

        public void WaitForAllToComplete()
        {
            foreach (Task t in _runningTasks.Values.ToList())
            {
                t.Wait();
            }
        }

        public void UnregisterAll()
        {
            _incomingPackets.Clear();
        }
    }
}