using System;
using FrostweepGames.Plugins.GoogleCloud.SpeechRecognition.Tools;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine.EventSystems;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine.Networking; // 👈 thêm dòng này để dùng UnityWebRequest
using TMPro;
namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition.V1.Examples
{
    public class GCSR_DoCommandsExample : MonoBehaviour
    {
        [Header("Text ChatBot")]
        [SerializeField] TextMeshProUGUI TextComponent;
        [TextAreaAttribute(8, 100)]
        public string[] language_VN_EN;
        private GCSpeechRecognition _speechRecognition;
        public int Index;
        private Image _speechRecognitionState;
        public Button _startRecordButton;
        public InputField _commandsInputField;
        private Text _resultText;
        private Dropdown _languageDropdown;
        public Dropdown _microphoneDevicesDropdown;
        public AudioSource audioSource;
        public Image _voiceLevelImage;
        public GameObject[] ButtonLanguage;
        [SerializeField] TextDialogue_EN textDialogue_EN;
        [SerializeField] TextDialogue_VN textDialogue_VN;
        public bool voice_VN_EN;
        private void Awake()
        {

        }
        private void Start()
        {
            textDialogue_EN.enabled = false;
            textDialogue_VN.enabled = true;
            ButtonLanguage[0].SetActive(true);
            ButtonLanguage[1].SetActive(false);

            _speechRecognition = GCSpeechRecognition.Instance;
            _speechRecognition.RecognizeSuccessEvent += RecognizeSuccessEventHandler;
            _speechRecognition.RecognizeFailedEvent += RecognizeFailedEventHandler;
            _speechRecognition.FinishedRecordEvent += FinishedRecordEventHandler;
            _speechRecognition.StartedRecordEvent += StartedRecordEventHandler;
            _speechRecognition.RecordFailedEvent += RecordFailedEventHandler;
            _speechRecognition.EndTalkigEvent += EndTalkigEventHandler;

            _speechRecognitionState = transform.Find("Canvas/Image_RecordState").GetComponent<Image>();
            _resultText = transform.Find("Canvas/Text_Result").GetComponent<Text>();
            _commandsInputField = transform.Find("Canvas/InputField_Commands").GetComponent<InputField>();
            _languageDropdown = transform.Find("Canvas/Dropdown_Language").GetComponent<Dropdown>();
            _microphoneDevicesDropdown = transform.Find("Canvas/Dropdown_MicrophoneDevices").GetComponent<Dropdown>();
            _microphoneDevicesDropdown.onValueChanged.AddListener(MicrophoneDevicesDropdownOnValueChangedEventHandler);
            _startRecordButton.interactable = true;
            _speechRecognitionState.color = Color.yellow;

            _languageDropdown.ClearOptions();
            _speechRecognition.RequestMicrophonePermission(null);
            RefreshMicsButtonOnClickHandler();
            _commandsInputField.text = language_VN_EN[0];

            for (int i = 0; i < Enum.GetNames(typeof(Enumerators.LanguageCode)).Length; i++)
                _languageDropdown.options.Add(new Dropdown.OptionData(((Enumerators.LanguageCode)i).Parse()));

            _languageDropdown.value = _languageDropdown.options.IndexOf(_languageDropdown.options.Find(x => x.text == Enumerators.LanguageCode.vi_VN.Parse()));

            if (_speechRecognition.HasConnectedMicrophoneDevices())
                _speechRecognition.SetMicrophoneDevice(_speechRecognition.GetMicrophoneDevices()[0]);
        }

        private void OnDestroy()
        {
            _speechRecognition.RecognizeSuccessEvent -= RecognizeSuccessEventHandler;
            _speechRecognition.RecognizeFailedEvent -= RecognizeFailedEventHandler;
            _speechRecognition.FinishedRecordEvent -= FinishedRecordEventHandler;
            _speechRecognition.StartedRecordEvent -= StartedRecordEventHandler;
            _speechRecognition.RecordFailedEvent -= RecordFailedEventHandler;
            _speechRecognition.EndTalkigEvent -= EndTalkigEventHandler;
        }

        public void StartRecordButtonOnClickHandler(BaseEventData data)
        {
            StartCoroutine(TemporaryStart());
        }

        IEnumerator TemporaryStart()
        {
            yield return new WaitForSeconds(0.13f);
            _startRecordButton.interactable = false;
            _resultText.text = string.Empty;
            _speechRecognition.StartRecord(false);
        }

        private void RefreshMicsButtonOnClickHandler()
        {
            _speechRecognition.RequestMicrophonePermission(null);
            _microphoneDevicesDropdown.ClearOptions();
            _microphoneDevicesDropdown.AddOptions(_speechRecognition.GetMicrophoneDevices().ToList());
            MicrophoneDevicesDropdownOnValueChangedEventHandler(0);
        }

        private void Update()
        {
            if (_speechRecognition.IsRecording)
            {
                if (_speechRecognition.GetMaxFrame() > 0)
                {
                    float max = (float)_speechRecognition.configs[_speechRecognition.currentConfigIndex].voiceDetectionThreshold;
                    float current = _speechRecognition.GetLastFrame() / max;
                    _voiceLevelImage.fillAmount = Mathf.Lerp(_voiceLevelImage.fillAmount, Mathf.Clamp(current / 2f, 0, 1f), 30 * Time.deltaTime);
                    _voiceLevelImage.color = current >= 1f ? Color.green : Color.red;
                }
            }
            else
            {
                _voiceLevelImage.fillAmount = 0f;
            }
        }

        private void MicrophoneDevicesDropdownOnValueChangedEventHandler(int value)
        {
            if (!_speechRecognition.HasConnectedMicrophoneDevices())
                return;
            _speechRecognition.SetMicrophoneDevice(_speechRecognition.GetMicrophoneDevices()[value]);
        }

        public void StopRecordButtonOnClickHandler(BaseEventData data)
        {
            StartCoroutine(TemporaryStop());
        }

        IEnumerator TemporaryStop()
        {
            yield return new WaitForSeconds(0.13f);
            _startRecordButton.interactable = true;
            _speechRecognition.StopRecord();
        }

        private void StartedRecordEventHandler() => _speechRecognitionState.color = Color.red;

        private void RecordFailedEventHandler()
        {
            _speechRecognitionState.color = Color.yellow;
            _resultText.text = "<color=red>Start record Failed. Please check microphone device and try again.</color>";
            _startRecordButton.interactable = true;
        }

        private void EndTalkigEventHandler(AudioClip clip, float[] raw) => FinishedRecordEventHandler(clip, raw);

        private void FinishedRecordEventHandler(AudioClip clip, float[] raw)
        {
            if (_startRecordButton.interactable) _speechRecognitionState.color = Color.yellow;
            if (clip == null) return;

            RecognitionConfig config = RecognitionConfig.GetDefault();
            config.languageCode = ((Enumerators.LanguageCode)_languageDropdown.value).Parse();
            config.audioChannelCount = clip.channels;

            GeneralRecognitionRequest recognitionRequest = new GeneralRecognitionRequest()
            {
                audio = new RecognitionAudioContent() { content = raw.ToBase64() },
                config = config
            };

            _speechRecognition.Recognize(recognitionRequest);
        }

        private void RecognizeFailedEventHandler(string error)
        {
            _resultText.text = "Recognize Failed: " + error;
        }

        public void On_EN()
        {
             voice_VN_EN = false;
            ButtonLanguage[0].SetActive(false);
            ButtonLanguage[1].SetActive(true);
            _commandsInputField.text = language_VN_EN[1];
            _languageDropdown.value = _languageDropdown.options.IndexOf(_languageDropdown.options.Find(x => x.text == Enumerators.LanguageCode.en_US.Parse()));
        }

        public void On_VN()
        {
            voice_VN_EN = true;
            ButtonLanguage[1].SetActive(false);
            ButtonLanguage[0].SetActive(true);
            _commandsInputField.text = language_VN_EN[0];
            _languageDropdown.value = _languageDropdown.options.IndexOf(_languageDropdown.options.Find(x => x.text == Enumerators.LanguageCode.vi_VN.Parse()));
        }
    // ✅ Hàm tự động phát hiện tiếng Việt / tiếng Anh
      private void RecognizeSuccessEventHandler(RecognitionResponse recognitionResponse)
{
    if (_resultText == null)
    {
        Debug.LogError("❌ _resultText is not assigned in the inspector!");
        return;
    }

    if (_commandsInputField == null)
    {
        Debug.LogError("❌ _commandsInputField is not assigned in the inspector!");
        return;
    }

    if (recognitionResponse == null || recognitionResponse.results == null)
    {
        _resultText.text = "⚠️ No recognition results received.";
        Debug.LogWarning("recognitionResponse or its results is null!");
        return;
    }

    _resultText.text = "Detected: ";

    // lấy danh sách các lệnh được khai báo
    string[] commands = _commandsInputField.text.Split(',');
    for (int i = 0; i < commands.Length; i++)
        commands[i] = commands[i].Trim().ToLowerInvariant();

    // xử lý từng kết quả nhận dạng
    foreach (var result in recognitionResponse.results)
    {
        if (result.alternatives == null) continue;

        foreach (var alternative in result.alternatives)
        {
            if (alternative == null || string.IsNullOrEmpty(alternative.transcript))
                continue;

            string cleanTranscript = alternative.transcript.Trim().ToLowerInvariant();
            cleanTranscript = Regex.Replace(cleanTranscript, @"[^\p{L}\p{N}\s]", "");
            _resultText.text += "\nUser said: " + cleanTranscript;

            foreach (var command in commands)
            {
                if (cleanTranscript.Contains(command))
                {
                    _resultText.text += "\n✅ Did command: " + command;

                    // 👉 Thực thi lệnh và chatbot
                    DoCommand(command);
                    ChatbotResponse(cleanTranscript); // Truyền cả ngôn ngữ vào
                    break;
                }
            }
        }
    }
}


string DetectLanguage(string text)
{
    string vietnameseChars = "ăâđêôơưàáảãạằắẳẵặầấẩẫậèéẻẽẹềếểễệòóỏõọồốổỗộờớởỡợùúủũụừứửữựìíỉĩịỳýỷỹỵ";
    foreach (char c in vietnameseChars)
    {
        if (text.Contains(c.ToString(), StringComparison.OrdinalIgnoreCase))
            return "vi-VN";
    }
    return "en-US";
}
       
        void ChatbotResponse(string message)
{
    StartCoroutine(new ChatbotAPIs().ChatbotRequest(
        message,
        (result) =>
        {
            Debug.Log("🤖 Chatbot response: " + result);
            _resultText.text += "\n🤖 " + result;
                string detectedLang = DetectLanguage(result);
            if (voice_VN_EN == true)
            {
                TextComponent.text = result;
                   textDialogue_VN?.ShowChatbotResponse(result);
                StartCoroutine(SpeakText(result, detectedLang)); 
            }
            else if (voice_VN_EN == false)
            {
                TextComponent.text = result;
                textDialogue_EN?.ShowChatbotResponse(result);
                   StartCoroutine(SpeakText(result, detectedLang)); // tiếng Anh
            }
        },
        (error) =>
        {
            Debug.LogError("❌ Chatbot error: " + error);
            _resultText.text += "\n❌ Chatbot error: " + error;
        }
    ));
}

  [System.Serializable]
public class TextToSpeechRequest
{
    public InputData input;
    public VoiceData voice;
    public AudioConfigData audioConfig;
}

[System.Serializable]
public class InputData
{
    public string text;
}

[System.Serializable]
public class VoiceData
{
    public string languageCode;
    public string name;
    public string ssmlGender;
}

[System.Serializable]
public class AudioConfigData
{
    public string audioEncoding;
}

[System.Serializable]
public class TextToSpeechResponse
{
    public string audioContent;
}

IEnumerator SpeakText(string text, string languageCode)
{
    string apiKey = _speechRecognition.apiKey;
    string url = $"https://texttospeech.googleapis.com/v1/text:synthesize?key={apiKey}";

    string voiceName = (languageCode == "vi-VN") ? "vi-VN-Wavenet-A" : "en-US-Wavenet-D";

    // Dùng class để tạo JSON (Unity JsonUtility chỉ hỗ trợ class, không hỗ trợ anonymous object)
    TextToSpeechRequest requestData = new TextToSpeechRequest
    {
        input = new InputData { text = text },
        voice = new VoiceData { languageCode = languageCode, name = voiceName, ssmlGender = "NEUTRAL" },
        audioConfig = new AudioConfigData { audioEncoding = "LINEAR16" }
    };

    string jsonBody = JsonUtility.ToJson(requestData);

    using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
    {
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            TextToSpeechResponse response = JsonUtility.FromJson<TextToSpeechResponse>(www.downloadHandler.text);
            byte[] audioBytes = Convert.FromBase64String(response.audioContent);

            WAV wav = new WAV(audioBytes);
            AudioClip audioClip = AudioClip.Create("TTS", wav.SampleCount, 1, wav.Frequency, false);
            audioClip.SetData(wav.LeftChannel, 0);

            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {
            Debug.LogError($"❌ TTS Error ({languageCode}): " + www.downloadHandler.text);
        }
    }
}


        // ✅ CLASS GIẢI MÃ WAV
        public class WAV
        {
            public float[] LeftChannel;
            public int ChannelCount;
            public int SampleCount;
            public int Frequency;

            public WAV(byte[] wav)
            {
                ChannelCount = BitConverter.ToInt16(wav, 22);
                Frequency = BitConverter.ToInt32(wav, 24);
                int pos = 44;
                int samples = (wav.Length - pos) / 2;
                LeftChannel = new float[samples];
                for (int i = 0; i < samples; i++)
                    LeftChannel[i] = BytesToFloat(wav[pos + i * 2], wav[pos + i * 2 + 1]);
                SampleCount = samples;
            }

            private static float BytesToFloat(byte firstByte, byte secondByte)
            {
                short s = (short)((secondByte << 8) | firstByte);
                return s / 32768.0F;
            }
        }

        // ✅ CÁC HÀM COMMAND CŨ GIỮ NGUYÊN
        private void DoCommand(string rawCommand)
        {
            string command = NormalizeText(rawCommand);
            Debug.Log($"💬 DO COMMAND với text: [{command}]");

            Dictionary<string, System.Action> commandMap = new Dictionary<string, System.Action>();
            string[] vietnameseCommands = (language_VN_EN.Length > 0) ? language_VN_EN[0].Split(',') : new string[0];
            string[] englishCommands = (language_VN_EN.Length > 1) ? language_VN_EN[1].Split(',') : new string[0];
            int maxLen = Math.Max(vietnameseCommands.Length, englishCommands.Length);

            for (int i = 0; i < maxLen; i++)
            {
                int index = i;
                if (i < vietnameseCommands.Length)
                {
                    string keyVN = NormalizeText(vietnameseCommands[i]);
                    if (!commandMap.ContainsKey(keyVN))
                    {
                        commandMap.Add(keyVN, () =>
                        {
                         //   if (Audio_language_VN != null && index < Audio_language_VN.Length && Audio_language_VN[index] != null)
                         //   {
                               // audioSource.clip = Audio_language_VN[index];
                               // audioSource.Play();
                          //  }
                           
                        });
                    }
                }

                if (i < englishCommands.Length)
                {
                    string keyEN = NormalizeText(englishCommands[i]);
                    if (!commandMap.ContainsKey(keyEN))
                    {
                        commandMap.Add(keyEN, () =>
                        {
                            //if (Audio_language_EN != null && index < Audio_language_EN.Length && Audio_language_EN[index] != null)
                         //   {
                             //   audioSource.clip = Audio_language_EN[index];
                              //  audioSource.Play();
                         //   }
                         
                        });
                    }
                }
            }

            foreach (var kvp in commandMap)
            {
                if (command.Contains(kvp.Key))
                {
                    kvp.Value.Invoke();
                    return;
                }
            }
        }

        private string NormalizeText(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            input = Regex.Replace(input.ToLowerInvariant().Trim(), @"[^\p{L}\p{N}\s]", "");
            input = RemoveVietnameseDiacritics(input);
            return Regex.Replace(input, @"\s+", " ").Trim();
        }

        public static string RemoveVietnameseDiacritics(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            var normalized = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
    [System.Serializable]
        public class TextToSpeechResponse { public string audioContent; }
}
