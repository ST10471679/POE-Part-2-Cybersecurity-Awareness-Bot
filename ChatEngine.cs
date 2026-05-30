using System;
using System.Collections.Generic;
using System.Linq;

namespace CybersecurityChatbot
{
    public class ChatEngine
    {
        // Generic Collections for optimal performance and code organization
        private readonly Dictionary<string, string> _keywordResponses;
        private readonly List<string> _phishingTips;
        private readonly Dictionary<string, string> _sentimentTriggers;

        // Track conversation history/state
        public UserData SessionUser { get; private set; }
        private string _lastDiscussedTopic = string.Empty;

        public ChatEngine()
        {
            SessionUser = new UserData();

            // Initialize Keyword Recognition
            _keywordResponses = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "password", "Make sure to use strong, unique passwords for each account. Avoid using personal details in your passwords." },
                { "scam", "Scams often look like urgent requests from banks or delivery services. Never click links or provide OTPs unexpectedly." },
                { "privacy", "Protect your digital footprint by reviewing social media settings regularly and disabling tracking permissions." }
            };

            // Initialize Random Response Array Data
            _phishingTips = new List<string>
            {
                "Be cautious of emails asking for personal information. Scammers often disguise themselves as trusted organisations.",
                "Check the sender's actual email address carefully. A misplaced letter usually indicates a fraudulent phishing domain.",
                "Hover over hyperlinks to preview the actual destination URL before clicking on any link.",
                "If an email creates an artificial sense of extreme urgency, treat it as highly suspicious."
            };

            // Initialize Sentiment Detection Triggers
            _sentimentTriggers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "worried", "It's completely understandable to feel that way. Scammers can be very convincing. Let me share some tips to help you stay safe." },
                { "frustrated", "Cyber threats are annoying and overwhelming, but taking small security precautions puts control back in your hands." },
                { "curious", "I love that initiative! Staying curious is the single best defense mechanism against social engineering attacks." }
            };
        }

        public string ProcessInput(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
                return "I'm not sure I understand. Can you try rephrasing?";

            string lowerInput = userInput.ToLower().Trim();

            // 1. Check for Memory Introduction (e.g., "My name is...")
            if (lowerInput.Contains("my name is"))
            {
                string name = userInput.Substring(userInput.IndexOf("is", StringComparison.OrdinalIgnoreCase) + 2).Trim();
                SessionUser.Name = name;
                return $"Great to meet you, {name}! What cybersecurity topic would you like to explore today?";
            }

            // 2. Check Conversation Flow Follow-ups
            if (lowerInput.Contains("explain more") || lowerInput.Contains("give me another tip") || lowerInput.Contains("tell me more"))
            {
                if (_lastDiscussedTopic == "phishing" || lowerInput.Contains("tip"))
                {
                    return GetRandomPhishingTip();
                }
                if (!string.IsNullOrEmpty(_lastDiscussedTopic) && _keywordResponses.ContainsKey(_lastDiscussedTopic))
                {
                    return $"Expanding on {_lastDiscussedTopic}: Remember that proactive protection is key. Always use multi-factor authentication where possible.";
                }
                return "We can explore deep dives into passwords, phishing scams, or network privacy. Which one is worrying you?";
            }

            // 3. Check Sentiment Detection
            foreach (var trigger in _sentimentTriggers)
            {
                if (lowerInput.Contains(trigger.Key))
                {
                    // Requirements state to share a tip immediately and cleanly close input flow on this topic
                    string genericTip = " Standard security advice: Always verify communication channels offline.";
                    return trigger.Value + genericTip;
                }
            }

            // 4. Check Keyword Recognition
            foreach (var keyword in _keywordResponses.Keys)
            {
                if (lowerInput.Contains(keyword))
                {
                    _lastDiscussedTopic = keyword;

                    // Capture favorite topic into memory dynamically if not already set
                    if (!SessionUser.HasProvidedTopic)
                    {
                        SessionUser.FavoriteTopic = keyword;
                    }

                    // Personalize response if memory conditions match
                    if (SessionUser.FavoriteTopic == keyword && SessionUser.Name != "User")
                    {
                        return $"Hey {SessionUser.Name}, since you are interested in {keyword}: {_keywordResponses[keyword]}";
                    }

                    return _keywordResponses[keyword];
                }
            }

            // 5. Check explicitly for Phishing requests
            if (lowerInput.Contains("phishing") || lowerInput.Contains("scam"))
            {
                _lastDiscussedTopic = "phishing";
                return GetRandomPhishingTip();
            }

            // 6. Error Handling & Edge Cases (Fallback response)
            return "I'm not sure I understand that query. I can help you with password safety, identifying phishing scams, or online privacy guidelines. Can you try rephrasing?";
        }

        private string GetRandomPhishingTip()
        {
            Random random = new Random();
            int index = random.Next(_phishingTips.Count);
            return _phishingTips[index];
        }
    }
}
