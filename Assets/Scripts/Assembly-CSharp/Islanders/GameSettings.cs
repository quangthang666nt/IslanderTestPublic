using System;
using FlatBuffers;

namespace Islanders
{
	public struct GameSettings : IFlatbufferObject
	{
		private Table __p;

		public ByteBuffer ByteBuffer => __p.bb;

		public string CurrentLanguage
		{
			get
			{
				int num = __p.__offset(4);
				if (num == 0)
				{
					return null;
				}
				return __p.__string(num + __p.bb_pos);
			}
		}

		public bool ShowTutorial
		{
			get
			{
				int num = __p.__offset(6);
				if (num == 0)
				{
					return false;
				}
				return __p.bb.Get(num + __p.bb_pos) != 0;
			}
		}

		public bool EnableTooltips
		{
			get
			{
				int num = __p.__offset(8);
				if (num == 0)
				{
					return true;
				}
				return __p.bb.Get(num + __p.bb_pos) != 0;
			}
		}

		public bool DragCameraInBuildMode
		{
			get
			{
				int num = __p.__offset(10);
				if (num == 0)
				{
					return true;
				}
				return __p.bb.Get(num + __p.bb_pos) != 0;
			}
		}

		public int ResolutionWidth
		{
			get
			{
				int num = __p.__offset(12);
				if (num == 0)
				{
					return 0;
				}
				return __p.bb.GetInt(num + __p.bb_pos);
			}
		}

		public int ResolutionHeight
		{
			get
			{
				int num = __p.__offset(14);
				if (num == 0)
				{
					return 0;
				}
				return __p.bb.GetInt(num + __p.bb_pos);
			}
		}

		public int ResolutionRefreshRate
		{
			get
			{
				int num = __p.__offset(16);
				if (num == 0)
				{
					return 0;
				}
				return __p.bb.GetInt(num + __p.bb_pos);
			}
		}

		public bool FullScreen
		{
			get
			{
				int num = __p.__offset(18);
				if (num == 0)
				{
					return false;
				}
				return __p.bb.Get(num + __p.bb_pos) != 0;
			}
		}

		public int AntiAliasing
		{
			get
			{
				int num = __p.__offset(20);
				if (num == 0)
				{
					return 4;
				}
				return __p.bb.GetInt(num + __p.bb_pos);
			}
		}

		public bool SSAO
		{
			get
			{
				int num = __p.__offset(22);
				if (num == 0)
				{
					return true;
				}
				return __p.bb.Get(num + __p.bb_pos) != 0;
			}
		}

		public bool VSync
		{
			get
			{
				int num = __p.__offset(24);
				if (num == 0)
				{
					return true;
				}
				return __p.bb.Get(num + __p.bb_pos) != 0;
			}
		}

		public int ShadowResolution
		{
			get
			{
				int num = __p.__offset(26);
				if (num == 0)
				{
					return 3;
				}
				return __p.bb.GetInt(num + __p.bb_pos);
			}
		}

		public float VolumeMaster
		{
			get
			{
				int num = __p.__offset(28);
				if (num == 0)
				{
					return 1f;
				}
				return __p.bb.GetFloat(num + __p.bb_pos);
			}
		}

		public float VolumeWorld
		{
			get
			{
				int num = __p.__offset(30);
				if (num == 0)
				{
					return 1f;
				}
				return __p.bb.GetFloat(num + __p.bb_pos);
			}
		}

		public float VolumeScore
		{
			get
			{
				int num = __p.__offset(32);
				if (num == 0)
				{
					return 1f;
				}
				return __p.bb.GetFloat(num + __p.bb_pos);
			}
		}

		public float VolumeMusic
		{
			get
			{
				int num = __p.__offset(34);
				if (num == 0)
				{
					return 1f;
				}
				return __p.bb.GetFloat(num + __p.bb_pos);
			}
		}

		public float VolumeUI
		{
			get
			{
				int num = __p.__offset(36);
				if (num == 0)
				{
					return 1f;
				}
				return __p.bb.GetFloat(num + __p.bb_pos);
			}
		}

		public static void ValidateVersion()
		{
			FlatBufferConstants.FLATBUFFERS_1_12_0();
		}

		public static GameSettings GetRootAsGameSettings(ByteBuffer _bb)
		{
			return GetRootAsGameSettings(_bb, default(GameSettings));
		}

		public static GameSettings GetRootAsGameSettings(ByteBuffer _bb, GameSettings obj)
		{
			return obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb);
		}

		public static bool GameSettingsBufferHasIdentifier(ByteBuffer _bb)
		{
			return Table.__has_identifier(_bb, "HTF0");
		}

		public void __init(int _i, ByteBuffer _bb)
		{
			__p = new Table(_i, _bb);
		}

		public GameSettings __assign(int _i, ByteBuffer _bb)
		{
			__init(_i, _bb);
			return this;
		}

		public ArraySegment<byte>? GetCurrentLanguageBytes()
		{
			return __p.__vector_as_arraysegment(4);
		}

		public byte[] GetCurrentLanguageArray()
		{
			return __p.__vector_as_array<byte>(4);
		}

