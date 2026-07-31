using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Settings : MonoBehaviour, IDataPersistence
{
    public string SaveFileName;
    [SerializeField]
    private TMP_Dropdown resolusionDropdown;
    [SerializeField]
    private Toggle fullScreenToggle;
    [SerializeField]
    private Toggle vSyncToggle;
    [SerializeField]
    private TMP_InputField maxFpsInputField;
    [SerializeField]
    private TMP_Dropdown colorBlindDropdown;
    public Material colorBlindMaterial;
    [SerializeField]
    private AudioMixer audioMixer;
    [SerializeField]
    private Slider masterSlider;
    [SerializeField]
    private Slider SFXSlider;
    [SerializeField]
    private Slider musicSlider;
    [SerializeField]
    private Button backButton;
    [SerializeField]
    private Button enterButton;

    private RefreshRate maxFps;
    private int width, height;
    private int isVSyncOn = 1;
    private FullScreenMode isInFullScreen;
    private int masterVolume, SFXVolume, musicVolume;
    private LocalKeyword none;
    private LocalKeyword tritanopia;
    private LocalKeyword protanopia;
    private LocalKeyword deuteranopia;

    public static Settings instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        DataPersistenceManager.instance.dataPersistenceObjects.Add(instance);
    }

    private void Start()
    {
        
        none = new LocalKeyword(colorBlindMaterial.shader, "_COLORBLINDMODE_NONE");
        tritanopia = new LocalKeyword(colorBlindMaterial.shader, "_COLORBLINDMODE_TRITANOPIA");
        protanopia = new LocalKeyword(colorBlindMaterial.shader, "_COLORBLINDMODE_PROTANOPIA");
        deuteranopia = new LocalKeyword(colorBlindMaterial.shader, "_COLORBLINDMODE_DEUTERANOPIA");
        colorBlindMaterial.SetKeyword(none, true);
        width = Screen.width; 
        height = Screen.height;
        maxFps.numerator = 0;
        maxFps.denominator = 1;
        isInFullScreen = FullScreenMode.FullScreenWindow;
        resolusionDropdown.onValueChanged.AddListener(delegate { onResolutionChange(); });
        fullScreenToggle.onValueChanged.AddListener(delegate { onFullScreenChange(); });
        vSyncToggle.onValueChanged.AddListener (delegate { onVSyncChange(); });
        maxFpsInputField.onEndEdit.AddListener(delegate { onMaxFPSChange(); });
        colorBlindDropdown.onValueChanged.AddListener(delegate { onColorBlindChange(); });
        masterSlider.onValueChanged.AddListener(delegate { onMasterVolumeChange(); });
        SFXSlider.onValueChanged.AddListener(delegate { onSFXVolumeChange(); });
        musicSlider.onValueChanged.AddListener(delegate { onMusicVolumeChange(); });
        backButton.onClick.AddListener(delegate { saveOnBack(); });
        enterButton.onClick.AddListener(delegate { onLoad(); });
        DataPersistenceManager.instance.LoadGameFor("appSettings");
    }


    void Update()
    {
        
        QualitySettings.vSyncCount = isVSyncOn;
        Application.targetFrameRate = (int)maxFps.value;
        Screen.SetResolution(width,height,isInFullScreen);
        Debug.Log(1f / Time.unscaledDeltaTime);
    }

    void saveOnBack()
    {
        DataPersistenceManager.instance.SaveGameFor("appSettings");
    }
    void onLoad()
    {
        DataPersistenceManager.instance.LoadGameFor("appSettings");
    }
    
    void onResolutionChange()
    {
        if (resolusionDropdown.value == 0)
        { width = 640; height = 480; }
        if (resolusionDropdown.value == 1)
        { width = 720; height = 400; }
        if (resolusionDropdown.value == 2)
        { width = 720; height = 480; }
        if (resolusionDropdown.value == 3)
        { width = 800; height = 600; }
        if (resolusionDropdown.value == 4)
        { width = 1024; height = 768; }
        if (resolusionDropdown.value == 5)
        { width = 1128; height = 634; }
        if (resolusionDropdown.value == 6)
        { width = 1150; height = 864; }
        if (resolusionDropdown.value == 7)
        { width = 1280; height = 720; }
        if (resolusionDropdown.value == 8)
        { width = 1280; height = 960; }
        if (resolusionDropdown.value == 9)
        { width = 1280; height = 1024; }
        if (resolusionDropdown.value == 10)
        { width = 1366; height = 768; }
        if (resolusionDropdown.value == 11)
        { width = 1440; height = 900; }
        if (resolusionDropdown.value == 12)
        { width = 1600; height = 900; }
        if (resolusionDropdown.value == 13)
        { width = 1600; height = 1200; }
        if (resolusionDropdown.value == 14)
        { width = 1680; height = 1050; }
        if (resolusionDropdown.value == 15)
        { width = 1760; height = 990; }
        if (resolusionDropdown.value == 16)
        { width = 1920; height = 1080; }
        if (resolusionDropdown.value == 17)
        { width = 1920; height = 1200; }
        if (resolusionDropdown.value == 18)
        { width = 2560; height = 1440; }
    }

    void onFullScreenChange()
    {
        if (fullScreenToggle.isOn)
            isInFullScreen = FullScreenMode.FullScreenWindow;
        else
            isInFullScreen = FullScreenMode.Windowed;
    }

    void onVSyncChange()
    {
        if (vSyncToggle.isOn)
        {
            isVSyncOn = 1;
            maxFpsInputField.gameObject.SetActive(false);
        }
        else
        {
            isVSyncOn = 0;
            maxFpsInputField.gameObject.SetActive(true);
        }
    
    }

    void onMaxFPSChange()
    {
        uint fps = 0;
        if(uint.TryParse(maxFpsInputField.text, out fps))
            maxFps.numerator = fps;
    }

    void onColorBlindChange()
    {
        colorBlindMaterial.SetKeyword(none, false);
        colorBlindMaterial.SetKeyword(tritanopia, false);
        colorBlindMaterial.SetKeyword(protanopia, false);
        colorBlindMaterial.SetKeyword(deuteranopia, false);
        if (colorBlindDropdown.value == 0)
            colorBlindMaterial.SetKeyword(none, true);
        if (colorBlindDropdown.value == 1)
            colorBlindMaterial.SetKeyword(tritanopia, true);
        if (colorBlindDropdown.value == 2)
            colorBlindMaterial.SetKeyword(protanopia, true);
        if (colorBlindDropdown.value == 3)
            colorBlindMaterial.SetKeyword(deuteranopia, true);

    }

    void onMasterVolumeChange()
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterSlider.value / 100f) * 20f);
    }
    void onSFXVolumeChange()
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(SFXSlider.value / 100f) * 20f);
    }
    void onMusicVolumeChange()
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicSlider.value / 100f) * 20f);
    }

    public void LoadGame(IGameData data)
    {
        SettingsData tmp = data as SettingsData;
        if (tmp != null)
        {
            resolusionDropdown.value = tmp.resolution;
            fullScreenToggle.isOn = tmp.fullscreen;
            vSyncToggle.isOn = tmp.Vsync;
            maxFps.numerator = (uint)tmp.maxfps;
            colorBlindDropdown.value = tmp.colorBlindOption;
            masterSlider.value = tmp.masterVolume;
            SFXSlider.value = tmp.SFXVolume;
            musicSlider.value = tmp.musicVolume;
        }
        onResolutionChange();
        onFullScreenChange();
        onVSyncChange();
        onMaxFPSChange();
        onColorBlindChange();
        onMasterVolumeChange();
        onSFXVolumeChange();
        onMusicVolumeChange();
    }

    public void SaveGame(IGameData data)
    {
        SettingsData tmp = data as SettingsData;
        if (tmp != null)
        {
            tmp.resolution = resolusionDropdown.value;
            tmp.fullscreen = fullScreenToggle.isOn;
            tmp.Vsync = vSyncToggle.isOn;
            tmp.maxfps = (int)maxFps.numerator;
            tmp.colorBlindOption = colorBlindDropdown.value;
            tmp.masterVolume = masterSlider.value;
            tmp.SFXVolume = SFXSlider.value;
            tmp.musicVolume = musicSlider.value;
        }

    }

    public string getDataFileName()
    {
        return SaveFileName;
    }
}
