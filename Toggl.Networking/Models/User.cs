using System;
using Newtonsoft.Json;
using Toggl.Shared;
using Toggl.Shared.Models;
using Toggl.Networking.Serialization.Converters;

namespace Toggl.Networking.Models
{
    internal sealed partial class User : IUser
    {
        public long Id { get; set; }

        public string ApiToken { get; set; }
        
        public long? DefaultWorkspaceId { get; set; }

        [JsonConverter(typeof(EmailConverter))]
        public Email Email { get; set; }

        public string Fullname { get; set; }

        public BeginningOfWeek BeginningOfWeek { get; set; }

        public string Language { get; set; }

        public string ImageUrl { get; set; }

        public string Timezone { get; set; }

        public DateTimeOffset At { get; set; }
    }
}
