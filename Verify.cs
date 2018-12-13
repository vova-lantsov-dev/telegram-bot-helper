using System;

namespace Telegram.Bot.Helper
{
    /// <summary>
    /// Verify status of user that writes to bot. Unchecked matches all statuses.
    /// </summary>
    [Flags]
    public enum Verify
    {
        Unchecked = 1,
        Unknown = 2,
        Unverified = 4,
        Verified = 8
    }
}
