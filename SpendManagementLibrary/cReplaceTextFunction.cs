using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infragistics.WebUI.CalcEngine;

namespace SpendManagementLibrary
{
    public class cReplaceTextFunction : Infragistics.WebUI.CalcEngine.UltraCalcUserDefinedFunction
    {
        public override string[] ArgDescriptors
        {
            get
            {
                return new[]
                           {
                               "The original text.", "The text to match.", "The character number to start searching.",
                               "The new Text.", "The number of timies to replace the text."
                           };
            }
        }

        public override string[] ArgList
        {
            get { return new[] { "old_text", "match_text", "start_num", "new_text", "num_replacements" }; }
        }

        public override string Category
        {
            get { return "TextAndData"; }
        }

        public override string Description
        {
            get { return "Replaces all occurrences of a text string with the specified replacement string."; }
        }

        protected override Infragistics.WebUI.CalcEngine.UltraCalcValue Evaluate(UltraCalcNumberStack numberStack, int argumentCount)
        {
            UltraCalcValue num_replacements = numberStack.Pop();
            UltraCalcValue new_text = numberStack.Pop();
            UltraCalcValue start_num = numberStack.Pop();
            UltraCalcValue match_text = numberStack.Pop();
            UltraCalcValue old_text = numberStack.Pop();

            if (old_text.IsDBNull || !old_text.IsString)
                return old_text;

            if (match_text.IsDBNull || match_text.IsNull || !match_text.IsString)
                return old_text;

            if (new_text.IsDBNull || !new_text.IsString)
                return old_text;

            int startIdx;
            if (start_num.IsDBNull || start_num.IsNull || !int.TryParse(start_num.ToString(), out startIdx) || int.Parse(start_num.ToString()) > old_text.ToString().Length)
                return old_text;

            startIdx--; // adjust -1 for zero based string indexing
            if (startIdx < 0)
                startIdx = 0;

            int numMatches;
            if (num_replacements.IsDBNull || num_replacements.IsNull || !int.TryParse(num_replacements.ToString(), out numMatches))
                return old_text;

            if (numMatches < 0)
                numMatches = 0;

            UltraCalcValue a;
            if (numMatches == 0)
            {
                string textToActionOn = string.Empty;
                string stringPrefix = string.Empty;
                if (startIdx > 0)
                {
                    stringPrefix = old_text.ToString().Substring(0, startIdx);
                    textToActionOn = old_text.ToString().Substring(startIdx);
                }
                else
                {
                    textToActionOn = old_text.ToString();
                }

                a = new UltraCalcValue(stringPrefix + textToActionOn.Replace(match_text.ToString(), new_text.ToString()));
            }
            else
            {
                if (!old_text.ToString().Substring(startIdx).Contains(match_text.ToString()))
                    return old_text;

                int matchCount = 0;

                // if start index is not beginning, copy lead characters up to the start index
                string newString = (startIdx > 0 ? old_text.ToString().Substring(0, startIdx) : string.Empty);

                while (matchCount < numMatches && startIdx < old_text.ToString().Length)
                {
                    int findIdx = old_text.ToString().IndexOf(match_text.ToString(), startIdx, StringComparison.InvariantCulture);
                    if (findIdx >= 0)
                    {
                        newString += old_text.ToString().Substring(startIdx, (findIdx - startIdx));
                        newString += new_text.ToString();

                        startIdx = findIdx + match_text.ToString().Length;
                        matchCount++;
                    }
                    else
                    {
                        break;
                    }
                }

                // copy remainder of string
                newString += old_text.ToString().Substring(startIdx);

                a = new UltraCalcValue(newString);
            }

            return a;
        }

        public override int MaxArgs
        {
            get { return 5; }
        }

        public override int MinArgs
        {
            get { return 5; }
        }

        public override string Name
        {
            get { return "REPLACETEXT"; }
        }
    }
}
