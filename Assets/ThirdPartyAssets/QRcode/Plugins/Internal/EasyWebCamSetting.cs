using UnityEngine;
using System.Collections;

namespace TBEasyWebCam.Setting
{
	public enum FocusMode
	{
		Off = 0,
		TapToFocus = 1,
		AutoFocus = 2,
		SoftFocus = 4,
		MacroFocus = 8
	}

	public enum ResolutionMode : byte
	{
		HD = 1,
		FullHD = 2,
		HighestResolution = 4,
		MediumResolution = 8,
		LowestResolution = 16,
		CustomResolution = 32
	}

	public enum FlashMode
	{
		Auto,
		On,
		Off
	}

	public enum TorchMode
	{
		Off=0,
		On
	}

}