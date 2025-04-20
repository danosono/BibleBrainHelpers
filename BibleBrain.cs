using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using Gg;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Debug = UnityEngine.Debug;
using Input = UnityEngine.Input;


public class BibleBrain : MonoBehaviour
{
    [SerializeField] private BibleManager bibleManager;
    [SerializeField] private ButtonInstantiator _buttonInstantiator;
    [SerializeField] private ChaptersCanvas _chaptersCanvas;
    [SerializeField] private BooksCanvas _booksCanvas;
    [SerializeField] private TypingVerses _typingVerses;
    [SerializeField] private VersesTypedMemory versesTypedMemory;
    [SerializeField] private Toggle resumeOnStartToggle;
    [SerializeField] private Toggle verseNumberToggle;
    [SerializeField] private GameObject verseNumberTgl;
    [SerializeField] private Toggle newLineToggle;
    [SerializeField] private GameObject verseNewLineTgl;
    [SerializeField] private Toggle typingToggle;
    [SerializeField] private Toggle maxWpmToggle;
    [SerializeField] private GameObject maxWpmGo;
    [SerializeField] private Toggle aveWpmToggle;
    [SerializeField] private GameObject aveWpmGo;
    [SerializeField] private GameObject wpmSliderParentGo;
    [SerializeField] private GameObject inputfieldVerseMemParentGo;
    [SerializeField] private Toggle wpmToggle;
    [SerializeField] private Toggle inputFieldToggle;
    [SerializeField] private Toggle stenographToggle;
    [SerializeField] private Toggle checkInputPreTextToggle;
    //[SerializeField] private Toggle rememberVersesToggle; //exists on BibleManager
    [SerializeField] private Text versesText;
    [SerializeField] private Text copyrightText;
    [SerializeField] private Text currentPageText;
    [SerializeField] private Text infoText;
    public string infoTextPrefix, infoTextLang, infoTextBible, infoTextBook, infoTextChapter;
    [SerializeField] private Text verseInfoText;
    [SerializeField] private Text currentLanguage;
    [SerializeField] private Text lastCharEntered;
    [SerializeField] private GameObject idleText;
    [SerializeField] public InputField inputFieldSearchLang;
    [SerializeField] private GameObject contextMenu;
    [SerializeField] private GameObject switchTestamentContextMenu;
    [SerializeField] private TextMeshProUGUI switchTestamentMenuText;
    [SerializeField] private GameObject foregroundPanel;
    [SerializeField] private InputFieldContextMenu inputFieldContextMenu;
    [SerializeField] private Toggle dontShowMsgAgain;
    [SerializeField] private TextMeshProUGUI defaultLanguageText;

    private string _textFromInputFieldOnChange;
    private string _textFromInputFieldOnSubmit;

    public GameObject inputFieldTypingGoNewUi;
    [SerializeField] public GameObject inputFieldTypingGo;
    [SerializeField] public InputField inputFieldVerseTyping;
    [SerializeField] public InputFieldHelper inputFieldHelper;
    [SerializeField] private InputField inputField;
    [SerializeField] private Text inputFieldPreText;


    [SerializeField] private GameObject versesTextContent;

    [SerializeField] private GameObject versesScrollViewGameObj;
    [SerializeField] private GameObject verseButtonsScrollViewGameObj;

    [SerializeField] private Text maxWpm;
    [SerializeField] private Text aveWpm;
    [SerializeField] private TextMeshProUGUI wpmTextNewUi;
    [SerializeField] private Text wpmText;
    [SerializeField] private Slider wpmSlider;

    private Transform versesTextObj;
    [SerializeField] private Scrollbar versesButtonsScrlBar;

    private bool _showVerseNumbers;
    private bool _newLineForEachVerse;
    private bool _typing; //was true
    private bool _aveWpm;
    private bool _maxWpm;
    private bool _wpm; //was true
    private bool _nullFilesetsInitialized;

    public static bool CorrectChar;

    private float maxWpmValue;
    private float aveWpmValue;
    private int numberOfWpmToAve;
    private int numberOfCharsTyped;
    private int numOfCharsExceedsBillionMultiplier;
    private int numberOfVersesTyped;
    private int numOfVersesExceedsBillionMultiplier;
    [SerializeField] private Text CharsVersesTypedText;

    public NullFilesets nullFilesets;

    private string sTypingText;  // this is the API key and my way of hiding it from malicious game hackers lol; at least make them work for it
    private string sScore;

    public string _clickedLanguageIso;
    public string prevClickedLangIso;
    public string _clickedLanguageAutonym;
    public string prevClickedLangAutonym;
    public string clickedFileset;
    public string clickedBibleSize;
    public string clickedAbbr;
    public string clickedVolume;
    public string clickedBookID;
    public string clickedBookName;
    public string clickedTestament;
    public int clickedChapter;
    public int clickedBookNumberOfChapters;

    //public int clickedVerse;

    public string stagedBibleId;
    public string stagedBibleSize;

    // *******************************************************************
    // *** Active variables are only set once the Verses are displayed ***
    //********************************************************************

    private string _activeLanguageIso; //BibleLanguages.data.iso                SAVE  
    private string _activeLanguageAutonym; //BibleLanguages.data.autonym        SAVE
    private string _activeFileset; //                                           SAVE
    private string _activeAbbr; //BibleData.data.abbr                           SAVE
    public string activeBibleId; // either FilesetID or Abbr; which ever returns books data SAVE
    public string activeBibleSize;
    private string _activeVolume; // BibleData.data.filesets.prod.volume        SAVE - Long name of Bible 
    public string activeBookID; // BookData.data.book_id                      SAVE
    private string _activeBookName; // BookData.data.name or .name_short        SAVE
    private string _activeTestament;
    public int activeChapter; //                                              SAVE
    private int _activeBookNumberOfChapters; // BookData.data.chapters.length   SAVE                        
    public int activeVerse; // only used for verse typing buttons; not used for chapter at a time viewing

    public bool rememberVerses; // if set negative, does not delete any memory 
    public bool rememberVersesBoolSetInStartMethod;



    private string _defaultLanguageIso;
    private string _defaultLanguageAutonym;
    private bool _resumeOnStart;

    public static bool useInputFieldVerseTypingButtons; //was true, configured
    public static bool stenographInput;   // AKA Match Entire Verse
    public static bool checkInputPreText;
    public static bool UsingOldUi;
    


    private VerseData activeVersesDATA; // used for ConvertVerses() and ReloadTypingVerses() and RecallReadingVerses
    private BookData recallBookData;

    private int _chaptersInActiveBook;


    private void Awake()
    {
        sScore = GetLastLevel();
        sTypingText = GetTypingSpeed();
        LoadActiveData();
    }

    private void OnEnable()
    {
        InputFieldContextMenu.okButtonClicked += ContextMenuOkButtonClicked;
    }

    void Start()
    {
        LoadData();
        versesTextObj = versesTextContent.transform;
        LoadToggles();
        LoadBools();
        //GetOnlyLanguagesWithText();
        _buttonInstantiator.CommonLangButtonClicked();
        LoadInitialContent();
        //UpdateCurrentLanguage(); 

        UpdateVersesCharsTypedText();
        
        //Debug.Log(rememberVerses);

    }
    


    private void OnDisable()
    {
        SteamworksHelper.maxWpm = (int)maxWpmValue;
        SteamworksHelper.aveWpm = (int)aveWpmValue;
        SteamworksHelper.numberOfWpmInAve = numberOfWpmToAve;

        SaveActiveData();

        SaveWpmData();

        InputFieldContextMenu.okButtonClicked -= ContextMenuOkButtonClicked;

        //StopAllCoroutines(); // Coroutines stop themselves when complete
    }

    public bool TypingIsOn()
    {
        return _typing;
    }

    public enum State
    {
        VdAllMatch,
        VdLngBbl,
        VdLngBblVrs,
        VndLngBbl,
        VndLngBblBk,
        VndLngBblChptBk
    }

    public State state;

    public void SetState()
    {
        switch (state)
        {
            case State.VdAllMatch:

                break;
            case State.VdLngBbl:

                break;
            case State.VndLngBbl:

                break;
            case State.VdLngBblVrs:

                break;
            case State.VndLngBblBk:

                break;
            case State.VndLngBblChptBk:

                break;
        }
    }


    [ContextMenu("TestScript")]
    public void TestScript()
    {
        // Debug.Log("-------------------------- activeLanguageIso: " + _activeLanguageIso);
        // Debug.Log("---------------------- activeLanguageAutonym: " + _activeLanguageAutonym);
        // Debug.Log("------------------------------ activeFileset: " + _activeFileset);
        // Debug.Log("--------------------------------- activeAbbr: " + _activeAbbr);
         Debug.Log("------------------------------ activeBibleId: " + activeBibleId);
        // Debug.Log("------------------------------- activeVolume: " + _activeVolume);
        // Debug.Log("------------------------------- activeBookID: " + activeBookID);
        // Debug.Log("----------------------------- activeBookName: " + _activeBookName);
        // Debug.Log("------------------------------ activeChapter: " + activeChapter);
        // Debug.Log("----------------- activeBookNumberOfChapters: " + _activeBookNumberOfChapters);
        // Debug.Log("---------------------------- activeTestament: " + _activeTestament);
        // Debug.Log("---------------------------- stagedBibleSize: " + stagedBibleSize);
        // Debug.Log("---------------------------- stagedBibleId: " + stagedBibleId);
        // Debug.Log("-----------------------------------------------------------------------_clickedLanguageIso: " +
        //           _clickedLanguageIso);
        // Debug.Log("------------------------------------------------------------------ _clickedLanguageAutonym: " +
        //           _clickedLanguageAutonym);
        // Debug.Log("--------------------------------------------------------------------------- clickedFileset: " +
        //           clickedFileset);
        // Debug.Log("------------------------------------------------------------------------- clickedBibleSize: " +
        //           clickedBibleSize);
        // Debug.Log("------------------------------------------------------------------------------ clickedAbbr: " +
        //           clickedAbbr);
        // Debug.Log("---------------------------------------------------------------------------- clickedVolume: " +
        //           clickedVolume);
        // Debug.Log("---------------------------------------------------------------------------- clickedBookID: " +
        //           clickedBookID);
        // Debug.Log("-------------------------------------------------------------------------- clickedBookName: " +
        //           clickedBookName);
        // Debug.Log("------------------------------------------------------------------------- clickedTestament: " +
        //           clickedTestament);
        // Debug.Log("--------------------------------------------------------------------------- clickedChapter: " +
        //           clickedChapter);
        // Debug.Log("-------------------------------------------------------------- clickedBookNumberOfChapters: " +
        //           clickedBookNumberOfChapters);
    }

    
    
    public void TestingProblems(string message)
    {
        Debug.Log($"ACTIVE - LangIso: {_activeLanguageIso}, LangAutonym: {_activeLanguageAutonym}, Testament: {_activeTestament}, " +
                  $"Bible size: {activeBibleSize},  fileset: {_activeFileset}, " +
                  $"ABBR: {_activeAbbr}, BibleID: {activeBibleId}, Volume: {_activeVolume}, " +
                  $"Book ID: {activeBookID}, Booke Name: {_activeBookName}" +
                  $"Chapter: {activeChapter}, #of Chapters: {_activeBookNumberOfChapters}");
        
        Debug.Log($"STAGED - Bible size: {stagedBibleSize}, Bible ID: {stagedBibleId}");
        
        Debug.Log($"CLICKED - LangIso: {_clickedLanguageIso}, LangAutonym: {_clickedLanguageAutonym}, Testament: {clickedTestament}, " +
                  $"Bible size: {clickedBibleSize},  fileset: {clickedFileset}, " +
                  $"ABBR: {clickedAbbr}, BibleID: {activeBibleId}, Volume: {clickedVolume}, " +
                  $"Book ID: {clickedBookID}, Booke Name: {clickedBookName}" +
                  $"Chapter: {clickedChapter}, #of Chapters: {clickedBookNumberOfChapters}");
        
    }

    private void LoadData()
    {
        var settings = new ES3Settings(ES3.EncryptionType.AES, sScore);
        if (ES3.KeyExists("maxWpmValue"))
        {
            maxWpmValue = ES3.Load<float>("maxWpmValue", settings);
            maxWpm.text = maxWpmValue.ToString("0");
        }

        if (ES3.KeyExists("aveWpmValue"))
        {
            aveWpmValue = ES3.Load<float>("aveWpmValue", settings);
            aveWpm.text = aveWpmValue.ToString("0");
        }

        if (ES3.KeyExists("numberOfWpmToAve"))
            numberOfWpmToAve = ES3.Load<int>("numberOfWpmToAve", settings);
        if (ES3.KeyExists("numberOfCharsTyped"))
            numberOfCharsTyped = ES3.Load<int>("numberOfCharsTyped", settings);
        if (ES3.KeyExists("numOfCharsExceedsMaxInt"))
            numOfCharsExceedsBillionMultiplier = ES3.Load<int>("numOfCharsExceedsMaxIntMultiplier", settings);
        if (ES3.KeyExists("numberOfVersesTyped"))
            numberOfVersesTyped = ES3.Load<int>("numberOfVersesTyped", settings);
        if (ES3.KeyExists("numOfVersesExceedsMaxInt"))
            numOfVersesExceedsBillionMultiplier = ES3.Load<int>("numOfVersesExceedsMaxIntMultiplier", settings);
        if (ES3.KeyExists("_nullFilesetsInitialized"))
            _nullFilesetsInitialized = ES3.Load<bool>("_nullFilesetsInitialized");

        if (ES3.KeyExists("nullFilesets"))
            nullFilesets = JsonConvert.DeserializeObject<NullFilesets>(ES3.Load<string>("nullFilesets"));

        if (!_nullFilesetsInitialized)
        {
            InitializeNullFilesets();

        }
    }

    private bool resumedOnStart;

    private void LoadInitialContent()
    {
        if (!_resumeOnStart)
        {
            if (_defaultLanguageIso != null && _defaultLanguageAutonym != null)
            {
                if (_defaultLanguageIso != "none" && _defaultLanguageAutonym != "none")
                {
                    GetTextBibles(_defaultLanguageIso, _defaultLanguageAutonym);
                }
                else
                {
                    GetTextBibles("eng", "English");
                }
            }
            else
            {
                
                
                // *** Romans 12 NLT ***
                _activeLanguageIso = "eng";
                _activeLanguageAutonym = "English";

                _activeFileset = "ENGNLTN_ET";
                clickedFileset = "ENGNLTN_ET";
                _activeAbbr = "ENGNLT";
                activeBibleSize = "NT";
                _activeVolume = "New Living Translation\u00ae, Holy Sanctuary version";

                activeBibleId = "ENGNLT";
                activeBookID = "JHN";
                _activeBookName = "John";
                activeChapter = 1;

                clickedVolume = "New Living Translation\u00ae, Holy Sanctuary version";
                clickedAbbr = "ENGNLT"; 
                clickedVolume = "New Living Translation\u00ae, Holy Sanctuary version";
                clickedBookName = "John";
                clickedTestament = "NT";
                _clickedLanguageIso = "eng";
                _clickedLanguageAutonym = "English";
                clickedBookNumberOfChapters = 21;
                
                stagedBibleSize = "NT";
                stagedBibleId = "ENGNLT";  // fileset ID or abbr ; whichever returns a result
                
                
               StartCoroutine(GetTextBiblesAndCallForButtonsNoSavedData(_activeLanguageIso, _activeLanguageAutonym));
               StartCoroutine(GetBooksAndCallForButtonsNoSavedData(_activeFileset, _activeAbbr, activeBibleSize, _activeVolume));
               StartCoroutine(RequestBibleVersesNoSavedData(_activeFileset, activeBookID, activeChapter));
               
               for (int i = 1; i <= 16; i++)
               {
                   _buttonInstantiator.InstantiateChapterButtons(i, activeBookID, _activeBookName);
               }

               infoTextLang = "English";
               infoTextBible = "New Living Translation\u00ae, Holy Sanctuary version";
               infoTextBook = "John";
               infoTextChapter = "1";
               
               SetInfoText();
            }

        }
        else
        {
            resumedOnStart = true;
            StartCoroutine(GetLastBiblesAndCallForButtons(_activeLanguageIso));
            StartCoroutine(GetLastBooksAndCallForButtons(activeBibleId)); //_activeFileset, _activeAbbr
            for (int i = 1; i < _activeBookNumberOfChapters + 1; i++)
            {
                _buttonInstantiator.InstantiateChapterButtons(i, activeBookID, _activeBookName);
            }


            _clickedLanguageIso = _activeLanguageIso;
            _clickedLanguageAutonym = _activeLanguageAutonym;
            clickedFileset = _activeFileset;
            clickedBibleSize = activeBibleSize;
            clickedAbbr = _activeAbbr;
            clickedVolume = _activeVolume;
            clickedBookID = activeBibleId;
            clickedBookName = _activeBookName;
            clickedTestament = _activeTestament;
            clickedChapter = activeChapter;
            clickedBookNumberOfChapters = activeChapter;


            stagedBibleSize = activeBibleSize;
            stagedBibleId = activeBibleId;

            //Debug.Log("_activeFileset = " + _activeFileset);

            StartCoroutine(RequestBibleVerses(_activeFileset, activeBookID, activeChapter));
            //StartCoroutine(RequestCopyright(_activeFileset));

        }

        if (_defaultLanguageIso != null && _defaultLanguageAutonym != null)
        {
            defaultLanguageText.text = $"Default: {_defaultLanguageIso}";
        }

    }

    public void LoadActiveData()
    {
        if (ES3.KeyExists("activeLanguageIso"))
            _activeLanguageIso = ES3.Load<string>("activeLanguageIso");
        if (ES3.KeyExists("activeLanguageAutonym"))
            _activeLanguageAutonym = ES3.Load<string>("activeLanguageAutonym");
        if (ES3.KeyExists("activeAbbr"))
            _activeAbbr = ES3.Load<string>("activeAbbr");
        if (ES3.KeyExists("activeFileset"))
            _activeFileset = ES3.Load<string>("activeFileset");
        if (ES3.KeyExists("activeBibleSize"))
            activeBibleSize = ES3.Load<string>("activeBibleSize");
        if (ES3.KeyExists("activeVolume"))
            _activeVolume = ES3.Load<string>("activeVolume");
        if (ES3.KeyExists("activeBookID"))
            activeBookID = ES3.Load<string>("activeBookID");
        if (ES3.KeyExists("activeBookName"))
            _activeBookName = ES3.Load<string>("activeBookName");
        if (ES3.KeyExists("activeBookNumberOfChapters"))
            _activeBookNumberOfChapters = ES3.Load<int>("activeBookNumberOfChapters");
        if (ES3.KeyExists("activeChapter"))
            activeChapter = ES3.Load<int>("activeChapter");
        if (ES3.KeyExists("defaultLanguageIso"))
            _defaultLanguageIso = ES3.Load<string>("defaultLanguageIso");
        if (ES3.KeyExists("defaultLanguageAutonym"))
            _defaultLanguageAutonym = ES3.Load<string>("defaultLanguageAutonym");
        if (ES3.KeyExists("activeVerse"))
            activeVerse = ES3.Load<int>("activeVerse");
        if (ES3.KeyExists("activeBibleId"))
            activeBibleId = ES3.Load<string>("activeBibleId");
        if (ES3.KeyExists("activeTestament"))
            _activeTestament = ES3.Load<string>("activeTestament");
    }

    public void SaveActiveData()
    {
        ES3.Save("activeLanguageIso", _activeLanguageIso);
        ES3.Save("activeLanguageAutonym", _activeLanguageAutonym);
        ES3.Save("activeAbbr", _activeAbbr);
        ES3.Save("activeFileset", _activeFileset);
        ES3.Save("activeBibleSize", activeBibleSize);
        ES3.Save("activeBibleId", activeBibleId);
        ES3.Save("activeVolume", _activeVolume);
        ES3.Save("activeBookID", activeBookID);
        ES3.Save("activeBookName", _activeBookName);
        ES3.Save("activeBookNumberOfChapters", _activeBookNumberOfChapters);
        ES3.Save("activeChapter", activeChapter);
        ES3.Save("activeVerse", activeVerse);
        ES3.Save("activeTestament", _activeTestament);
    }

