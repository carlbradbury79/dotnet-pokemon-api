using System;
using System.Linq;
using pokemonApi.Services;
using Xunit;

namespace pokemonApi.UnitTests.Services;

public class GuessValidatorTests
{
    private readonly IGuessValidator _validator = new GuessValidator();

    #region Basic Cases

    [Theory]
    [InlineData("PIKACHU", "PIKACHU")]
    [InlineData("ABCDE", "ABCDE")]
    [InlineData("HELLO", "HELLO")]
    public void ValidateGuess_AllCorrect_ReturnsAllGreen(string guess, string answer)
    {
        var result = _validator.ValidateGuess(guess, answer);

        Assert.All(result, letterResult => Assert.Equal(LetterStatus.Correct, letterResult.Status));
    }

    [Theory]
    [InlineData("ABCDE", "FGHIJ")]
    [InlineData("XXXXX", "YYYYY")]
    public void ValidateGuess_NoMatches_ReturnsAllGray(string guess, string answer)
    {
        var result = _validator.ValidateGuess(guess, answer);

        Assert.All(result, letterResult => Assert.Equal(LetterStatus.NotInWord, letterResult.Status));
    }

    #endregion

    #region Wrong Position Cases

    [Theory]
    [InlineData("ABCDE", "EABCD")]
    [InlineData("PIKACHU", "UPIKHAC")]
    public void ValidateGuess_AllWrongPositions_ReturnsAllYellow(string guess, string answer)
    {
        var result = _validator.ValidateGuess(guess, answer);

        Assert.All(result, letterResult => Assert.Equal(LetterStatus.WrongPosition, letterResult.Status));
    }

    #endregion

    #region Mixed Cases

    [Theory]
    [InlineData("PLANT", "SLANT")]
    [InlineData("ROBOT", "NOTCH")]
    public void ValidateGuess_MixedStatuses(string guess, string answer)
    {
        var result = _validator.ValidateGuess(guess, answer);

        Assert.NotNull(result);
        Assert.Equal(guess.Length, result.Count);
        Assert.NotEmpty(result);
    }

    #endregion

    #region Duplicate Letter Edge Cases

    [Theory]
    [InlineData("SPEED", "SPEED", new[] { LetterStatus.Correct, LetterStatus.Correct, LetterStatus.Correct, LetterStatus.Correct, LetterStatus.Correct })]
    [InlineData("SPEED", "ERASE", new[] { LetterStatus.WrongPosition, LetterStatus.NotInWord, LetterStatus.WrongPosition, LetterStatus.WrongPosition, LetterStatus.NotInWord })]
    [InlineData("ABBEY", "SWEET", new[] { LetterStatus.NotInWord, LetterStatus.NotInWord, LetterStatus.NotInWord, LetterStatus.Correct, LetterStatus.NotInWord })]
    [InlineData("FLOOR", "ROBOT", new[] { LetterStatus.NotInWord, LetterStatus.NotInWord, LetterStatus.WrongPosition, LetterStatus.Correct, LetterStatus.WrongPosition })]
    public void ValidateGuess_DuplicateLetters_HandleCorrectly(string guess, string answer, LetterStatus[] expectedStatuses)
    {
        var result = _validator.ValidateGuess(guess, answer);

        for (int i = 0; i < expectedStatuses.Length; i++)
        {
            Assert.Equal(expectedStatuses[i], result[i].Status);
        }
    }

    /// <summary>
    /// When the guess has 2 E's but the answer only has 1 E (at position 3),
    /// the first E should be WrongPosition (E exists but at position 3),
    /// the second E should be NotInWord (already used the only E).
    /// A at position 2 exists in answer at position 4, so it's WrongPosition.
    /// </summary>
    [Fact]
    public void ValidateGuess_MoreDuplicatesInGuessThanAnswer_OnlyMarksAvailable()
    {
        var result = _validator.ValidateGuess("EEABC", "XYZEA");

        Assert.Equal(LetterStatus.WrongPosition, result[0].Status); // E at pos 0, exists at pos 3 in answer
        Assert.Equal(LetterStatus.NotInWord, result[1].Status);     // E at pos 1, already used
        Assert.Equal(LetterStatus.WrongPosition, result[2].Status); // A at pos 2, exists at pos 4 in answer
        Assert.Equal(LetterStatus.NotInWord, result[3].Status);     // B not in answer
        Assert.Equal(LetterStatus.NotInWord, result[4].Status);     // C not in answer
    }

