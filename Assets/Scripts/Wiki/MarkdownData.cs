namespace TownWorldWiki
{
    public class MarkdownData
    {
		public MarkdownData(string data = null)
        {
			if (data != null) markdownData1 = data;
        }

		public string Data { get { return markdownData1; } }

		private string markdownData1 = @"

# Town World


Summary
=======
Town World is somewhere in [Superspace](./Town_World/Locations/Superspace.markdown). It is a planet which has been [tidally locked](./Town_World/Concepts/Tidal_Lock.markdown) to its star at some point in the past. Time may not pass in the same manner here as in [normal space](./Town_World/Locations/Space.markdown), where Earth resides. It’s very difficult to cross between space and superspace. Additionally, the current time, and even the [timeline](./Town_World/Concepts/Timeline.markdown) of events, on the other side of each crossing, may be different than expected.

It is planet of small towns with different mix of [creatures](./Town_World/Creatures.markdown) in each. They form [factions](./Town_World/Concepts/Factions.markdown) that may or may not cooperate. 



*****


Town World!
===========

[Locations](./Town_World/Locations.markdown)

[Characters](./Town_World/Characters.markdown)

[Creatures](./Town_World/Creatures.markdown)

[Events](./Town_World/Events.markdown)

[Resources](./Town_World/Resources.markdown)

[Concepts](./Town_World/Concepts.markdown)

[Vehicles](./Town_World/Vehicles.markdown)

[Activities](./Town_World/Activities.markdown)

[Hazards](./Town_World/Hazards.markdown)


*****


Game Stuff
==========
[Gameplay](./Town_World/Gameplay.markdown)

[Guidelines For Town World Design](./Town_World/Design_Guide.markdown)

Temporary Notes
===============

[Loose Ends](./Town_World/Loose_Ends.markdown)




  
";
	}
}
