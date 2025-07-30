using UnityEngine;
using System.Collections.Generic;

namespace TurnBasedStrategy
{
    [CreateAssetMenu]
    public class MissionData : ScriptableObject
    {
        public List<string> missionTips { get; private set; }

        private Dictionary<string, List<string>> unitTypeTextDict;
        private Dictionary<string, List<string>> weaponTypeTextDict;
        private Dictionary<string, List<string>> damageTypeTextDict;
        private Dictionary<string, List<string>> difficultyTextDict;

        public string GetUnitTypeText(UnitType? unitType)
        {
            string key = unitType.HasValue ? unitType.Value.ToString() : "balanced";
            return GetRandomText(unitTypeTextDict, key);
        }

        public string GetWeaponTypeText(WeaponType? weaponType)
        {
            string key = weaponType.HasValue ? weaponType.Value.ToString() : "balanced";
            return GetRandomText(weaponTypeTextDict, key);
        }

        public string GetDamageTypeText(DamageType? damageType)
        {
            string key = damageType.HasValue ? damageType.Value.ToString() : "balanced";
            return GetRandomText(damageTypeTextDict, key);
        }

        public string GetDifficultyText(int difficulty)
        {
            return GetRandomText(difficultyTextDict, difficulty.ToString());
        }

        private string GetRandomText(Dictionary<string, List<string>> dict, string key)
        {
            if (dict.ContainsKey(key) && dict[key].Count > 0)
            {
                int index = UnityEngine.Random.Range(0, dict[key].Count);
                return dict[key][index];
            }
            return "Unknown";
        }

        public void InitializeMissionTips()
        {
            missionTips = new List<string>
            {
                "Lasers deal double damage to shields but only quarter damage to armor. " +
                "Use them strategically against shielded units.",

                "Missiles excel at destroying armored targets but are ineffective against shields." +
                " Consider your target's defenses before firing.",

                "Ground units and hover units receive protection bonuses in forests, making them harder to hit." +
                " Draw them into open terrain when possible.",

                "Hover units generate heat when moving—plan" +
                " your positioning carefully to avoid overheating.",

                "Shield regeneration occurs naturally over time, but armor and health don't." +
                " Preserve units with damaged armor by keeping them away from direct fire.",

                "Rough terrain reduces damage by 30%. Consider this when" +
                " positioning your units or targeting enemies.",

                "Units with depleted shields and armor are vulnerable to all weapon types. " +
                "Focus fire to bring down shields quickly, then finish with appropriate weapons.",

                "Remember the damage cascade: shields absorb damage first, " +
                "then armor, and finally health. Plan your attacks accordingly.",

                "Bullets provide balanced damage against all defense types, " +
                "making them reliable in mixed combat scenarios.",

                "If your unit overheats, it will explode! Monitor heat levels closely " +
                "during extended engagements.",

                "Conserve ammunition for both missile weapons and guns. Unlike energy weapons, " +
                "they can't be fired indefinitely.",
            };
        }

        public void InitializeTextVariants()
        {
            unitTypeTextDict = new Dictionary<string, List<string>>
            {
                { "balanced", new List<string> {
                    "Enemy forces consist of a mix of ground, hover, and aerial units.",
                    "Expect to encounter diverse opposition including infantry, hover tanks, and air support.",
                    "Sensor readings show a balanced enemy force with multiple unit types."
                }},

                { "Ground", new List<string> {
                    "Heavy ground forces have been detected in the area.",
                    "The enemy has deployed significant ground-based mechanized units.",
                    "Satellite imaging shows concentrated ground units in the target zone."
                }},

                { "Hover", new List<string> {
                    "Fast-moving hover units are patrolling the region.",
                    "The enemy is fielding hover tanks for rapid deployment.",
                    "Expect hover craft that can traverse both land and water obstacles."
                }},

                { "Air", new List<string> {
                    "Enemy aerial units control the skies above the mission area.",
                    "Prepare for attacks from airborne hostiles.",
                    "Air superiority fighters have been spotted in your operational zone."
                }}
            };

            weaponTypeTextDict = new Dictionary<string, List<string>>
            {
                { "balanced", new List<string> {
                    "They're equipped with a variety of weapon systems.",
                    "Intel shows mixed armaments among enemy forces.",
                    "The opposition has deployed a balanced arsenal against us."
                }},

                { "Bullet", new List<string> {
                    "Enemy forces are armed with rapid-fire ballistic weapons.",
                    "Expect heavy automatic weapon fire from enemy positions.",
                    "Our scouts report ballistic-based armaments on most units."
                }},

                { "Missile", new List<string> {
                    "Intel shows they're heavily reliant on missile systems.",
                    "Guided missile platforms have been spotted in the area.",
                    "The enemy has deployed missile batteries across the battlefield."
                }},

                { "Laser", new List<string> {
                    "Be cautious of their advanced energy weapon systems.",
                    "Enemy units are equipped with cutting-edge laser technology.",
                    "Thermal scans detect high-energy laser emitters in the area."
                }}
            };

            damageTypeTextDict = new Dictionary<string, List<string>>
            {
                { "balanced", new List<string> {
                    "Their defenses are well-balanced across shields, armor, and structural integrity.",
                    "Enemy units show no particular defensive specialization.",
                    "Balanced protection systems detected on hostile forces."
                }},

                { "Shield", new List<string> {
                    "Our scans show they've prioritized shield technology.",
                    "Enemy units are equipped with advanced energy shielding.",
                    "Prepare for opponents with powerful shield generators."
                }},

                { "Armor", new List<string> {
                    "These units feature reinforced armor plating.",
                    "Heavy armor is the primary defense of enemy forces.",
                    "The opposition relies on thick armor for protection."
                }},

                { "Health", new List<string> {
                    "Intelligence suggests robust structural integrity but minimal protective systems.",
                    "Enemy units lack special defenses but have reinforced frames.",
                    "These opponents prioritize raw durability over specialized protection."
                }}
            };

            difficultyTextDict = new Dictionary<string, List<string>>
            {
                { 1.ToString(), new List<string> {
                    "Intelligence reports indicate a routine operation ahead.",
                    "This mission has been classified as low threat.",
                    "We're expecting minimal resistance."
                }},

                { 2.ToString(), new List<string> {
                    "Our scouts have identified moderate enemy presence in the area.",
                    "Expect standard opposition forces.",
                    "A routine engagement with moderate difficulty."
                }},

                { 3.ToString(), new List<string> {
                    "Be advised: this operation involves substantial opposition forces.",
                    "Command rates this mission as moderately challenging.",
                    "The enemy presence is significant but manageable."
                }},

                { 4.ToString(), new List<string> {
                    "Command has classified this as a high-risk engagement.",
                    "Strong enemy forces are deployed in the target area.",
                    "Prepare for heavy resistance in this operation."
                }},

                { 5.ToString(), new List<string> {
                    "WARNING: Extreme threat level detected. Prepare for heavy resistance.",
                    "This is a maximum difficulty operation. Exercise extreme caution.",
                    "The strongest enemy forces we've encountered are in this area."
                }}
            };
        }
    }
}