using System;
using System.Collections.Generic;
using System.Text;

namespace CybersecurityChatbot
{
    public class UserData
    {
        public string Name { get; set; } = "User";
        public string FavoriteTopic { get; set; } = string.Empty;
        public bool HasProvidedTopic => !string.IsNullOrEmpty(FavoriteTopic);
    }
}