    public void SetActiveDataAfterVerseDisplayed(
        string newFileset,
        string newBibleSize,
        string newAbbr,
        string stagedBibleId,
        string newVolume,
        string newBookID,
        string newBookName,
        string newTestament,
        int newChapter,
        int newBookNumberOfChapters)
    {
        _activeFileset = newFileset;
        activeBibleSize = newBibleSize;
        _activeAbbr = newAbbr;
        activeBibleId = stagedBibleId;
        _activeVolume = newVolume;
        activeBookID = newBookID;
        _activeBookName = newBookName;
        _activeTestament = newTestament;
        activeChapter = newChapter;
        _activeBookNumberOfChapters = newBookNumberOfChapters;
    }

    public void SetActiveLangAfterVerseDisplayed(string newLanguageIso, string newLanguageAutonym)
    {
        _activeLanguageIso = newLanguageIso;
        _activeLanguageAutonym = newLanguageAutonym;
    }

public void ClearActiveData()
    {
        _activeLanguageIso = "";
        _activeLanguageAutonym = "";
        _activeFileset = "";
        activeBibleSize = "";
        _activeAbbr = "";
        activeBibleId = "";
        _activeVolume = "";
        activeBookID = "";
        _activeBookName = "";
        _activeTestament = "";
        activeChapter = 1;
        _activeBookNumberOfChapters = 1;
        activeVerse = 1;
    }





    public void UpdateLastChar(char character)
    {
        lastCharEntered.text = character.ToString();
    }

    public void ClearCurrentChar()
    {
        lastCharEntered.text = "";
    }





    public void SetDefaultLanguage()
    {
        if (_activeLanguageIso != null && _activeLanguageAutonym != null)
        {
            _defaultLanguageIso = _activeLanguageIso;
            _defaultLanguageAutonym = _activeLanguageAutonym;
            defaultLanguageText.text = $"Default: {_defaultLanguageIso}";
            ES3.Save("_defaultLanguageIso", _defaultLanguageIso);
            ES3.Save("_defaultLanguageAutonym", _defaultLanguageAutonym);
            InformationMessageTemporary($"Default language set to {_defaultLanguageIso} / {_defaultLanguageAutonym}",
                7f, Color.yellow);
        }
        else
        {
            InformationMessageTemporary("No current language; please select one.", 7f, Color.yellow);
        }

    }

    public void DefaultLanguageButtonClicked()
    {
        if (_defaultLanguageIso != null && _defaultLanguageAutonym != null)
        {
            if (_defaultLanguageIso != "none" && _defaultLanguageAutonym != "none")
            {
                GetTextBibles(_defaultLanguageIso, _defaultLanguageAutonym);
            }
            else
            {
                InformationMessageTemporary($"Please set a default language.",
                    6f, Color.yellow);
            }
        }
    }

    public void ClearDefaultLanguage()
    {
        _defaultLanguageIso = "none";
        _defaultLanguageAutonym = "none";
        InformationMessageTemporary("Default language has been cleared.", 7f, Color.yellow);
    }

    private void InitializeNullFilesets()
    {
        nullFilesets.null_filesets_list.Add("Initialized");
        _nullFilesetsInitialized = true;
        ES3.Save<string>("nullFilesets", JsonConvert.SerializeObject(nullFilesets));
        ES3.Save("_nullFilesetsInitialized", _nullFilesetsInitialized);
    }

    public void ShowNullFilesets()
    {
        foreach (var entry in nullFilesets.null_filesets_list)
        {
            Debug.Log(entry);
        }
    }


    public void ClearNullBibleList()
    {
        ES3.DeleteKey("nullFilesets");
        nullFilesets.null_filesets_list.Clear();
        nullFilesets.null_filesets_list.Add("InitializedAfterReset");
        ES3.Save<string>("nullFilesets", JsonConvert.SerializeObject(nullFilesets));
        InformationMessageTemporary("Null filesets have been reset; all bibles will be attempted", 8f, Color.yellow);
    }

    public void SetCorrectCharTrue()
    {
        if (!CorrectChar)
        {
            CorrectChar = true;
        }
    }

    public void SetCorrectCharFalse()
    {
        if (CorrectChar)
        {
            CorrectChar = false;
        }
    }

    public void ToggleVerseNumber()
    {

        if (verseNumberToggle.isOn)
        {
            _showVerseNumbers = true;
            ES3.Save("_showVerseNumbers", true);
        }
        else
        {
            _showVerseNumbers = false;
            ES3.Save("_showVerseNumbers", false);
        }
    }

    public void ToggleNewLine()
    {
        if (newLineToggle.isOn)
        {
            _newLineForEachVerse = true;
            ES3.Save("_newLineForEachVerse", true);
        }
        else
        {
            _newLineForEachVerse = false;
            ES3.Save("_newLineForEachVerse", false);
        }
    }

    public void SetResumeOnStartBool()
    {
        if (resumeOnStartToggle.isOn)
        {
            _resumeOnStart = true;
            ES3.Save("_resumeOnStart", true);
        }
        else
        {
            _resumeOnStart = false;
            ES3.Save("_resumeOnStart", false);
        }
    }

    private string GetAChapter()
    {
        return aChapter;
    }

    public void ToggleTyping()
    {
        if (typingToggle.isOn)
        {
            _typing = true;
            ES3.Save("_typing", true);

            wpmSliderParentGo.SetActive(true);
            maxWpmGo.SetActive(true);
            aveWpmGo.SetActive(true);
            inputfieldVerseMemParentGo.SetActive(true);

            verseNewLineTgl.SetActive(false);
            verseNumberTgl.SetActive(false);
        }
        else
        {
            _typing = false;
            ES3.Save("_typing", false);

            verseNewLineTgl.SetActive(true);
            verseNumberTgl.SetActive(true);

            wpmSliderParentGo.SetActive(false);
            maxWpmGo.SetActive(false);
            aveWpmGo.SetActive(false);
            inputfieldVerseMemParentGo.SetActive(false);
        }
    }

    public void ToggleVerseMemory()
    {

    }

    private string low = "jibberish";

    public void ToggleAveWpm()
    {
        if (aveWpmToggle.isOn)
        {
            _aveWpm = true;
            ES3.Save("_aveWpm", true);
        }
        else
        {
            _aveWpm = false;
            ES3.Save("_aveWpm", false);
        }
    }

    public void ToggleMaxWpm()
    {
        if (maxWpmToggle.isOn)
        {
            _maxWpm = true;
            ES3.Save("_maxWpm", true);
        }
        else
        {
            _maxWpm = false;
            ES3.Save("_maxWpm", false);
        }
    }

    public void ToggleWpm()
    {
        if (wpmToggle.isOn)
        {
            _wpm = true;
            ES3.Save("_wpm", true);
        }
        else
        {
            _wpm = false;
            ES3.Save("_wpm", false);
        }
    }

    public void ClearInputFieldPreTextText()
    {
        if (inputFieldVerseTyping != null)
            inputFieldVerseTyping.text = "";
        if (inputFieldPreText != null)
            inputFieldPreText.text = "";
    }

    public void ClearInputFieldForPreTextGameplay()
    {
        if (inputFieldVerseTyping != null)
            inputFieldVerseTyping.text = "";
        if (inputFieldPreText != null)
            inputFieldPreText.text = "";
    }

    private enum Context
    {
        InputFieldOn,
        InputFieldOff,
        StenographOn,
        StenographOff,
        DetectPreTextOn,
        DetectPreTextOff,
        NoContext
    }

    private Context _context;

    private bool _dontShowContextMenuForInputFieldOn;
    private bool _dontShowContextMenuForStenographModeOn;
    private bool _dontShowContextMenuForDetectPreTextOn;

    private bool _dontShowContextMenuForInputFieldOff;
    private bool _dontShowContextMenuForStenographModeOff;
    private bool _dontShowContextMenuForDetectPreTextOff;

    // the below bools do not need to be persisted between gameplay sessions
    private bool _inputFieldBoolSetInStartMethod;
    private bool _stenographModeBoolSetInStartMethod;
    private bool _detectPreTextBoolSetInStartMethod;

    private bool _inputFieldToggleSetByCancelButton;
    private bool _stenographToggleSetByCancelButton;
    private bool _preTextToggleSetByCancelButton;

    public void ToggleInputField()
    {
        if (_inputFieldToggleSetByCancelButton)
        {
            _inputFieldToggleSetByCancelButton = false;
            return;
        }

        if (inputFieldToggle.isOn)
        {
            if (!_inputFieldBoolSetInStartMethod)
            {
                if (!_dontShowContextMenuForInputFieldOff)
                {
                    foregroundPanel.SetActive(true);
                    _context = Context.InputFieldOn;

                    if (_typing)
                    {
                        OpenContextMenu("An input field will be placed below the active verse.");
                    }
                    else
                    {
                        OpenContextMenu(
                            "You are in reading mode; if you choose typing mode, an input field will be placed below the active verse.");
                    }

                }
                else
                {
                    useInputFieldVerseTypingButtons = true;
                    AddInputFieldButton(activeVerse - 1);
                    ES3.Save("useInputFieldVerseTypingButtons", useInputFieldVerseTypingButtons);
                }
            }
            else
            {
                _inputFieldBoolSetInStartMethod = false;
            }
        }
        else
        {
            if (!_inputFieldBoolSetInStartMethod)
            {
                if (_dontShowContextMenuForInputFieldOff)
                {
                    foregroundPanel.SetActive(true);
                    _context = Context.InputFieldOff;
                    if (_typing)
                    {
                        OpenContextMenu("No input field will be added below the active verse; just start typing.");
                    }
                    else
                    {
                        OpenContextMenu("No input field will be used for typing mode; type without the input field.");
                    }
                }
                else
                {
                    useInputFieldVerseTypingButtons = false;
                    _buttonInstantiator.ParkInputField();
                    ES3.Save("useInputFieldVerseTypingButtons", useInputFieldVerseTypingButtons);
                }
            }
            else
            {
                _inputFieldBoolSetInStartMethod = false;
            }

        }


    }

    public void OpenContextMenu(string msg)
    {
        contextMenu.SetActive(true);
        if (UsingOldUi)
        {
            inputFieldContextMenu.message.text = msg;
        }
        else
        {
            inputFieldContextMenu.msgTmp.text = msg;
        }
    }

    public void ContextMenuCancelButtonClicked()
    {
        switch (_context)
        {
            case Context.NoContext:
                break;
            case Context.InputFieldOn:
                foregroundPanel.SetActive(false);
                _inputFieldToggleSetByCancelButton = true;
                inputFieldToggle.isOn = false;
                _context = Context.NoContext;
                break;
            case Context.InputFieldOff:
                foregroundPanel.SetActive(false);
                _inputFieldToggleSetByCancelButton = true;
                inputFieldToggle.isOn = true;
                _context = Context.NoContext;
                break;
            case Context.StenographOn:
                foregroundPanel.SetActive(false);
                _stenographToggleSetByCancelButton = true;
                stenographToggle.isOn = false;
                _context = Context.NoContext;
                break;
            case Context.StenographOff:
                foregroundPanel.SetActive(false);
                _stenographToggleSetByCancelButton = true;
                stenographToggle.isOn = true;
                _context = Context.NoContext;
                break;
            case Context.DetectPreTextOn:
                foregroundPanel.SetActive(false);
                _preTextToggleSetByCancelButton = true;
                checkInputPreTextToggle.isOn = false;
                _context = Context.NoContext;
                break;
            case Context.DetectPreTextOff:
                foregroundPanel.SetActive(false);
                _preTextToggleSetByCancelButton = true;
                checkInputPreTextToggle.isOn = true;
                _context = Context.NoContext;
                break;
        }

        contextMenu.SetActive(false);
    }

    public void ContextMenuOkButtonClicked(bool dontShow)
    {
        switch (_context)
        {
            case Context.NoContext:
                break;
            case Context.InputFieldOn:
                useInputFieldVerseTypingButtons = true;
                ES3.Save("useInputFieldVerseTypingButtons", useInputFieldVerseTypingButtons);
                AddInputFieldButton(activeVerse - 1);
                if (dontShow)
                {
                    _dontShowContextMenuForInputFieldOn = dontShow;
                    ES3.Save("_dontShowContextMenuForInputFieldOn", _dontShowContextMenuForInputFieldOn);
                }

                foregroundPanel.SetActive(false);
                _context = Context.NoContext;
                break;
            case Context.InputFieldOff:
                useInputFieldVerseTypingButtons = false;
                ES3.Save("useInputFieldVerseTypingButtons", useInputFieldVerseTypingButtons);
                _buttonInstantiator.ParkInputField();
                if (dontShow)
                {
                    _dontShowContextMenuForInputFieldOff = dontShow;
                    ES3.Save("_dontShowContextMenuForInputFieldOff", _dontShowContextMenuForInputFieldOff);
                }

                foregroundPanel.SetActive(false);
                _context = Context.NoContext;
                break;
            case Context.StenographOn:
                stenographInput = true;
                ES3.Save("stenographInput", stenographInput);
                if (dontShow)
                {
                    _dontShowContextMenuForStenographModeOn = dontShow;
                    ES3.Save("_dontShowContextMenuForStenographModeOn", _dontShowContextMenuForStenographModeOn);
                }

                foregroundPanel.SetActive(false);
                _context = Context.NoContext;
                break;
            case Context.StenographOff:
                stenographInput = false;
                ES3.Save("stenographInput", stenographInput);
                if (dontShow)
                {
                    _dontShowContextMenuForStenographModeOff = dontShow;
                    ES3.Save("_dontShowContextMenuForStenographModeOff", _dontShowContextMenuForStenographModeOff);
                }

                foregroundPanel.SetActive(false);
                _context = Context.NoContext;
                break;
            case Context.DetectPreTextOn:
                checkInputPreText = true;
                ES3.Save("checkInputPreText", checkInputPreText);
                if (dontShow)
                {
                    _dontShowContextMenuForDetectPreTextOn = dontShow;
                    ES3.Save("_dontShowContextMenuForDetectPreTextOn", _dontShowContextMenuForDetectPreTextOn);
                }

                foregroundPanel.SetActive(false);
                _context = Context.NoContext;
                break;
            case Context.DetectPreTextOff:
                checkInputPreText = false;
                ES3.Save("checkInputPreText", checkInputPreText);
                if (dontShow)
                {
                    _dontShowContextMenuForDetectPreTextOff = dontShow;
                    ES3.Save("_dontShowContextMenuForDetectPreTextOff", _dontShowContextMenuForDetectPreTextOff);
                }

                foregroundPanel.SetActive(false);
                _context = Context.NoContext;
                break;
        }
    }

    public void ToggleStenographicGameplay()
    {
        if (_stenographToggleSetByCancelButton)
        {
            _stenographToggleSetByCancelButton = false;
            return;
        }

        if (stenographToggle.isOn)
        {
            if (!_stenographModeBoolSetInStartMethod)
            {
                if (!_dontShowContextMenuForStenographModeOn)
                {
                    foregroundPanel.SetActive(true);
                    _context = Context.StenographOn;

                    if (_typing)
                    {
                        OpenContextMenu(
                            "Next time you click a Chapter button, Word2 will check for the entire verse typed correctly instead of checking each character as you type.");
                    }
                    else
                    {
                        OpenContextMenu(
                            "You are in reading mode; in typing mode, Word2 will check for the entire verse typed correctly instead of checking each character as you type.");
                    }

                    // if on and player types but input field not focused, pop up , input field is not in focus inputfield.isfocused 
                }
                else
                {
                    stenographInput = true;
                    ES3.Save("stenographInput", true);
                }
            }
            else
            {
                _stenographModeBoolSetInStartMethod = false;
            }
        }
        else
        {
            if (!_stenographModeBoolSetInStartMethod)
            {
                if (!_dontShowContextMenuForStenographModeOff)
                {
                    foregroundPanel.SetActive(true);
                    _context = Context.StenographOff;
                    if (_typing)
                    {
                        OpenContextMenu(
                            "Next time you click a chapter button, Word 2 will check each character as you type.");
                    }
                    else
                    {
                        OpenContextMenu(
                            "You are in reading mode; in typing mode, Word 2 will check each character as you type.");
                    }
                }
                else
                {
                    stenographInput = false;
                    ES3.Save("stenographInput", false);
                }
            }
            else
            {
                _stenographModeBoolSetInStartMethod = false;
            }

        }


    }

    public void ToggleCheckInputPreTextGameplay()
    {
        if (_preTextToggleSetByCancelButton)
        {
            _preTextToggleSetByCancelButton = false;
            return;
        }

        if (checkInputPreTextToggle.isOn)
        {
            if (!_detectPreTextBoolSetInStartMethod)
            {
                if (!_dontShowContextMenuForDetectPreTextOn)
                {
                    foregroundPanel.SetActive(true);
                    _context = Context.DetectPreTextOn;

                    if (_typing)
                    {
                        OpenContextMenu(
                            "Word2 will try to recognize characters for languages that combine characters, such as Korean.");
                    }
                    else
                    {
                        OpenContextMenu(
                            "In typing mode, Word2 will try to recognize characters for languages that combine characters such, as Korean.");
                    }

                }
                else
                {
                    checkInputPreText = true;
                    ES3.Save("checkInputPreText", true);
                }
            }
            else
            {
                _detectPreTextBoolSetInStartMethod = false;
            }
        }
        else
        {
            if (!_detectPreTextBoolSetInStartMethod)
            {
                if (!_dontShowContextMenuForDetectPreTextOff)
                {
                    foregroundPanel.SetActive(true);
                    _context = Context.DetectPreTextOff;
                    if (_typing)
                    {
                        OpenContextMenu(
                            "Detect characters the regular way; this causes issues with languages that combine characters such as Korean.");
                    }
                    else
                    {
                        OpenContextMenu(
                            "In typing mode, detect characters the regular way; this causes issues with languages that combine characters such as Korean.");
                    }
                }
                else
                {
                    checkInputPreText = false;
                    ES3.Save("checkInputPreText", false);
                }
            }
            else
            {
                _detectPreTextBoolSetInStartMethod = false;
            }

        }

        _detectPreTextBoolSetInStartMethod = false;
    }

