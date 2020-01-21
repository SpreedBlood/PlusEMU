using Microsoft.Extensions.DependencyInjection;
using Plus.Communication.Packets;
using Plus.HabboHotel;
using Plus.HabboHotel.Achievements;
using System;
using System.Linq;
using System.Reflection;

namespace Plus
{
    internal static class DependencyUtils
    {
        public static void RegisterDependencies(this IServiceCollection serviceCollection)
        {
            serviceCollection.RegisterEvents();
            serviceCollection.RegisterManagers();
            serviceCollection
                .AddSingleton<Game>()
                .AddSingleton<PacketManager>();
        }

        private static void RegisterManagers(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<AchievementManager>().AddSingleton<AchievementDao>();
        }

        private static void RegisterEvents(this IServiceCollection serviceCollection)
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.GetInterfaces().Contains(typeof(IPacketEvent)))
                {
                    serviceCollection.AddSingleton(typeof(IPacketEvent), type);
                }
            }
        }
    }
}
