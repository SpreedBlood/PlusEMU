using Plus.Communication.Packets.Outgoing.BuildersClub;
using Plus.Communication.Packets.Outgoing.Handshake;
using Plus.Communication.Packets.Outgoing.Inventory.Achievements;
using Plus.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.Communication.Packets.Outgoing.Navigator;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.Communication.Packets.Outgoing.Sound;
using Plus.Core;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Achievements;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Permissions;
using Plus.HabboHotel.Subscriptions;
using Plus.HabboHotel.Users.Messenger.FriendBar;
using Plus.HabboHotel.Users.UserData;
using System;

namespace Plus.HabboHotel.Users
{
    internal class AuthenticationHandler
    {
        private readonly AchievementManager _achievementManager;

        public AuthenticationHandler(AchievementManager achievementManager)
        {
            _achievementManager = achievementManager;
        }

        public bool TryAuthenticate(string authTicket, GameClient client)
        {
            try
            {
                UserData.UserData userData = UserDataFactory.GetUserData(authTicket, out byte errorCode);
                if (errorCode == 1 || errorCode == 2)
                {
                    client.Disconnect();
                    return false;
                }

                #region Ban Checking
                //Let's have a quick search for a ban before we successfully authenticate..
                if (!string.IsNullOrEmpty(client.MachineId))
                {
                    if (PlusEnvironment.GetGame().GetModerationManager().IsBanned(client.MachineId, out _))
                    {
                        if (PlusEnvironment.GetGame().GetModerationManager().MachineBanCheck(client.MachineId))
                        {
                            client.Disconnect();
                            return false;
                        }
                    }
                }

                if (userData.user != null)
                {
                    if (PlusEnvironment.GetGame().GetModerationManager().IsBanned(userData.user.Username, out _))
                    {
                        if (PlusEnvironment.GetGame().GetModerationManager().UsernameBanCheck(userData.user.Username))
                        {
                            client.Disconnect();
                            return false;
                        }
                    }
                }
                #endregion

                if (userData.user == null) //Possible NPE
                {
                    return false;
                }

                PlusEnvironment.GetGame().GetClientManager().RegisterClient(client, userData.UserId, userData.user.Username);
                client.Habbo = userData.user;
                if (client.Habbo != null)
                {
                    userData.user.Init(client, userData);

                    client.SendPacket(new AuthenticationOKComposer());
                    client.SendPacket(new AvatarEffectsComposer(client.Habbo.Effects().GetAllEffects));
                    client.SendPacket(new NavigatorSettingsComposer(client.Habbo.HomeRoom));
                    client.SendPacket(new FavouritesComposer(userData.user.FavoriteRooms));
                    client.SendPacket(new FigureSetIdsComposer(client.Habbo.GetClothing().GetClothingParts));
                    client.SendPacket(new UserRightsComposer(client.Habbo.Rank));
                    client.SendPacket(new AvailabilityStatusComposer());
                    client.SendPacket(new AchievementScoreComposer(client.Habbo.GetStats().AchievementPoints));
                    client.SendPacket(new BuildersClubMembershipComposer());
                    client.SendPacket(new CfhTopicsInitComposer(PlusEnvironment.GetGame().GetModerationManager().UserActionPresets));

                    client.SendPacket(new BadgeDefinitionsComposer(_achievementManager.Achievements));
                    client.SendPacket(new SoundSettingsComposer(client.Habbo.ClientVolume, client.Habbo.ChatPreference, client.Habbo.AllowMessengerInvites, client.Habbo.FocusPreference, FriendBarStateUtility.GetInt(client.Habbo.FriendbarState)));
                    //SendMessage(new TalentTrackLevelComposer());

                    if (client.Habbo.GetMessenger() != null)
                        client.Habbo.GetMessenger().OnStatusChanged(true);

                    if (!string.IsNullOrEmpty(client.MachineId))
                    {
                        if (client.Habbo.MachineId != client.MachineId)
                        {
                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("UPDATE `users` SET `machine_id` = @MachineId WHERE `id` = @id LIMIT 1");
                                dbClient.AddParameter("MachineId", client.MachineId);
                                dbClient.AddParameter("id", client.Habbo.Id);
                                dbClient.RunQuery();
                            }
                        }

                        client.Habbo.MachineId = client.MachineId;
                    }

                    if (PlusEnvironment.GetGame().GetPermissionManager().TryGetGroup(client.Habbo.Rank, out PermissionGroup group))
                    {
                        if (!string.IsNullOrEmpty(group.Badge))
                            if (!client.Habbo.GetBadgeComponent().HasBadge(group.Badge))
                                client.Habbo.GetBadgeComponent().GiveBadge(group.Badge, true, client);
                    }

                    if (PlusEnvironment.GetGame().GetSubscriptionManager().TryGetSubscriptionData(client.Habbo.VIPRank, out SubscriptionData subData))
                    {
                        if (!string.IsNullOrEmpty(subData.Badge))
                        {
                            if (!client.Habbo.GetBadgeComponent().HasBadge(subData.Badge))
                                client.Habbo.GetBadgeComponent().GiveBadge(subData.Badge, true, client);
                        }
                    }

                    if (!PlusEnvironment.GetGame().GetCacheManager().ContainsUser(client.Habbo.Id))
                        PlusEnvironment.GetGame().GetCacheManager().GenerateUser(client.Habbo.Id);

                    client.Habbo.Look = PlusEnvironment.GetFigureManager().ProcessFigure(client.Habbo.Look, client.Habbo.Gender, client.Habbo.GetClothing().GetClothingParts, true);
                    client.Habbo.InitProcess();

                    if (userData.user.GetPermissions().HasRight("mod_tickets"))
                    {
                        client.SendPacket(new ModeratorInitComposer(
                          PlusEnvironment.GetGame().GetModerationManager().UserMessagePresets,
                          PlusEnvironment.GetGame().GetModerationManager().RoomMessagePresets,
                          PlusEnvironment.GetGame().GetModerationManager().GetTickets));
                    }

                    if (PlusEnvironment.GetSettingsManager().TryGetValue("user.login.message.enabled") == "1")
                        client.SendPacket(new MotdNotificationComposer(PlusEnvironment.GetLanguageManager().TryGetValue("user.login.message")));

                    PlusEnvironment.GetGame().GetRewardManager().CheckRewards(client);
                    return true;
                }
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException(e);
            }
            return false;
        }
    }
}