    public void LoadToggles()
    {
        
        if (ES3.KeyExists("_typing"))
        {
            _typing = ES3.Load<bool>("_typing");
            if (typingToggle.isOn != _typing)
                typingToggle.isOn = _typing;
        }
        else
        {
            // default is false so set it true for new players
            typingToggle.isOn = true;
            // since toggle is off, changing the value will call its onChange method automatically
        }
            

        if (ES3.KeyExists("_showVerseNumbers"))
            _showVerseNumbers = ES3.Load<bool>("_showVerseNumbers");
        verseNumberToggle.isOn = _showVerseNumbers;

        if (ES3.KeyExists("_newLineForEachVerse"))
            _newLineForEachVerse = ES3.Load<bool>("_newLineForEachVerse");
        newLineToggle.isOn = _newLineForEachVerse;

        // if (_typing)
        // {
        //     inputFieldToggle.interactable = true;
        //     if (useInputFieldVerseTypingButtons)
        //     {
        //         checkInputPreTextToggle.interactable = true;
        //         stenographToggle.interactable = true;
        //     }
        //
        //     newLineToggle.interactable = false;
        //     verseNumberToggle.interactable = false;
        // }
        // else
        // {
        //     newLineToggle.interactable = true;
        //     verseNumberToggle.interactable = true;
        //     
        //     inputFieldToggle.interactable = false;
        //     checkInputPreTextToggle.interactable = false;
        //     stenographToggle.interactable = false;
        // }

        if (ES3.KeyExists("_rememberVerses"))
        {
            rememberVerses = ES3.Load<bool>("_rememberVerses");

            if (bibleManager.GetVerseMemoryToggleIsOn() != rememberVerses)
            {
                rememberVersesBoolSetInStartMethod = true;
                bibleManager.SetVerseMemoryToggle(rememberVerses);
            }
            else
            {
                //bibleManager.VerseMemoryToggle doesn't need to change
            }
            
        }
        else
        {
            // either first time player has played or player has never set 1) Default Lang. or 2) Resume on Start
            rememberVersesBoolSetInStartMethod = true;
            bibleManager.SetVerseMemoryToggle(true);
            // bibleManager.ToggleVerseMemory(); // Changing the toggle calls this method 
        }

        if (ES3.KeyExists("_aveWpm"))
        {
            _aveWpm = ES3.Load<bool>("_aveWpm");
            aveWpmToggle.isOn = _aveWpm;
        }
        else
        {
            aveWpmToggle.isOn = true;
        }

        if (ES3.KeyExists("_maxWpm"))
        {
            _maxWpm = ES3.Load<bool>("_maxWpm");
            maxWpmToggle.isOn = _maxWpm;
        }
        else
        {
            maxWpmToggle.isOn = true;
        }

        if (ES3.KeyExists("_wpm"))
        {
            _wpm = ES3.Load<bool>("_wpm");
            wpmToggle.isOn = _wpm;
        }
        else
        {
            wpmToggle.isOn = true;
        }
            

        if (ES3.KeyExists("_resumeOnStart"))
            _resumeOnStart = ES3.Load<bool>("_resumeOnStart");
        resumeOnStartToggle.isOn = _resumeOnStart;

        if (ES3.KeyExists("useInputFieldVerseTypingButtons"))
        {
            useInputFieldVerseTypingButtons = ES3.Load<bool>("useInputFieldVerseTypingButtons");
            // stenographToggle.interactable = useInputFieldVerseTypingButtons;
            // checkInputPreTextToggle.interactable = useInputFieldVerseTypingButtons;
            if (inputFieldToggle.isOn != useInputFieldVerseTypingButtons)
            {
                _inputFieldBoolSetInStartMethod = useInputFieldVerseTypingButtons;
                inputFieldToggle.isOn = useInputFieldVerseTypingButtons;
            }
        }
        else
        {
            useInputFieldVerseTypingButtons = true;
            
            if (inputFieldToggle.isOn != useInputFieldVerseTypingButtons)
            {
                _inputFieldBoolSetInStartMethod = true;
                inputFieldToggle.isOn = useInputFieldVerseTypingButtons;
                
            }
        }

        if (ES3.KeyExists("stenographInput"))
        {
            stenographInput = ES3.Load<bool>("stenographInput");
            if (stenographToggle.isOn != stenographInput)
            {
                _stenographModeBoolSetInStartMethod = true;
                stenographToggle.isOn = stenographInput;
            }
        }

        if (ES3.KeyExists("checkInputPreText"))
        {
            checkInputPreText = ES3.Load<bool>("checkInputPreText");
            if (checkInputPreTextToggle.isOn != checkInputPreText)
            {
                _detectPreTextBoolSetInStartMethod = true;
                checkInputPreTextToggle.isOn = checkInputPreText;
            }
        }
    }

    public void LoadBools()
    {
        if (ES3.KeyExists("_dontShowContextMenuForInputFieldOn"))
            _dontShowContextMenuForInputFieldOn = ES3.Load<bool>("_dontShowContextMenuForInputFieldOn");
        if (ES3.KeyExists("_dontShowContextMenuForStenographModeOn"))
            _dontShowContextMenuForStenographModeOn = ES3.Load<bool>("_dontShowContextMenuForStenographModeOn");
        if (ES3.KeyExists("_dontShowContextMenuForDetectPreTextOn"))
            _dontShowContextMenuForDetectPreTextOn = ES3.Load<bool>("_dontShowContextMenuForDetectPreTextOn");
        if (ES3.KeyExists("_dontShowContextMenuForInputFieldOff"))
            _dontShowContextMenuForInputFieldOff = ES3.Load<bool>("_dontShowContextMenuForInputFieldOff");
        if (ES3.KeyExists("_dontShowContextMenuForStenographModeOff"))
            _dontShowContextMenuForStenographModeOff = ES3.Load<bool>("_dontShowContextMenuForStenographModeOff");
        if (ES3.KeyExists("_dontShowContextMenuForDetectPreTextOff"))
            _dontShowContextMenuForDetectPreTextOff = ES3.Load<bool>("_dontShowContextMenuForDetectPreTextOff");
    }

    private string aBook = "lp";


    public void SetActiveTestament(string testament)
    {
        _activeTestament = testament;
    }

    public void SetActiveVolume(string volume) // Bible name
    {
        _activeVolume = volume;
    }



    public void SetActiveBookName(string bookName, string bookID, int chapters) // Check before using
    {
        _activeBookName = bookName;
        infoTextBook = bookName;
        activeBookID = bookID;
        _activeBookNumberOfChapters = chapters;
    }

    public void SetClickedBookName(string bookName, string bookID, int chapters, string testament)
    {
        clickedBookName = bookName;
        clickedBookID = bookID;
        clickedBookNumberOfChapters = chapters;
        clickedTestament = testament;
    }

    public void GetAllLanguages()
    {
        StartCoroutine(GetBibleLanguagesAndCallForButtons("https://4.dbt.io/api/languages?v=4&key="));
    }

    //[ContextMenu("TestBibleLangData")]
    public void TestBibleLanguagesData()
    {
        StartCoroutine(TestBibleLanguages("https://4.dbt.io/api/languages?v=4&key="));
    }

    private string aChapter = "kerfi";

    public void GetLangForSearchField()
    {
        var searchText = inputFieldSearchLang.text;
        infoText.text = "Searching: " + searchText;
        StartCoroutine(SearchBibleLanguagesAndCallForButtons(
            $"https://4.dbt.io/api/languages/search/{searchText}?v=4&key=", searchText, true));
    }

    public void GetOnlyLanguagesWithText()
    {
        StartCoroutine(
            GetBibleLanguagesAndCallForButtons("https://4.dbt.io/api/languages?v=4&set_type_code=text_plain&key="));
    }


    public void GetEnglishLanguages()
    {
        StartCoroutine(
            GetBibleLanguagesAndCallForButtons("https://4.dbt.io/api/languages/search/english?v=4&page=1&key="));
    }

    public void GetPreviousLanguages(string link) // NOT USED CURRENTLY
    {
        StartCoroutine(GetBibleLanguagesAndCallForButtons(link + "&v=4&set_type_code=text_plain&key="));
    }

    public void GetPageOfLanguages(string link, bool generatePages)
    {
        StartCoroutine(GetBibleLanguagesAndCallForButtons(link, generatePages));
    }

    public void GetSearchPageOfLanguages(string link, string searchText, bool generatePages)
    {
        StartCoroutine(SearchBibleLanguagesAndCallForButtons(link, searchText, generatePages));
    }

    private int aVerse = jibberish;

    //[ContextMenu("TestBibleData")]
    public void TestBibleInfo()
    {
        StartCoroutine(TestBibleData());
    }

    public void GetNewBiblesButLeaveExistingVerses()
    {

    }

    public void GetTextBibles(string iso, string autonym)
    {

        StartCoroutine(GetTextBiblesAndCallForButtons(iso, autonym));
    }

    public void GetAllBibles(string iso, string autonym)
    {
        //StartCoroutine(GetAllBiblesAndCallForButtons(iso, autonym));
    }



    public void GetCopyrightInfo(string fileset)
    {
        StartCoroutine(RequestCopyright(fileset));
    }

    public void GetBooks(string fileset, string abbr, string size, string volume)
    {
        verseInfoText.text = "";
        StartCoroutine(GetBooksAndCallForButtons(fileset, abbr, size, volume));
    }

    private bool callingApocryphaFromOT;
    private bool callingAppocryphaFromComplete;
    private bool langHasChanged;
    private bool bibleEmpty;

    public void SetLangHasChanged(bool changed)
    {
        langHasChanged = changed;
    }

    public void SetBibleEmpty(bool empty)
    {
        bibleEmpty = empty;
    }
    
    
    

public void ReplaceBooks(string fileset, string abbr, string sizeOfNewBible, string volume)
    {

        callingApocryphaFromOT = false; 
        callingAppocryphaFromComplete = false;
        
            
        if (fileset == _activeFileset || abbr == _activeAbbr) 
        {
            if (sizeOfNewBible == activeBibleSize)
                InformationMessageTemporary("You clicked the active Bible", 5f, Color.yellow);
            else
            {
                foregroundPanel.SetActive(true);
                switchTestamentContextMenu.SetActive(true);
                switchTestamentMenuText.text = $"{activeBibleSize} is currently active. Switch to {sizeOfNewBible} volume of this Bible?";
            }
        }
        else if (sizeOfNewBible == "NT")
        {
            if (_activeTestament == "OT" || _activeTestament == "AP")
            {
                //InformationMessageTemporary("New Testament Bible; to view, Clear Chapters and Books.", 5f, Color.yellow);
                
                // DO CONTEXT MENU
                
                switchTestamentFileset = fileset;
                switchTestamentAbbr = abbr;
                switchTestamentVolume = volume;
                switchTestamentSize = sizeOfNewBible;
                
                foregroundPanel.SetActive(true);
                switchTestamentContextMenu.SetActive(true);
                switchTestamentMenuText.text = $"Old Testament is currently active. Switch to {sizeOfNewBible}?";
                
                SetBibleEmpty(true);
            }
            else
            {
                
                StartCoroutine(TryReplaceBibleForCurrentBook(fileset, abbr, sizeOfNewBible, volume));
            }
        }
        else if (sizeOfNewBible == "OT")
        {
            if (_activeTestament == "NT")
            {
                //InformationMessageTemporary("Old Testament Bible; to view, Clear Chapters and Books.", 5f, Color.yellow);
                
                // DO CONTEXT MENU

                switchTestamentFileset = fileset;
                switchTestamentAbbr = abbr;
                switchTestamentVolume = volume;
                switchTestamentSize = sizeOfNewBible;
                
                foregroundPanel.SetActive(true);
                switchTestamentContextMenu.SetActive(true);
                switchTestamentMenuText.text = $"New Testament is currently active. Switch to {sizeOfNewBible}?";
                
                SetBibleEmpty(true);
            }

            else if (_activeTestament == "AP")
            {
                callingApocryphaFromOT = true;
                StartCoroutine(TryReplaceBibleForCurrentBook(fileset, abbr, sizeOfNewBible, volume));
            }
            else
            {
                StartCoroutine(TryReplaceBibleForCurrentBook(fileset, abbr, sizeOfNewBible, volume));
            }
        }
        else if (sizeOfNewBible == "C")
        {
            if (_activeTestament == "AP")
            {
                callingAppocryphaFromComplete = true;
                StartCoroutine(TryReplaceBibleForCurrentBook(fileset, abbr, sizeOfNewBible, volume));
            }
            else
            {
                StartCoroutine(TryReplaceBibleForCurrentBook(fileset, abbr, sizeOfNewBible, volume));
            }
        }
        else
        {
            StartCoroutine(TryReplaceBibleForCurrentBook(fileset, abbr, sizeOfNewBible, volume));
        }
        
    }

    private string switchTestamentFileset;
    private string switchTestamentAbbr;
    private string switchTestamentVolume;
    private string switchTestamentSize;

    public void SwitchTestamentOkButtonClicked()
    {
        // Clear books, chapters, verses
        ClearBooksChaptersVerses();
        
        // switch to sizeOfNewBible; GetBooks for the testament/bible clicked
        clickedFileset = switchTestamentFileset;
        clickedAbbr = switchTestamentAbbr;
        clickedVolume = switchTestamentVolume;
        clickedBibleSize = switchTestamentSize;
                
        GetBooks(clickedFileset, clickedAbbr, clickedBibleSize, clickedVolume);
        
        infoTextPrefix = "";
        infoTextLang = _activeLanguageAutonym;
        infoTextBible = clickedVolume;
        infoTextBook = "";
        infoTextChapter = "";
        
        SetInfoText();
    }

    public void HideSwitchTestamentContextMenu()
    {
        if (switchTestamentContextMenu.activeSelf)
            switchTestamentContextMenu.SetActive(false);
    }

    public void ShowVerses(string bookId, int chapter)
    {
        //TestingLanguageProblems("BEFORE RequestBibleVerses()");
        
        StartCoroutine(RequestBibleVerses(clickedFileset, clickedBookID, clickedChapter));
        //TestingLanguageProblems("AFTER RequestBibleVerses()");
    }
    
    public void ReplaceVerses(string fileset, int chapter, 
        string infoTxtBible, string infoTxtBook, string vol, string replaceAbbr, string recallSize)
    {
        //OK to use activeBookID here
        StartCoroutine(TryReplaceBibleVerses(fileset, activeBookID, chapter,  
            infoTxtBible,  infoTxtBook,  vol, replaceAbbr, recallSize));
        
        // Debug.Log("                ReplaceVerses:: " + "ActiveFileset: " + _activeFileset + ", _activeAbbr: " + _activeAbbr 
        //           + ", activeBibleId: " + activeBibleId + ", _activeVolume: " + _activeVolume + ", activeBookID: " + activeBookID
        //           + ", activeChapter: " + activeChapter);

    }
    

    public void SetCurrentPageText(int page)
    {
        currentPageText.text = $"Page {page}";
    }

    private string theEndBook = "jibberish";

    public void ClearLanguageButtons()
    {
        _buttonInstantiator.ClearLangButtons();
    }

    public void ClearLanguagePageButtons()
    {
        _buttonInstantiator.ClearLangPageButtons();
    }

    public void ClearBibleButtons()
    {
        _buttonInstantiator.ClearBibleButtons();
    }

    private string GetLowValue()
    {
        return GetABook() + GetAChapter();
    }


    public void ClearBookButtons()
    {
        _buttonInstantiator.ClearBookButtons();
    }

    public void SetInfoText()
    {
        restoreInfoTextAfterTempMsg = $"{infoTextPrefix} {infoTextLang} {infoTextBible} {infoTextBook} {infoTextChapter}";
        infoText.text = $"{infoTextPrefix} {infoTextLang} {infoTextBible} {infoTextBook} {infoTextChapter}";
    }

    public void ClearInfoText()
    {
        infoText.text = "";
    }

    public void AddToInfoText(string stringToAdd)
    {
        infoText.text += stringToAdd;
    }

    public void SetVerseInfoText(string stringToAdd)
    {
        verseInfoText.text = stringToAdd;
    }

    public void AddInputFieldButton(int index)
    {
        _buttonInstantiator.MoveInputFieldBelowActiveVerse(index);
    }

    public void ActivateInputField()
    {
        _buttonInstantiator.SetInputFieldActive();
    }

    public void InformationMessage(string message, float duration, Color color)
    {
        StartCoroutine(InfoMsg(message, duration, color));
    }

    private bool isInfoMsgTempRunning;
    private string restoreInfoTextAfterTempMsg;
    private Color restorInfoTextColorAfterTempMsg;

    private string tempInfoMsgMessage;
    private float tempInfoMsgDuration;
    private Color tempInfoMsgColor;
    
    public void InformationMessageTemporary(string message, float duration, Color color)
    {
        if (!isInfoMsgTempRunning)
        {
            restoreInfoTextAfterTempMsg = infoText.text;
            restorInfoTextColorAfterTempMsg = Color.white;
            //restorInfoTextColorAfterTempMsg = infoText.color;

            tempInfoMsgMessage = message;
            tempInfoMsgDuration = duration;
            tempInfoMsgColor = color;

            StartCoroutine(InfoMsgTemp());
            //Debug.Log("Coroutine Started: " + message);
        }
        else
        {
            StopCoroutine(InfoMsgTemp());
            //Debug.Log("Coroutine STOPPED");

            tempInfoMsgMessage = message;
            tempInfoMsgDuration = duration;
            tempInfoMsgColor = color;
            
            StartCoroutine(InfoMsgTemp());
            //Debug.Log("Coroutine ReStarted: " + message);
        }
    }

    

    public IEnumerator InfoMsgTemp()
    {
        isInfoMsgTempRunning = true;
        infoText.text = tempInfoMsgMessage;
        infoText.color = tempInfoMsgColor;
        yield return new WaitForSeconds(tempInfoMsgDuration);
        infoText.text = restoreInfoTextAfterTempMsg;
        infoText.color = restorInfoTextColorAfterTempMsg;
        isInfoMsgTempRunning = false;
    }
    
    public IEnumerator InfoMsg(string msg, float dur, Color color)
    {
        infoText.text = msg;
        infoText.color = color;
        yield return new WaitForSeconds(dur);
        infoText.text = "";
        infoText.color = Color.white;
    }

    public void ShowIdleText()
    {
        if (!idleText.activeSelf)
            idleText.SetActive(true);
    }

    public void AdvanceToNextTypingVerse()
    {
        UpdateVersesCharsTypedText();
        _typingVerses.AdvanceSelectedButton();
    }

    public void HideIdleText()
    {
        if (idleText.activeSelf)
            idleText.SetActive(false);
    }

    public void ChapterAutoScroll(int chapter)
    {
        _chaptersCanvas.AutoScroll(chapter);
    }

    public void BooksAutoScroll(GameObject button)
    {
        _booksCanvas.AutoScroll(button);
    }

