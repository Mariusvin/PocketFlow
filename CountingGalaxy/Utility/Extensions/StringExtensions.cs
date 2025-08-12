namespace Utility.Extensions
{
    public static class StringExtensions
    {
        public static string TrimToMaxCharacters(this string _input, int _maxLength)
        {
            if (string.IsNullOrEmpty(_input) || _input.Length <= _maxLength)
            {
                return _input;
            }
            
            return _input[.._maxLength];
        }
    }
}
