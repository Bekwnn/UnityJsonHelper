// written by Bekwnn, 2015
// contributed by Guney Ozsan, 2016

using System.Text.RegularExpressions;
using System.Collections.Generic;

public class JsonHelper
{
    //Returns the first occurence of a json object with variable name "handle". This doesn't decompose object arrays.
    public static string GetJsonObject(string jsonString, string handle)
    {
        string pattern = "\"" + handle + "\"\\s*:\\s*\\{";

        Regex regx = new Regex(pattern);

        Match match = regx.Match(jsonString);

        if (match.Success)
        {
            int bracketCount = 1;
            int i;
            int startOfObj = match.Index + match.Length;
            for (i = startOfObj; bracketCount > 0; i++)
            {
                if (jsonString[i] == '{') bracketCount++;
                else if (jsonString[i] == '}') bracketCount--;
            }
            return "{" + jsonString.Substring(startOfObj, i - startOfObj);
        }

        //no match, return null
        return null;
    }

    //Returns array of all occurences of a json object with variable name "handle". This doesn't decompose object arrays.
    public static string[] GetJsonObjects(string jsonString, string handle)
    {
        string pattern = "\"" + handle + "\"\\s*:\\s*\\{";

        Regex regx = new Regex(pattern);

        //check if there's a match at all, return null if not
        if (!regx.IsMatch(jsonString)) return null;

        List<string> jsonObjList = new List<string>();

        //find each regex match
        foreach (Match match in regx.Matches(jsonString))
        {
            int bracketCount = 1;
            int i;
            int startOfObj = match.Index + match.Length;
            for (i = startOfObj; bracketCount > 0; i++)
            {
                if (jsonString[i] == '{') bracketCount++;
                else if (jsonString[i] == '}') bracketCount--;
            }
            jsonObjList.Add("{" + jsonString.Substring(startOfObj, i - startOfObj));
        }

        return jsonObjList.ToArray();
    }

    //Returns first occurence of a json object array with variable name "handle".
    public static string[] GetJsonObjectArray(string jsonString, string handle)
    {
        string pattern = "\"" + handle + "\"\\s*:\\s*\\[\\s*{";

        Regex regx = new Regex(pattern);

        List<string> jsonObjList = new List<string>();

        Match match = regx.Match(jsonString);

        if (match.Success)
        {
            int squareBracketCount = 1;
            int curlyBracketCount = 1;
            int startOfObjArray = match.Index + match.Length;
            int i = startOfObjArray;
            while (true)
            {
                int startOfObj = i;

                if (jsonString[i] == '[') squareBracketCount++;
                else if (jsonString[i] == ']') squareBracketCount--;

                while (curlyBracketCount > 0)
                {
                    if (jsonString[i] == '{') curlyBracketCount++;
                    else if (jsonString[i] == '}') curlyBracketCount--;
                    i++;
                }

                jsonObjList.Add("{" + jsonString.Substring(startOfObj, i - startOfObj));

                // continue with the next array element or return object array if there is no more left
                while (jsonString[i] != '{')
                {
                    if (jsonString[i] == ']' && squareBracketCount == 1)
                    {
                        return jsonObjList.ToArray();
                    }
                    i++;
                }

                curlyBracketCount = 1;

                i++;
            }
        }

        //no match, return null
        return null;
    }

    //Returns jagged array of all occurences of json object arrays with variable name "handle".
    //Every occurence of that variable should be a json array.
    public static string[][] GetJsonObjectArrays(string jsonString, string handle)
    {
        string pattern = "\"" + handle + "\"\\s*:\\s*\\[\\s*{";

        Regex regx = new Regex(pattern);

        //check if there's a match at all, return null if not
        if (!regx.IsMatch(jsonString)) return null;

        MatchCollection matchCollection = regx.Matches(jsonString);
        string[][] jsonObjArray = new string[matchCollection.Count][];

        List<string> jsonObjList = new List<string>();

        //find each regex match
        var matchIndex = 0;
        foreach (Match match in regx.Matches(jsonString))
        {
            int squareBracketCount = 1;
            int curlyBracketCount = 1;
            int startOfObjArray = match.Index + match.Length;
            int i = startOfObjArray;
            bool arrayObjectFinished = false;

            while (!arrayObjectFinished)
            {
                int startOfObj = i;

                if (jsonString[i] == '[') squareBracketCount++;
                else if (jsonString[i] == ']') squareBracketCount--;

                while (curlyBracketCount > 0)
                {
                    if (jsonString[i] == '{') curlyBracketCount++;
                    else if (jsonString[i] == '}') curlyBracketCount--;
                    i++;
                }

                jsonObjList.Add("{" + jsonString.Substring(startOfObj, i - startOfObj));

                // continue with the next array element or add object array to the main array if there is no more left
                while (jsonString[i] != '{' && !arrayObjectFinished)
                {
                    //add the current object array to main array, escape while sequences and continue with the next match if any
                    if (jsonString[i] == ']' && squareBracketCount == 1)
                    {
                        jsonObjArray[matchIndex] = jsonObjList.ToArray();
                        matchIndex++;
                        arrayObjectFinished = true;
                    }
                    else
                    {
                        i++;
                    }
                }

                curlyBracketCount = 1;

                if (!arrayObjectFinished)
                {
                    i++;
                }
            }
        }
        return jsonObjArray;
    }
}
