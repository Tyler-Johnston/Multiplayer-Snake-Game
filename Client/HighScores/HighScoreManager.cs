using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization.Json;
using System.Threading;

namespace CS5410.HighScores
{
    public static class HighScoreManager
    {
        private static readonly Mutex mutex = new Mutex(false, "Global\\HighScoresMutex");
        public static List<HighScore> HighScores { get; private set; } = new List<HighScore>();

        static HighScoreManager()
        {
            LoadHighScores();
        }

        private static void LoadHighScores()
        {
            mutex.WaitOne(); // Wait until it is safe to enter.
            try
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (storage.FileExists("HighScores3.json"))
                    {
                        using (IsolatedStorageFileStream fs = storage.OpenFile("HighScores3.json", FileMode.Open))
                        {
                            if (fs.Length > 0)
                            {
                                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<HighScore>));
                                HighScores = (List<HighScore>)serializer.ReadObject(fs);
                            }
                        }
                    }
                }
            }
            finally
            {
                mutex.ReleaseMutex(); // Release the mutex.
            }
        }

        public static void SaveHighScores()
        {
            mutex.WaitOne(); // Wait until it is safe to enter.
            try
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream fs = storage.CreateFile("HighScores3.json"))
                    {
                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<HighScore>));
                        serializer.WriteObject(fs, HighScores);
                    }
                }
            }
            finally
            {
                mutex.ReleaseMutex(); // Release the mutex.
            }
        }

        public static void AddHighScore(HighScore highScore)
        {
            HighScores.Add(highScore);
            HighScores.Sort((hs1, hs2) => hs2.Score.CompareTo(hs1.Score));
            if (HighScores.Count > 10)
            {
                HighScores.RemoveRange(10, HighScores.Count - 10);
            }
            SaveHighScores();
        }
    }
}
