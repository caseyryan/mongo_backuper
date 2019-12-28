using System;

namespace Services.Packagers {

    public class AccountBlockData {
        private int AttemptCount { get; set;}
        private int MaxAttempts { get; set;}
        private long UnlockTime { get; set;}

        public void IncreaseCounter(int maxAttempts, long maxBlockTimeSeconds = 900) {
            var curTimeSeconds = DateTimeOffset.Now.ToUnixTimeSeconds();
            if (IsLocked) {
                if (curTimeSeconds >= UnlockTime) {
                    AttemptCount = 0;
                } else {
                    return;
                }
            }
            MaxAttempts = maxAttempts;
            AttemptCount++;
            if (IsLocked) {
                UnlockTime = curTimeSeconds + maxBlockTimeSeconds;
            }
            
        }
        public void Reset() {
            UnlockTime = 0;
            AttemptCount = 0;
        }
        public bool IsLocked {
            get {
                return AttemptCount > MaxAttempts;
            }
        }
    }
}