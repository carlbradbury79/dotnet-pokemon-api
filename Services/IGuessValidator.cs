namespace pokemonApi.Services;

public interface IGuessValidator
{
    /// <summary>Validates a guess against the answer and returns letter statuses.</summary>
    List<LetterResult> ValidateGuess(string guessedWord, string answerWord);
}

public record LetterResult(char Letter, LetterStatus Status);

public enum LetterStatus
{
    Correct,        // Green - correct position
    WrongPosition,  // Yellow - in word, wrong position
    NotInWord       // Gray - not in word
}
