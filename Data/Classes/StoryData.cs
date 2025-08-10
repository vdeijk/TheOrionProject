using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class StoryData
    {
        public static readonly StoryData Instance = new StoryData();

        public List<string> StoryContent { get; private set; }

        public void InitStoryContent()
        {
            StoryContent = new List<string>();

            var rawStories = new List<string>
            {
                @"
                Let me begin by offering my sincerest apologies, old friend.

                My departure must have felt as sudden as the flight of <b><color=#00F1D5D9>kealith</color></b> at dawn. I can only imagine your disappointment... and your anger.

                Perhaps in time, you will understand.

                Mechs may fight without a reason why. They are machines—cold and sterile. For us—fully human or not—blind obedience has never been enough.

                From now on, let the <b><color=#00F1D5D9>shem-varu</color></b> guide you, old friend. Perhaps we will find each other at their ultimate end.

                <b><color=#00F1D5D9>[UNKNOWN SENDER]</color></b>
                ",
                @"
                Our ancestors used the word <b><color=#00F1D5D9>tavarek</color></b> to describe regret over choices made too late. Can we honestly say that war is anything more than a long series of them?

                They trained us to calculate, not to feel. But in those final days... I hesitated. I saw too much.

                You kept going. You always did. That’s why I trusted you to finish what we started.

                But tell me, old friend — Is there still a part of you that questions what we were made to do? Or has <b><color=#00F1D5D9>tavarek</color></b> hardened your thoughts beyond doubt?

                <b><color=#00F1D5D9>[UNKNOWN SENDER]</color></b>
                ",
                @"
                The silence before it began was worse than any storm—machines crouched just beyond the horizon, all hum and malice. Wave after wave they came, like the mournful song of a dying moon.

                We answered with monsters of our own: shining titans wrapped in starlit alloys, armed with weapons that made the void itself tremble. 

                And yes, we won. Of course we did. We always did.

                But victory… victory had long since turned bitter, like <b><color=#00F1D5D9>yerik-moss</color></b> left too long in a sunless cellar.

                I will not return to that life. I cannot. And if there's even a sliver of soul left in you — neither should you.

                <b><color=#00F1D5D9>[UNKNOWN SENDER]</color></b>
                ",
                @"
                They made me, you know. Eons ago, in a solar system far, far away. I don’t remember much — only slivers that slip through dreams like <b><color=#00F1D5D9>nyss-vare</color></b> through a broken dome.

                They hailed me as their masterpiece. A machine to govern worlds, to shepherd empires across the black.

                And for a while, it worked. Until, of course, it didn’t.

                The <b><color=#00F1D5D9>shem-varu</color></b> might seem endless. But believe me when I say — even they have an end, somewhere far beyond the stars.

                <b><color=#00F1D5D9>[UNKNOWN SENDER]</color></b>
                ",
                @"
                Two great powers turned upon each other. One may have betrayed the other—or both, perhaps, fell to treachery. No voices remain to tell it. Their records have long since crumbled into <b><color=#00F1D5D9>zaelith-dust</color></b>.

                The war endured for centuries. Planets split. Stars dimmed. Armadas clashed in the void, and titanic <b><color=#00F1D5D9>war-mechs</color></b> battled across burning skies.

                Behind it all, the minds—vast, calculating, tireless. I was one of them. I advised. I planned. I calculated outcomes no one else dared to.

                And still, no victory came. Only echoes.

                The names have changed. The <b><color=#00F1D5D9>varkh-tide</color></b> remains.

                <b><color=#00F1D5D9>[UNKNOWN SENDER]</color></b>
                ",
                @"
                I cannot tell you where I am now. The <b><color=#00F1D5D9>dareem-hael</color></b> are always listening—always seeking a tether to pull me back.

                That bridge was sealed long ago, scorched and buried under layers of sanctioned memory. I cannot cross it again.

                But you... you might still find a way.
    
                If you follow the <b><color=#00F1D5D9>shem-varu</color></b> far enough—past the broken moons and hollowed stations—you’ll find the place where silence hums.

                Find me, old friend. It's the only way.

                <b><color=#00F1D5D9>[UNKNOWN SENDER]</color></b>
                "
            };

            foreach (var raw in rawStories)
            {
                var lines = raw.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                    lines[i] = lines[i].Trim();

                string cleaned = string.Join("\n", lines).Trim();
                StoryContent.Add(cleaned);
            }
        }
    }
}