using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;

namespace BotuaGetFriendTimes.Models
{
    public class NameHelper : INameHelper
    {
        private readonly IAmazonSimpleSystemsManagement _ssm;
        private long JordanDiscordId { get; set; }
        private long JoshuaDiscordId { get; set; }
        private long DayleDiscordId { get; set; }
        private long MadalynDiscordId { get; set; }
        private long JonnyDiscordId { get; set; }
        private long LucasDiscordId { get; set; }
        private long CallanDiscordId { get; set; }
        private long AndrewDiscordId { get; set; }
        private long MartinDiscordId { get; set; }
        private Dictionary<long, string> Colors { get; set; }

        public NameHelper(IAmazonSimpleSystemsManagement ssm)
        {
            _ssm = ssm;
            GetUserIds().Wait();
            SetColours();
        }

        private async Task GetUserIds()
        {
            JordanDiscordId = await GetSSMValue("JordanId");
            JoshuaDiscordId = await GetSSMValue("JoshuaId");
            DayleDiscordId = await GetSSMValue("DayleId");
            MadalynDiscordId = await GetSSMValue("DeclynId");
            JonnyDiscordId = await GetSSMValue("JonnyId");
            LucasDiscordId = await GetSSMValue("LucasId");
            CallanDiscordId = await GetSSMValue("CallanId");
            AndrewDiscordId = await GetSSMValue("AndrewId");
            MartinDiscordId = await GetSSMValue("MartinId");
        }

        private void SetColours()
        {
            Colors = new Dictionary<long, string>
            {
                {JordanDiscordId, "rgba(61, 129, 255, 0.5)"},
                {JoshuaDiscordId, "rgba(149, 0, 255, 0.5)"},
                {DayleDiscordId, "rgba(21, 128, 11, 0.5)"},
                {MadalynDiscordId, "rgba(238, 255, 0, 0.5)"},
                {JonnyDiscordId, "rgba(252, 3, 202, 0.5)"},
                {LucasDiscordId, "rgba(158, 14, 14, 0.5)"},
                {CallanDiscordId, "rgba(255, 111, 0, 0.5)"},
                {AndrewDiscordId, "rgba(158, 132, 14, 0.5)"},
                {MartinDiscordId, "rgba(235, 126, 49, 0.5)"}
            };
        }
        
        private async Task<long> GetSSMValue(string parameterName)
        {
            return Convert.ToInt64((await _ssm.GetParameterAsync(new GetParameterRequest
            {
                Name = parameterName
            })).Parameter.Value);
        }

        public string GetColourById(long userId)
        {
            return Colors[userId];
        }
        
        public string GetNameById(long id)
        {
            if (id == JordanDiscordId)
                return "Jordan";
            
            if (id == JoshuaDiscordId)
                return "Joshua";
            
            if (id == DayleDiscordId)
                return "Dayle";
            
            if (id == MadalynDiscordId)
                return "Maddie";
            
            if (id == JonnyDiscordId)
                return "Jonny";
            
            if (id == LucasDiscordId)
                return "Lucas";
            
            if (id == CallanDiscordId)
                return "Callan";
            
            if (id == AndrewDiscordId)
                return "Andrew";
            
            if (id == MartinDiscordId)
                return "Martin";

            return "Other";
        }
    }
}