using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuScript : MonoBehaviour
{
    private UIDocument _document;
    private Button _startButton;
    private Button _previousButton;
    private Button _nextButton;
    public int SelectedCharacterIndex;

    private GameObject _selectedCharacter;

    private void Awake()
    {
        _document = GetComponent<UIDocument>();

        _startButton = _document.rootVisualElement.Q("StartButton") as Button;
        _startButton.RegisterCallback<ClickEvent>(OnClickStart);

        _nextButton = _document.rootVisualElement.Q("NextButton") as Button;
        _nextButton.RegisterCallback<ClickEvent>(OnClickNextCharacter);

        _previousButton = _document.rootVisualElement.Q("PreviousButton") as Button;
        _previousButton.RegisterCallback<ClickEvent>(OnClickPreviousCharacter);

        if (_selectedCharacter)
            GameObject.Find("CharacterSpawner").GetComponent<CharacterSpawner>().DestroyCharacter(_selectedCharacter.GetInstanceID());
        _selectedCharacter = GameObject.Find("CharacterSpawner").GetComponent<CharacterSpawner>().SpawnCharacter(SelectedCharacterIndex);

        DontDestroyOnLoad(gameObject);
    }

    private void OnDisable()
    {
        _startButton.UnregisterCallback<ClickEvent>(OnClickStart);
        _nextButton.UnregisterCallback<ClickEvent>(OnClickNextCharacter);
        _previousButton.UnregisterCallback<ClickEvent>(OnClickPreviousCharacter);
    }

    private void OnClickStart(ClickEvent evt)
    {
        SceneManager.LoadScene("GameScene");
        SceneManager.sceneLoaded += OnGameSceneLoaded;

        GetComponent<UIDocument>().enabled = false;
    }

    private void OnGameSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            //Camera.main.enabled = false;
            GameObject character = GameObject.Find("CharacterSpawner").GetComponent<CharacterSpawner>().SpawnCharacter(SelectedCharacterIndex);
            character.transform.GetChild(0).GetChild(0).GetComponent<Camera>().enabled = true;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("GameScene"));
            SceneManager.sceneLoaded -= OnGameSceneLoaded;
        }
    }

    private void OnClickNextCharacter(ClickEvent evt)
    {
        int charactersCount = GameObject.Find("CharacterSpawner").GetComponent<CharacterSpawner>().AvailableCharacters.Count;
        SelectedCharacterIndex++;
        if (SelectedCharacterIndex > charactersCount - 1)
            SelectedCharacterIndex = 0;

        if (_selectedCharacter)
            GameObject.Find("CharacterSpawner").GetComponent<CharacterSpawner>().DestroyCharacter(_selectedCharacter.GetInstanceID());
        _selectedCharacter = GameObject.Find("CharacterSpawner").GetComponent<CharacterSpawner>().SpawnCharacter(SelectedCharacterIndex);
    }

    private void OnClickPreviousCharacter(ClickEvent evt)
    {
        int charactersCount = GameObject.Find("CharacterSpawner").GetComponent<CharacterSpawner>().AvailableCharacters.Count;
        SelectedCharacterIndex--;
        if (SelectedCharacterIndex < 0)
            SelectedCharacterIndex = charactersCount - 1;

        if (_selectedCharacter)
            GameObject.Find("CharacterSpawner").GetComponent<CharacterSpawner>().DestroyCharacter(_selectedCharacter.GetInstanceID());
        _selectedCharacter = GameObject.Find("CharacterSpawner").GetComponent<CharacterSpawner>().SpawnCharacter(SelectedCharacterIndex);
    }
}
