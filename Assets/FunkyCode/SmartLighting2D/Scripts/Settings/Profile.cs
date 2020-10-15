using UnityEngine;

namespace LightingSettings {
	[CreateAssetMenu(fileName = "Data", menuName = "Lighting2D/Profile Settings", order = 1)]

	public class Profile : ScriptableObject {
		public BufferPresetList bufferPresets;

		public LightPresetList lightPresets;

		public QualitySettings qualitySettings;

		public DayLightingSettings dayLightingSettings;

		public FogOfWar fogOfWar;

		public bool disable = false;

		public Color DarknessColor
		{
			get => bufferPresets.list[0].darknessColor;

			set => bufferPresets.list[0].darknessColor = value;
		}

		public Profile() {
			qualitySettings = new QualitySettings();

			bufferPresets = new BufferPresetList();
			bufferPresets.list[0] = new BufferPreset(0);
			bufferPresets.list[0].darknessColor = new Color(0, 0, 0, 1);

			lightPresets = new LightPresetList();
			lightPresets.list[0] = new LightPreset(0);
			
			dayLightingSettings = new DayLightingSettings();

			fogOfWar = new FogOfWar();
		}
	}
}