    private IEnumerator TestBibleLanguages(string link, bool generatePages = true)
    {
        var getRequest = CreateRequest(link + sTypingText);

        yield return getRequest.SendWebRequest();

        var languagesData = JsonConvert.DeserializeObject<BibleLanguages>(getRequest.downloadHandler.text,
            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

        if (getRequest.isDone)
        {



            int count = languagesData.meta.pagination.per_page;
            int currentPage = languagesData.meta.pagination.current_page;
            int totalPages = languagesData.meta.pagination.total_pages;

            if (languagesData.data != null)
            {

                for (int i = 0; i < languagesData.data.Count; i++)
                {
                    var data = languagesData.data;


                    Debug.Log(data[i].id);
                    if (data[i].glotto_id != null)
                        Debug.Log(data[i].glotto_id);
                    if (data[i].iso != null)
                        Debug.Log(data[i].iso);
                    if (data[i].name != null)
                        Debug.Log(data[i].name);
                    if (data[i].autonym != null)
                        Debug.Log(data[i].autonym);
                    Debug.Log(data[i].bibles);
                    Debug.Log(data[i].filesets);
                    Debug.Log(data[i].rolv_code);
                }

                var meta = languagesData.meta;

                Debug.Log(meta.pagination.total);
                Debug.Log(meta.pagination.count);
                Debug.Log(meta.pagination.per_page);
                Debug.Log(meta.pagination.current_page);
                Debug.Log(meta.pagination.total_pages);
            }

        }

    }


    private IEnumerator GetBibleLanguagesAndCallForButtons(string link, bool generatePages = true)
    {
        _buttonInstantiator.ClearLangButtons();
        var getRequest = CreateRequest(link + sTypingText);
        yield return getRequest.SendWebRequest();

        var languagesData = JsonConvert.DeserializeObject<BibleLanguages>(getRequest.downloadHandler.text,
            new JsonSerializerSettings
                { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore });
        if (getRequest.isDone)
        {

            if (languagesData.data != null)
            {

                var meta = languagesData.meta;


                for (int i = 0; i < languagesData.data.Count; i++)
                {
                    var data = languagesData.data;
                    _buttonInstantiator.InstantiateLanguageButtons(data[i].name, data[i].autonym, data[i].iso);
                }


                if (generatePages)
                {

                    ClearLanguagePageButtons();
                    for (int i = 1; i < meta.pagination.total_pages + 1; i++)
                    {
                        _buttonInstantiator.InstantiateLangPageButtons(i,
                            $"https://4.dbt.io/api/languages?page={i}&v=4&set_type_code=text_plain&key=");

                    }

                }

            }
            else
            {
                _buttonInstantiator.InstantiateLanguageButtons("no results", "no results", "no results");
            }

        }

    }

    private IEnumerator SearchBibleLanguagesAndCallForButtons(string link, string searchText,
        bool generatePages = false)
    {
        _buttonInstantiator.ClearLangButtons();
        var getRequest = CreateRequest(link + sTypingText);

        yield return getRequest.SendWebRequest();

        var languagesData = JsonConvert.DeserializeObject<BibleLanguages>(getRequest.downloadHandler.text,
            new JsonSerializerSettings
                { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore });

        if (getRequest.isDone)
        {

            if (languagesData.data != null)
            {

                var meta = languagesData.meta;


                for (int i = 0; i < languagesData.data.Count; i++)
                {
                    var data = languagesData.data;
                    _buttonInstantiator.InstantiateLanguageButtons(data[i].name, data[i].autonym, data[i].iso);
                }



                if (generatePages)
                {
                    if (meta.pagination.total_pages > 1)
                    {
                        ClearLanguagePageButtons();

                        for (int i = 1; i < meta.pagination.total_pages + 1; i++)
                        {
                            _buttonInstantiator.InstantiateSearchLangPageButtons(i,
                                $"https://4.dbt.io/api/languages/search/{searchText}?page={i}&v=4&key=",
                                searchText);
                        }
                    }
                    else
                    {
                        ClearLanguagePageButtons();
                    }
                }



            }
            else
            {
                _buttonInstantiator.InstantiateLanguageButtons("no results", "no results", "no results");
            }

        }

    }
    
    private IEnumerator SearchLangByCountryPlusBtns(string country, string searchText)
    {
        _buttonInstantiator.ClearLangButtons();
        var getRequest = CreateRequest($"https://4.dbt.io/api/countries/{country}?v=4&key=" + sTypingText);

        yield return getRequest.SendWebRequest();

        var languagesData = JsonConvert.DeserializeObject<BibleLanguages>(getRequest.downloadHandler.text,
            new JsonSerializerSettings
                { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore });

        if (getRequest.isDone)
        {

            if (languagesData.data != null)
            {

                var meta = languagesData.meta;


                for (int i = 0; i < languagesData.data.Count; i++)
                {
                    var data = languagesData.data;
                    _buttonInstantiator.InstantiateLanguageButtons(data[i].name, data[i].autonym, data[i].iso);
                }



            }
            else
            {
                _buttonInstantiator.InstantiateLanguageButtons("no results", "no results", "no results");
            }

        }

    }


    private IEnumerator TestBibleData()
    {
        string iso = "eng";
        var getRequest = CreateRequest("https://4.dbt.io/api/bibles?language_code=" + iso +
                                       "&page=1&v=4&set_type_code=text_plain&key=" + sTypingText);

        yield return getRequest.SendWebRequest();


        var bibleData = JsonConvert.DeserializeObject<BibleData>(getRequest.downloadHandler.text);

        if (getRequest.isDone)
        {
            for (int i = 0; i < bibleData.data.Count; i++)
            {
                Debug.Log("BIBLE DATA: [" + i + "]");

                if (bibleData.data[i].abbr != null)
                    Debug.Log(bibleData.data[i].abbr);
                if (bibleData.data[i].name != null)
                    Debug.Log(bibleData.data[i].name);
                if (bibleData.data[i].vname != null)
                    Debug.Log(bibleData.data[i].vname);
                if (bibleData.data[i].language != null)
                    Debug.Log(bibleData.data[i].language);
                if (bibleData.data[i].autonym != null)
                    Debug.Log(bibleData.data[i].autonym);
                Debug.Log(bibleData.data[i].language_id);
                if (bibleData.data[i].language_rolv_code != null)
                    Debug.Log(bibleData.data[i].language_rolv_code);
                if (bibleData.data[i].iso != null)
                    Debug.Log(bibleData.data[i].iso);
                if (bibleData.data[i].date != null)
                    Debug.Log(bibleData.data[i].date);


                if (bibleData.data[i].filesets.prod != null)
                {
                    for (int k = 0; k < bibleData.data[i].filesets.prod.Count; k++)
                    {

                        if (bibleData.data[i].filesets.prod != null)
                        {
                            Debug.Log("BIBLE DATA: [" + i + "] " + "data.prod : [" + k + "]");

                            if (bibleData.data[i].filesets.prod[k].id != null)
                                Debug.Log("prod.id: " + bibleData.data[i].filesets.prod[k].id);
                            if (bibleData.data[i].filesets.prod[k].type != null)
                                Debug.Log("prod.id: " + bibleData.data[i].filesets.prod[k].type);
                            if (bibleData.data[i].filesets.prod[k].size != null)
                                Debug.Log("prod.id: " + bibleData.data[i].filesets.prod[k].size);
                            if (bibleData.data[i].filesets.prod[k].stock_no != null)
                                Debug.Log("prod.id: " + bibleData.data[i].filesets.prod[k].stock_no);
                            if (bibleData.data[i].filesets.prod[k].bitrate != null)
                                Debug.Log("prod.id: " + bibleData.data[i].filesets.prod[k].bitrate);
                            if (bibleData.data[i].filesets.prod[k].codec != null)
                                Debug.Log("prod.id: " + bibleData.data[i].filesets.prod[k].codec);
                            if (bibleData.data[i].filesets.prod[k].container != null)
                                Debug.Log("prod.id: " + bibleData.data[i].filesets.prod[k].container);
                            if (bibleData.data[i].filesets.prod[k].timing_est_err != null)
                                Debug.Log("prod.id: " + bibleData.data[i].filesets.prod[k].timing_est_err);
                            if (bibleData.data[i].filesets.prod[k].volume != null)
                                Debug.Log("prod.id: " + bibleData.data[i].filesets.prod[k].volume);
                        }

                    }

                    if (bibleData.data[i].filesets.vid != null)
                    {
                        for (int k = 0; k < bibleData.data[i].filesets.vid.Count; k++)
                        {

                            if (bibleData.data[i].filesets.vid != null)
                            {
                                Debug.Log("BIBLE DATA: [" + i + "] " + "data.vid : [" + k + "]");

                                if (bibleData.data[i].filesets.vid[k].id != null)
                                    Debug.Log("vid.id: " + bibleData.data[i].filesets.vid[k].id);
                                if (bibleData.data[i].filesets.vid[k].type != null)
                                    Debug.Log("vid.id: " + bibleData.data[i].filesets.vid[k].type);
                                if (bibleData.data[i].filesets.vid[k].size != null)
                                    Debug.Log("vid.id: " + bibleData.data[i].filesets.vid[k].size);
                                if (bibleData.data[i].filesets.vid[k].stock_no != null)
                                    Debug.Log("vid.id: " + bibleData.data[i].filesets.vid[k].stock_no);
                                if (bibleData.data[i].filesets.vid[k].volume != null)
                                    Debug.Log("vid.id: " + bibleData.data[i].filesets.vid[k].volume);
                                if (bibleData.data[i].filesets.vid[k].youtube_playlist_id_JHN != null)
                                    Debug.Log("vid.id: " + bibleData.data[i].filesets.vid[k].youtube_playlist_id_JHN);
                                if (bibleData.data[i].filesets.vid[k].youtube_playlist_id_LUK != null)
                                    Debug.Log("vid.id: " + bibleData.data[i].filesets.vid[k].youtube_playlist_id_LUK);
                                if (bibleData.data[i].filesets.vid[k].youtube_playlist_id_MAT != null)
                                    Debug.Log("vid.id: " + bibleData.data[i].filesets.vid[k].youtube_playlist_id_MAT);
                                if (bibleData.data[i].filesets.vid[k].youtube_playlist_id_MRK != null)
                                    Debug.Log("vid.id: " + bibleData.data[i].filesets.vid[k].youtube_playlist_id_MRK);
                            }

                        }
                    }
                }

            }

            Debug.Log(bibleData.meta.pagination.total);
            Debug.Log(bibleData.meta.pagination.per_page);
            Debug.Log(bibleData.meta.pagination.current_page);
            Debug.Log(bibleData.meta.pagination.last_page);
            if (bibleData.meta.pagination.next_page_url != null)
                Debug.Log(bibleData.meta.pagination.next_page_url);
            if (bibleData.meta.pagination.prev_page_url != null)
                Debug.Log(bibleData.meta.pagination.prev_page_url);
            Debug.Log(bibleData.meta.pagination.from);
            Debug.Log(bibleData.meta.pagination.to);
        }


    }

    private string GetHighValue()
    {
        return GetNewLow() + GetAVerse();
    }
    

    private IEnumerator GetTextBiblesAndCallForButtons(string iso, string autonym)
    {
        
        var getRequest = CreateRequest("https://4.dbt.io/api/bibles?language_code=" + iso +
                                       "&page=1&v=4&set_type_code=text_plain&key=" + sTypingText);
       
        yield return getRequest.SendWebRequest();


        var bibleData = JsonConvert.DeserializeObject<BibleData>(getRequest.downloadHandler.text,
            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });


        if (getRequest.isDone)
        {
            if (bibleData.data != null)
            {
                prevClickedLangIso = _clickedLanguageIso;
                _clickedLanguageIso = iso;
                prevClickedLangAutonym = _clickedLanguageAutonym;
                _clickedLanguageAutonym = autonym;
                SetClickedLanguageText(autonym);

                //_activeLanguageIso = iso;
                //_activeLanguageAutonym = autonym;
            
                if (VersesAreDisplayed())
                {
                    SetLangHasChanged(true);
                    //Debug.Log("New Feature Called: GetBibles");
                    _buttonInstantiator.ClearBibleButtons();
                    InformationMessageTemporary("Choose a Bible to update verses", 5f, Color.yellow);
                }
                else
                {
                    _buttonInstantiator.ClearBibleButtons();
                    _buttonInstantiator.ClearBookButtons();
                    ClearChapterButtons();
                    ClearVersesText();
                    ClearVerseBtns();
                    ClearCopyrightText();
                }


                
                int bibleButtons = 0;

                for (int i = 0; i < bibleData.data.Count; i++)
                {
                    string vName = bibleData.data[i].vname;
                    string name = bibleData.data[i].name;
                    string abbr = bibleData.data[i].abbr;

                    if (bibleData.data[i].filesets.prod != null)
                    {

                        for (int j = 0; j < bibleData.data[i].filesets.prod.Count; j++)
                        {
                            var prod = bibleData.data[i].filesets.prod[j];
                            string volume = prod.volume;

                            if (prod != null)
                            {
                                if (prod.type == "text_plain")
                                {
                                    if (nullFilesets != null)
                                    {
                                        if (!nullFilesets.null_filesets_list.Contains(prod.id))
                                        {
                                            if (vName != null && vName != "")
                                            {
                                                _buttonInstantiator.InstantiateBibleButtons(vName, volume, prod.size,
                                                    prod.id, abbr);
                                                bibleButtons += 1;
                                            }
                                            else if (name != null && name != "")
                                            {
                                                _buttonInstantiator.InstantiateBibleButtons(name, volume, prod.size,
                                                    prod.id, abbr);
                                                bibleButtons += 1;
                                            }

                                        }
                                    }
                                    else
                                    {
                                        if (vName != null && vName != "")
                                        {
                                            _buttonInstantiator.InstantiateBibleButtons(vName, volume, prod.size, prod.id,
                                                abbr);
                                            bibleButtons += 1;
                                        }
                                        else if (name != null && name != "")
                                        {
                                            _buttonInstantiator.InstantiateBibleButtons(name, volume, prod.size, prod.id,
                                                abbr);
                                            bibleButtons += 1;
                                        }

                                    }
                                }
                            }
                        }
                    }
                }

                if (bibleButtons < 1)
                {
                    InformationMessageTemporary(
                        "Sorry, no bibles with text content currently available here for this language. Please check back later",
                        10f, Color.yellow);
                }
            }
        }
    }
    
   

    private IEnumerator GetLastBiblesAndCallForButtons(string iso)
    {

        var getRequest = CreateRequest("https://4.dbt.io/api/bibles?language_code=" + iso +
                                       "&page=1&v=4&set_type_code=text_plain&key=" + sTypingText);

        yield return getRequest.SendWebRequest();


        var bibleData = JsonConvert.DeserializeObject<BibleData>(getRequest.downloadHandler.text,
            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });


