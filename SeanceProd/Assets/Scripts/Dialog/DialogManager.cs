using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using TMPro;

public class DialogManager : MonoBehaviour
{

    //Singleton
    public static DialogManager Instance;

    public bool _isActive;
    public bool _isAudioLinked;

    public float _timeBetweenSentences = 3f;
    private float _timeLastSentences;
    private StreamReader _inputStream;
    private int _nextLineToRead;

    public GameObject _uiCanva;
    public TMP_Text _dialogUI;

    //random reading
    int _lastRandomLineRead;


    void Awake()
    {
        #region Make Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        #endregion
    }

    private void Start()
    {
        _isActive = false;
        _isAudioLinked = false;
        _uiCanva.SetActive(false);
        _nextLineToRead = 0;

        //test

        //float totalTime = CalculateTimeToReadFile(Application.dataPath + "/Data/dialogTest.txt");
        //Debug.Log("total amount of time to read file = " + totalTime);

        //try
        //{
        //    //StartDialogFromFile(Application.dataPath + "/Dialog/dialogTest.txt");
        //}
        //catch (Exception e)
        //{
        //    Debug.LogError("Cand find file at given PATH");
        //    Debug.LogError(e);
        //}
    }

    private void Update()
    { 
        DialogTimer();

        //if (_nextLineToRead != 0 && !_isActive)
        //PlayAtLine(Application.dataPath + "/dialogTest.txt", _nextLineToRead);
    }

	public void PlaySingleLine(string filePath, int line)
	{
        line *= 2;
		filePath = Application.dataPath + "/StreamingAssets/" + filePath;

		if (_isActive)
			return;
		else
		{
			_inputStream = new StreamReader(filePath);
			_uiCanva.SetActive(true);

			//read line until desired one
			for (int i = 0; i <= line; i++)
			{
				_inputStream.ReadLine();
			}
			string inp_ln = _inputStream.ReadLine();

			//check if at end of file
			if (_inputStream.EndOfStream)
				return;

			DisplayDialog(inp_ln);

			_timeLastSentences = Time.time;
			_uiCanva.SetActive(true);
			_isActive = true;

			while (_inputStream.EndOfStream)
			{
				_inputStream.ReadLine();
			}
		}
	}

	private void DialogTimer()
    {
        if (_isActive && !_inputStream.EndOfStream && Time.time > _timeLastSentences + _timeBetweenSentences)
        {
            //display sentence
            string inp_ln = _inputStream.ReadLine();
            if (DisplayDialog(inp_ln))
            {
                //reset timer
                _timeLastSentences = Time.time;
            }
            else
            {
                //pass to next line
            }
        }
        else if (_isActive && _inputStream.EndOfStream && Time.time > _timeLastSentences + _timeBetweenSentences)
        {
            _inputStream.Close();
            _isActive = false;
            _dialogUI.text = "";
            _uiCanva.SetActive(false);
        }
        else if (_inputStream != null && !_isActive && Time.time > _timeLastSentences + _timeBetweenSentences)
        {
            _inputStream.Close();
            _isActive = false;
            _dialogUI.text = "";
            _uiCanva.SetActive(false);
        }
    }

    public void StartDialogFromFile(string filePath)
    {
        if (_isActive)
            return;
        else
        {
            filePath = Application.dataPath + "/StreamingAssets/" + filePath;

            //reset initial var
            _timeBetweenSentences = 3f;

            //activate UI
            _isActive = true;
            _isAudioLinked = false;
            _inputStream = new StreamReader(filePath);
            _uiCanva.SetActive(true);

            //display first line
            _nextLineToRead = 0;
            string inp_ln = _inputStream.ReadLine();
            DisplayDialog(inp_ln);

            _timeLastSentences = Time.time;
        }
    }

