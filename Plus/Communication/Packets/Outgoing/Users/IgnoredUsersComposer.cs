﻿using System.Collections.Generic;

namespace Plus.Communication.Packets.Outgoing.Users
{
    public class IgnoredUsersComposer : ServerPacket
    {
        public IgnoredUsersComposer(IReadOnlyCollection<string> ignoredUsers)
            : base(ServerPacketHeader.IgnoredUsersMessageComposer)
        {
            WriteInteger(ignoredUsers.Count);
            foreach (string username in ignoredUsers)
            {
                WriteString(username);
            }
        }
    }
}
