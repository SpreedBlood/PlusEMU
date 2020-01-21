using System;
using Plus.Core;
using Plus.Communication.Packets.Incoming;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;
using Plus.Communication.Interfaces;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.Communication.Packets.Outgoing.Moderation;

using Plus.Communication.Encryption.Crypto.Prng;

using Plus.Database.Interfaces;
using Plus.Communication.ConnectionManager;
using Plus.Communication;

namespace Plus.HabboHotel.GameClients
{
    public class GameClient
    {
        public Habbo Habbo { get; set; }
        public string MachineId;
        private bool _disconnected;
        public ARC4 Rc4Client;
        private GamePacketParser _packetParser;
        private ConnectionInformation _connection;
        public int PingCount { get; set; }

        public GameClient(int clientId, ConnectionInformation connection)
        {
            ConnectionId = clientId;
            _connection = connection;
            _packetParser = new GamePacketParser(this);

            PingCount = 0;
        }

        private void SwitchParserRequest()
        {
            _packetParser.SetConnection(_connection);
            _packetParser.OnNewPacket += Parser_onNewPacket;
            byte[] data = (_connection.Parser as InitialPacketParser).CurrentData;
            _connection.Parser.Dispose();
            _connection.Parser = _packetParser;
            _connection.Parser.HandlePacketData(data);
        }

        private void Parser_onNewPacket(ClientPacket message)
        {
            try
            {
                PlusEnvironment.GetGame().GetPacketManager().TryExecutePacket(this, message);
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException(e);
            }
        }

        private void PolicyRequest()
        {
            _connection.SendData(PlusEnvironment.GetDefaultEncoding().GetBytes("<?xml version=\"1.0\"?>\r\n" +
                   "<!DOCTYPE cross-domain-policy SYSTEM \"/xml/dtds/cross-domain-policy.dtd\">\r\n" +
                   "<cross-domain-policy>\r\n" +
                   "<allow-access-from domain=\"*\" to-ports=\"1-31111\" />\r\n" +
                   "</cross-domain-policy>\x0"));
        }


        public void StartConnection()
        {
            if (_connection == null)
                return;

            PingCount = 0;

            (_connection.Parser as InitialPacketParser).PolicyRequest += PolicyRequest;
            (_connection.Parser as InitialPacketParser).SwitchParserRequest += SwitchParserRequest;
            _connection.StartPacketProcessing();
        }

        public void SendWhisper(string message, int colour = 0)
        {
            if (Habbo == null || Habbo.CurrentRoom == null)
                return;

            RoomUser user = Habbo.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Habbo.Username);
            if (user == null)
                return;

            SendPacket(new WhisperComposer(user.VirtualId, message, 0, (colour == 0 ? user.LastBubble : colour)));
        }

        public void SendNotification(string message)
        {
            SendPacket(new BroadcastMessageAlertComposer(message));
        }

        public void SendPacket(IServerPacket message)
        {
            if (GetConnection() == null)
                return;

            GetConnection().SendData(message.GetBytes());
        }

        public int ConnectionId { get; }

        public ConnectionInformation GetConnection()
        {
            return _connection;
        }

        public void Disconnect()
        {
            try
            {
                if (Habbo != null)
                {
                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery(Habbo.GetQueryString);
                    }

                    Habbo.OnDisconnect();
                }
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException(e);
            }

            if (!_disconnected)
            {
                if (_connection != null)
                    _connection.Dispose();
                _disconnected = true;
            }
        }

        public void Dispose()
        {
            if (Habbo != null)
                Habbo.OnDisconnect();

            MachineId = string.Empty;
            _disconnected = true;
            Habbo = null;
            _connection = null;
            Rc4Client = null;
            _packetParser = null;
        }
    }
}