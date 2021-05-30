using Terraria.ModLoader;

namespace QoLPrime.Backgrounds
{
	public class ExampleUndergroundBackgroundStyle : ModUndergroundBackgroundStyle
	{
		public override void FillTextureArray(int[] textureSlots) {
			textureSlots[0] = BackgroundTextureLoader.GetBackgroundSlot("QoLPrime/Assets/Textures/Backgrounds/ExampleBiomeUnderground0");
			textureSlots[1] = BackgroundTextureLoader.GetBackgroundSlot("QoLPrime/Assets/Textures/Backgrounds/ExampleBiomeUnderground1");
			textureSlots[2] = BackgroundTextureLoader.GetBackgroundSlot("QoLPrime/Assets/Textures/Backgrounds/ExampleBiomeUnderground2");
			textureSlots[3] = BackgroundTextureLoader.GetBackgroundSlot("QoLPrime/Assets/Textures/Backgrounds/ExampleBiomeUnderground3");
		}
	}
}