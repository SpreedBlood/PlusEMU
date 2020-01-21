using System;
using System.Linq;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users.Relationships;
using Plus.Communication.Packets.Outgoing;

namespace Plus.HabboHotel.Users.Messenger
{
    public class MessengerBuddy
    {
        #region Fields

        public int UserId;
        public bool mAppearOffline;
        public bool mHideInroom;
        public int mLastOnline;
        public string mLook;
        public string mMotto;

        public GameClient client;
        private Room currentRoom;
        public string mUsername;

        #endregion

        #region Return values

        public int Id
        {
            get { return UserId; }
        }

        public bool IsOnline
        {
            get
            {
                return (client != null && client.Habbo != null && client.Habbo.GetMessenger() != null &&
                        !client.Habbo.GetMessenger().AppearOffline);
            }
        }

        public bool InRoom
        {
            get { return (currentRoom != null); }
        }

        public Room CurrentRoom
        {
            get { return currentRoom; }
            set { currentRoom = value; }
        }

        #endregion

        #region Constructor

        public MessengerBuddy(int UserId, string pUsername, string pLook, string pMotto, int pLastOnline,
                                bool pAppearOffline, bool pHideInroom)
        {
            this.UserId = UserId;
            mUsername = pUsername;
            mLook = pLook;
            mMotto = pMotto;
            mLastOnline = pLastOnline;
            mAppearOffline = pAppearOffline;
            mHideInroom = pHideInroom;
        }

        #endregion

        #region Methods
        public void UpdateUser(GameClient client)
        {
            this.client = client;
            if (client != null && client.Habbo != null)
                currentRoom = client.Habbo.CurrentRoom;
        }

        public void Serialize(ServerPacket Message, GameClient Session)
        {
            Relationship Relationship = null;

            if (Session != null && Session.Habbo != null && Session.Habbo.Relationships != null)
                Relationship = Session.Habbo.Relationships.FirstOrDefault(x => x.Value.UserId == Convert.ToInt32(UserId)).Value;

            int y = Relationship == null ? 0 : Relationship.Type;

            Message.WriteInteger(UserId);
            Message.WriteString(mUsername);
            Message.WriteInteger(1);
            Message.WriteBoolean(!mAppearOffline || Session.Habbo.GetPermissions().HasRight("mod_tool") ? IsOnline : false);
            Message.WriteBoolean(!mHideInroom || Session.Habbo.GetPermissions().HasRight("mod_tool") ? InRoom : false);
            Message.WriteString(IsOnline ? mLook : "");
            Message.WriteInteger(0); // categoryid
            Message.WriteString(mMotto);
            Message.WriteString(string.Empty); // Facebook username
            Message.WriteString(string.Empty);
            Message.WriteBoolean(true); // Allows offline messaging
            Message.WriteBoolean(false); // ?
            Message.WriteBoolean(false); // Uses phone
            Message.WriteShort(y);
        }

        #endregion
    }
}