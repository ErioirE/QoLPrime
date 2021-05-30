using QoLPrime.Content.Tiles;
using Terraria.ModLoader;

namespace QoLPrime.Common.Systems
{
	public class ExampleBiomeTileCount : ModSystem
	{
		public int exampleBlockCount;

		public override void TileCountsAvailable(int[] tileCounts) {
			exampleBlockCount = tileCounts[ModContent.TileType<ExampleBlock>()];
		}
	}
}
