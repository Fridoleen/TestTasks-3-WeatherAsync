using System;
using System.Collections.Generic;

namespace TestTasks.VowelCounting
{
    public class StringProcessor
    {
        public (char symbol, int count)[] GetCharCount(string veryLongString, char[] countedChars)
        {
            var result = new List<(char symbol, int count)>();

            foreach(char ch in countedChars)
            {
                result.Add((ch, 0));
            }

            var resultArr = result.ToArray();

            for(int i=0; i<veryLongString.Length; i++)
            {
               for(int j=0; j<result.Count; j++)
               {
                    if(veryLongString[i] == resultArr[j].symbol)
                    {
                        resultArr[j].count += 1;
                        break;
                    }
               }
            }         

            return resultArr;
        }
    }
}