        if (getRequest.isDone)
        {
            int bibleButtons = 0;

            for (int i = 0; i < bibleData.data.Count; i++)
            {
                string vName = bibleData.data[i].vname;
                string name = bibleData.data[i].name;
                string abbr = bibleData.data[i].abbr;

                if (bibleData.data[i].filesets.prod != null)
                {

                    for (int j = 0; j < bibleData.data[i].filesets.prod.Count; j++)
                    {
                        var prod = bibleData.data[i].filesets.prod[j];
                        string volume = prod.volume;

                        if (prod != null)
                        {
                            if (prod.type == "text_plain")
                            {
                                if (nullFilesets != null)
                                {
                                    if (!nullFilesets.null_filesets_list.Contains(prod.id))
                                    {
                                        if (vName != null && vName != "")
                                        {
                                            _buttonInstantiator.InstantiateBibleButtons(vName, volume, prod.size,
                                                prod.id, abbr);
                                            bibleButtons += 1;
                                        }
                                        else if (name != null && name != "")
                                        {
                                            _buttonInstantiator.InstantiateBibleButtons(name, volume, prod.size,
                                                prod.id, abbr);
                                            bibleButtons += 1;
                                        }

                                    }
                                }
                                else
                                {
                                    if (vName != null && vName != "")
                                    {
                                        _buttonInstantiator.InstantiateBibleButtons(vName, volume, prod.size, prod.id,
                                            abbr);
                                        bibleButtons += 1;
                                    }
                                    else if (name != null && name != "")
                                    {
                                        _buttonInstantiator.InstantiateBibleButtons(name, volume, prod.size, prod.id,
                                            abbr);
                                        bibleButtons += 1;
                                    }

                                }
                            }
                        }
                    }
                }
            }

            if (bibleButtons < 1)
            {
                InformationMessageTemporary(
                    "Sorry, no bibles with text content currently available here for this language. Please check back later",
                    10f, Color.yellow);
            }


        }
    }

    
    private IEnumerator GetTextBiblesAndCallForButtonsNoSavedData(string iso, string autonym)
    {
        
        var getRequest = CreateRequest("https://4.dbt.io/api/bibles?language_code=" + iso +
                                       "&page=1&v=4&set_type_code=text_plain&key=" + sTypingText);
       
        yield return getRequest.SendWebRequest();


        var bibleData = JsonConvert.DeserializeObject<BibleData>(getRequest.downloadHandler.text,
            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });


        if (getRequest.isDone)
        {
            if (bibleData.data != null)
            {
                //prevClickedLangIso = _clickedLanguageIso;
                _clickedLanguageIso = iso;
                //prevClickedLangAutonym = _clickedLanguageAutonym;
                _clickedLanguageAutonym = autonym;
                //SetClickedLanguageText(autonym);

                //_activeLanguageIso = iso;
                //_activeLanguageAutonym = autonym;
            
                /*if (VersesAreDisplayed())
                {
                    SetLangHasChanged(true);
                    //Debug.Log("New Feature Called: GetBibles");
                    _buttonInstantiator.ClearBibleButtons();
                    InformationMessageTemporary("Choose a Bible to update verses", 5f, Color.yellow);
                }
                else
                {
                    _buttonInstantiator.ClearBibleButtons();
                    _buttonInstantiator.ClearBookButtons();
                    ClearChapterButtons();
                    ClearVersesText();
                    ClearVerseBtns();
                    ClearCopyrightText();
                }*/


                
                int bibleButtons = 0;

                for (int i = 0; i < bibleData.data.Count; i++)
                {
                    string vName = bibleData.data[i].vname;
                    string name = bibleData.data[i].name;
                    string abbr = bibleData.data[i].abbr;

                    if (bibleData.data[i].filesets.prod != null)
                    {

                        for (int j = 0; j < bibleData.data[i].filesets.prod.Count; j++)
                        {
                            var prod = bibleData.data[i].filesets.prod[j];
                            string volume = prod.volume;

                            if (prod != null)
                            {
                                if (prod.type == "text_plain")
                                {
                                    if (nullFilesets != null)
                                    {
                                        if (!nullFilesets.null_filesets_list.Contains(prod.id))
                                        {
                                            if (vName != null && vName != "")
                                            {
                                                _buttonInstantiator.InstantiateBibleButtons(vName, volume, prod.size,
                                                    prod.id, abbr);
                                                bibleButtons += 1;
                                            }
                                            else if (name != null && name != "")
                                            {
                                                _buttonInstantiator.InstantiateBibleButtons(name, volume, prod.size,
                                                    prod.id, abbr);
                                                bibleButtons += 1;
                                            }

                                        }
                                    }
                                    else
                                    {
                                        if (vName != null && vName != "")
                                        {
                                            _buttonInstantiator.InstantiateBibleButtons(vName, volume, prod.size, prod.id,
                                                abbr);
                                            bibleButtons += 1;
                                        }
                                        else if (name != null && name != "")
                                        {
                                            _buttonInstantiator.InstantiateBibleButtons(name, volume, prod.size, prod.id,
                                                abbr);
                                            bibleButtons += 1;
                                        }

                                    }
                                }
                            }
                        }
                    }
                }

                if (bibleButtons < 1)
                {
                    InformationMessageTemporary(
                        "Sorry, no bibles with text content currently available here for this language. Please check back later",
                        10f, Color.yellow);
                }
            }
        }
    }

    private IEnumerator GetAllBiblesAndCallForButtons(string iso, string autonym) // not used; if used, update similar to GetTextBiblesAndCallForButtons
    {
        _clickedLanguageIso = iso;
        _clickedLanguageAutonym = autonym;
        //_activeLanguageIso = iso;
        //_activeLanguageAutonym = autonym;
        var getRequest =
            CreateRequest(
                "https://4.dbt.io/api/bibles?language_code=" + iso + "&page=1&limit=25&v=4&key=" + sTypingText);

        yield return getRequest.SendWebRequest();


        var bibleData = JsonConvert.DeserializeObject<BibleData>(getRequest.downloadHandler.text,
            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

        if (getRequest.isDone)
        {
            
            _buttonInstantiator.ClearBibleButtons();
            ClearChapterButtons();
            ClearVersesText();
            ClearVerseBtns();
            ClearCopyrightText();

            for (int i = 0; i < bibleData.data.Count; i++)
            {
                string vName = bibleData.data[i].vname;
                string name = bibleData.data[i].name;
                string abbr = bibleData.data[i].abbr;
                
                if (bibleData.data[i].filesets.prod != null)
                {

                    for (int j = 0; j < bibleData.data[i].filesets.prod.Count; j++)
                    {
                        if (bibleData.data[i].filesets.prod[j] != null)
                        {
                            if (bibleData.data[i].filesets.prod[j].type == "text_plain")
                            {
                                if (vName != null && vName != "")
                                {
                                    _buttonInstantiator.InstantiateBibleButtons(vName, abbr,
                                        bibleData.data[i].filesets.prod[j].size, bibleData.data[i].filesets.prod[j].id,
                                        abbr);
                                }
                                else if (name != null && name != "")
                                {
                                    _buttonInstantiator.InstantiateBibleButtons(name, abbr,
                                        bibleData.data[i].filesets.prod[j].size, bibleData.data[i].filesets.prod[j].id,
                                        abbr);
                                }
                                else
                                {
                                    _buttonInstantiator.InstantiateBibleButtons(abbr, abbr,
                                        bibleData.data[i].filesets.prod[j].size, bibleData.data[i].filesets.prod[j].id,
                                        abbr);
                                }

                            }
                        }

                    }
                }


            }



        }

    }


    public bool VersesAreDisplayed()
    {
        return versesScrollViewGameObj.activeSelf || verseButtonsScrollViewGameObj.activeSelf;
    }



    public void ClearBooksChaptersVerses()
    {
        _buttonInstantiator.ClearBookButtons();
        ClearChapterButtons();
        ClearVersesText();
        ClearVerseBtns();
        ClearVerseInfoText();
        _activeTestament = "";
        ClearVerseInfoText();
        ClearCopyrightText();
        SetClickedBookText("");
        SetClickedChapterText("");
        ClearInfoText();

        if (versesScrollViewGameObj.activeSelf)
            versesScrollViewGameObj.SetActive(false);
        if (verseButtonsScrollViewGameObj.activeSelf)
            verseButtonsScrollViewGameObj.SetActive(false);
    }
    
    private IEnumerator GetBooksAndCallForButtons(string fileset, string abbr, string size, string volume)
    {
        var getRequest = CreateRequest("https://4.dbt.io/api/bibles/" + fileset + "/book?v=4&key=" + sTypingText);
        yield return getRequest.SendWebRequest();

        
        
        var bookData = JsonConvert.DeserializeObject<BookData>(getRequest.downloadHandler.text);

        if (getRequest.isDone)
        {

            if (bookData.data != null)
            {
                //Debug.Log("getRequest 1 using fileset: " + fileset);

                ClearChapterButtons();
                ClearBookButtons();
                
                stagedBibleId = fileset;
                stagedBibleSize = size;
                    
                for (int i = 0; i < bookData.data.Count; i++)
                {
                    if (bookData.data[i] != null)
                    {
                        SetClickedBibleText(volume);
                        
                        var name_short = bookData.data[i].name_short;
                        var name = bookData.data[i].name;
                        var bookID = bookData.data[i].book_id;
                        var bookChapters = bookData.data[i].chapters.Length;
                        var testament = bookData.data[i].testament;

                        if (name_short != null && name_short != "")
                        {
                            if (name_short != "Preface" && name_short != "Glossary")
                            {
                                if (size == "NT")
                                {
                                    if (testament == "NT")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons(name_short, bookID, bookChapters, testament);
                                    }// Apocrypha does not populate verses when called with NT
                                }
                                else if (size == "OT")
                                {
                                    if (testament == "OT")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons(name_short, bookID, bookChapters, testament);
                                    }
                                    if (testament == "AP")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons("Apoc. " + name_short, bookID, bookChapters, testament);
                                    }
                                }
                                else
                                {
                                    if (testament == "AP")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons("Apoc. " + name_short, bookID, bookChapters, testament);
                                    }
                                    else
                                    {
                                        _buttonInstantiator.InstantiateBookButtons(name_short, bookID, bookChapters, testament);
                                    }
                                }
                                
                            }
                                
                        }

                        else if (name != null && name != "")
                        {
                            if (name != "Preface" && name != "Glossary")
                            {
                                if (size == "NT")
                                {
                                    if (testament == "NT")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons(name, bookID, bookChapters, testament);
                                    } // Apocrypha does not populate verses when called with NT
                                }
                                else if (size == "OT")
                                {
                                    if (testament == "OT")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons(name, bookID, bookChapters, testament);
                                    }
                                    if (testament == "AP")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons("Apoc. " + name, bookID, bookChapters, testament);
                                    }
                                }
                                else
                                {
                                    if (testament == "AP")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons("Apoc. " + name, bookID, bookChapters, testament);
                                    }
                                    else
                                    {
                                        _buttonInstantiator.InstantiateBookButtons(name, bookID, bookChapters, testament);
                                    }
                                }
                                
                            }
                                
                        }
                    }
                }
                
                

            }
            else
            {
                
                var getRequest2 = CreateRequest("https://4.dbt.io/api/bibles/" + abbr + "/book?v=4&key=" + sTypingText);
                yield return getRequest2.SendWebRequest();

                var bookData2 = JsonConvert.DeserializeObject<BookData>(getRequest2.downloadHandler.text,
                new JsonSerializerSettings
                    { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore });

                if (getRequest2.isDone)
                {
                    if (bookData2.data != null)
                    {
                        //Debug.Log("getRequest2 using abbr: " + abbr);
                        
                        ClearChapterButtons();
                        
                        ClearBookButtons();

                        stagedBibleId = abbr;
                        stagedBibleSize = size;
                            
                        for (int i = 0; i < bookData2.data.Count; i++)
                        {
                            if (bookData2.data[i] != null)
                            {
                                SetClickedBibleText(volume);
                                
                                var name_short2 = bookData2.data[i].name_short;
                                var name2 = bookData2.data[i].name;
                                var testament2 = bookData2.data[i].testament;

                                if (name_short2 != null && name_short2 != "")
                                {
                                    if (name_short2 != "Preface"  && name_short2 != "Glossary")
                                    {
                                        if (size == "NT")
                                        {
                                            if (testament2 == "NT")
                                            {
                                                _buttonInstantiator.InstantiateBookButtons(name_short2, bookData2.data[i].book_id,
                                                    bookData2.data[i].chapters.Length, testament2);
                                            } // Apocrypha does not populate verses when called with NT
                                        }
                                        else if (size == "OT")
                                        {
                                            if (testament2 == "OT")
                                            {
                                                _buttonInstantiator.InstantiateBookButtons(name_short2, bookData2.data[i].book_id,
                                                    bookData2.data[i].chapters.Length, testament2);
                                            }
                                            if (testament2 == "AP")
                                            {
                                                _buttonInstantiator.InstantiateBookButtons("Apoc. " + name_short2, bookData2.data[i].book_id,
                                                    bookData2.data[i].chapters.Length, testament2);
                                            }
                                        }
                                        else
                                        {
                                            if (testament2 == "AP")
                                            {
                                                _buttonInstantiator.InstantiateBookButtons("Apoc. " + name_short2, bookData2.data[i].book_id,
                                                    bookData2.data[i].chapters.Length, testament2);
                                            }
                                            else
                                            {
                                                _buttonInstantiator.InstantiateBookButtons(name_short2, bookData2.data[i].book_id,
                                                    bookData2.data[i].chapters.Length, testament2);
                                            }
                                                
                                        }

                                    }

                                }

                                else if (name2 != null && name2 != "")
                                {
                                    if (name2 != "Preface" && name2 != null && name2 != "Glossary")
                                    {
                                        if (size == "NT")
                                        {
                                            if (testament2 == "NT")
                                            {
                                                _buttonInstantiator.InstantiateBookButtons(name2, bookData2.data[i].book_id,
                                                    bookData2.data[i].chapters.Length, testament2);
                                            }// Apocrypha does not populate verses when called with NT
                                        }
                                        else if (size == "OT")
                                        {
                                            if (testament2 == "OT")
                                            {
                                                _buttonInstantiator.InstantiateBookButtons(name2, bookData2.data[i].book_id,
                                                    bookData2.data[i].chapters.Length, testament2);
                                            }
                                            if (testament2 == "AP")
                                            {
                                                _buttonInstantiator.InstantiateBookButtons("Apoc. " + name2, bookData2.data[i].book_id,
                                                    bookData2.data[i].chapters.Length, testament2);
                                            }
                                        }
                                        else
                                        {
                                            if (testament2 == "AP") 
                                            {
                                                _buttonInstantiator.InstantiateBookButtons("Apoc. " + name2, bookData2.data[i].book_id,
                                                    bookData2.data[i].chapters.Length, testament2);
                                            }
                                            else 
                                            {
                                                _buttonInstantiator.InstantiateBookButtons(name2, bookData2.data[i].book_id,
                                                    bookData2.data[i].chapters.Length, testament2);
                                            }
                                        }

                                    }
                                        
                                }
                            }
                        }
                    }
                    else
                    {
                        var errorInfo = JsonConvert.DeserializeObject<ErrorData>(getRequest.downloadHandler.text);
                        if (errorInfo != null)
                        {
                            copyrightText.text =
                                $" *** NOTE *** {errorInfo.error.message}. Status code: {errorInfo.error.status_code} : Action: {errorInfo.error.action}  *** NOTE ***";
                            nullFilesets.null_filesets_list.Add(fileset);
                            InformationMessageTemporary(
                                "Remembering this bible so it does not show next time, because it currently has no content; click RESET to reset",
                                10f, Color.yellow);
                            
                            //Debug.Log("Alert!");
                            
                            ES3.Save("nullFilesets", JsonConvert.SerializeObject(nullFilesets));
                            //Debug.Log(ES3.Load<string>("nullFilesets"));
                        }
                        else
                        {
                            InformationMessageTemporary(
                                "Sorry, no data available for this Bible currently",
                                10f, Color.yellow);
                        }

                    }
                }
            }
        }



    }

     private IEnumerator GetLastBooksAndCallForButtons(string activeBibleID) //string fileset, string abbr
    {

        var getRequest = CreateRequest("https://4.dbt.io/api/bibles/" + activeBibleID + "/book?v=4&key=" + sTypingText);

        yield return getRequest.SendWebRequest();


        var bookData = JsonConvert.DeserializeObject<BookData>(getRequest.downloadHandler.text,
            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

        if (getRequest.isDone)
        {

            if (bookData.data != null)
            {
                for (int i = 0; i < bookData.data.Count; i++)
                {
                    if (bookData.data[i] != null)
                    {
                        var name_short = bookData.data[i].name_short;
                        var name = bookData.data[i].name;
                        var bookID = bookData.data[i].book_id;
                        var bookChapters = bookData.data[i].chapters.Length;
                        var testament = bookData.data[i].testament;

                        if (name_short != null && name_short != "")
                        {
                            if (name_short != "Preface" && name_short != "Glossary")
                            {
                                if (activeBibleSize == "NT")
                                {
                                    if (testament == "NT")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons(name_short, bookID, bookChapters, testament);
                                    }// Apocrypha does not populate verses when called with NT
                                }
                                else if (activeBibleSize == "OT")
                                {
                                    if (testament == "OT")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons(name_short, bookID, bookChapters, testament);
                                    }
                                    if (testament == "AP")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons("Apoc. " + name_short, bookID, bookChapters, testament);
                                    }
                                }
                                else
                                {
                                    if (testament == "AP")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons("Apoc. " + name_short, bookID, bookChapters, testament);
                                    }
                                    else
                                    {
                                        _buttonInstantiator.InstantiateBookButtons(name_short, bookID, bookChapters, testament);
                                    }
                                }
                                
                            }
                                
                        }

                        else if (name != null && name != "")
                        {
                            if (name != "Preface" && name != "Glossary")
                            {
                                if (activeBibleSize == "NT")
                                {
                                    if (testament == "NT")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons(name, bookID, bookChapters, testament);
                                    } // Apocrypha does not populate verses when called with NT
                                }
                                else if (activeBibleSize == "OT")
                                {
                                    if (testament == "OT")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons(name, bookID, bookChapters, testament);
                                    }
                                    if (testament == "AP")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons("Apoc. " + name, bookID, bookChapters, testament);
                                    }
                                }
                                else
                                {
                                    if (testament == "AP")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons("Apoc. " + name, bookID, bookChapters, testament);
                                    }
                                    else
                                    {
                                        _buttonInstantiator.InstantiateBookButtons(name, bookID, bookChapters, testament);
                                    }
                                }
                                
                            }
                                
                        }
                    }
                }

            }
            else
            {
                var errorInfo = JsonConvert.DeserializeObject<ErrorData>(getRequest.downloadHandler.text);
                if (errorInfo != null)
                {
                    copyrightText.text =
                        $" *** NOTE *** {errorInfo.error.message}. Status code: {errorInfo.error.status_code} : Action: {errorInfo.error.action}  *** NOTE ***";
                            
                    InformationMessageTemporary(
                        "Remembering this bible so it does not show next time, because it currently has no content; click RESET to reset",
                        10f, Color.yellow);
                    
                    //Debug.Log("Alert!");
                }
                else
                {
                    InformationMessageTemporary(
                        "Sorry, no data available for this Bible currently",
                        10f, Color.yellow);
                }
            }
        }
    }
     
     private IEnumerator GetBooksAndCallForButtonsNoSavedData(string fileset, string abbr, string size, string volume)
    {
        var getRequest = CreateRequest("https://4.dbt.io/api/bibles/" + fileset + "/book?v=4&key=" + sTypingText);
        yield return getRequest.SendWebRequest();

        
        
        var bookData = JsonConvert.DeserializeObject<BookData>(getRequest.downloadHandler.text);

        if (getRequest.isDone)
        {

            if (bookData.data != null)
            {
                //Debug.Log("getRequest 1 using fileset: " + fileset);

                // ClearChapterButtons();
                // ClearBookButtons();
                
                stagedBibleId = fileset;
                stagedBibleSize = size;
                    
                for (int i = 0; i < bookData.data.Count; i++)
                {
                    if (bookData.data[i] != null)
                    {
                        //SetClickedBibleText(volume);
                        
                        var name_short = bookData.data[i].name_short;
                        var name = bookData.data[i].name;
                        var bookID = bookData.data[i].book_id;
                        var bookChapters = bookData.data[i].chapters.Length;
                        var testament = bookData.data[i].testament;

                        if (name_short != null && name_short != "")
                        {
                            if (name_short != "Preface" && name_short != "Glossary")
                            {
                                if (size == "NT")
                                {
                                    if (testament == "NT")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons(name_short, bookID, bookChapters, testament);
                                    }// Apocrypha does not populate verses when called with NT
                                }
                                else if (size == "OT")
                                {
                                    if (testament == "OT")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons(name_short, bookID, bookChapters, testament);
                                    }
                                    if (testament == "AP")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons("Apoc. " + name_short, bookID, bookChapters, testament);
                                    }
                                }
                                else
                                {
                                    if (testament == "AP")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons("Apoc. " + name_short, bookID, bookChapters, testament);
                                    }
                                    else
                                    {
                                        _buttonInstantiator.InstantiateBookButtons(name_short, bookID, bookChapters, testament);
                                    }
                                }
                                
                            }
                                
                        }

                        else if (name != null && name != "")
                        {
                            if (name != "Preface" && name != "Glossary")
                            {
                                if (size == "NT")
                                {
                                    if (testament == "NT")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons(name, bookID, bookChapters, testament);
                                    } // Apocrypha does not populate verses when called with NT
                                }
                                else if (size == "OT")
                                {
                                    if (testament == "OT")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons(name, bookID, bookChapters, testament);
                                    }
                                    if (testament == "AP")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons("Apoc. " + name, bookID, bookChapters, testament);
                                    }
                                }
                                else
                                {
                                    if (testament == "AP")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons("Apoc. " + name, bookID, bookChapters, testament);
                                    }
                                    else
                                    {
                                        _buttonInstantiator.InstantiateBookButtons(name, bookID, bookChapters, testament);
                                    }
                                }
                                
                            }
                                
                        }
                    }
                }
                
                

            }
            else
            {
                
                var getRequest2 = CreateRequest("https://4.dbt.io/api/bibles/" + abbr + "/book?v=4&key=" + sTypingText);
                yield return getRequest2.SendWebRequest();

                var bookData2 = JsonConvert.DeserializeObject<BookData>(getRequest2.downloadHandler.text,
                new JsonSerializerSettings
                    { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore });

                if (getRequest2.isDone)
                {
                    if (bookData2.data != null)
                    {
                        //Debug.Log("getRequest2 using abbr: " + abbr);
                        
                        // ClearChapterButtons();
                        //
                        // ClearBookButtons();

                        stagedBibleId = abbr;
                        stagedBibleSize = size;
                            
                        for (int i = 0; i < bookData2.data.Count; i++)
                        {
                            if (bookData2.data[i] != null)
                            {
                                //SetClickedBibleText(volume);
                                
                                var name_short2 = bookData2.data[i].name_short;
                                var name2 = bookData2.data[i].name;
                                var testament2 = bookData2.data[i].testament;

                                if (name_short2 != null && name_short2 != "")
                                {
                                    if (name_short2 != "Preface"  && name_short2 != "Glossary")
                                    {
                                        if (size == "NT")
                                        {
                                            if (testament2 == "NT")
                                            {
                                                _buttonInstantiator.InstantiateBookButtons(name_short2, bookData2.data[i].book_id,
                                                    bookData2.data[i].chapters.Length, testament2);
                                            } // Apocrypha does not populate verses when called with NT
                                        }
                                        else if (size == "OT")
                                        {
                                            if (testament2 == "OT")
                                            {
                                                _buttonInstantiator.InstantiateBookButtons(name_short2, bookData2.data[i].book_id,
                                                    bookData2.data[i].chapters.Length, testament2);
                                            }
                                            if (testament2 == "AP")
                                            {
                                                _buttonInstantiator.InstantiateBookButtons("Apoc. " + name_short2, bookData2.data[i].book_id,
                                                    bookData2.data[i].chapters.Length, testament2);
                                            }
                                        }
                                        else
                                        {
                                            if (testament2 == "AP")
                                            {
                                                _buttonInstantiator.InstantiateBookButtons("Apoc. " + name_short2, bookData2.data[i].book_id,
                                                    bookData2.data[i].chapters.Length, testament2);
                                            }
                                            else
                                            {
                                                _buttonInstantiator.InstantiateBookButtons(name_short2, bookData2.data[i].book_id,
                                                    bookData2.data[i].chapters.Length, testament2);
                                            }
                                                
                                        }

                                    }

                                }

                                else if (name2 != null && name2 != "")
                                {
                                    if (name2 != "Preface" && name2 != null && name2 != "Glossary")
                                    {
                                        if (size == "NT")
                                        {
                                            if (testament2 == "NT")
                                            {
                                                _buttonInstantiator.InstantiateBookButtons(name2, bookData2.data[i].book_id,
                                                    bookData2.data[i].chapters.Length, testament2);
                                            }// Apocrypha does not populate verses when called with NT
                                        }
                                        else if (size == "OT")
                                        {
                                            if (testament2 == "OT")
                                            {
                                                _buttonInstantiator.InstantiateBookButtons(name2, bookData2.data[i].book_id,
                                                    bookData2.data[i].chapters.Length, testament2);
                                            }
                                            if (testament2 == "AP")
                                            {
                                                _buttonInstantiator.InstantiateBookButtons("Apoc. " + name2, bookData2.data[i].book_id,
                                                    bookData2.data[i].chapters.Length, testament2);
                                            }
                                        }
                                        else
                                        {
                                            if (testament2 == "AP") 
                                            {
                                                _buttonInstantiator.InstantiateBookButtons("Apoc. " + name2, bookData2.data[i].book_id,
                                                    bookData2.data[i].chapters.Length, testament2);
                                            }
                                            else 
                                            {
                                                _buttonInstantiator.InstantiateBookButtons(name2, bookData2.data[i].book_id,
                                                    bookData2.data[i].chapters.Length, testament2);
                                            }
                                        }

                                    }
                                        
                                }
                            }
                        }
                    }
                    else
                    {
                        var errorInfo = JsonConvert.DeserializeObject<ErrorData>(getRequest.downloadHandler.text);
                        if (errorInfo != null)
                        {
                            copyrightText.text =
                                $" *** NOTE *** {errorInfo.error.message}. Status code: {errorInfo.error.status_code} : Action: {errorInfo.error.action}  *** NOTE ***";
                            nullFilesets.null_filesets_list.Add(fileset);
                            InformationMessageTemporary(
                                "Remembering this bible so it does not show next time, because it currently has no content; click RESET to reset",
                                10f, Color.yellow);
                            
                            //Debug.Log("Alert!");
                            
                            ES3.Save("nullFilesets", JsonConvert.SerializeObject(nullFilesets));
                            //Debug.Log(ES3.Load<string>("nullFilesets"));
                        }
                        else
                        {
                            InformationMessageTemporary(
                                "Sorry, no data available for this Bible currently",
                                10f, Color.yellow);
                        }

                    }
                }
            }
        }



    }
     
    
    private IEnumerator TryReplaceBibleForCurrentBook(string fileset, string abbr, string sizeOfNewBible, string volume)
    {
        var getRequest = CreateRequest("https://4.dbt.io/api/bibles/" + fileset + "/book?v=4&key=" + sTypingText);
        yield return getRequest.SendWebRequest();
        
        var bookData = JsonConvert.DeserializeObject<BookData>(getRequest.downloadHandler.text);
        
        if (getRequest.isDone)
        {
            if (bookData.data != null)
            {
                bool foundBook = false;
                for (int i = 0; i < bookData.data.Count; i++)
                {
                    //Debug.Log(bookData.data[i].book_id);
                    if (bookData.data[i].book_id == activeBookID)
                    {
                        foundBook = true;
                        if (bookData.data[i].chapters.Contains(activeChapter))
                        {
                            stagedBibleId = fileset;
                            
                            if (bookData.data[i].name_short != null)
                            {
                                SetBibleEmpty(false);
                                recallBookData = bookData;
                                ReplaceVerses(fileset,activeChapter, 
                                    volume, bookData.data[i].name_short, volume, abbr, sizeOfNewBible);
                            }
                            else if (bookData.data[i].name != null)
                            {
                                SetBibleEmpty(false);
                                recallBookData = bookData;
                                ReplaceVerses(fileset,activeChapter, 
                                    volume, bookData.data[i].name, volume, abbr, sizeOfNewBible);
                            }
                            else
                            {
                                //Debug.Log("name_short and name are null ");
                                if (callingApocryphaFromOT)
                                {
                                    InformationMessageTemporary("The Apocrypha is not contained in all Old Testament volumes.", 5f, Color.yellow);
                                    callingApocryphaFromOT = false;
                                    SetBibleEmpty(true);
                                }
                                else if (callingAppocryphaFromComplete)
                                {
                                    InformationMessageTemporary("The Apocrypha is not contained in all complete volumes.", 5f, Color.yellow);
                                    callingAppocryphaFromComplete = false;
                                    SetBibleEmpty(true);
                                }
                                else
                                {
                                    InformationMessageTemporary("Try another Bible.", 5f, Color.yellow );
                                    SetBibleEmpty(true);
                                }
                                    
                            }

                        }
                    }
                    else if (!foundBook && i >= bookData.data.Count - 1)
                    {
                        
                        if (callingApocryphaFromOT)
                        {
                            InformationMessageTemporary("The Apocrypha is not contained in all Old Testament volumes.", 5f, Color.yellow);
                            callingApocryphaFromOT = false;
                            SetBibleEmpty(true);
                        }
                        else if (callingAppocryphaFromComplete)
                        {
                            InformationMessageTemporary("The Apocrypha is not contained in all complete volumes.", 5f, Color.yellow);
                            callingAppocryphaFromComplete = false;
                            SetBibleEmpty(true);
                        }
                        else
                        {
                            InformationMessageTemporary("Try another Bible.", 5f, Color.yellow );
                            SetBibleEmpty(true);
                        }

                        
                    }
                }
            }
            else
            {
                var getRequest2 = CreateRequest("https://4.dbt.io/api/bibles/" + abbr + "/book?v=4&key=" + sTypingText);
                yield return getRequest2.SendWebRequest();

                var bookData2 = JsonConvert.DeserializeObject<BookData>(getRequest2.downloadHandler.text,
                    new JsonSerializerSettings
                        { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore });

                if (getRequest2.isDone)
                {
                    if (bookData2.data != null)
                    {
                        bool foundBook = false;
                        for (int i = 0; i < bookData2.data.Count; i++)
                        {
                            //Debug.Log(bookData2.data[i].book_id);
                            if (bookData2.data[i].book_id == activeBookID)
                            {
                                foundBook = true;
                                if (bookData2.data[i].chapters.Contains(activeChapter))
                                {
                                    stagedBibleId = abbr;
                                    
                                    if (bookData2.data[i].name_short != null)
                                    {
                                        SetBibleEmpty(false);
                                        recallBookData = bookData2;
                                        ReplaceVerses(fileset, activeChapter, 
                                            volume, bookData2.data[i].name_short, volume, abbr, sizeOfNewBible);
                                        
                                    }
                                    else if (bookData2.data[i].name != null)
                                    {
                                        SetBibleEmpty(false);
                                        recallBookData = bookData2;
                                        ReplaceVerses(fileset, activeChapter, 
                                            volume, bookData2.data[i].name, volume, abbr, sizeOfNewBible); 
                                    }
                                    else
                                    {
                                        if (callingApocryphaFromOT)
                                        {
                                            InformationMessageTemporary("The Apocrypha is not contained in all Old Testament volumes.", 5f, Color.yellow);
                                            callingApocryphaFromOT = false;
                                            SetBibleEmpty(true);
                                        }
                                        else if (callingAppocryphaFromComplete)
                                        {
                                            InformationMessageTemporary("The Apocrypha is not contained in all complete volumes.", 5f, Color.yellow);
                                            callingAppocryphaFromComplete = false;
                                            SetBibleEmpty(true);
                                        }
                                        else
                                        {
                                            InformationMessageTemporary("Try another Bible.", 5f, Color.yellow );
                                            SetBibleEmpty(true);
                                        }
                                    }

                                    //break;
                                }
                            }
                            else if (!foundBook && i >= bookData2.data.Count - 1)
                            {
                                
                                if (callingApocryphaFromOT)
                                {
                                    InformationMessageTemporary("The Apocrypha is not contained in all Old Testament volumes.", 5f, Color.yellow);
                                    callingApocryphaFromOT = false;
                                    SetBibleEmpty(true);
                                }
                                else if (callingAppocryphaFromComplete)
                                {
                                    InformationMessageTemporary("The Apocrypha is not contained in all complete volumes.", 5f, Color.yellow);
                                    callingAppocryphaFromComplete = false;
                                    SetBibleEmpty(true);
                                }
                                else
                                {
                                    InformationMessageTemporary("Try another Bible.", 5f, Color.yellow );
                                    SetBibleEmpty(true);
                                }
                            }
                            
                        }
                    }
                }
            }
        }
    }


    public void ReplaceBookAndChapterButtons(string fileset, string abbr, string sizeOfNewBible, string volume)
    {
        var bookData = recallBookData;
        if (bookData != null)
        {
            if (bookData.data != null)
            {
                ClearBookButtons();

                for (int i = 0; i < bookData.data.Count; i++) // for each book
                {
                    if (bookData.data[i] != null)
                    {

                        var name_short = bookData.data[i].name_short;
                        var name = bookData.data[i].name;
                        var bookID = bookData.data[i].book_id;
                        var bookChapters = bookData.data[i].chapters.Length;
                        var testament = bookData.data[i].testament;

                        if (name_short != null && name_short != "")
                        {
                            if (name_short != "Preface" && name_short != "Glossary")
                            {
                                bool createdButtons = false;
                                if (sizeOfNewBible == "NT")
                                {
                                    if (testament == "NT")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons(name_short, bookID, bookChapters, testament);
                                        
                                        createdButtons = true;
                                    }// Apocrypha does not populate verses when called with NT
                                }
                                else if (sizeOfNewBible == "OT")
                                {
                                    if (testament == "OT")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons(name_short, bookID, bookChapters, testament);
                                        createdButtons = true;
                                        
                                    }
                                    if (testament == "AP")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons("Apoc. " + name_short, bookID, bookChapters, testament);
                                        createdButtons = true;
                                        
                                    }
                                    
                                    
                                }
                                else
                                {
                                    if (testament == "AP")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons("Apoc. " + name_short, bookID, bookChapters, testament);
                                        createdButtons = true;
                                        
                                    }
                                    else
                                    {
                                        _buttonInstantiator.InstantiateBookButtons(name_short, bookID, bookChapters, testament);
                                        createdButtons = true;
                                        
                                    }
                                    
                                }

                                if (createdButtons && bookID == activeBookID) // only make chapters buttons if the same book is replaced
                                {
                                    ClearChapterButtons();
                                    MakeChapterButtons(bookID, name_short, bookChapters, false);
                                    infoTextBook = name_short;
                                    SetInfoText();
                                    SetClickedBookText(name_short); ;
                                    SetClickedChapterText($"{name_short}: {activeChapter}");
                                    //Debug.Log("--------------------------------------Book and Chapter Text updated");
                                    createdButtons = false;
                                }


                            }
                                
                        }

                        else if (name != null && name != "")
                        {
                            if (name != "Preface" && name != "Glossary")
                            {
                                bool createdButtons = false;
                                if (sizeOfNewBible == "NT")
                                {
                                    if (testament == "NT")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons(name, bookID, bookChapters, testament);
                                        createdButtons = true;
                                        
                                    } // Apocrypha does not populate verses when called with NT
                                }
                                else if (sizeOfNewBible == "OT")
                                {
                                    if (testament == "OT")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons(name, bookID, bookChapters, testament);
                                        createdButtons = true;
                                        
                                    }
                                    if (testament == "AP")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons("Apoc. " + name, bookID, bookChapters, testament);
                                        createdButtons = true;
                                        
                                    }
                                }
                                else
                                {
                                    if (testament == "AP")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons("Apoc. " + name, bookID, bookChapters, testament);
                                        createdButtons = true;
                                        
                                    }
                                    else
                                    {
                                        _buttonInstantiator.InstantiateBookButtons(name, bookID, bookChapters, testament);
                                        createdButtons = true;
                                        
                                    }
                                }

                                if (createdButtons && bookID == activeBookID)
                                {
                                    ClearChapterButtons();
                                    MakeChapterButtons(bookID, name, bookChapters);
                                    infoTextBook = name;
                                    SetInfoText();
                                    SetClickedBookText(name); ;
                                    SetClickedChapterText($"{name}: {activeChapter}");
                                    //Debug.Log("--------------------------------------Book and Chapter Text updated");
                                    createdButtons = false;
                                }

                            }
                                
                        }
                            
                    }
                }

            }
        }
        
    }
    

    private IEnumerator ReplaceBooksAndCallForButtons(string fileset, string abbr, string sizeOfNewBible, string volume)
    {
        var getRequest = CreateRequest("https://4.dbt.io/api/bibles/" + fileset + "/book?v=4&key=" + sTypingText);
                                            
        yield return getRequest.SendWebRequest();
        
        var bookData = JsonConvert.DeserializeObject<BookData>(getRequest.downloadHandler.text);
        
        if (getRequest.isDone)
        {

            if (bookData.data != null)
            {
                stagedBibleId = fileset;
                activeBibleId = fileset;
                
                ClearBookButtons();
                

                for (int i = 0; i < bookData.data.Count; i++) // for each book
                {
                    if (bookData.data[i] != null)
                    {

                        var name_short = bookData.data[i].name_short;
                        var name = bookData.data[i].name;
                        var bookID = bookData.data[i].book_id;
                        var bookChapters = bookData.data[i].chapters.Length;
                        var testament = bookData.data[i].testament;

                        if (name_short != null && name_short != "")
                        {
                            if (name_short != "Preface" && name_short != "Glossary")
                            {
                                bool createdButtons = false;
                                if (sizeOfNewBible == "NT")
                                {
                                    if (testament == "NT")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons(name_short, bookID, bookChapters, testament);
                                        
                                        createdButtons = true;
                                    }// Apocrypha does not populate verses when called with NT
                                }
                                else if (sizeOfNewBible == "OT")
                                {
                                    if (testament == "OT")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons(name_short, bookID, bookChapters, testament);
                                        createdButtons = true;
                                        
                                    }
                                    if (testament == "AP")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons("Apoc. " + name_short, bookID, bookChapters, testament);
                                        createdButtons = true;
                                        
                                    }
                                    
                                    
                                }
                                else
                                {
                                    if (testament == "AP")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons("Apoc. " + name_short, bookID, bookChapters, testament);
                                        createdButtons = true;
                                        
                                    }
                                    else
                                    {
                                        _buttonInstantiator.InstantiateBookButtons(name_short, bookID, bookChapters, testament);
                                        createdButtons = true;
                                        
                                    }
                                    
                                }

                                if (createdButtons && bookID == activeBookID) // only make chapters buttons if the same book is replaced
                                {
                                    ClearChapterButtons();
                                    MakeChapterButtons(bookID, name_short, bookChapters, false);
                                    infoTextBook = name_short;
                                    SetInfoText();
                                    SetClickedBookText(name_short); ;
                                    SetClickedChapterText($"{name_short}: {activeChapter}");
                                    //Debug.Log("--------------------------------------Book and Chapter Text updated");
                                    createdButtons = false;
                                }


                            }
                                
                        }

                        else if (name != null && name != "")
                        {
                            if (name != "Preface" && name != "Glossary")
                            {
                                bool createdButtons = false;
                                if (sizeOfNewBible == "NT")
                                {
                                    if (testament == "NT")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons(name, bookID, bookChapters, testament);
                                        createdButtons = true;
                                        
                                    } // Apocrypha does not populate verses when called with NT
                                }
                                else if (sizeOfNewBible == "OT")
                                {
                                    if (testament == "OT")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons(name, bookID, bookChapters, testament);
                                        createdButtons = true;
                                        
                                    }
                                    if (testament == "AP")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons("Apoc. " + name, bookID, bookChapters, testament);
                                        createdButtons = true;
                                        
                                    }
                                }
                                else
                                {
                                    if (testament == "AP")
                                    {
                                        _buttonInstantiator.InstantiateBookButtons("Apoc. " + name, bookID, bookChapters, testament);
                                        createdButtons = true;
                                        
                                    }
                                    else
                                    {
                                        _buttonInstantiator.InstantiateBookButtons(name, bookID, bookChapters, testament);
                                        createdButtons = true;
                                        
                                    }
                                }

                                if (createdButtons && bookID == activeBookID)
                                {
                                    ClearChapterButtons();
                                    MakeChapterButtons(bookID, name, bookChapters);
                                    infoTextBook = name;
                                    SetInfoText();
                                    SetClickedBookText(name); ;
                                    SetClickedChapterText($"{name}: {activeChapter}");
                                    //Debug.Log("--------------------------------------Book and Chapter Text updated");
                                    createdButtons = false;
                                }

                            }
                                
                        }
                            
                    }
                }
                    

            }
            else
            {
                
                var getRequest2 = CreateRequest("https://4.dbt.io/api/bibles/" + abbr + "/book?v=4&key=" + sTypingText);
                yield return getRequest2.SendWebRequest();

                var bookData2 = JsonConvert.DeserializeObject<BookData>(getRequest2.downloadHandler.text,
                new JsonSerializerSettings
                    { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore });

                if (getRequest2.isDone)
                {
                    if (bookData2.data != null)
                    {
                        stagedBibleId = abbr;
                        activeBibleId = abbr;

                        ClearBookButtons();


                        for (int i = 0; i < bookData2.data.Count; i++)
                        {
                            if (bookData2.data[i] != null)
                            {
                                var name_short2 = bookData2.data[i].name_short;
                                var name2 = bookData2.data[i].name;
                                var testament2 = bookData2.data[i].testament;

                                if (name_short2 != null && name_short2 != "")
                                {
                                    if (name_short2 != "Preface"  && name_short2 != "Glossary")
                                    {
                                        bool createdButtons = false;
                                        if (sizeOfNewBible == "NT")
                                        {
                                            if (testament2 == "NT")
                                            {
                                                _buttonInstantiator.InstantiateBookButtons(name_short2, bookData2.data[i].book_id,
                                                    bookData2.data[i].chapters.Length, testament2);
                                                createdButtons = true;
                                                
                                            } // Apocrypha does not populate verses when called with NT
                                        }
                                        else if (sizeOfNewBible == "OT")
                                        {
                                            if (testament2 == "OT")
                                            {
                                                _buttonInstantiator.InstantiateBookButtons(name_short2, bookData2.data[i].book_id,
                                                    bookData2.data[i].chapters.Length, testament2);
                                                createdButtons = true;
                                                
                                            }
                                            if (testament2 == "AP")
                                            {
                                                _buttonInstantiator.InstantiateBookButtons("Apoc. " + name_short2, bookData2.data[i].book_id,
                                                    bookData2.data[i].chapters.Length, testament2);
                                                createdButtons = true;
                                                
                                            }
                                        }
                                        else
                                        {
                                            if (testament2 == "AP")
                                            {
                                                _buttonInstantiator.InstantiateBookButtons("Apoc. " + name_short2, bookData2.data[i].book_id,
                                                    bookData2.data[i].chapters.Length, testament2);
                                                createdButtons = true;
                                                
                                            }
                                            else
                                            {
                                                _buttonInstantiator.InstantiateBookButtons(name_short2, bookData2.data[i].book_id,
                                                    bookData2.data[i].chapters.Length, testament2);
                                                createdButtons = true;
                                                
                                            }
                                                
                                        }

                                        if (createdButtons && bookData2.data[i].book_id == activeBookID)
                                        {
                                            ClearChapterButtons();
                                            MakeChapterButtons(bookData2.data[i].book_id, name_short2, bookData2.data[i].chapters.Length);
                                            infoTextBook = name_short2;
                                            SetInfoText();
                                            SetClickedBookText(name_short2); ;
                                            SetClickedChapterText($"{name_short2}: {activeChapter}");
                                            createdButtons = false;
                                        }
                                        
                                    }
                                       
                                    
                                }

                                else if (name2 != null && name2 != "")
                                {
                                    if (name2 != "Preface" && name2 != null && name2 != "Glossary")
                                    {
                                        bool createdButtons = false;
                                        if (sizeOfNewBible == "NT")
                                        {
                                            if (testament2 == "NT")
                                            {
                                                _buttonInstantiator.InstantiateBookButtons(name2, bookData2.data[i].book_id,
                                                    bookData2.data[i].chapters.Length, testament2);
                                                createdButtons = true;
                                                
                                            }// Apocrypha does not populate verses when called with NT
                                        }
                                        else if (sizeOfNewBible == "OT")
                                        {
                                            if (testament2 == "OT")
                                            {
                                                _buttonInstantiator.InstantiateBookButtons(name2, bookData2.data[i].book_id,
                                                    bookData2.data[i].chapters.Length, testament2);
                                                createdButtons = true;
                                                
                                            }
                                            if (testament2 == "AP")
                                            {
                                                _buttonInstantiator.InstantiateBookButtons("Apoc. " + name2, bookData2.data[i].book_id,
                                                    bookData2.data[i].chapters.Length, testament2);
                                                createdButtons = true;
                                                
                                            }
                                        }
                                        else
                                        {
                                            if (testament2 == "AP") 
                                            {
                                                _buttonInstantiator.InstantiateBookButtons("Apoc. " + name2, bookData2.data[i].book_id,
                                                    bookData2.data[i].chapters.Length, testament2);
                                                createdButtons = true;
                                                
                                            }
                                            else 
                                            {
                                                _buttonInstantiator.InstantiateBookButtons(name2, bookData2.data[i].book_id,
                                                    bookData2.data[i].chapters.Length, testament2);
                                                createdButtons = true;
                                                
                                            }
                                        }
                                            
                                        if (createdButtons && bookData2.data[i].book_id == activeBookID)
                                        {
                                            ClearChapterButtons();
                                            MakeChapterButtons(bookData2.data[i].book_id, name2, bookData2.data[i].chapters.Length);
                                            infoTextBook = name2;
                                            SetInfoText();
                                            SetClickedBookText(name2); ;
                                            SetClickedChapterText($"{name2}: {activeChapter}");
                                            createdButtons = false;
                                        }
                                        
                                    }
                                        
                                }
                            }
                        }
                        
                        

                    }
                    else
                    {
                        var errorInfo = JsonConvert.DeserializeObject<ErrorData>(getRequest.downloadHandler.text);
                        if (errorInfo != null)
                        {
                            copyrightText.text =
                                $" *** NOTE *** {errorInfo.error.message}. Status code: {errorInfo.error.status_code} : Action: {errorInfo.error.action}  *** NOTE ***";
                            nullFilesets.null_filesets_list.Add(fileset);
                            InformationMessageTemporary(
                                "Remembering this bible so it does not show next time, because it currently has no content; click RESET to reset",
                                10f, Color.yellow);
                            
                            //Debug.Log("Alert!");
                            
                            ES3.Save("nullFilesets", JsonConvert.SerializeObject(nullFilesets));
                            //Debug.Log(ES3.Load<string>("nullFilesets"));
                        }
                        else
                        {
                            InformationMessageTemporary(
                                "Sorry, something went wrong; please try again",
                                10f, Color.yellow);
                            //Debug.Log("Something");
                        }

                    }
                }
            }
        }



    }

    
    public void ClearChapterButtons()
    {
        _buttonInstantiator.ClearChapterButtons();
    }

    private string GetLastLevel()
    {
        var lastLevel = GetLowValue() + GetNewLow();
        return lastLevel.Substring(0, 18);
    }

    public void MakeChapterButtons(string bookID, string bookName, int chapters, bool clearVerses = false)
    {
        ClearChapterButtons();
        if (clearVerses)
        {
            ClearVersesText();
            ClearVerseBtns();
        }
        
        for (int i = 1; i < chapters + 1; i++)
        {
            _buttonInstantiator.InstantiateChapterButtons(i, bookID, bookName);
            //Debug.Log($"Made chapter button: {i}");
        }
    }

    public UnityEvent onTypingVerse;

    public void ScrollToActiveVerse()
    {
           
    }
    
    private IEnumerator RequestBibleVerses(string filesetId, string bookId, int chapter)
    {
        //Debug.Log("                              RequestBibleVerses Just Called using filesetId of: " + filesetId);
        var getRequest =
            CreateRequest($"https://4.dbt.io/api/bibles/filesets/{filesetId}/{bookId}/{chapter}?v=4&key={sTypingText}");
        yield return getRequest.SendWebRequest();

        //var deserializedGetData = JsonUtility.FromJson<VerseData>(getRequest.downloadHandler.text);
        var versesData = JsonConvert.DeserializeObject<VerseData>(getRequest.downloadHandler.text);

        if (getRequest.isDone)
        {
            if (versesData.data == null)
            {
                InformationMessageTemporary("Something went wrong; please try again.", 6f, Color.yellow);
            }
            else
            {
                
                activeVersesDATA = versesData;
                var data = versesData.data;
                
                if (data.Count > 0)
                {

                    if (resumedOnStart)
                    {
                        //StartCoroutine(ReplaceBooksAndCallForButtons(filesetId, clickedAbbr, clickedBibleSize, clickedVolume));
                        
                        SetClickedBookText(_activeBookName); ;
                        SetClickedChapterText($"{_activeBookName}: {chapter}");
                        resumedOnStart = false; // From LoadInitialData()
                    }
                    else
                    {
                        SetActiveDataAfterVerseDisplayed( filesetId, stagedBibleSize, clickedAbbr, stagedBibleId, clickedVolume, 
                            bookId, clickedBookName, clickedTestament, chapter, clickedBookNumberOfChapters);

                        if (!bibleEmpty) // in case player tries a bible w/ diff. lang but it is empty, then clicks on current chptr of current lang
                        {
                            SetActiveLangAfterVerseDisplayed(_clickedLanguageIso, _clickedLanguageAutonym);
                        }

                    }
                    
                    infoTextLang = _activeLanguageAutonym;
                    infoTextBible = _activeVolume;
                    infoTextBook = _activeBookName;
                    infoTextChapter = chapter.ToString();
                    SetInfoText();
                    
                    GetCopyrightInfo(filesetId);    
                    
                }

                if (_typing)
                {
                    if (versesScrollViewGameObj.activeSelf)
                        versesScrollViewGameObj.SetActive(false);
                    if (!verseButtonsScrollViewGameObj.activeSelf)
                        verseButtonsScrollViewGameObj.SetActive(true);
                    ClearVerseBtns();
                    int firstUntypedButtonIndex = 0;
                    bool firstButtonSet = false;
                    bool typed = false;
                    for (int i = 0; i < data.Count; i++)
                    {
                        if (rememberVerses)
                        {
                            typed = versesTypedMemory.CheckIfVerseWasTyped(activeBibleId, activeBookID, activeChapter,
                                i + 1);
                            // if (typed)
                            // {
                            //     Debug.Log($"Typed: Active Bible ID et al: {activeBibleId} {activeBookID} {activeChapter}; RequestBibleVerses()");
                            // }
                        }
                        
                        
                        //Debug.Log($"Instantiating Verse Button: {activeBibleId} {bookId} {activeChapter}: {i}. Active ");
                        _buttonInstantiator.InstantiateVerseTypingButtons(data[i].verse_text, i + 1, typed);

                        if (!typed && !firstButtonSet)
                        {
                            firstUntypedButtonIndex = i;
                            firstButtonSet = true;
                        }

                        if (i >= data.Count - 1 && !firstButtonSet)
                            firstUntypedButtonIndex = 0;

                    }

                    versesButtonsScrlBar.value = 1; // REPLACE THIS WITH CALCULATION FOR SCROLL
                    
                    var firstButton = _buttonInstantiator.verseButtonsList[firstUntypedButtonIndex];
                    EventSystem.current.SetSelectedGameObject(firstButton);
                    firstButton.GetComponent<VerseButton>().InvokeVerseClickedEvent();
                    
                    ShowIdleText();

                }
                else if (_showVerseNumbers && _newLineForEachVerse)
                {
                    if (!versesScrollViewGameObj.activeSelf)
                        versesScrollViewGameObj.SetActive(true);
                    if (verseButtonsScrollViewGameObj.activeSelf)
                        verseButtonsScrollViewGameObj.SetActive(false);
                    versesText.text = "";
                    var versesTextObjPosition = versesTextObj.position;
                    versesTextObjPosition = new Vector3(versesTextObjPosition.x, 0, versesTextObjPosition.z);
                    for (int i = 0; i < data.Count; i++)
                    {
                        versesText.text = String.Concat(versesText.text, $"{i + 1} {data[i].verse_text}\n");
                    }

                    HideIdleText();

                }
                else if (_showVerseNumbers)
                {
                    if (!versesScrollViewGameObj.activeSelf)
                        versesScrollViewGameObj.SetActive(true);
                    if (verseButtonsScrollViewGameObj.activeSelf)
                        verseButtonsScrollViewGameObj.SetActive(false);
                    versesText.text = "";
                    var versesTextObjPosition = versesTextObj.position;
                    versesTextObjPosition = new Vector3(versesTextObjPosition.x, 0, versesTextObjPosition.z);
                    for (int i = 0; i < data.Count; i++)
                    {
                        versesText.text = String.Concat(versesText.text, $"{i + 1} {data[i].verse_text} ");
                    }

                    HideIdleText();

                }
                else if (_newLineForEachVerse)
                {
                    if (!versesScrollViewGameObj.activeSelf)
                        versesScrollViewGameObj.SetActive(true);
                    if (verseButtonsScrollViewGameObj.activeSelf)
                        verseButtonsScrollViewGameObj.SetActive(false);
                    versesText.text = "";
                    var versesTextObjPosition = versesTextObj.position;
                    versesTextObjPosition = new Vector3(versesTextObjPosition.x, 0, versesTextObjPosition.z);
                    for (int i = 0; i < data.Count; i++)
                    {
                        versesText.text = String.Concat(versesText.text, $"{data[i].verse_text}\n");
                    }

                    HideIdleText();

                }
                else
                {
                    if (!versesScrollViewGameObj.activeSelf)
                        versesScrollViewGameObj.SetActive(true);
                    if (verseButtonsScrollViewGameObj.activeSelf)
                        verseButtonsScrollViewGameObj.SetActive(false);
                    versesText.text = "";
                    var versesTextObjPosition = versesTextObj.position;
                    versesTextObjPosition = new Vector3(versesTextObjPosition.x, 0, versesTextObjPosition.z);
                    //versesTextObj.position = new Vector3(versesTextObj.position.x, 0, versesTextObj.position.z);
                    for (int i = 0; i < data.Count; i++)
                    {
                        versesText.text = String.Concat(versesText.text, $"{data[i].verse_text} ");
                    }

                    HideIdleText();

                }

            }

        }

    }
    
    private IEnumerator RequestBibleVersesNoSavedData(string filesetId, string bookId, int chapter)
    {
        //Debug.Log("                              RequestBibleVerses Just Called using filesetId of: " + filesetId);
        var getRequest =
            CreateRequest($"https://4.dbt.io/api/bibles/filesets/{filesetId}/{bookId}/{chapter}?v=4&key={sTypingText}");
        yield return getRequest.SendWebRequest();

        //var deserializedGetData = JsonUtility.FromJson<VerseData>(getRequest.downloadHandler.text);
        var versesData = JsonConvert.DeserializeObject<VerseData>(getRequest.downloadHandler.text);

        if (getRequest.isDone)
        {
            if (versesData.data == null)
            {
                InformationMessageTemporary("Something went wrong; please try again.", 6f, Color.yellow);
            }
            else
            {
                
                activeVersesDATA = versesData;
                var data = versesData.data;
                
                if (data.Count > 0)
                {

                    /*if (resumedOnStart)
                    {
                        //StartCoroutine(ReplaceBooksAndCallForButtons(filesetId, clickedAbbr, clickedBibleSize, clickedVolume));
                        
                        SetClickedBookText(_activeBookName); ;
                        SetClickedChapterText($"{_activeBookName}: {chapter}");
                        resumedOnStart = false; // From LoadInitialData()
                    }
                    else
                    {
                        SetActiveDataAfterVerseDisplayed( filesetId, stagedBibleSize, clickedAbbr, stagedBibleId, clickedVolume, 
                            bookId, clickedBookName, clickedTestament, chapter, clickedBookNumberOfChapters);

                        if (!bibleEmpty) // in case player tries a bible w/ diff. lang but it is empty, then clicks on current chptr of current lang
                        {
                            SetActiveLangAfterVerseDisplayed(_clickedLanguageIso, _clickedLanguageAutonym);
                        }

                    }
                    
                    infoTextLang = _activeLanguageAutonym;
                    infoTextBible = _activeVolume;
                    infoTextBook = _activeBookName;
                    infoTextChapter = chapter.ToString();*/
                    //SetInfoText();
                    
                    GetCopyrightInfo(filesetId);    
                    
                }

                if (_typing)
                {
                    if (versesScrollViewGameObj.activeSelf)
                        versesScrollViewGameObj.SetActive(false);
                    if (!verseButtonsScrollViewGameObj.activeSelf)
                        verseButtonsScrollViewGameObj.SetActive(true);
                    //ClearVerseBtns();
                    int firstUntypedButtonIndex = 0;
                    bool firstButtonSet = false;
                    bool typed = false;
                    for (int i = 0; i < data.Count; i++)
                    {
                        if (rememberVerses)
                        {
                            typed = versesTypedMemory.CheckIfVerseWasTyped(activeBibleId, activeBookID, activeChapter,
                                i + 1);
                            // if (typed)
                            // {
                            //     Debug.Log($"Typed: Active Bible ID et al: {activeBibleId} {activeBookID} {activeChapter}; RequestBibleVerses()");
                            // }
                        }
                        
                        
                        //Debug.Log($"Instantiating Verse Button: {activeBibleId} {bookId} {activeChapter}: {i}. Active ");
                        _buttonInstantiator.InstantiateVerseTypingButtons(data[i].verse_text, i + 1, typed);

                        if (!typed && !firstButtonSet)
                        {
                            firstUntypedButtonIndex = i;
                            firstButtonSet = true;
                        }

                        if (i >= data.Count - 1 && !firstButtonSet)
                            firstUntypedButtonIndex = 0;

                    }

                    versesButtonsScrlBar.value = 1; // REPLACE THIS WITH CALCULATION FOR SCROLL
                    
                    var firstButton = _buttonInstantiator.verseButtonsList[firstUntypedButtonIndex];
                    EventSystem.current.SetSelectedGameObject(firstButton);
                    firstButton.GetComponent<VerseButton>().InvokeVerseClickedEvent();
                    
                    ShowIdleText();

                }
                else if (_showVerseNumbers && _newLineForEachVerse)
                {
                    if (!versesScrollViewGameObj.activeSelf)
                        versesScrollViewGameObj.SetActive(true);
                    if (verseButtonsScrollViewGameObj.activeSelf)
                        verseButtonsScrollViewGameObj.SetActive(false);
                    versesText.text = "";
                    var versesTextObjPosition = versesTextObj.position;
                    versesTextObjPosition = new Vector3(versesTextObjPosition.x, 0, versesTextObjPosition.z);
                    for (int i = 0; i < data.Count; i++)
                    {
                        versesText.text = String.Concat(versesText.text, $"{i + 1} {data[i].verse_text}\n");
                    }

                    HideIdleText();

                }
                else if (_showVerseNumbers)
                {
                    if (!versesScrollViewGameObj.activeSelf)
                        versesScrollViewGameObj.SetActive(true);
                    if (verseButtonsScrollViewGameObj.activeSelf)
                        verseButtonsScrollViewGameObj.SetActive(false);
                    versesText.text = "";
                    var versesTextObjPosition = versesTextObj.position;
                    versesTextObjPosition = new Vector3(versesTextObjPosition.x, 0, versesTextObjPosition.z);
                    for (int i = 0; i < data.Count; i++)
                    {
                        versesText.text = String.Concat(versesText.text, $"{i + 1} {data[i].verse_text} ");
                    }

                    HideIdleText();

                }
                else if (_newLineForEachVerse)
                {
                    if (!versesScrollViewGameObj.activeSelf)
                        versesScrollViewGameObj.SetActive(true);
                    if (verseButtonsScrollViewGameObj.activeSelf)
                        verseButtonsScrollViewGameObj.SetActive(false);
                    versesText.text = "";
                    var versesTextObjPosition = versesTextObj.position;
                    versesTextObjPosition = new Vector3(versesTextObjPosition.x, 0, versesTextObjPosition.z);
                    for (int i = 0; i < data.Count; i++)
                    {
                        versesText.text = String.Concat(versesText.text, $"{data[i].verse_text}\n");
                    }

                    HideIdleText();

                }
                else
                {
                    if (!versesScrollViewGameObj.activeSelf)
                        versesScrollViewGameObj.SetActive(true);
                    if (verseButtonsScrollViewGameObj.activeSelf)
                        verseButtonsScrollViewGameObj.SetActive(false);
                    versesText.text = "";
                    var versesTextObjPosition = versesTextObj.position;
                    versesTextObjPosition = new Vector3(versesTextObjPosition.x, 0, versesTextObjPosition.z);
                    //versesTextObj.position = new Vector3(versesTextObj.position.x, 0, versesTextObj.position.z);
                    for (int i = 0; i < data.Count; i++)
                    {
                        versesText.text = String.Concat(versesText.text, $"{data[i].verse_text} ");
                    }

                    HideIdleText();

                }

            }

        }

    }
    
    private IEnumerator TryReplaceBibleVerses(string filesetId, string bookId, int chapter, 
        string infoTxtBible, string infoTxtBook, string vol, string recallAbbr, string sizeOfNewBible)
    {
        //Debug.Log("                              REPLACEBibleVerses Just Called using filesetId (from button) of: " + filesetId);
        //TestingProblems($"$TryReplaceBibleVerses with button-clicked fileset of {filesetId}. ");
        var getRequest =
            CreateRequest($"https://4.dbt.io/api/bibles/filesets/{filesetId}/{bookId}/{chapter}?v=4&key={sTypingText}");
        yield return getRequest.SendWebRequest();
        
        //var deserializedGetData = JsonUtility.FromJson<VerseData>(getRequest.downloadHandler.text);
        var versesData = JsonConvert.DeserializeObject<VerseData>(getRequest.downloadHandler.text);

        if (getRequest.isDone)
        {
            if (versesData.data == null)
            {
                InformationMessageTemporary("No verses found; perhaps the Bible you clicked doesn't contain the current book.", 6f, Color.yellow);
                //Debug.Log("versesData.data is null using filesetId of: " + filesetId);
            }
            else
            {
                
                activeVersesDATA = versesData;
                var data = versesData.data;

                if (data.Count > 0)
                {
                    
                    clickedFileset = filesetId;
                    clickedAbbr = recallAbbr;
                    clickedVolume = vol;
                    clickedBibleSize = sizeOfNewBible;
                                    
                    infoTextLang = _clickedLanguageAutonym;
                    infoTextBible = infoTxtBible;
                    infoTextBook = infoTxtBook;
                    infoTextChapter = chapter.ToString();
                    SetInfoText();
                    stagedBibleSize = sizeOfNewBible;
                    activeBibleSize = sizeOfNewBible;
                    SetClickedBibleText(vol);
                    
                    SetActiveDataAfterVerseDisplayed( filesetId, stagedBibleSize, clickedAbbr, stagedBibleId, clickedVolume, 
                        bookId, clickedBookName, clickedTestament, chapter, clickedBookNumberOfChapters);
                    
                    SetActiveLangAfterVerseDisplayed(_clickedLanguageIso, _clickedLanguageAutonym);
                    
                    ReplaceBookAndChapterButtons(filesetId, recallAbbr, sizeOfNewBible, vol);
                    
                    GetCopyrightInfo(filesetId);

                    
                    
                    //TestingProblems($"After replacing verses, books and copyright. ");
                }
                
                //Debug.Log("TryReplaceBibleVerses() " + data[0].book_id);

                if (_typing)
                {
                    if (versesScrollViewGameObj.activeSelf)
                        versesScrollViewGameObj.SetActive(false);
                    if (!verseButtonsScrollViewGameObj.activeSelf)
                        verseButtonsScrollViewGameObj.SetActive(true);
                    ClearVerseBtns();
                    int firstUntypedButtonIndex = 0;
                    bool firstButtonSet = false;
                    bool typed = false;
                    for (int i = 0; i < data.Count; i++)
                    {
                        if (rememberVerses)
                        {
                            typed = versesTypedMemory.CheckIfVerseWasTyped(activeBibleId, activeBookID, activeChapter,
                                i + 1);
                            // if (typed)
                            // {
                            //     Debug.Log($"Typed: Active Bible ID et al: {activeBibleId} {activeBookID} {activeChapter}; TryReplaceBibleVerses()");
                            // }
                            
                        }
                        

                        _buttonInstantiator.InstantiateVerseTypingButtons(data[i].verse_text, i + 1,
                            typed);

                        if (!typed && !firstButtonSet)
                        {
                            firstUntypedButtonIndex = i;
                            firstButtonSet = true;
                        }

                        if (i >= data.Count - 1 && !firstButtonSet)
                            firstUntypedButtonIndex = 0;

                    }
                    versesButtonsScrlBar.value = 1;
                    var firstButton = _buttonInstantiator.verseButtonsList[firstUntypedButtonIndex];
                    EventSystem.current.SetSelectedGameObject(firstButton);
                    firstButton.GetComponent<VerseButton>().InvokeVerseClickedEvent();

                    ShowIdleText();

                }
                else if (_showVerseNumbers && _newLineForEachVerse)
                {
                    if (!versesScrollViewGameObj.activeSelf)
                        versesScrollViewGameObj.SetActive(true);
                    if (verseButtonsScrollViewGameObj.activeSelf)
                        verseButtonsScrollViewGameObj.SetActive(false);
                    versesText.text = "";
                    var versesTextObjPosition = versesTextObj.position;
                    versesTextObjPosition = new Vector3(versesTextObjPosition.x, 0, versesTextObjPosition.z);
                    for (int i = 0; i < data.Count; i++)
                    {
                        versesText.text = String.Concat(versesText.text, $"{i + 1} {data[i].verse_text}\n");
                    }

                    HideIdleText();

                }
                else if (_showVerseNumbers)
                {
                    if (!versesScrollViewGameObj.activeSelf)
                        versesScrollViewGameObj.SetActive(true);
                    if (verseButtonsScrollViewGameObj.activeSelf)
                        verseButtonsScrollViewGameObj.SetActive(false);
                    versesText.text = "";
                    var versesTextObjPosition = versesTextObj.position;
                    versesTextObjPosition = new Vector3(versesTextObjPosition.x, 0, versesTextObjPosition.z);
                    for (int i = 0; i < data.Count; i++)
                    {
                        versesText.text = String.Concat(versesText.text, $"{i + 1} {data[i].verse_text} ");
                    }

                    HideIdleText();

                }
                else if (_newLineForEachVerse)
                {
                    if (!versesScrollViewGameObj.activeSelf)
                        versesScrollViewGameObj.SetActive(true);
                    if (verseButtonsScrollViewGameObj.activeSelf)
                        verseButtonsScrollViewGameObj.SetActive(false);
                    versesText.text = "";
                    var versesTextObjPosition = versesTextObj.position;
                    versesTextObjPosition = new Vector3(versesTextObjPosition.x, 0, versesTextObjPosition.z);
                    for (int i = 0; i < data.Count; i++)
                    {
                        versesText.text = String.Concat(versesText.text, $"{data[i].verse_text}\n");
                    }

                    HideIdleText();

                }
                else
                {
                    if (!versesScrollViewGameObj.activeSelf)
                        versesScrollViewGameObj.SetActive(true);
                    if (verseButtonsScrollViewGameObj.activeSelf)
                        verseButtonsScrollViewGameObj.SetActive(false);
                    versesText.text = "";
                    var versesTextObjPosition = versesTextObj.position;
                    versesTextObjPosition = new Vector3(versesTextObjPosition.x, 0, versesTextObjPosition.z);
                    //versesTextObj.position = new Vector3(versesTextObj.position.x, 0, versesTextObj.position.z);
                    for (int i = 0; i < data.Count; i++)
                    {
                        versesText.text = String.Concat(versesText.text, $"{data[i].verse_text} ");
                    }

                    HideIdleText();

                }

            }

        }

    }

    private string GetTypingText()
    {
        return GetAChapter() + GetLowValue();
    }


    private string GetABook()
    {
        return aBook;
    }
    

    public void ConvertVerses()
    {

        if (versesScrollViewGameObj.activeSelf)
        {
            versesScrollViewGameObj.SetActive(false);
            if (!verseButtonsScrollViewGameObj.activeSelf)
            {
                verseButtonsScrollViewGameObj.SetActive(true);
            }

            ClearVerseBtns();
            var data = activeVersesDATA.data;
            
            int firstUntypedButtonIndex = 0;
            bool firstButtonSet = false;
            bool typed = false;
            for (int i = 0; i < data.Count; i++)
            {
                
                if (rememberVerses)
                {
                    typed = versesTypedMemory.CheckIfVerseWasTyped(activeBibleId, activeBookID, activeChapter,
                        i + 1);
                    // if (typed)
                    // {
                    //     Debug.Log($"Typed: Active Bible ID et al: {activeBibleId} {activeBookID} {activeChapter}; ConvertVerses()");
                    // }
                }
                
                _buttonInstantiator.InstantiateVerseTypingButtons(data[i].verse_text, i + 1,
                    typed);
                
                if (!typed && !firstButtonSet)
                {
                    firstUntypedButtonIndex = i;
                    firstButtonSet = true;
                }
            }
            
            versesButtonsScrlBar.value = 1;
            ClearVerseInfoText();
            var firstButton = _buttonInstantiator.verseButtonsList[firstUntypedButtonIndex];
            EventSystem.current.SetSelectedGameObject(firstButton);
            firstButton.GetComponent<VerseButton>().InvokeVerseClickedEvent();
            ShowIdleText();
        }

        else if (verseButtonsScrollViewGameObj.activeSelf)
        {
            verseButtonsScrollViewGameObj.SetActive(false);
            if (!versesScrollViewGameObj.activeSelf)
            {
                versesScrollViewGameObj.SetActive(true);
            }

            ClearVerseBtns();

            var data = activeVersesDATA.data;


            if (_showVerseNumbers && _newLineForEachVerse)
            {

                versesText.text = "";
                var versesTextObjPosition = versesTextObj.position;
                versesTextObjPosition = new Vector3(versesTextObjPosition.x, 0, versesTextObjPosition.z);
                for (int i = 0; i < data.Count; i++)
                {
                    versesText.text = String.Concat(versesText.text, $"{i + 1} {data[i].verse_text}\n");
                }

                HideIdleText();
                ClearVerseInfoText();
            }
            else if (_showVerseNumbers)
            {

                versesText.text = "";
                var versesTextObjPosition = versesTextObj.position;
                versesTextObjPosition = new Vector3(versesTextObjPosition.x, 0, versesTextObjPosition.z);
                for (int i = 0; i < data.Count; i++)
                {
                    versesText.text = String.Concat(versesText.text, $"{i + 1} {data[i].verse_text} ");
                }

                HideIdleText();
                ClearVerseInfoText();
            }
            else if (_newLineForEachVerse)
            {

                versesText.text = "";
                var versesTextObjPosition = versesTextObj.position;
                versesTextObjPosition = new Vector3(versesTextObjPosition.x, 0, versesTextObjPosition.z);
                for (int i = 0; i < data.Count; i++)
                {
                    versesText.text = String.Concat(versesText.text, $"{data[i].verse_text}\n");
                }

                HideIdleText();
                ClearVerseInfoText();
            }
            else
            {

                versesText.text = "";
                var versesTextObjPosition = versesTextObj.position;
                versesTextObjPosition = new Vector3(versesTextObjPosition.x, 0, versesTextObjPosition.z);
                //versesTextObj.position = new Vector3(versesTextObj.position.x, 0, versesTextObj.position.z);
                for (int i = 0; i < data.Count; i++)
                {
                    versesText.text = String.Concat(versesText.text, $"{data[i].verse_text} ");
                }

                HideIdleText();
                ClearVerseInfoText();
            }

        }

    }

    public void ReloadTypingVerses()
    {
        if (activeVersesDATA != null)
        {
            if (activeVersesDATA.data != null)
            {
                if (!verseButtonsScrollViewGameObj.activeSelf)
                {
                    verseButtonsScrollViewGameObj.SetActive(true);
                }
                ClearVerseBtns();
            
                var data = activeVersesDATA.data;
        
                int firstUntypedButtonIndex = 0;
                bool firstButtonSet = false;
                bool typed = false;
                for (int i = 0; i < data.Count; i++)
                {
            
                    if (rememberVerses)
                    {
                        typed = versesTypedMemory.CheckIfVerseWasTyped(activeBibleId, activeBookID, activeChapter,
                            i + 1);
                    }
            
                    _buttonInstantiator.InstantiateVerseTypingButtons(data[i].verse_text, i + 1,
                        typed);
            
                    if (!typed && !firstButtonSet)
                    {
                        firstUntypedButtonIndex = i;
                        firstButtonSet = true;
                    }
                }
        
                versesButtonsScrlBar.value = 1;
                ClearVerseInfoText();
                var firstButton = _buttonInstantiator.verseButtonsList[firstUntypedButtonIndex];
                EventSystem.current.SetSelectedGameObject(firstButton);
                firstButton.GetComponent<VerseButton>().InvokeVerseClickedEvent();
                ShowIdleText();
            }
        }

    }

    public void RecallReadingVerses()
    {
        if (versesScrollViewGameObj.activeSelf)
        {
            var data = activeVersesDATA.data;
        
            if (_showVerseNumbers && _newLineForEachVerse)
            {
                versesText.text = "";
                var versesTextObjPosition = versesTextObj.position;
                versesTextObjPosition = new Vector3(versesTextObjPosition.x, 0, versesTextObjPosition.z);
                for (int i = 0; i < data.Count; i++)
                {
                    versesText.text = String.Concat(versesText.text, $"{i + 1} {data[i].verse_text}\n");
                }

                HideIdleText();
                ClearVerseInfoText();
            }
            else if (_showVerseNumbers)
            {

                versesText.text = "";
                var versesTextObjPosition = versesTextObj.position;
                versesTextObjPosition = new Vector3(versesTextObjPosition.x, 0, versesTextObjPosition.z);
                for (int i = 0; i < data.Count; i++)
                {
                    versesText.text = String.Concat(versesText.text, $"{i + 1} {data[i].verse_text} ");
                }

                HideIdleText();
                ClearVerseInfoText();
            }
            else if (_newLineForEachVerse)
            {

                versesText.text = "";
                var versesTextObjPosition = versesTextObj.position;
                versesTextObjPosition = new Vector3(versesTextObjPosition.x, 0, versesTextObjPosition.z);
                for (int i = 0; i < data.Count; i++)
                {
                    versesText.text = String.Concat(versesText.text, $"{data[i].verse_text}\n");
                }

                HideIdleText();
                ClearVerseInfoText();
            }
            else
            {

                versesText.text = "";
                var versesTextObjPosition = versesTextObj.position;
                versesTextObjPosition = new Vector3(versesTextObjPosition.x, 0, versesTextObjPosition.z);
                //versesTextObj.position = new Vector3(versesTextObj.position.x, 0, versesTextObj.position.z);
                for (int i = 0; i < data.Count; i++)
                {
                    versesText.text = String.Concat(versesText.text, $"{data[i].verse_text} ");
                }

                HideIdleText();
                ClearVerseInfoText();
            }
        }
        
    }
    
    
    int inputFieldOnValueCngCounter;
    public void ReadInputFieldTextOnChange(string s)
    {
        if (useInputFieldVerseTypingButtons)
        {
            Actions.OnChangeInputField(s);
        }
        
    }

    public string GetInputFieldPreText()
    {
        return inputFieldPreText.text;
    }

    public void ClearInputFieldPreText()
    {
        inputFieldPreText.text = String.Empty;
    }
    

