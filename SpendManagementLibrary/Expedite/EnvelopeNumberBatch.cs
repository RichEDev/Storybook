namespace SpendManagementLibrary.Expedite
{  
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using Helpers;

    internal class EnvelopeNumberBatch
    {
        #region Private Constants 

        const string CharsToExclude = @"QJU" + "\"";
        const int Radix = 23;

        #endregion Private Constants 

        #region Properties + Constructor

        /// <summary>
        /// The size of the batch during creation. 
        /// Should be 1000 in production.
        /// </summary>
        public uint BatchSize { get; private set; }

        /// <summary>
        /// A list of alphabetic characters 
        /// </summary>
        public static List<String> AlphaCharacters { get; private set; }
        
        public EnvelopeNumberBatch(uint batchSize)
        {
            BatchSize = batchSize;
            AlphaCharacters = CreateAlphabeticCharacterList();
        }

        #endregion Properties + Constructor

        #region Envelope Number Batch Generation

        /// <summary>
        /// Generates a batch of Envelope Numbers
        /// </summary>
        /// <returns>A List of Envelope Numbers</returns>
        public List<string> GenerateEnvelopeNumbers()
        {       
            List<string> alphaCodes = GenerateListOfAlphaCodes();
            List<string> metabaseAlphaCodes = GetDistinctAlphaCodesFromMetabase();

            //remove alpha codes found in the Metabase from the generated list of alpha codes
            var results = alphaCodes.Where(alphaCode => !metabaseAlphaCodes.Contains(alphaCode));
            alphaCodes = results.ToList();

            //pick an alpha code at random which will form the basis of the envelope number batch
            var rnd = new Random();
            int i = rnd.Next(alphaCodes.Count);
            string randomAlphaCode = alphaCodes[i];

            return CreateEnvelopeNumerBatch(randomAlphaCode);
        }

        /// <summary>
        /// Creates a list of alpha codes (AAA-ZZZ) exlcuding alpha codes that contain certain characters
        /// </summary>
        /// <returns>A list of alpha codes</returns>
        private List<string> GenerateListOfAlphaCodes()
        {
            var alphaCodes = new List<string>();
            int i = 0;
            string alphaCode = "";

            while (alphaCode != "ZZZ")
            {
                alphaCode = DetermineAlphaCode(i);

                if (alphaCode.IndexOfAny(CharsToExclude.ToCharArray()) == -1)
                {
                    alphaCodes.Add(alphaCode);
                }

                i++;
            }

            return alphaCodes;
        }

        /// <summary>
        /// Generates the alpha code based on the iteration 
        /// </summary>
        /// <param name="i">The iteration</param>
        /// <returns>An alpha code i.e. AFG or GGD</returns>
        private static string DetermineAlphaCode(int i)
        {
            var result = new StringBuilder();

            result.Insert(0, IntToAlpha(i));
            i /= 26;
            result.Insert(0, IntToAlpha(i));
            i /= 26;
            result.Insert(0, IntToAlpha(i));

            return result.ToString();
        }

        /// <summary>
        /// Creates the batch using the randomly picked alpha code.
        /// </summary>
        /// <param name="alphaCode">The random alphacode</param>
        /// <returns>The complete list of gernated Envelop Numbers</returns>
        private List<string> CreateEnvelopeNumerBatch(string alphaCode)
        {
            var envelopeNumberBatch = new List<string>();
            string alpha1 = alphaCode.Substring(0, 1);
            string alpha2 = alphaCode.Substring(1, 1);
            string alpha3 = alphaCode.Substring(2, 1);
            int alphaDigit1 = AlphaToInt(alpha1);
            int alphaDigit2 = AlphaToInt(alpha2);
            int alphaDigit3 = AlphaToInt(alpha3);
 
            for (var i = 0; i < BatchSize; i++)
            {
                string suffix = i.ToString("000");             
                //Generate the checksum based on the suffix and alpha code
                int temp1 = 3 * (alphaDigit1 + alphaDigit3 + int.Parse(suffix.Substring(0, 1)) + int.Parse(suffix.Substring(2, 1)));
                int temp2 = alphaDigit2 + int.Parse(suffix.Substring(1, 1));
                int remainder = (temp1 + temp2) % Radix;

                envelopeNumberBatch.Add(IntToAlphaFromList(remainder) + "-" + alphaCode + "-" + suffix);
            }

            return envelopeNumberBatch;
        }

        /// <summary>
        /// Creates a list of alphabetic characters, with certain characters excluded 
        /// </summary>
        /// <returns>A list alphabetic characters. </returns>
        private static List<string> CreateAlphabeticCharacterList()
        {
            var alphaCharacters = new List<string>();

            for (char c = 'A'; c <= 'Z'; c++)
            {
                string alphaChar = "" + c;

                if (alphaChar.IndexOfAny(CharsToExclude.ToCharArray()) == -1)
                {
                    alphaCharacters.Add("" + alphaChar);
                }
            }

            return alphaCharacters;
        }

        /// <summary>
        /// Converts an int to its corresponding alphabetic character 
        /// </summary>
        /// <param name="i"></param>
        /// <returns>An alphabetic character</returns>
        private static char IntToAlpha(int i)
        {
            return (char)('A' + (i % 26));
        }

        /// <summary>
        /// Converts an int to its corresponding alphabetic character from a list of refined alphabetic characters 
        /// </summary>
        /// <param name="i"></param>
        /// <returns>An alphabetic character</returns>
        private static char IntToAlphaFromList(int i)
        {
            return Convert.ToChar(AlphaCharacters[i]);
        }

        /// <summary>
        /// Converts an alphabetic character to its corresponding Int
        /// </summary>
        /// <param name="alpha"></param>
        /// <returns>An int</returns>
        private static int AlphaToInt(string alpha)
        {  
          return AlphaCharacters.IndexOf(alpha);        
        }

        /// <summary>
        /// Reads the Metabase for distinct envelope numbers and extracts the alpha codes 
        /// </summary>
        /// <returns>A list of alpha codes that are in the metabase</returns>
        private static List<string> GetDistinctAlphaCodesFromMetabase()
        {
            const string columnNameEnvelopeNumber = "EnvelopeNumber";
            string baseEnvelopeNumberSql = string.Format("SELECT Distinct ({0})" + "FROM [dbo].[Envelopes] ", columnNameEnvelopeNumber);
            IEnumerable<string> distinctMetabaseNumbers = ExtractDistinctEnvelopeNumbersFromSql(baseEnvelopeNumberSql);

            //Read the alpha code from the envelope number and add to a list. I.E. A-GFR-002 is extracted to GFR
            var alphaCodes = (from envelopNumber in distinctMetabaseNumbers
                              let start = envelopNumber.IndexOf("-", StringComparison.Ordinal) + 1
                              let end = envelopNumber.LastIndexOf("-", StringComparison.Ordinal)
                              select envelopNumber.Substring(start, end - start)).ToList();

            return alphaCodes;
        }

        /// <summary>
        /// Reads all the distinct envelope numbers from the Metabase
        /// </summary>
        /// <param name="sql">The SQL to execute.</param>
        /// <returns>A list of envelop numbers that are in the metabase.</returns>
        private static IEnumerable<string> ExtractDistinctEnvelopeNumbersFromSql(string sql)
        {
            var list = new List<string>();
            using (var connection = new DatabaseConnection(GlobalVariables.MetabaseConnectionString))
            {
                using (IDataReader reader = connection.GetReader(sql))
                {
                    int ordEnvelopNumber = reader.GetOrdinal("EnvelopeNumber");
                 
                    while (reader.Read())
                    {
                        list.Add(reader.GetString(ordEnvelopNumber));
                    }
                }
                connection.ClearParameters();
            }
            return list;
        }
    }

    #endregion Envelope Number Batch Generation
}
