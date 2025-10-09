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
namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition.V1.Examples
{
    public class GCSR_DoCommandsExample : MonoBehaviour
    {
		[TextAreaAttribute(8,100)]
		public string[] language_VN_EN;
        private GCSpeechRecognition _speechRecognition;
        public int Index;
        private Image _speechRecognitionState;
        public Button _startRecordButton;
        public InputField _commandsInputField;
        private Text _resultText;
        private Dropdown _languageDropdown;
        public Dropdown  _microphoneDevicesDropdown;
        public AudioSource audioSource;
        public Image _voiceLevelImage;
        public GameObject[] ButtonLanguage;
        [SerializeField] TextDialogue_EN textDialogue_EN;
        [SerializeField] TextDialogue_VN textDialogue_VN;
        [Header("Aduio Clip Language VN & EN")]
        public AudioClip[] Audio_language_VN;
        public AudioClip[] Audio_language_EN;
        private void Start()
        {
            textDialogue_EN.enabled = false;
            textDialogue_VN.enabled = true;
           // _commandsInputField.text = language_VN_EN[1];
               ButtonLanguage[0].SetActive(true);
             ButtonLanguage[1].SetActive(false);
            _speechRecognition = GCSpeechRecognition.Instance;

            // event handlers
            _speechRecognition.RecognizeSuccessEvent += RecognizeSuccessEventHandler;
            _speechRecognition.RecognizeFailedEvent += RecognizeFailedEventHandler;
            _speechRecognition.FinishedRecordEvent += FinishedRecordEventHandler;
            _speechRecognition.StartedRecordEvent += StartedRecordEventHandler;
            _speechRecognition.RecordFailedEvent += RecordFailedEventHandler;
            _speechRecognition.EndTalkigEvent += EndTalkigEventHandler;

            // UI setup
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
            // load languages
            for (int i = 0; i < Enum.GetNames(typeof(Enumerators.LanguageCode)).Length; i++)
            {
                _languageDropdown.options.Add(new Dropdown.OptionData(((Enumerators.LanguageCode)i).Parse()));
            }
            _languageDropdown.value = _languageDropdown.options.IndexOf(_languageDropdown.options.Find(x => x.text == Enumerators.LanguageCode.vi_VN.Parse())); // đổi sang tiếng Việt

            // select first mic
            if (_speechRecognition.HasConnectedMicrophoneDevices())
            {
                _speechRecognition.SetMicrophoneDevice(_speechRecognition.GetMicrophoneDevices()[0]);
            }
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
            if(_speechRecognition.IsRecording)
			{
				if (_speechRecognition.GetMaxFrame() > 0)
				{
					float max = (float)_speechRecognition.configs[_speechRecognition.currentConfigIndex].voiceDetectionThreshold;
					float current = _speechRecognition.GetLastFrame() / max;

					if(current >= 1f)
					{
						_voiceLevelImage.fillAmount = Mathf.Lerp(_voiceLevelImage.fillAmount, Mathf.Clamp(current / 2f, 0, 1f), 30 * Time.deltaTime);
					}
					else
					{
						_voiceLevelImage.fillAmount = Mathf.Lerp(_voiceLevelImage.fillAmount, Mathf.Clamp(current / 2f, 0, 0.5f), 30 * Time.deltaTime);
					}

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

        private void StartedRecordEventHandler()
        {
            _speechRecognitionState.color = Color.red;
        }

        private void RecordFailedEventHandler()
        {
            _speechRecognitionState.color = Color.yellow;
            _resultText.text = "<color=red>Start record Failed. Please check microphone device and try again.</color>";
            _startRecordButton.interactable = true;
        }

        private void EndTalkigEventHandler(AudioClip clip, float[] raw)
        {
            FinishedRecordEventHandler(clip, raw);
        }

        private void FinishedRecordEventHandler(AudioClip clip, float[] raw)
        {
            if (_startRecordButton.interactable)
            {
                _speechRecognitionState.color = Color.yellow;
            }

            if (clip == null)
                return;

            RecognitionConfig config = RecognitionConfig.GetDefault();
            config.languageCode = ((Enumerators.LanguageCode)_languageDropdown.value).Parse();
            config.audioChannelCount = clip.channels;

            GeneralRecognitionRequest recognitionRequest = new GeneralRecognitionRequest()
            {
                audio = new RecognitionAudioContent()
                {
                    content = raw.ToBase64()
                },
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
            ButtonLanguage[0].SetActive(false);
             ButtonLanguage[1].SetActive(true);
			_commandsInputField.text = language_VN_EN[1];
			_languageDropdown.value = _languageDropdown.options.IndexOf(_languageDropdown.options.Find(x => x.text == Enumerators.LanguageCode.en_US.Parse()));
		}
		public void On_VN()
		{
		      ButtonLanguage[1].SetActive(false);
               ButtonLanguage[0].SetActive(true);
			_commandsInputField.text = language_VN_EN[0];
			 _languageDropdown.value = _languageDropdown.options.IndexOf(
                _languageDropdown.options.Find(x => x.text == Enumerators.LanguageCode.vi_VN.Parse())); // đổi sang tiếng Việt
				
			
		}
        private void RecognizeSuccessEventHandler(RecognitionResponse recognitionResponse)
        {
            _resultText.text = "Detected: ";

            string[] commands = _commandsInputField.text.Split(',');

            foreach (var result in recognitionResponse.results)
            {
                foreach (var alternative in result.alternatives)
                {
                    string cleanTranscript = alternative.transcript.Trim().ToLowerInvariant();
                    cleanTranscript = System.Text.RegularExpressions.Regex.Replace(cleanTranscript, @"[^\p{L}\p{N}\s]", "");


                    _resultText.text += "\nIncome text: " + cleanTranscript;

                    foreach (var command in commands)
                    {
                        string cleanCommand = command.Trim().ToLowerInvariant();

                        if (cleanTranscript.Contains(cleanCommand))
                        {
                            _resultText.text += "\nDid command: " + command +","; 
                            DoCommand(cleanCommand);
                        }
                    }
                }
            }
        }
    private void DoCommand(string rawCommand)
{
    // chuẩn hóa transcript đầu vào
    string command = NormalizeText(rawCommand);
    Debug.Log($"💬 DO COMMAND với text: [{command}]");

    // build commandMap động
    Dictionary<string, System.Action> commandMap = new Dictionary<string, System.Action>();

    string[] vietnameseCommands = (language_VN_EN.Length > 0) ? language_VN_EN[0].Split(',') : new string[0];
    string[] englishCommands = (language_VN_EN.Length > 1) ? language_VN_EN[1].Split(',') : new string[0];

    int maxLen = Math.Max(vietnameseCommands.Length, englishCommands.Length);

    for (int i = 0; i < maxLen; i++)
    {
        int index = i;

        // VN
        if (i < vietnameseCommands.Length)
        {
            string rawVN = vietnameseCommands[i];
            string keyVN = NormalizeText(rawVN);
            if (!string.IsNullOrEmpty(keyVN) && !commandMap.ContainsKey(keyVN))
            {
                // thêm hành động cho VN tại index
                commandMap.Add(keyVN, () =>
                {
                    if (Audio_language_VN != null && index < Audio_language_VN.Length && Audio_language_VN[index] != null)
                    {
                        audioSource.clip = Audio_language_VN[index];
                        audioSource.Play();
                    }
                    textDialogue_VN?.Update_Text(index);
                    Debug.Log($"✅ Executed VN index: {index} for key: [{keyVN}]");
                });

                Debug.Log($"Added VN key [{keyVN}] => index {index}");
            }
        }

        // EN
        if (i < englishCommands.Length)
        {
            string rawEN = englishCommands[i];
            string keyEN = NormalizeText(rawEN);
            if (!string.IsNullOrEmpty(keyEN) && !commandMap.ContainsKey(keyEN))
            {
                commandMap.Add(keyEN, () =>
                {
                    if (Audio_language_EN != null && index < Audio_language_EN.Length && Audio_language_EN[index] != null)
                    {
                        audioSource.clip = Audio_language_EN[index];
                        audioSource.Play();
                    }
                    textDialogue_EN?.Update_Text(index);
                    Debug.Log($"✅ Executed EN index: {index} for key: [{keyEN}]");
                });

                Debug.Log($"Added EN key [{keyEN}] => index {index}");
            }
        }
    }

    bool found = false;
    foreach (var kvp in commandMap)
    {
        if (command.Contains(kvp.Key))
        {
            kvp.Value.Invoke();
            Debug.Log("✅ Executed command (match): " + kvp.Key);
            found = true;
            break;
        }
    }

    if (!found)
    {
        Debug.Log("❌ NOT FOUND COMMAND: " + command + " (len=" + command.Length + ")");
    }

    }
    private string NormalizeText(string input)
{
    if (string.IsNullOrEmpty(input)) return string.Empty;

    input = input.Trim().ToLowerInvariant();

   
    input = Regex.Replace(input, @"[^\p{L}\p{N}\s]", "");

    input = RemoveVietnameseDiacritics(input);

    input = Regex.Replace(input, @"\s+", " ").Trim();

    return input;
}

public static string RemoveVietnameseDiacritics(string input)
{
    if (string.IsNullOrEmpty(input)) return string.Empty;
    var normalized = input.Normalize(NormalizationForm.FormD);
    var sb = new StringBuilder();
    foreach (var c in normalized)
    {
        var uc = CharUnicodeInfo.GetUnicodeCategory(c);
        if (uc != UnicodeCategory.NonSpacingMark)
            sb.Append(c);
    }
    return sb.ToString().Normalize(NormalizationForm.FormC);
}
}
}