public void ClearVersesText()
    {
        versesText.text = "";
    }

    public void ClearVerseInfoText()
    {
        verseInfoText.text = "";
    }

    public void ClearVerseBtns()
    {
        _buttonInstantiator.ClearVerseButtons();
        _buttonInstantiator.ParkInputField();
    }

    public void UpdateWpm(float wpm)
    {
        UpdateAveWpmText(wpm);
        UpdateMaxWpmText(wpm);
        UpdateWpmSlider(wpm);
    }

    private string GetTypingSpeed()
    {
        var typeSpeedString = GetHighValue();
        return typeSpeedString.Substring(11);
    }
    
    public void UpdateMaxWpmText(float wpm)
    {
        if (wpm > maxWpmValue)
        {
            maxWpmValue = wpm;
            SteamworksHelper.maxWpm = (int)wpm;
        }
        maxWpm.text = maxWpmValue.ToString("0");
    }

    public void SaveWpmData()
    {
        var settings = new ES3Settings(ES3.EncryptionType.AES, sScore);
        ES3.Save("maxWpmValue", maxWpmValue, settings);
        ES3.Save("numberOfWpmToAve", numberOfWpmToAve, settings);
        ES3.Save("aveWpmValue", aveWpmValue, settings);
    }
    
    public void UpdateAveWpmText(float wpm)
    {
        numberOfWpmToAve++;
        aveWpmValue = ((aveWpmValue * numberOfWpmToAve) + wpm) / (numberOfWpmToAve + 1);
        if (numberOfWpmToAve > int.MaxValue - 1000)
            numberOfWpmToAve = 50000;
        aveWpm.text = aveWpmValue.ToString("0");
        SteamworksHelper.aveWpm = (int)aveWpmValue;
        SteamworksHelper.numberOfWpmInAve = numberOfWpmToAve;
    }

    public void UpdateWpmSlider(float wpm)
    {
        if (UsingOldUi)
        {
            wpmText.text = wpm.ToString("0");
        }
        else
        {
            wpmTextNewUi.text = wpm.ToString("0");
        }
        
        wpmSlider.value = wpm / 100;
    }

    public void IncrementCharactersTyped(int chars)
    {
        numberOfCharsTyped += chars;
        ES3.Save("numberOfCharsTyped", numberOfCharsTyped);
        if (numberOfCharsTyped > 999999999)
        {
            numOfCharsExceedsBillionMultiplier++;
            ES3.Save("numOfCharsExceedsMaxIntMultiplier", numOfCharsExceedsBillionMultiplier);
            numberOfCharsTyped = 1;
        }
    }

    public void IncrementVersesTyped()
    {
        numberOfVersesTyped++;
        ES3.Save("numberOfVersesTyped", numberOfVersesTyped);
        if (numberOfVersesTyped > 999999999)
        {
            numOfVersesExceedsBillionMultiplier++;
            ES3.Save("numOfVersesExceedsMaxIntMultiplier", numOfVersesExceedsBillionMultiplier);
            numberOfVersesTyped = 1;
        }
    }

    public void UpdateVersesCharsTypedText()
    {
        if (numOfCharsExceedsBillionMultiplier > 0 && numOfVersesExceedsBillionMultiplier > 0)
        {
            CharsVersesTypedText.text = $"Verses typed: 1 Billion * {numOfVersesExceedsBillionMultiplier} plus {numberOfVersesTyped}  |  Chars typed: 1 Billion * {numOfCharsExceedsBillionMultiplier} plus {numberOfCharsTyped}";
        }
        else if (numOfCharsExceedsBillionMultiplier > 0)
        {
            CharsVersesTypedText.text = $"Verses typed:  {numberOfVersesTyped}  |  Chars typed: 1 Billion *  {numOfCharsExceedsBillionMultiplier} plus {numberOfCharsTyped}";
        }
        else if (numOfVersesExceedsBillionMultiplier > 0)
        {
            CharsVersesTypedText.text = $"Verses typed: 1 Billion *  {numOfVersesExceedsBillionMultiplier} plus {numberOfVersesTyped}  |  Chars typed: {numberOfCharsTyped}";
        }
        else
        {
            CharsVersesTypedText.text = $"Verses typed: {numberOfVersesTyped}  |  Chars typed: {numberOfCharsTyped}";
        }
    }

