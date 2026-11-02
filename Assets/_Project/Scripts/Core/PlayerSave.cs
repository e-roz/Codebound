using System;

namespace Codebound.Core
{
    [Serializable]
    public class PlayerSave
    {
        public const int CurrentVersion = 1;

        public int version = CurrentVersion;
        public string playerName;
        public int lookId;

        public bool IsValid() => version >= 1 && !string.IsNullOrWhiteSpace(playerName);
    }
}