    public bool DisplayDialog(string txt)
    {
        _nextLineToRead++;

        if (txt == "" || txt == null)
            return false;
        else if (txt[0].Equals('#'))
        {
            //exception handler
            if (txt.Equals("#time=voice"))
            {
                _isAudioLinked = true;
            }
            else if (txt.Contains("time"))
            {
                string newTimeInChar = txt[txt.Length - 1] + "";
                int newTime = Int16.Parse(newTimeInChar);
                _timeBetweenSentences = newTime;
                _isAudioLinked = false;
            }
            else if (txt.Equals("#break"))
            {
                StopReading();
            }
            return false;
        }
        else
        {
            //display texte on UI
            _dialogUI.text = txt;

            //link to display time to audio
            if(_isAudioLinked && AudioManager.Instance != null && AudioManager.Instance._mjSource.isPlaying)
            {
                _timeBetweenSentences = AudioManager.Instance._mjSource.clip.length + 2f;
            }
            else if (_isAudioLinked && AudioManager.Instance == null)
            {
                Debug.LogError("AudioManager missing is Scene");
            }
            else if(_isAudioLinked && !AudioManager.Instance._mjSource.isPlaying)
            {
                Debug.LogError("Audio clip must start before displaying dialog");
            }

            return true;
        }
    }

    public void StopReading()
    {
        Debug.Log("reader is stopped");
        _inputStream.Close();
        _isActive = false;

        //deactive display UI display
        _dialogUI.text = "";
        _uiCanva.SetActive(false);

    }

    public void PlayAtLine(string filePath, int line)
    {

        filePath = Application.dataPath + "/StreamingAssets/" + filePath;

        if (_isActive)
            return;
        else
        {
            _inputStream = new StreamReader(filePath);
            _uiCanva.SetActive(true);

            //read line until desired one
            for (int i = 0; i < _nextLineToRead; i++)
            {
                _inputStream.ReadLine();
            }
            string inp_ln = _inputStream.ReadLine();

            //check if at end of file
            if (_inputStream.EndOfStream)
                return;

            DisplayDialog(inp_ln);
            Debug.Log("reading continues at line : " + inp_ln);

            _timeLastSentences = Time.time;
            _uiCanva.SetActive(true);
            _isActive = true;
        }
    }

    public float CalculateTimeToReadFile(string filePath)
    {
        if (_isAudioLinked)
            return -1f;

        filePath = Application.dataPath + "/StreamingAssets/" + filePath;

        StreamReader tmpStream = new StreamReader(filePath);
        float totalTime = 0;

        while (!tmpStream.EndOfStream)
        {
            string inp_ln = tmpStream.ReadLine();

            if (inp_ln == "" || inp_ln == null)
            {
                //do nothing
            }
            else if (inp_ln[0].Equals('#'))
            {
                //exception handler
                if (inp_ln.Contains("time"))
                {
                    string newTimeInChar = inp_ln[inp_ln.Length - 1] + "";
                    int newTime = Int16.Parse(newTimeInChar);
                    _timeBetweenSentences = newTime;
                }
                else if (inp_ln.Equals("#break"))
                {
                    //StopReading();

                    ///do something?
                }
            }
            else
            {
                totalTime += _timeBetweenSentences;
            }
        }


        return totalTime;
    }

    public void RandomReatAtLine(string filePath, int[] lines)
    {

        filePath = Application.dataPath + "/StreamingAssets/" + filePath;

        //rdm nb
        int rdmNb = lines[UnityEngine.Random.Range(0, lines.Length)];


        if (_isActive)
            return;
        else
        {
            _inputStream = new StreamReader(filePath);
            _uiCanva.SetActive(true);

            //read line until desired one
            for (int i = 0; i < rdmNb; i++)
            {
                _inputStream.ReadLine();
            }
            string inp_ln = _inputStream.ReadLine();


            DisplayDialog(inp_ln);
            //Debug.Log("reading continues at line : " + inp_ln);

            _timeLastSentences = Time.time;
            _uiCanva.SetActive(true);
            _isActive = true;

            //check if at end of file
            if (_inputStream.EndOfStream)
                return;
            else
            {
                _isActive = false;
            }
        }
    }
}