private IEnumerator TestCopyright(string filesetId)
    {
        filesetId = "";
        ClearVersesText();
        var getRequest = CreateRequest($"https://4.dbt.io/api/bibles/filesets/{filesetId}/copyright?v=4&key={sTypingText}"); 
        
        yield return getRequest.SendWebRequest();
        
        var copyrightData = JsonConvert.DeserializeObject<CopyrightData>(getRequest.downloadHandler.text, new JsonSerializerSettings{NullValueHandling = NullValueHandling.Ignore});

        if (getRequest.isDone)
        {
            if (copyrightData == null)
            {
                Debug.Log("Sorry, there was an error retrieving copyright data");
            }
            else
            {
                
                if (copyrightData.id != null)
                    Debug.Log(copyrightData.id);
                if (copyrightData.asset_id != null)
                    Debug.Log(copyrightData.asset_id);
                if (copyrightData.type != null)
                    Debug.Log(copyrightData.type);
                if (copyrightData.size != null)
                    Debug.Log(copyrightData.size);
                
                if (copyrightData.copyright.copyright_date != null)
                    Debug.Log(copyrightData.copyright.copyright_date);
                if (copyrightData.copyright.copyright != null)
                    Debug.Log(copyrightData.copyright.copyright);
                if (copyrightData.copyright.copyright_description != null)
                    Debug.Log(copyrightData.copyright.copyright_description);
                if (copyrightData.copyright.created_at != null)
                    Debug.Log(copyrightData.copyright.created_at);
                if (copyrightData.copyright.updated_at != null)
                    Debug.Log(copyrightData.copyright.updated_at);
                
                Debug.Log(copyrightData.copyright.open_access);

                if (copyrightData.copyright.organizations != null)
                {
                    for (int i = 0; i < copyrightData.copyright.organizations.Count; i++)
                    {
                        Debug.Log($"Copyright.Organizations[{i}]");
                        var org = copyrightData.copyright.organizations[i];
                        
                        Debug.Log(org.id);
                        if (org.slug != null)
                            Debug.Log(org.slug);
                        if (org.abbreviation != null)
                            Debug.Log(org.abbreviation);
                        if (org.primaryColor != null)
                            Debug.Log(org.primaryColor);
                        if (org.secondaryColor != null)
                            Debug.Log(org.secondaryColor);
                        
                        Debug.Log(org.inactive);
                        if (org.url_facebook != null)
                            Debug.Log(org.url_facebook);
                        if (org.url_website != null)
                            Debug.Log(org.url_website);
                        if (org.url_donate != null)
                            Debug.Log(org.url_donate);
                        if (org.url_twitter != null)
                            Debug.Log(org.url_twitter);
                        if (org.address != null)
                            Debug.Log(org.address);
                        if (org.address2 != null)
                            Debug.Log(org.address2);
                        if (org.city != null)
                            Debug.Log(org.city);
                        if (org.state != null)
                            Debug.Log(org.state);
                        if (org.country != null)
                            Debug.Log(org.country);
                        
                        Debug.Log(org.zip);
                        if (org.phone != null)
                            Debug.Log(org.phone);
                        if (org.email != null)
                            Debug.Log(org.email);
                        if (org.email_director != null)
                            Debug.Log(org.email_director);
                        Debug.Log(org.latitude);
                        Debug.Log(org.longitude);
                        if (org.laravel_through_key != null)
                            Debug.Log(org.laravel_through_key);


                        for (int j = 0; j < org.logos.Count; j++)
                        {
                            var logos = org.logos[j];
                            
                            Debug.Log(logos.language_id);
                            if (logos.language_iso != null)
                                Debug.Log(logos.language_iso);
                            if (logos.url != null)
                                Debug.Log(logos.url);
                            Debug.Log(logos.icon);
                        }

                        for (int j = 0; j < org.translations.Count; j++)
                        {
                            var translations = org.translations[j];
                            
                            Debug.Log(translations.language_id);
                            Debug.Log(translations.vernacular);
                            Debug.Log(translations.alt);
                            if (translations.name != null)
                                Debug.Log(translations.name);
                            if (translations.description_short != null)
                                Debug.Log(translations.description_short);
                        }
                        
                        
                    }
                }
                
                
                
            }
            
        }

    }
    
    
    private IEnumerator RequestCopyright(string filesetId)
    {
        
        var getRequest = CreateRequest($"https://4.dbt.io/api/bibles/filesets/{filesetId}/copyright?v=4&key={sTypingText}"); 
        
        yield return getRequest.SendWebRequest();
        
        var copyrightData = JsonConvert.DeserializeObject<CopyrightData>(getRequest.downloadHandler.text, 
            new JsonSerializerSettings{NullValueHandling = NullValueHandling.Ignore});

        if (getRequest.isDone)
        {
            if (copyrightData == null)
            {
                
            }
            else
            {
                if (copyrightData.copyright != null)
                {
                    copyrightText.text = "";
                
                    string date = "";
                    string copyright = "";
                
                    if (copyrightData.copyright.copyright_date != null)
                    {
                        date = copyrightData.copyright.copyright_date;
                    }

                    if (copyrightData.copyright.copyright != null)
                    {
                        copyright = copyrightData.copyright.copyright;
                    }

                    if (copyrightData.copyright.organizations != null)
                    {
                        copyrightText.text = $"Copyright information: {date} {copyright}. Organization(s): ";
                        foreach (var org in copyrightData.copyright.organizations)
                        {
                            string slug = "";
                            string website = "";

                            if (org.slug != null)
                            {
                                slug = org.slug;
                            }

                            if (org.url_website!= null)
                            {
                                website = org.url_website;
                            }
                    
                            copyrightText.text = String.Concat(copyrightText.text, $"{slug}: {website} ");
                        }
                    }
                }
            }
            
        }

    }

    public void ClearCopyrightText()
    {
        copyrightText.text = "";
    }
    
    
    
    private IEnumerator SampleCreateRequest(string TEMP)
    {
        
        var getRequest = CreateRequest("https://4.dbt.io/api/bibles/" + TEMP + "/book?v=4&key=" + sTypingText);
        
        yield return getRequest.SendWebRequest();
        
        var bookData = JsonConvert.DeserializeObject<BookData>(getRequest.downloadHandler.text);
        

        if (getRequest.isDone)
        {
            

        }

    }
    
    private string GetNewLow()
    {
        return low;
    }
    
    private UnityWebRequest CreateRequest(string path) //, RequestType type = RequestType.GET, object data = null)
    {
        var request = new UnityWebRequest(path);  //, type.ToString());
        
        // if (data !null)
        // {
        //     var bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
        //     request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        // }

        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        return request;
    }

    private void AttachHeader(UnityWebRequest request, string key, string value)
    {
        request.SetRequestHeader(key, value);
    }
    
    public enum RequestType
    {
        Get = 0,
        Post = 1,
        Put = 2
    }

    private int GetAVerse()
    {
        return aVerse;
    }
    
    public void SetVerseAsTyped()
    {
        Debug.Log($"{activeBibleId} {activeBookID} {activeChapter} {activeVerse}");   
        if (rememberVerses)
        {
            versesTypedMemory.AddTypedVerseToBiblesDictionary(activeBibleId, activeBookID, activeChapter, activeVerse);
            Debug.Log(versesTypedMemory.CheckIfVerseWasTyped(activeBibleId, activeBookID, activeChapter, activeVerse));
        }
    }

    public bool CheckVerseIsTyped(string bibleID, string book, int chapter, int verse)
    {
        return versesTypedMemory.CheckIfVerseWasTyped(bibleID, book, chapter, verse);
    }
    
    public class PostData
    {
        public string Hero;
        public int PowerLevel;
    }

    public class PostResult
    {
        public string success { get; set; }
    }

    public void SetClickedLanguageText(string lang)
    {
        bibleManager.UpdateClickedLangText(lang);
    }

    public void SetClickedBibleText(string bible)
    {
        bibleManager.UpdateClickedBibleText(bible);
    }

    public void SetClickedChapterText(string chapter)
    {
        bibleManager.UpdateClickedChapterText(chapter);
    }

    public void SetClickedBookText(string book)
    {
        bibleManager.UpdateClickedBookText(book);
    }

    
}
