﻿namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition.V1
{
    public interface IVoiceDetectionManager
    {
        bool HasDetectedVoice(float[] data);

#if !NET_2_0 && !NET_2_0_SUBSET
		void DetectThreshold(int durationSec = 3);
#endif
	}
}