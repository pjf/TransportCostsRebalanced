using System.Collections.Generic;
using ProjectAutomata;
using UnityEngine;

// In the vanilla game, trucks end up *always* being the best transport option, since they're the cheapest
// per unit of cargo (2.33 per tile per cargo with upgrades), the lowest fixed dispatch cost, and can use
// infrastructure that's already been built!
//
// Sure, everything else is faster, but without item decay or other time penalties, it's throughput that matters,
// not speed.
//
// This mod changes the dispatch costs of vehicles so there's now a greater distinction between the different
// transport systems.
//
// Delivery trucks now have *no* fixed dispatch cost, but a much higher cost-per-tile. This means you're not penalised
// for having a warehouse in your production chain, and short trips between adjacent buildings (such as a warehouse and
// its terminal) are much cheaper. However you *are* penalised if you have delivery trucks travelling long
// distances.
//
// Depot (bulk) trucks are the champions of mid-range deliveries. Their dispatch cost is less than that of trains, but
// their per-tile cost is higher (but not as high as delivery trucks).
//
// Trains are now more cost-effective when transporting cargo over longer distances. Their dispatch cost is higher than
// that of trucks, but their cost per cargo per tile is much lower. The extra costs of creating a
// train network are now justified in the savings they can provide.
//
// Boats are the most cost-effective way of moving bulk cargo long distances, but this is balanced by them being *much* slower
// than in vanilla. They're also only useful if rivers/coasts go where you want them to.
//
// Zeppelins are currently unchanged. They're still the most expensive form of transport, but have the advantage of not requiring
// any infrastructure to be built.
//
// = Future Work =
//
// - Adjust vehicle capacities so they better suit their roles.

namespace TransportCostsRebalanced
{
	public class TransportCostsRebalanced : Mod
    {
		public override void OnModWasLoaded()
		{
			base.OnModWasLoaded();

			Debug.Log("Loading TransportCostsRebalanced");

			// Delivery trucks are now $10/tile (was $100+$5/tile). They're cheaper
			// for short-range trips, about the same for a nearby town run, but
			// *much* more expensive for long trips.

			// Depot trucks were $250 + $7/tile. 2 cargo (3 upgraded)
			// They're now $150 + $10/tile, making them more a mid-range option.

			// Trains were $500 + $18/tile. 4 cargo (6 with upgrades)
            // They're now $500 + $12/tile, making them cheaper than vanilla over long distances.

			// Boats were  $750 + $21/tile, 6 cargo (9 with upgrades)
            // They're now $500 + $16/tile, making them cheaper than trains over the same distance.

            // Zeppelins are currently unchanged.

            // Here comes some new cost calculations!
			var formulae = new Dictionary<string, string>
			{
				{ "AwhDispatchCost",               "(   0 + 10 * distance) * difficulty" },
				{ "ManualDestinationDispatchCost", "(   0 + 10 * distance) * difficulty" },
				{ "TruckDepotDispatchCost",        "( 150 + 10 * distance) * difficulty" },
				{ "TrainTerminalDispatchCost",     "( 500 + 12 * distance) * difficulty" },
				{ "BoatDepotDispatchCost",         "( 500 + 16 * distance) * difficulty" },
				{ "ZeppelinFieldDispatchCost",     "(1000 + 30 * distance) * difficulty" }
			};

            // Patch our formula assets to our new costs.
			foreach (var formula in formulae) {
				GameData.instance.GetAsset<Formula>(formula.Key).formula = formula.Value;
			}

			// maxSpeed is 1.5 for trucks. We're nerfing boats from speed 1.875 (faster than a truck)
			// down to 1 (2/3 the speed of a truck).
			GameData.instance.GetAsset<Vehicle>("CargoBoat").maxSpeed = 1;

			Debug.Log("TransportCostsRebalanced patches applied");
		}
	}
}
