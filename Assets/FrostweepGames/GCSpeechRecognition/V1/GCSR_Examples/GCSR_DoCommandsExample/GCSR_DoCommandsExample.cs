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
        private InputField _commandsInputField;
        private Text _resultText;
        private Dropdown _languageDropdown;
        public Dropdown  _microphoneDevicesDropdown;
        public AudioSource audioSource;
        public AudioClip Ban_Da_Co;
        public Image _voiceLevelImage;
        public GameObject[] ButtonLanguage;
        [SerializeField] TextDialogue textDialogue;
        private void Start()
        {
           // _commandsInputField.text = language_VN_EN[1];
               ButtonLanguage[0].SetActive(false);
             ButtonLanguage[1].SetActive(true);
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
			_languageDropdown.value = _languageDropdown.options.IndexOf(_languageDropdown.options.Find(x => x.text == Enumerators.LanguageCode.en_GB.Parse()));
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
    private void DoCommand(string command)
{
    command = command.Trim().ToLowerInvariant();
    Debug.Log("💬 DO COMMAND với text: [" + command + "]");

    // Tạo map lệnh động từ language_VN_EN
    Dictionary<string, System.Action> commandMap = new Dictionary<string, System.Action>();

    // Tách từng câu trong language_VN_EN[0] và language_VN_EN[1]
    string[] vietnameseCommands = language_VN_EN[0].Split(',');
    string[] englishCommands = language_VN_EN[1].Split(',');

    // Đảm bảo cả hai mảng có cùng độ dài
    int commandCount = Mathf.Min(vietnameseCommands.Length, englishCommands.Length);

    for (int i = 0; i < commandCount; i++)
    {
        string vietnameseCmd = vietnameseCommands[i].Trim().ToLowerInvariant();
        string englishCmd = englishCommands[i].Trim().ToLowerInvariant();

        int currentIndex = i;

        // Thêm cả phiên bản tiếng Việt và tiếng Anh
        if (!commandMap.ContainsKey(vietnameseCmd))
        {
            commandMap.Add(vietnameseCmd, () =>
            {
                audioSource.clip = Ban_Da_Co;
                audioSource.Play();
                textDialogue.Update_Text(currentIndex);
                Debug.Log("✅ Executed command for index: " + currentIndex);
            });
        }

        if (!commandMap.ContainsKey(englishCmd))
        {
            commandMap.Add(englishCmd, () =>
            {
                audioSource.clip = Ban_Da_Co;
                audioSource.Play();
                textDialogue.Update_Text(currentIndex);
                Debug.Log("✅ Executed command for index: " + currentIndex);
            });
        }
    }

    // Tìm và chạy lệnh phù hợp
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

    }
}
