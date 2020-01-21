using System;

using Plus.Utilities;
using Plus.HabboHotel.Quests;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms.Chat.Logs;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.HabboHotel.Rooms.Chat.Styles;

namespace Plus.Communication.Packets.Incoming.Rooms.Chat
{
    public class ChatEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.ChatMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.Habbo == null || !session.Habbo.InRoom)
                return;

            Room room = session.Habbo.CurrentRoom;
            if (room == null)
                return;

            RoomUser user = room.GetRoomUserManager().GetRoomUserByHabbo(session.Habbo.Id);
            if (user == null)
                return;

            string message = StringCharFilter.Escape(packet.PopString());
            if (message.Length > 100)
                message = message.Substring(0, 100);

            int colour = packet.PopInt();

            if (!PlusEnvironment.GetGame().GetChatManager().GetChatStyles().TryGetStyle(colour, out ChatStyle style) || style.RequiredRight.Length > 0 && !session.Habbo.GetPermissions().HasRight(style.RequiredRight))
                colour = 0;

            user.UnIdle();

            if (PlusEnvironment.GetUnixTimestamp() < session.Habbo.FloodTime && session.Habbo.FloodTime != 0)
                return;

            if (session.Habbo.TimeMuted > 0)
            {
                session.SendPacket(new MutedComposer(session.Habbo.TimeMuted));
                return;
            }

            if (!session.Habbo.GetPermissions().HasRight("room_ignore_mute") && room.CheckMute(session))
            {
                session.SendWhisper("Oops, you're currently muted.");
                return;
            }

            user.LastBubble = session.Habbo.CustomBubbleId == 0 ? colour : session.Habbo.CustomBubbleId;

            if (!session.Habbo.GetPermissions().HasRight("mod_tool"))
            {
                if (user.IncrementAndCheckFlood(out int muteTime))
                {
                    session.SendPacket(new FloodControlComposer(muteTime));
                    return;
                }
            }

            PlusEnvironment.GetGame().GetChatManager().GetLogs().StoreChatlog(new ChatlogEntry(session.Habbo.Id, room.Id, message, UnixTimestamp.GetNow(), session.Habbo, room));

            if (message.StartsWith(":", StringComparison.CurrentCulture) && PlusEnvironment.GetGame().GetChatManager().GetCommands().Parse(session, message))
                return;

            if (PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckBannedWords(message))
            {
                session.Habbo.BannedPhraseCount++;
                if (session.Habbo.BannedPhraseCount >= Convert.ToInt32(PlusEnvironment.GetSettingsManager().TryGetValue("room.chat.filter.banned_phrases.chances")))
                {
                    PlusEnvironment.GetGame().GetModerationManager().BanUser("System", HabboHotel.Moderation.ModerationBanType.Username, session.Habbo.Username, "Spamming banned phrases (" + message + ")", PlusEnvironment.GetUnixTimestamp() + 78892200);
                    session.Disconnect();
                    return;
                }

                session.SendPacket(new ChatComposer(user.VirtualId, message, 0, colour));
                return;
            }

            if (!session.Habbo.GetPermissions().HasRight("word_filter_override"))
                message = PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(message);


            PlusEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.SocialChat);

            user.OnChat(user.LastBubble, message, false);
        }
    }
}
