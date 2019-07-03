using System;

namespace Telegram.Bot.Helper
{
    /// <summary>
    /// Inline command is callback data of inline button. This command is split by selected separator.
    /// </summary>
    public class InlineCommand
    {
        /// <summary>
        /// Count of commands
        /// </summary>
        public int Count => Commands.Length;

        /// <summary>
        /// Commands parsed from callback data
        /// </summary>
        public readonly string[] Commands;

        internal InlineCommand(string command, string separator)
        {
            Commands = command.Split(new[] { separator }, StringSplitOptions.None);
        }

        public bool Equals(InlineCommand valueToCompareWith)
        {
            if (valueToCompareWith == null || Count != valueToCompareWith.Count)
                return false;

            for (int commandIndex = 0; commandIndex < Count; commandIndex++)
            {
                if (string.IsNullOrWhiteSpace(Commands[commandIndex]))
                    continue;
                if (string.IsNullOrWhiteSpace(valueToCompareWith.Commands[commandIndex]))
                    continue;

                if (Commands[commandIndex] != valueToCompareWith.Commands[commandIndex])
                    return false;
            }
            return true;
        }
    }
}
