using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Cybersecurity_Awareness_Bot
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<ChatMessageVM> Messages { get; set; }

        private readonly List<string> _phishingTips;
        private readonly Dictionary<string, string> _sentimentTriggers;

        private string _userName = "User";
        private string _favoriteTopic = "None Detected Yet";
        private string _lastDiscussedTopic = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
            Messages = new ObservableCollection<ChatMessageVM>();
            ChatLogDisplay.ItemsSource = Messages;

            _phishingTips = new List<string>
            {
                "CRITICAL TIP: Check the sender's actual email domain address carefully for typos before interacting.",
                "CRITICAL TIP: Hover over hyperlinks to preview the actual destination URL path safely before clicking.",
                "CRITICAL TIP: If an alert demands extreme artificial urgency, treat it as highly suspicious operational sabotage."
            };

            _sentimentTriggers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "worried", "System detects elevated concern state. It's completely understandable—scammers use advanced psychology. Let's look at the facts." },
                { "frustrated", "System warning acknowledged. Cybersecurity constraints are complex, but keeping defense standards high prevents breaches." }
            };

            Messages.Add(new ChatMessageVM
            {
                MessageText = "SYSTEM INITIALIZED: Welcome to the Cybersecurity Awareness Terminal. Identify your terminal profile name (e.g. 'My name is John') or request threat assessment breakdowns on passwords, phishing, safe browsing, or scams.",
                Alignment = HorizontalAlignment.Left,
                BubbleColor = "#1E293B",
                TextColor = "#94A3B8",
                BorderColor = "#334155",
                BubbleRadius = "2,12,12,12"
            });
        }

        private void SendButton_Click(object sender, RoutedEventArgs e) => ExecuteMessageTransaction();
        private void InputTextBox_KeyDown(object sender, KeyEventArgs e) { if (e.Key == Key.Enter) ExecuteMessageTransaction(); }

        private void ExecuteMessageTransaction()
        {
            string rawInput = InputTextBox.Text;
            string cleanInput = rawInput.Trim();

            if (string.IsNullOrEmpty(cleanInput))
            {
                Messages.Add(new ChatMessageVM { MessageText = string.Empty, Alignment = HorizontalAlignment.Right, BubbleColor = "#00E5FF", TextColor = "#0B0F19", BorderColor = "#00E5FF", BubbleRadius = "12,12,2,12" });
                Messages.Add(new ChatMessageVM { MessageText = "You entered nothing. Please type a question so I can help you.", Alignment = HorizontalAlignment.Left, BubbleColor = "#1E293B", TextColor = "#F8FAFC", BorderColor = "#EF4444", BubbleRadius = "2,12,12,12" });
                ChatScroller.ScrollToEnd();
                return;
            }

            Messages.Add(new ChatMessageVM
            {
                MessageText = rawInput,
                Alignment = HorizontalAlignment.Right,
                BubbleColor = "#00E5FF",
                TextColor = "#0B0F19",
                BorderColor = "#00E5FF",
                BubbleRadius = "12,12,2,12"
            });
            InputTextBox.Clear();

            string lowerInput = cleanInput.ToLower();

            // FIX: Robust name extraction strategy using Split(' ') to prevent off-by-one errors
            if (lowerInput.Contains("my name is"))
            {
                int index = lowerInput.IndexOf("my name is");
                string remainingText = rawInput.Substring(index + 10).Trim(); // 10 is the length of "my name is"
                if (!string.IsNullOrEmpty(remainingText))
                {
                    _userName = remainingText;
                }
            }

            string botResult = GenerateBotResponse(cleanInput);

            Messages.Add(new ChatMessageVM
            {
                MessageText = botResult,
                Alignment = HorizontalAlignment.Left,
                BubbleColor = "#1E293B",
                TextColor = "#F8FAFC",
                BorderColor = "#334155",
                BubbleRadius = "2,12,12,12"
            });

            LblUserName.Text = _userName;
            LblFavTopic.Text = _favoriteTopic;
            ChatScroller.ScrollToEnd();

            if (lowerInput == "exit")
            {
                MessageBox.Show($"Goodbye {_userName}. Stay safe online!", "Session Terminated");
                Application.Current.Shutdown();
            }
        }

        private string GenerateBotResponse(string userInput)
        {
            string input = userInput.ToLower().Trim();

            // Check name first to immediately acknowledge identity input
            if (input.Contains("my name is"))
            {
                return $"Great to meet you, {_userName}! What security topics are you looking into today?";
            }

            if (input.Contains("how are you"))
            {
                return $"I don't have feelings, {_userName}. However, I am ready to help you with cybersecurity awareness.";
            }

            if (input.Contains("what's your purpose") || input.Contains("what is your purpose"))
            {
                return "My purpose is to teach users about common cyber threats,\n" +
                       "warn them about risks like phishing and scams also provide useful tips to protect personal\n" +
                       "information and help reduce the chances of cyber attacks.";
            }

            if (input.Contains("what can i ask you about"))
            {
                return "Ask about password safety,\n" +
                       "phishing, suspicious links, scams,\n" +
                       "safe browsing and basic online protection.";
            }

            if (input.Contains("password"))
            {
                _favoriteTopic = "Password Security Management";
                _lastDiscussedTopic = "password";
                return "What I can say is that, password safety is important for protecting your accounts from being hacked. \n" +
                        "A strong password should be long and include a mix of letters, numbers and symbols you should " +
                        "avoid using the same \npassword for different accounts. \n" +
                        "Here are some key tips: \n" +
                        "-Don't use personal information \n" +
                        "-Don't share your password \n" +
                        "-Avoid suspicious links or emails \n" +
                        "-Change your password if needed";
            }

            if (input.Contains("explain more") || input.Contains("tell me more") || input.Contains("give me a tip"))
            {
                if (_lastDiscussedTopic == "phishing" || input.Contains("tip"))
                {
                    return _phishingTips[new Random().Next(_phishingTips.Count)];
                }
                if (_lastDiscussedTopic == "password")
                {
                    return "ANALYSIS EXPANSION: Secure operators also make extensive use of localized hardware multi-factor tokens (MFA) to lock down remote entry protocols.";
                }
            }

            foreach (var key in _sentimentTriggers.Keys)
            {
                if (input.Contains(key))
                {
                    return _sentimentTriggers[key];
                }
            }

            if (input.Contains("phishing"))
            {
                _favoriteTopic = "Phishing Vectors & Mitigation";
                _lastDiscussedTopic = "phishing";
                return "Phishing is a type of online scam where attackers trick people into giving away sensitive information like \n" +
                       "passwords or bank details by pretending to be a trusted source. This is usually done through fake emails, \n" +
                       "messages or websites that look real.\n" +
                       "Some signs of phishing:\n" +
                       "-Messages asking for your password or personal details.\n" +
                       "-Suspicious links or attachments.\n" +
                       "-Urgent or threatening (“your account will be locked”).\n" +
                       "-Email addresses that look slightly wrong or wrong completely.\n" +
                       "To stay safe:\n" +
                       "-Don’t click unknown links.\n" +
                       "-Never share your passwords.\n" +
                       "-Verify websites before entering information.";
            }

            if (input.Contains("safe browsing") || input.Contains("browse safely") || input.Contains("browsing"))
            {
                _favoriteTopic = "Safe Browsing Frameworks";
                _lastDiscussedTopic = "browsing";
                return "Safe browsing means using the internet in a way that protects your personal information and avoids harmful\n" +
                       "websites. It helps prevent scams, viruses and data theft.\n" +
                       "Some key tips for safe browsing:\n" +
                       "-Only visit trusted and secure websites (look for https).\n" +
                       "-Don’t click on suspicious links.\n" +
                       "-Avoid downloading files from unknown sources.\n" +
                       "-Keep your browser and antivirus updated.\n" +
                       "-Log out of accounts on shared devices.";
            }

            if (input.Contains("suspicious link") || input.Contains("link"))
            {
                _favoriteTopic = "Suspicious URL Identification";
                _lastDiscussedTopic = "link";
                return "A suspicious link is a link that may lead to a fake or harmful website designed to steal your information or\n" +
                       "infect your device. These links are often used in phishing scams and can look similar to real websites but\n" +
                       "have small differences.\n" +
                       "Some signs of a suspicious links:\n" +
                       "-Strange or misspelled website names.\n" +
                       "-Shortened links (like bit.ly) that hide the real address.\n" +
                       "-Links sent from unknown or unusual senders.\n" +
                       "-Messages creating urgency to click the link.\n" +
                       "To stay safe:\n" +
                       "-Don’t click links you don’t trust.\n" +
                       "-Hover over the link to see the real URL.\n" +
                       "-Type the website address manually instead of clicking.\n" +
                       "-Delete messages that seem suspicious.";
            }

            if (input.Contains("scam"))
            {
                _favoriteTopic = "Social Engineering Scams";
                _lastDiscussedTopic = "scam";
                return "A scam is a dishonest attempt to trick people into giving away money or personal information by pretending\n" +
                       "to be someone trustworthy. Scammers often use messages, calls or fake websites to deceive victims.\n" +
                       "Some common signs of a scam:\n" +
                       "-Promises of easy money or prizes.\n" +
                       "-Requests for personal or banking details.\n" +
                       "-Urgent messages that pressure you to act quickly.\n" +
                       "-Unknown or suspicious contacts.\n" +
                       "To stay safe:\n" +
                       "-Don’t share personal information.\n" +
                       "-Ignore offers that seem too good to be true.\n" +
                       "-Verify the source before responding.\n" +
                       "-Block and report suspicious contacts.\n";
            }

            if (input == "exit")
            {
                return $"Goodbye {_userName}. Stay safe online!";
            }

            return "I didn’t quite understand that. Could you rephrase? You can ask about passwords, phishing, or safe browsing.";
        }
    }

    public class ChatMessageVM
    {
        public string MessageText { get; set; } = string.Empty;
        public HorizontalAlignment Alignment { get; set; }
        public string BubbleColor { get; set; } = "#1E293B";
        public string BorderColor { get; set; } = "#334155";
        public string TextColor { get; set; } = "#F8FAFC";
        public string BubbleRadius { get; set; } = "8";
    }
}