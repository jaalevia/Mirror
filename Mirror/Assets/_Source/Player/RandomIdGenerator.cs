using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerSystem
{
    public static class RandomIdGenerator
    {
        public static string GetRandomID()
        {
            string ID = string.Empty;
            for (int i = 0; i < 5; i++)
            {
                int rand = Random.Range(0, 36);
                if (rand < 26)
                {
                    ID += (char)(rand + 65);
                }
                else
                {
                    ID += (rand - 26).ToString();
                }
            }
            return ID;
        }
    }
}