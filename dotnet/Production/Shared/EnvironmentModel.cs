using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class ClientEnvironmentModel
    {
        public string? Id { get; set; }
        public string? Secret { get; set; }
        public string? Environment { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    public class ApiEndpoint
    {
        public string ENVIRONMENT_NAME { get; set; }
        public string INVOICES_URL { get; set; }
        public string ORION_API_URL { get; set; }
        public string ANALYTICS_URL { get; set; }
        public string WALLETS_API_URL { get; set; }
        public string ADMIN_API_URL { get; set; }
        public string LOGS_URL { get; set; }
        public string REDIS_STREAMS_URL { get; set; }
        public string WATCHER_URL { get; set; }
        public string SETTINGS_URL { get; set; }
        public string CALLBACK_URL { get; set; }

    }
}
