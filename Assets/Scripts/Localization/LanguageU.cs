using System.Collections.Generic;
using UnityEngine;

public static class LanguageU
{
    public static Dictionary<SystemLanguage, Dictionary<string, string>> LoadTranslate(DataLocalization[] data)
    {
        var tempDic = new Dictionary<SystemLanguage, Dictionary<string, string>>();

        for (int i = 0; i < data.Length; i++)
        {
            var tempData = new Dictionary<string, string>();
            foreach (var trad in data[i].data)
            {
                if (trad == null) continue;

                var lines = trad.text.Split('\n');
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var cleanLine = line.Replace('{', ' ')
                                        .Replace('"', ' ')
                                        .Replace('}', ' ')
                                        .Split(':');

                    if (cleanLine.Length >= 2)
                    {
                        string key = cleanLine[0].Trim();

                        // Recombine the rest of the string
                        string value = string.Join(":", cleanLine, 1, cleanLine.Length - 1).Trim();

                        // FIX: Safely strip off the structural trailing JSON comma if it exists!
                        value = value.TrimEnd(',');
                        value = value.Trim(); // Quick extra clean to remove trailing empty spaces left behind

                        // Unescape the literal string \n character back into an actual line break
                        value = value.Replace("\\n", "\n");

                        if (!tempData.ContainsKey(key) && !string.IsNullOrEmpty(key))
                        {
                            tempData.Add(key, value);
                        }
                    }
                }
            }
            tempDic.Add(data[i].language, tempData);
        }
        return tempDic;
    }

}