    /// <summary>
    /// When the guess has 1 E at wrong position, but answer has 2 E's,
    /// the E should still be WrongPosition (an E exists elsewhere).
    /// A at position 4 is in answer at position 2, so it's WrongPosition not Correct.
    /// </summary>
    [Fact]
    public void ValidateGuess_FewerDuplicatesInGuessThanAnswer_StillMarksWrongPosition()
    {
        var result = _validator.ValidateGuess("XYZEA", "EEABC");

        Assert.Equal(LetterStatus.NotInWord, result[0].Status);     // X not in answer
        Assert.Equal(LetterStatus.NotInWord, result[1].Status);     // Y not in answer
        Assert.Equal(LetterStatus.NotInWord, result[2].Status);     // Z not in answer
        Assert.Equal(LetterStatus.WrongPosition, result[3].Status); // E at pos 3, exists at pos 0 or 1
        Assert.Equal(LetterStatus.WrongPosition, result[4].Status); // A at pos 4, exists at pos 2
    }

    #endregion

    #region Case Insensitivity

    [Theory]
    [InlineData("pikachu", "PIKACHU")]
    [InlineData("PiKaChU", "pikachu")]
    [InlineData("AbCdE", "aBcDe")]
    public void ValidateGuess_CaseInsensitive_HandledCorrectly(string guess, string answer)
    {
        var result = _validator.ValidateGuess(guess, answer);

        Assert.All(result, letterResult => Assert.Equal(LetterStatus.Correct, letterResult.Status));
    }

    #endregion

    #region Input Validation

    [Fact]
    public void ValidateGuess_NullGuess_ThrowsArgumentException()
    {
        string? guess = null;
        Assert.Throws<ArgumentException>(() => _validator.ValidateGuess(guess, "PIKACHU"));
    }

    [Fact]
    public void ValidateGuess_NullAnswer_ThrowsArgumentException()
    {
        string? answer = null;
        Assert.Throws<ArgumentException>(() => _validator.ValidateGuess("PIKACHU", answer));
    }

    [Fact]
    public void ValidateGuess_EmptyGuess_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _validator.ValidateGuess("", "PIKACHU"));
    }

    [Fact]
    public void ValidateGuess_EmptyAnswer_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _validator.ValidateGuess("PIKACHU", ""));
    }

    [Theory]
    [InlineData("ABC", "ABCD")]
    [InlineData("ABCDE", "ABCD")]
    [InlineData("AB", "ABCDE")]
    public void ValidateGuess_DifferentLengths_ThrowsArgumentException(string guess, string answer)
    {
        Assert.Throws<ArgumentException>(() => _validator.ValidateGuess(guess, answer));
    }

    #endregion

    #region Real Wordle Examples

    [Theory]
    [InlineData("CRANE", "ROBOT", new[] { LetterStatus.NotInWord, LetterStatus.WrongPosition, LetterStatus.NotInWord, LetterStatus.NotInWord, LetterStatus.NotInWord })]
    [InlineData("ROBOT", "STARE", new[] { LetterStatus.WrongPosition, LetterStatus.NotInWord, LetterStatus.NotInWord, LetterStatus.NotInWord, LetterStatus.WrongPosition })]
    [InlineData("SPEED", "CREEP", new[] { LetterStatus.NotInWord, LetterStatus.WrongPosition, LetterStatus.Correct, LetterStatus.Correct, LetterStatus.NotInWord })]
    public void ValidateGuess_RealExamples(string guess, string answer, LetterStatus[] expectedStatuses)
    {
        var result = _validator.ValidateGuess(guess, answer);

        for (int i = 0; i < expectedStatuses.Length; i++)
        {
            Assert.Equal(expectedStatuses[i], result[i].Status);
        }
    }

    #endregion
}
