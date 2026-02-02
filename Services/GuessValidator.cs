namespace pokemonApi.Services;

/// <summary>
/// Implements Wordle validation logic using a two-pass algorithm to handle duplicate letters correctly.
/// Pass 1: Mark exact position matches. Pass 2: Mark wrong position matches for remaining letters.
/// </summary>
public class GuessValidator : IGuessValidator
{
    public List<LetterResult> ValidateGuess(string guessedWord, string answerWord)
    {
        if (string.IsNullOrEmpty(guessedWord) || string.IsNullOrEmpty(answerWord))
            throw new ArgumentException("Words cannot be null or empty.");
        
        if (guessedWord.Length != answerWord.Length)
            throw new ArgumentException("Words must be the same length.");
        
        guessedWord = guessedWord.ToUpper();
        answerWord = answerWord.ToUpper();
        
        var results = new List<LetterResult>(guessedWord.Length);
        var answerLetters = new List<char>(answerWord.ToCharArray());
        var statusArray = new LetterStatus[guessedWord.Length];
        
        // Initialize all positions to NotInWord (will be overwritten if found)
        for (int i = 0; i < statusArray.Length; i++)
        {
            statusArray[i] = LetterStatus.NotInWord;
        }
        
        // Pass 1: Exact matches (green)
        for (int i = 0; i < guessedWord.Length; i++)
        {
            if (guessedWord[i] == answerWord[i])
            {
                statusArray[i] = LetterStatus.Correct;
                answerLetters[i] = '\0'; // Mark as used
            }
        }
        
        // Pass 2: Wrong positions (yellow)
        for (int i = 0; i < guessedWord.Length; i++)
        {
            if (statusArray[i] == LetterStatus.Correct)
                continue; // Already matched exactly
            
            int answerIndex = answerLetters.IndexOf(guessedWord[i]);
            if (answerIndex != -1)
            {
                statusArray[i] = LetterStatus.WrongPosition;
                answerLetters[answerIndex] = '\0'; // Mark as used
            }
            // else: remains NotInWord (set in initialization)
        }
        
        // Build result list
        for (int i = 0; i < guessedWord.Length; i++)
        {
            results.Add(new LetterResult(guessedWord[i], statusArray[i]));
        }
        
        return results;
    }
}