		public static Offset<GameSettings> CreateGameSettings(FlatBufferBuilder builder, StringOffset CurrentLanguageOffset = default(StringOffset), bool ShowTutorial = false, bool EnableTooltips = true, bool DragCameraInBuildMode = true, int ResolutionWidth = 0, int ResolutionHeight = 0, int ResolutionRefreshRate = 0, bool FullScreen = false, int AntiAliasing = 4, bool SSAO = true, bool VSync = true, int ShadowResolution = 3, float VolumeMaster = 1f, float VolumeWorld = 1f, float VolumeScore = 1f, float VolumeMusic = 1f, float VolumeUI = 1f)
		{
			builder.StartTable(17);
			AddVolumeUI(builder, VolumeUI);
			AddVolumeMusic(builder, VolumeMusic);
			AddVolumeScore(builder, VolumeScore);
			AddVolumeWorld(builder, VolumeWorld);
			AddVolumeMaster(builder, VolumeMaster);
			AddShadowResolution(builder, ShadowResolution);
			AddAntiAliasing(builder, AntiAliasing);
			AddResolutionRefreshRate(builder, ResolutionRefreshRate);
			AddResolutionHeight(builder, ResolutionHeight);
			AddResolutionWidth(builder, ResolutionWidth);
			AddCurrentLanguage(builder, CurrentLanguageOffset);
			AddVSync(builder, VSync);
			AddSSAO(builder, SSAO);
			AddFullScreen(builder, FullScreen);
			AddDragCameraInBuildMode(builder, DragCameraInBuildMode);
			AddEnableTooltips(builder, EnableTooltips);
			AddShowTutorial(builder, ShowTutorial);
			return EndGameSettings(builder);
		}

		public static void StartGameSettings(FlatBufferBuilder builder)
		{
			builder.StartTable(17);
		}

		public static void AddCurrentLanguage(FlatBufferBuilder builder, StringOffset CurrentLanguageOffset)
		{
			builder.AddOffset(0, CurrentLanguageOffset.Value, 0);
		}

		public static void AddShowTutorial(FlatBufferBuilder builder, bool ShowTutorial)
		{
			builder.AddBool(1, ShowTutorial, d: false);
		}

		public static void AddEnableTooltips(FlatBufferBuilder builder, bool EnableTooltips)
		{
			builder.AddBool(2, EnableTooltips, d: true);
		}

		public static void AddDragCameraInBuildMode(FlatBufferBuilder builder, bool DragCameraInBuildMode)
		{
			builder.AddBool(3, DragCameraInBuildMode, d: true);
		}

		public static void AddResolutionWidth(FlatBufferBuilder builder, int ResolutionWidth)
		{
			builder.AddInt(4, ResolutionWidth, 0);
		}

		public static void AddResolutionHeight(FlatBufferBuilder builder, int ResolutionHeight)
		{
			builder.AddInt(5, ResolutionHeight, 0);
		}

		public static void AddResolutionRefreshRate(FlatBufferBuilder builder, int ResolutionRefreshRate)
		{
			builder.AddInt(6, ResolutionRefreshRate, 0);
		}

		public static void AddFullScreen(FlatBufferBuilder builder, bool FullScreen)
		{
			builder.AddBool(7, FullScreen, d: false);
		}

		public static void AddAntiAliasing(FlatBufferBuilder builder, int AntiAliasing)
		{
			builder.AddInt(8, AntiAliasing, 4);
		}

		public static void AddSSAO(FlatBufferBuilder builder, bool SSAO)
		{
			builder.AddBool(9, SSAO, d: true);
		}

		public static void AddVSync(FlatBufferBuilder builder, bool VSync)
		{
			builder.AddBool(10, VSync, d: true);
		}

		public static void AddShadowResolution(FlatBufferBuilder builder, int ShadowResolution)
		{
			builder.AddInt(11, ShadowResolution, 3);
		}

		public static void AddVolumeMaster(FlatBufferBuilder builder, float VolumeMaster)
		{
			builder.AddFloat(12, VolumeMaster, 1.0);
		}

		public static void AddVolumeWorld(FlatBufferBuilder builder, float VolumeWorld)
		{
			builder.AddFloat(13, VolumeWorld, 1.0);
		}

		public static void AddVolumeScore(FlatBufferBuilder builder, float VolumeScore)
		{
			builder.AddFloat(14, VolumeScore, 1.0);
		}

		public static void AddVolumeMusic(FlatBufferBuilder builder, float VolumeMusic)
		{
			builder.AddFloat(15, VolumeMusic, 1.0);
		}

		public static void AddVolumeUI(FlatBufferBuilder builder, float VolumeUI)
		{
			builder.AddFloat(16, VolumeUI, 1.0);
		}

		public static Offset<GameSettings> EndGameSettings(FlatBufferBuilder builder)
		{
			return new Offset<GameSettings>(builder.EndTable());
		}

		public static void FinishGameSettingsBuffer(FlatBufferBuilder builder, Offset<GameSettings> offset)
		{
			builder.Finish(offset.Value, "HTF0");
		}

		public static void FinishSizePrefixedGameSettingsBuffer(FlatBufferBuilder builder, Offset<GameSettings> offset)
		{
			builder.FinishSizePrefixed(offset.Value, "HTF0");
		}
	}
}
