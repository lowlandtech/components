namespace LowlandTech.Components;

/// <summary>
/// Applies simple character-mask patterns to string input.
/// '0' = digit, 'A' = letter, '*' = any character.
/// </summary>
public static class FormFieldMask
{
    public static string? Apply(string? input, string? mask)
    {
        if (string.IsNullOrEmpty(mask) || string.IsNullOrEmpty(input))
            return input;

        var result = new StringBuilder();
        var inputIndex = 0;

        for (var maskIndex = 0; maskIndex < mask.Length && inputIndex < input.Length; maskIndex++)
        {
            var maskChar = mask[maskIndex];
            var inputChar = input[inputIndex];

            switch (maskChar)
            {
                case '0':
                    if (char.IsDigit(inputChar))
                    {
                        result.Append(inputChar);
                        inputIndex++;
                    }
                    else
                    {
                        return result.ToString();
                    }
                    break;
                case 'A':
                    if (char.IsLetter(inputChar))
                    {
                        result.Append(inputChar);
                        inputIndex++;
                    }
                    else
                    {
                        return result.ToString();
                    }
                    break;
                case '*':
                    result.Append(inputChar);
                    inputIndex++;
                    break;
                default:
                    result.Append(maskChar);
                    if (inputChar == maskChar)
                        inputIndex++;
                    break;
            }
        }

        return result.ToString();
    }

    public static bool IsComplete(string? value, string? mask)
    {
        if (string.IsNullOrEmpty(mask))
            return true;
        if (string.IsNullOrEmpty(value))
            return false;

        var requiredLength = mask.Count(c => c == '0' || c == 'A' || c == '*');
        return value.Length >= requiredLength;
    }
}
