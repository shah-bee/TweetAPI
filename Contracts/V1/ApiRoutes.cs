using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace TweetAPI.Contracts.V1
{
    public static class ApiRoutes
    {
        public const string Root = "api";
        public const string Version = "V1";
        public const string Base = Root + "/" + Version;

        public static class Posts
        {
            public const string GetAll = Base + "/Posts";
            public const string Create = Base + "/Posts";
            public const string Get = Base + "/Posts/{postId}";
            public const string Update = Base + "/Posts/{postId}";
            public const string Delete = Base + "/Posts/{postId}";
        }

        public static class Identity {

            public const string Login = Base + "/Identity/Login";
            public const string Register = Base + "/Identity/Register";
        }
    }
}
