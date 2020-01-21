using Plus.Database.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;

namespace Plus.HabboHotel.Rewards
{
    public class RewardManager
    {
        private ConcurrentDictionary<int, Reward> _rewards;
        private ConcurrentDictionary<int, List<int>> _rewardLogs;

        public RewardManager()
        {
            _rewards = new ConcurrentDictionary<int, Reward>();
            _rewardLogs = new ConcurrentDictionary<int, List<int>>();
        }

        public void Init()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `server_rewards` WHERE enabled = '1'");
                DataTable dTable = dbClient.GetTable();
                if (dTable != null)
                {
                    foreach (DataRow dRow in dTable.Rows)
                    {
                        _rewards.TryAdd((int)dRow["id"], new Reward(Convert.ToDouble(dRow["reward_start"]), Convert.ToDouble(dRow["reward_end"]), Convert.ToString(dRow["reward_type"]), Convert.ToString(dRow["reward_data"]), Convert.ToString(dRow["message"])));
                    }
                }

                dbClient.SetQuery("SELECT * FROM `server_reward_logs`");
                dTable = dbClient.GetTable();
                if (dTable != null)
                {
                    foreach (DataRow dRow in dTable.Rows)
                    {
                        int Id = (int)dRow["user_id"];
                        int RewardId = (int)dRow["reward_id"];

                        if (!_rewardLogs.ContainsKey(Id))
                            _rewardLogs.TryAdd(Id, new List<int>());

                        if (!_rewardLogs[Id].Contains(RewardId))
                            _rewardLogs[Id].Add(RewardId);
                    }
                }
            }
        }

        public bool HasReward(int Id, int RewardId)
        {
            if (!_rewardLogs.ContainsKey(Id))
                return false;

            if (_rewardLogs[Id].Contains(RewardId))
                return true;

            return false;
        }

        public void LogReward(int Id, int RewardId)
        {
            if (!_rewardLogs.ContainsKey(Id))
                _rewardLogs.TryAdd(Id, new List<int>());

            if (!_rewardLogs[Id].Contains(RewardId))
                _rewardLogs[Id].Add(RewardId);

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO `server_reward_logs` VALUES ('', @userId, @rewardId)");
                dbClient.AddParameter("userId", Id);
                dbClient.AddParameter("rewardId", RewardId);
                dbClient.RunQuery();
            }
        }

        public void CheckRewards(GameClient Session)
        {
            if (Session == null || Session.Habbo == null)
                return;

            foreach (KeyValuePair<int, Reward> Entry in _rewards)
            {
                int Id = Entry.Key;
                Reward Reward = Entry.Value;

                if (HasReward(Session.Habbo.Id, Id))
                    continue;

                if (Reward.Active)
                {
                    switch (Reward.Type)
                    {
                        case RewardType.Badge:
                            {
                                if (!Session.Habbo.GetBadgeComponent().HasBadge(Reward.RewardData))
                                    Session.Habbo.GetBadgeComponent().GiveBadge(Reward.RewardData, true, Session);
                                break;
                            }

                        case RewardType.Credits:
                            {
                                Session.Habbo.Credits += Convert.ToInt32(Reward.RewardData);
                                Session.SendPacket(new CreditBalanceComposer(Session.Habbo.Credits));
                                break;
                            }

                        case RewardType.Duckets:
                            {
                                Session.Habbo.Duckets += Convert.ToInt32(Reward.RewardData);
                                Session.SendPacket(new HabboActivityPointNotificationComposer(Session.Habbo.Duckets, Convert.ToInt32(Reward.RewardData)));
                                break;
                            }

                        case RewardType.Diamonds:
                            {
                                Session.Habbo.Diamonds += Convert.ToInt32(Reward.RewardData);
                                Session.SendPacket(new HabboActivityPointNotificationComposer(Session.Habbo.Diamonds, Convert.ToInt32(Reward.RewardData), 5));
                                break;
                            }
                    }

                    if (!String.IsNullOrEmpty(Reward.Message))
                        Session.SendNotification(Reward.Message);

                    LogReward(Session.Habbo.Id, Id);
                }
                else
                    continue;
            }
        }
    }
}
