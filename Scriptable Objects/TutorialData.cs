using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy
{
    [CreateAssetMenu]
    public class TutorialData : ScriptableObject
    {
        public struct TutorialContent
        {
            public string Title { get; }
            public string Body { get; }

            public TutorialContent(string title, string body)
            {
                Title = title;
                Body = body;
            }
        }

        public Dictionary<TutorialType, TutorialContent> tutorialContent { get; private set; } = new Dictionary<TutorialType, TutorialContent>
        {
            { TutorialType.Preparation, new TutorialContent("Preparation", "Welcome to Prep Mode. Organize your squad before deployment. Review available mechs, assign pilots to each unit, and optimize your team composition for the upcoming mission. Consider each mech's strengths and weaknesses, and ensure your squad is ready for the challenges ahead.") },
            { TutorialType.Preview, new TutorialContent("Preview", "Welcome to Preview Mode. Examine the mission objectives, enemy forces, and terrain layout. Use this opportunity to strategize and adapt your squad's loadout. Pay attention to enemy types and environmental hazards to maximize your chances of success.") },
            { TutorialType.Assemble, new TutorialContent("Assemble", "Welcome to Assemble Mode. Build and customize your mechs using salvaged parts. Experiment with different combinations for strategic advantages.") },
            { TutorialType.Repair, new TutorialContent("Repair", "Welcome to Repair Mode. Restore damaged mechs to fighting condition. Manage your resources to keep your squad operational.") },
            { TutorialType.Battle, new TutorialContent("Battle", "The battle has begun. Destroy enemy mechs while keeping as many of your own units alive. Good luck!") },
            { TutorialType.Salvage, new TutorialContent("Salvage", "Welcome to Salvage Mode. Collect parts or resources from the fallen mech. Choose wisely to upgrade your arsenal.") },
            { TutorialType.LevelCap, new TutorialContent("End of playtest", "You’ve reached the end of The Orion Project playtest! This demo includes about six levels, but the full game will have many more features and content. Thanks so much for playing—stay tuned for what’s coming next!") }
        };
    }
}