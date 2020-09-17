using UnityEngine;
using UnityEngine.UI;

public class MainScript : MonoBehaviour
{
    // Prompt to the user
    private string prompt = "";
    // The prompt box
    public Text textBox;
    // The input box
    public InputField inputBox;

    // Complete the code below
    //1. Make menuHint a const variable DONE
    const string menuHint = "You may type menu at any time to go back to the main menu.";

    // Complete the code below
    //2. Create 3 string arrays for 3 levels DONE
    //level 1: 10 words; Level 2: 8 words; Level 3: 6 words
    string[] level1Words = { "easy", "word", "back", "bat", "dad", "desk", "hit", "duck", "Zoom", "Boom" };
    string[] level2Words = { "crack", "input", "House", "Horse", "adult", "after", "ahead", "agree" };
    string[] level3Words = { "ability", "academy", "account", "achieve", "actions", "affairs" };

    // Game state
    [SerializeField] int level;
    [SerializeField] string selWord;

    // Different screens for the game   
    enum Screen { MainMenu, CheckingWord, Win };
    // Default screen is MainMenu. "currentScreen" is used to keep the current game status.
    [SerializeField] Screen currentScreen = Screen.MainMenu;

    void Start()
    {
        ShowMainMenu("Welcome to word scrabble game!");
    }
    void ShowMainMenu(string greeting)
    {
        currentScreen = Screen.MainMenu;
        prompt = "";
        prompt += greeting + "\n";
        prompt += "Which level do you want to play?\n\n";
        prompt += "Press 1 for easy\n";
        prompt += "Press 2 for medium\n";
        prompt += "Press 3 for difficult\n\n";
        prompt += "Enter your selection\n";

        //Display the prompt to the user
        textBox.GetComponent<Text>().text = prompt;
    }

    public void OnUserInput()
    {
        // Get User Input
        string input = inputBox.text.ToLower();
        // Clear the inputbox
        inputBox.text = "";
        // SetFocus to the inputbox
        inputBox.ActivateInputField();
        //Complete the code below with conditional statements
        //3. if user input "exit" or "quit", quit the game. DONE
        //   if input equals to "menu", show the main menu
        //   if input doesn't equal to "menu" and currentScreen is MainMenu, call RunMainMenu 
        //   if input doesn't equal to "menu" and currentScreen is CheckingWord, call CheckWord  
        if (input == "quit" || input == "exit")
		{
            Application.Quit();
		} else if ( input == "menu")
		{
            ShowMainMenu("Welcome.");
		} else if ( currentScreen == Screen.MainMenu )
		{
            RunMainMenu(input);
		} else if (currentScreen == Screen.CheckingWord)
        {
            CheckWord(input);
        } else
		{
            Debug.LogError("Current screen is not currently known, or input invalid.");
		}

    }
    private void RunMainMenu(string input)
    {
        bool isValidLevelNumber = (input == "1" || input == "2" || input == "3");
        if (isValidLevelNumber)
        {
            level = int.Parse(input);
            AskforWord();
        }
        else
        {
            prompt += "Invalid input, try again\n";
            prompt += menuHint + "\n";
            textBox.GetComponent<Text>().text = prompt;
        }
    }
    void AskforWord()
    {
        currentScreen = Screen.CheckingWord;
        prompt = "";
        SetRandomWord();
        prompt += "Enter your word, hint: " + Anagram(selWord) + "\n";
        prompt += menuHint + "\n";
        textBox.GetComponent<Text>().text = prompt;
    }

    //Complete the code for the "Anagram" method 
    //4. Scramble the word. You must make the scramble random to get full points.  DONE
    string Anagram(string inWord)
    {
        string res = "";

        // Your code here
        char[] wordArray = inWord.ToCharArray();
        int wordLength = wordArray.Length;
        Debug.Log("The word is " + inWord);
		for (int i = 0; i < wordLength; i++)
		{
            int randomSpot = i;
            do
            {
                randomSpot = Random.Range(0, wordLength);
            } while (randomSpot == i);
            char temp = wordArray[i];
            wordArray[i] = wordArray[randomSpot];
            wordArray[randomSpot] = temp;
        }

        for (int i = 0; i < wordLength; i++)
        {
            res += wordArray[i];
        }
        return res;
    }

    //Complete the code for the "SetRandomWord" method 
    //5. Randomly choose a word based on level and assign it to "selWord".  DONE
    //   You must use switch statement to get full points.
    void SetRandomWord()
    {
        // Your code here
        switch ( level)
		{
            case 1:
                selWord = level1Words[Random.Range(0,level1Words.Length)];
                break;

            case 2:
                selWord = level2Words[Random.Range(0, level2Words.Length)];
                break;

            case 3:
                selWord = level3Words[Random.Range(0, level3Words.Length)];
                break;

            default:
                selWord = level1Words[Random.Range(0, level1Words.Length)];
                break;
        }
    }

    void CheckWord(string input)
    {
        if (input == selWord)
        {

            DisplayWinScreen();
        }
        else
        {
            AskforWord();
        }
    }

    void DisplayWinScreen()
    {
        currentScreen = Screen.Win;
        prompt = "";
        ShowLevelReward();
        prompt += menuHint + "\n";
        textBox.GetComponent<Text>().text = prompt;
    }

    //Complete the code for the "ShowLevelReward" method 
    //6. Display different reward messages to the user based on levels. DONE
    //  Be creative and have fun.
    void ShowLevelReward()
    {
        switch (level)
        {
            case 1:
                // Your code here
                prompt = "Congrats on beating level 1! ";
                break;
            case 2:
                // Your code here
                prompt = "Good job you beat level 2! ";
                break;
            case 3:
                // Your code here
                prompt = "You beat the hardest level! ";
                break;
            default:
                Debug.LogError("Invalid input!");
                break;
        }
    }
}

