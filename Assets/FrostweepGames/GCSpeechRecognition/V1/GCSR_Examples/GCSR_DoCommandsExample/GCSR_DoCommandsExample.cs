using System;
using FrostweepGames.Plugins.GoogleCloud.SpeechRecognition.Tools;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections.Generic;
using System.Linq;
namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition.V1.Examples
{
    public class GCSR_DoCommandsExample : MonoBehaviour
    {
		[TextAreaAttribute(8,100)]
		public string[] language_VN_EN;
        private GCSpeechRecognition _speechRecognition;

        private Image _speechRecognitionState;
        private Button _startRecordButton, _stopRecordButton;
        private InputField _commandsInputField;
        private Text _resultText;
        private Dropdown _languageDropdown;
        public Dropdown  _microphoneDevicesDropdown;
        public RectTransform _objectForCommand;

        private void Start()
        {
            _speechRecognition = GCSpeechRecognition.Instance;

            // event handlers
            _speechRecognition.RecognizeSuccessEvent += RecognizeSuccessEventHandler;
            _speechRecognition.RecognizeFailedEvent += RecognizeFailedEventHandler;
            _speechRecognition.FinishedRecordEvent += FinishedRecordEventHandler;
            _speechRecognition.StartedRecordEvent += StartedRecordEventHandler;
            _speechRecognition.RecordFailedEvent += RecordFailedEventHandler;
            _speechRecognition.EndTalkigEvent += EndTalkigEventHandler;

            // UI setup
            _startRecordButton = transform.Find("Canvas/Button_StartRecord").GetComponent<Button>();
            _stopRecordButton = transform.Find("Canvas/Button_StopRecord").GetComponent<Button>();
            _speechRecognitionState = transform.Find("Canvas/Image_RecordState").GetComponent<Image>();
            _resultText = transform.Find("Canvas/Text_Result").GetComponent<Text>();
            _commandsInputField = transform.Find("Canvas/InputField_Commands").GetComponent<InputField>();
            _languageDropdown = transform.Find("Canvas/Dropdown_Language").GetComponent<Dropdown>();
            _objectForCommand = transform.Find("Canvas/Panel_PointArena/Image_Point").GetComponent<RectTransform>();
           _microphoneDevicesDropdown = transform.Find("Canvas/Dropdown_MicrophoneDevices").GetComponent<Dropdown>();
            _startRecordButton.onClick.AddListener(StartRecordButtonOnClickHandler);
            _stopRecordButton.onClick.AddListener(StopRecordButtonOnClickHandler);
            _microphoneDevicesDropdown.onValueChanged.AddListener(MicrophoneDevicesDropdownOnValueChangedEventHandler);
            _startRecordButton.interactable = true;
            _stopRecordButton.interactable = false;
            _speechRecognitionState.color = Color.yellow;

            _languageDropdown.ClearOptions();
            _speechRecognition.RequestMicrophonePermission(null);
			   RefreshMicsButtonOnClickHandler();

            // load languages
            for (int i = 0; i < Enum.GetNames(typeof(Enumerators.LanguageCode)).Length; i++)
            {
                _languageDropdown.options.Add(new Dropdown.OptionData(((Enumerators.LanguageCode)i).Parse()));
            }
            _languageDropdown.value = _languageDropdown.options.IndexOf(
                _languageDropdown.options.Find(x => x.text == Enumerators.LanguageCode.vi_VN.Parse())); // đổi sang tiếng Việt

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

        private void StartRecordButtonOnClickHandler()
        {
            _startRecordButton.interactable = false;
            _stopRecordButton.interactable = true;
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

		private void MicrophoneDevicesDropdownOnValueChangedEventHandler(int value)
		{
			if (!_speechRecognition.HasConnectedMicrophoneDevices())
				return;
			_speechRecognition.SetMicrophoneDevice(_speechRecognition.GetMicrophoneDevices()[value]);
		}
        private void StopRecordButtonOnClickHandler()
        {
            _stopRecordButton.interactable = false;
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

            _stopRecordButton.interactable = false;
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
			_commandsInputField.text = language_VN_EN[1];
			_languageDropdown.value = _languageDropdown.options.IndexOf(_languageDropdown.options.Find(x => x.text == Enumerators.LanguageCode.en_GB.Parse()));
		}
		public void On_VN()
		{
		
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
                    cleanTranscript = cleanTranscript.Replace(".", "").Replace(",", "").Replace("!", "");

                    _resultText.text += "\nIncome text: " + cleanTranscript;

                    foreach (var command in commands)
                    {
                        string cleanCommand = command.Trim().ToLowerInvariant();

                        if (cleanTranscript.Contains(cleanCommand))
                        {
                            _resultText.text += "\nDid command: " + command + ";"; 
                            DoCommand(cleanCommand);
                        }
                    }
                }
            }
        }

       private void DoCommand(string command)
{
    float speed = 100f;
    float scaleSpeed = 0.1f;

    // Chuẩn hóa: bỏ khoảng trắng, chuyển về thường, normalize Unicode
    command = command.Trim().ToLowerInvariant();
    command = command.Normalize(NormalizationForm.FormC); 

    Debug.Log("DO COMMAND với text: [" + command + "]");

      Dictionary<string, System.Action> commandMap = new Dictionary<string, System.Action>()
    {
        { "chạy lên", () => _objectForCommand.anchoredPosition += Vector2.up * speed },
        { "move up", () => _objectForCommand.anchoredPosition += Vector2.up * speed },

        { "down", () => _objectForCommand.anchoredPosition += Vector2.down * speed },
        { " left", () => _objectForCommand.anchoredPosition += Vector2.left * speed },
        { "move right", () => _objectForCommand.anchoredPosition += Vector2.right * speed },

        { "scale up", () => _objectForCommand.localScale += Vector3.one * scaleSpeed },
        { "scale down", () => _objectForCommand.localScale -= Vector3.one * scaleSpeed },

        { "rotate left", () => _objectForCommand.Rotate(0, 0, 30) },
        { "rotate right", () => _objectForCommand.Rotate(0, 0, -30) },
    };

    if (commandMap.ContainsKey(command))
    {
        commandMap[command].Invoke();
        Debug.Log("✅ Executed command: " + command);
    }
    else
    {
        Debug.Log("❌ NOT FOUND COMMAND: " + command + " (len=" + command.Length + ")");
    }
}

    }
}
