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
        [TextAreaAttribute(8,100)]
        public string[] language_VN_EN_Say;
        
        [Header("UI References - Kéo thả từ Hierarchy")]
        public Image _speechRecognitionState;
        public Button _startRecordButton;
        public InputField _commandsInputField;
        public Text _resultText;
        public Dropdown _languageDropdown;
        public Dropdown _microphoneDevicesDropdown;
        public Image _voiceLevelImage;
        
        [Header("Other References")]
        public AudioSource audioSource;
        public AudioClip[] Ban_Da_Co;
        public GameObject[] ButtonLanguage;
        public TextDialogue textDialogue;

        private GCSpeechRecognition _speechRecognition;
        public int Index;

        private void Start()
        {
            // Kiểm tra các reference
            if (_commandsInputField == null)
            {
                Debug.LogError("CommandsInputField is not assigned!");
                return;
            }

            if (language_VN_EN != null && language_VN_EN.Length > 1)
            {
                _commandsInputField.text = language_VN_EN[1];
            }

            
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

            // UI setup - Sử dụng trực tiếp các reference đã kéo thả
            if (_startRecordButton != null)
                _startRecordButton.interactable = true;

            if (_speechRecognitionState != null)
                _speechRecognitionState.color = Color.yellow;

            if (_languageDropdown != null)
            {
                _languageDropdown.ClearOptions();
                
                // load languages
                for (int i = 0; i < Enum.GetNames(typeof(Enumerators.LanguageCode)).Length; i++)
                {
                    _languageDropdown.options.Add(new Dropdown.OptionData(((Enumerators.LanguageCode)i).Parse()));
                }
                
                // Đổi sang tiếng Anh Mỹ (en_US)
                _languageDropdown.value = _languageDropdown.options.FindIndex(
                    x => x.text.Contains("en_US") || x.text.Contains("en-US"));
            }

            _speechRecognition.RequestMicrophonePermission(null);
            RefreshMicsButtonOnClickHandler();

            // select first mic
            if (_speechRecognition.HasConnectedMicrophoneDevices())
            {
                _speechRecognition.SetMicrophoneDevice(_speechRecognition.GetMicrophoneDevices()[0]);
            }
        }

        private void OnDestroy()
        {
            if (_speechRecognition != null)
            {
                _speechRecognition.RecognizeSuccessEvent -= RecognizeSuccessEventHandler;
                _speechRecognition.RecognizeFailedEvent -= RecognizeFailedEventHandler;
                _speechRecognition.FinishedRecordEvent -= FinishedRecordEventHandler;
                _speechRecognition.StartedRecordEvent -= StartedRecordEventHandler;
                _speechRecognition.RecordFailedEvent -= RecordFailedEventHandler;
                _speechRecognition.EndTalkigEvent -= EndTalkigEventHandler;
            }
        }

        public void StartRecordButtonOnClickHandler(BaseEventData data)
        {
            StartCoroutine(TemporaryStart());
        }

        IEnumerator TemporaryStart()
        {
            yield return new WaitForSeconds(0.13f);
            if (_startRecordButton != null)
                _startRecordButton.interactable = false;
            
            if (_resultText != null)
                _resultText.text = string.Empty;
                
            _speechRecognition.StartRecord(false);
        }

        private void RefreshMicsButtonOnClickHandler()
		{
			_speechRecognition.RequestMicrophonePermission(null);

            if (_microphoneDevicesDropdown != null)
            {
                _microphoneDevicesDropdown.ClearOptions();
                var mics = _speechRecognition.GetMicrophoneDevices();
                if (mics != null && mics.Length > 0)
                {
                    _microphoneDevicesDropdown.AddOptions(mics.ToList());
                    MicrophoneDevicesDropdownOnValueChangedEventHandler(0);
                }
            }
        }

        private void Update()
        {
            if(_speechRecognition.IsRecording && _voiceLevelImage != null)
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
			else if (_voiceLevelImage != null)
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
            if (_startRecordButton != null)
                _startRecordButton.interactable = true;
            _speechRecognition.StopRecord();
        }

        private void StartedRecordEventHandler()
        {
            if (_speechRecognitionState != null)
                _speechRecognitionState.color = Color.red;
        }

        private void RecordFailedEventHandler()
        {
            if (_speechRecognitionState != null)
                _speechRecognitionState.color = Color.yellow;
            
            if (_resultText != null)
                _resultText.text = "<color=red>Start record Failed. Please check microphone device and try again.</color>";
            
            if (_startRecordButton != null)
                _startRecordButton.interactable = true;
        }

        private void EndTalkigEventHandler(AudioClip clip, float[] raw)
        {
            FinishedRecordEventHandler(clip, raw);
        }

        private void FinishedRecordEventHandler(AudioClip clip, float[] raw)
        {
            if (_startRecordButton != null && _startRecordButton.interactable)
            {
                if (_speechRecognitionState != null)
                    _speechRecognitionState.color = Color.yellow;
            }

            if (clip == null)
                return;

            RecognitionConfig config = RecognitionConfig.GetDefault();
            if (_languageDropdown != null)
                config.languageCode = ((Enumerators.LanguageCode)_languageDropdown.value).Parse();
            else
                config.languageCode = "en-US"; // fallback
            
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
            if (_resultText != null)
                _resultText.text = "Recognize Failed: " + error;
        }

        public void On_EN()
		{
            if (ButtonLanguage != null && ButtonLanguage.Length >= 2)
            {
                ButtonLanguage[0].SetActive(false);
                ButtonLanguage[1].SetActive(true);
            }

            if (_commandsInputField != null && language_VN_EN != null && language_VN_EN.Length > 1)
			    _commandsInputField.text = language_VN_EN[1];
			
            if (_languageDropdown != null)
			    _languageDropdown.value = _languageDropdown.options.FindIndex(
                    x => x.text.Contains("en_US") || x.text.Contains("en-US"));
		}

		public void On_VN()
		{
            if (ButtonLanguage != null && ButtonLanguage.Length >= 2)
            {
                ButtonLanguage[1].SetActive(false);
                ButtonLanguage[0].SetActive(true);
            }

            if (_commandsInputField != null && language_VN_EN != null && language_VN_EN.Length > 0)
			    _commandsInputField.text = language_VN_EN[0];
			
            if (_languageDropdown != null)
			    _languageDropdown.value = _languageDropdown.options.FindIndex(
                    x => x.text.Contains("vi_VN") || x.text.Contains("vi-VN"));
		}

        private void RecognizeSuccessEventHandler(RecognitionResponse recognitionResponse)
        {
            if (_resultText == null || _commandsInputField == null)
            {
                Debug.LogError("UI references are null!");
                return;
            }

            _resultText.text = "Detected: ";

            if (recognitionResponse.results == null)
            {
                _resultText.text += "No results";
                return;
            }

            string[] commands = _commandsInputField.text.Split(',');

            foreach (var result in recognitionResponse.results)
            {
                if (result.alternatives == null) continue;

                foreach (var alternative in result.alternatives)
                {
                    if (string.IsNullOrEmpty(alternative.transcript)) continue;

                    string cleanTranscript = alternative.transcript.Trim().ToLowerInvariant();
                    cleanTranscript = Regex.Replace(cleanTranscript, @"[^\p{L}\p{N}\s]", "");

                    _resultText.text += "\nIncome text: " + cleanTranscript;

                    foreach (var command in commands)
                    {
                        if (string.IsNullOrEmpty(command)) continue;

                        string cleanCommand = command.Trim().ToLowerInvariant();

                        if (cleanTranscript.Contains(cleanCommand))
                        {
                            _resultText.text += "\nDid command: " + command + ","; 
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

    // KIỂM TRA TẤT CẢ CÁC BIẾN QUAN TRỌNG
    if (language_VN_EN == null || language_VN_EN.Length == 0)
    {
        Debug.LogError("language_VN_EN is not set!");
        return;
    }

    if (language_VN_EN_Say == null || language_VN_EN_Say.Length == 0)
    {
        Debug.LogError("language_VN_EN_Say is not set!");
        return;
    }

    if (Index < 0 || Index >= language_VN_EN.Length)
    {
        Debug.LogError($"Index {Index} is out of range for language_VN_EN (length: {language_VN_EN.Length})");
        return;
    }

    // LẤY COMMANDS TỪ language_VN_EN (dùng để so sánh)
    string[] availableCommands = language_VN_EN[Index].Split(',');
    
    // LẤY COMMANDS TỪ language_VN_EN_Say (dùng để thực thi)
    string[] sayCommands = language_VN_EN_Say[Index].Split(',');

    Debug.Log($"Available commands: {string.Join(", ", availableCommands)}");
    Debug.Log($"Say commands: {string.Join(", ", sayCommands)}");

    // TÌM COMMAND KHỚP NHẤT - PHƯƠNG PHÁP ĐƠN GIẢN & HIỆU QUẢ
    int foundIndex = -1;
    string foundCommand = "";

    for (int i = 0; i < availableCommands.Length; i++)
    {
        string cleanCmd = availableCommands[i].Trim().ToLowerInvariant();
        
        // PHƯƠNG PHÁP 1: Contains trực tiếp
        if (command.Contains(cleanCmd) || cleanCmd.Contains(command))
        {
            foundIndex = i;
            foundCommand = cleanCmd;
            Debug.Log($"🎯 Exact match: '{cleanCmd}'");
            break;
        }
        
        // PHƯƠNG PHÁP 2: So sánh từ khóa chính (cho tiếng Anh)
        if (IsEnglishCommandMatch(command, cleanCmd))
        {
            foundIndex = i;
            foundCommand = cleanCmd;
            Debug.Log($"🎯 English keyword match: '{cleanCmd}'");
            break;
        }
    }

    if (foundIndex >= 0)
    {
        Debug.Log($"✅ FOUND COMMAND: '{foundCommand}' at index: {foundIndex}");
        
        // Tìm index tương ứng trong sayCommands
        int sayIndex = FindMatchingSayIndex(foundCommand, foundIndex, sayCommands);
        
        if (sayIndex >= 0)
        {
            ExecuteCommand(sayIndex, foundCommand);
        }
        else
        {
            Debug.LogError($"❌ No matching say command found for: {foundCommand}");
        }
    }
    else
    {
        Debug.Log($"❌ NOT FOUND COMMAND: {command}");
        
        // HIỂN THỊ TẤT CẢ SO SÁNH ĐỂ DEBUG
        Debug.Log("🔍 DEBUG - All comparisons:");
        for (int i = 0; i < availableCommands.Length; i++)
        {
            string cleanCmd = availableCommands[i].Trim().ToLowerInvariant();
            bool contains1 = command.Contains(cleanCmd);
            bool contains2 = cleanCmd.Contains(command);
            bool englishMatch = IsEnglishCommandMatch(command, cleanCmd);
            
            Debug.Log($"Command '{cleanCmd}': contains1={contains1}, contains2={contains2}, englishMatch={englishMatch}");
        }
    }
}

// HÀM SO SÁNH CHO TIẾNG ANH - ĐƠN GIẢN & HIỆU QUẢ
private bool IsEnglishCommandMatch(string input, string command)
{
    // Xử lý các biến thể phổ biến của "what's your name"
    if ((input.Contains("whats your name") || input.Contains("what your name") || input.Contains("whats name")) &&
        (command.Contains("what your name") || command.Contains("whats your name")))
    {
        return true;
    }
    
    // Xử lý "how are you"
    if (input.Contains("how are you") && command.Contains("how are you"))
    {
        return true;
    }
    
    // So sánh theo từ khóa chính
    string[] inputWords = input.Split(' ');
    string[] commandWords = command.Split(' ');
    
    int matchCount = 0;
    foreach (string inputWord in inputWords)
    {
        foreach (string commandWord in commandWords)
        {
            if (inputWord == commandWord && inputWord.Length > 2)
            {
                matchCount++;
                break;
            }
        }
    }
    
    // Nếu có ít nhất 2 từ khớp nhau
    return matchCount >= 2;
}

// HÀM TÌM INDEX TƯƠNG ỨNG TRONG sayCommands
private int FindMatchingSayIndex(string foundCommand, int foundIndex, string[] sayCommands)
{
    // Ưu tiên 1: Tìm command giống hệt trong sayCommands
    for (int i = 0; i < sayCommands.Length; i++)
    {
        if (sayCommands[i].Trim().ToLowerInvariant() == foundCommand)
        {
            return i;
        }
    }
    
    // Ưu tiên 2: Dùng cùng index nếu nằm trong phạm vi
    if (foundIndex < sayCommands.Length)
    {
        return foundIndex;
    }
    
    // Ưu tiên 3: Tìm command có từ khóa tương tự
    string foundCommandLower = foundCommand.ToLowerInvariant();
    for (int i = 0; i < sayCommands.Length; i++)
    {
        string sayCmd = sayCommands[i].Trim().ToLowerInvariant();
        if (foundCommandLower.Contains(sayCmd) || sayCmd.Contains(foundCommandLower))
        {
            return i;
        }
    }
    
    // Ưu tiên 4: Tìm theo từ khóa chính
    if (foundCommandLower.Contains("name"))
    {
        for (int i = 0; i < sayCommands.Length; i++)
        {
            if (sayCommands[i].ToLowerInvariant().Contains("name"))
                return i;
        }
    }
    else if (foundCommandLower.Contains("how"))
    {
        for (int i = 0; i < sayCommands.Length; i++)
        {
            if (sayCommands[i].ToLowerInvariant().Contains("how"))
                return i;
        }
    }
    
    return 0; // Mặc định trả về index 0 nếu không tìm thấy
}

// HÀM THỰC THI COMMAND
private void ExecuteCommand(int sayIndex, string foundCommand)
{
    // XỬ LÝ AUDIO
    if (Ban_Da_Co != null && sayIndex < Ban_Da_Co.Length && Ban_Da_Co[sayIndex] != null)
    {
        audioSource.clip = Ban_Da_Co[sayIndex];
        audioSource.Play();
        Debug.Log($"🎵 Playing audio at sayIndex: {sayIndex}");
    }
    else
    {
        Debug.LogWarning($"⚠️ Audio not available for sayIndex: {sayIndex}");
    }

    // XỬ LÝ TEXT DIALOGUE
    if (textDialogue != null)
    {
        try
        {
            textDialogue.Update_Text(sayIndex);
            Debug.Log($"📝 Updated text dialogue with sayIndex: {sayIndex}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Error in TextDialogue.Update_Text: {e.Message}");
        }
    }
    else
    {
        Debug.LogError("❌ TextDialogue is null!");
    }
}
    }
}