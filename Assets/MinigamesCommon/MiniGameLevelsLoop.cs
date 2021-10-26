using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace MinigamesCommon
{
    public static class MiniGameLevelsLoop
    {
        private const int QueueLength = 3;
        private static Queue<int> LastRandomLevels = new Queue<int>();

        private static int _maxLevel;
        private static int _minLevel;

        public static void Initialize(int minLevel,int maxLevel)
        {
            _minLevel = minLevel;
            _maxLevel = maxLevel;
            LastRandomLevels = new Queue<int>();
        }
        
        public static int GetNextRandomLevel()
        {
            if (_maxLevel == 0)
            {
                throw new Exception("LevelsLoop is not initialized!");
            }
            while (true)
            {
                var randomLevel = Random.Range(_minLevel, _maxLevel);
                if (LastRandomLevels.Contains(randomLevel)) continue;
                
                LastRandomLevels.Enqueue(randomLevel);
                if (LastRandomLevels.Count > QueueLength)
                {
                    LastRandomLevels.Dequeue();
                }
                return randomLevel;
            }
        }
    }
}
