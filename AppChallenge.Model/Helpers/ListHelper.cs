using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppChallenge.Model
{
    /// <summary>
    /// Helper class that contains dictionaries for populating comboboxes.
    /// </summary>
    public static class ListHelper
    {
        public static Dictionary<string, string> DictionaryContactRelationship;
        public static Dictionary<string, string> DictionaryGender;
        public static Dictionary<string, string> DictionaryGenderStatus;
        public static Dictionary<string, string> DictionaryMaritalStatus;
        public static Dictionary<string, string> DictionaryIdentifierUse;
        public static Dictionary<string, string> DictionaryIdentifierTypeCode;
        public static List<string> ListCountries;

        const string CONTACT_RELATIONSHIP = "ListHelpers/ContactRelationship";
        const string GENDER = "ListHelpers/Gender";
        const string GENDER_STATUS = "ListHelpers/GenderStatus";
        const string MARITAL_STATUS = "ListHelpers/MaritalStatus";
        const string IDENTIFIER_USE = "ListHelpers/IdentifierUse";
        const string IDENTIFIER_TYPE_CODE = "ListHelpers/IdentifierTypeCode";

        static ListHelper()
        {
            DictionaryContactRelationship = GetValuesFromConfigurationFile(CONTACT_RELATIONSHIP);
            DictionaryGender = GetValuesFromConfigurationFile(GENDER);
            DictionaryGenderStatus = GetValuesFromConfigurationFile(GENDER_STATUS);
            DictionaryMaritalStatus = GetValuesFromConfigurationFile(MARITAL_STATUS);
            DictionaryIdentifierUse = GetValuesFromConfigurationFile(IDENTIFIER_USE);
            DictionaryIdentifierTypeCode = GetValuesFromConfigurationFile(IDENTIFIER_TYPE_CODE);

            ListCountries = GetCountryList();
            
        }

        /// <summary>
        /// Gets values from the configuration file under ListHelpers section.
        /// </summary>
        /// <param name="configurationSection">Subsection name under ListHelpers section.</param>
        /// <returns></returns>
        static Dictionary<string, string> GetValuesFromConfigurationFile(string configurationSection)
        {
            return DictionaryIdentifierTypeCode = (ConfigurationManager.GetSection(configurationSection) as System.Collections.Hashtable)
                    .Cast<System.Collections.DictionaryEntry>().OrderBy(x => x.Value)
                    .ToDictionary(n => n.Key.ToString(), n => n.Value.ToString());
        }

        /// <summary>
        /// Gets the country list from CultureInfo.
        /// </summary>
        /// <returns>List of countries.</returns>
        static List<string> GetCountryList()
        {
            var listCountries = new List<string>();
            CultureInfo[] getCultureInfo = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            // Add blank for default value.
            listCountries.Add("");

            foreach (CultureInfo getCulture in getCultureInfo)
            {
                RegionInfo GetRegionInfo = new RegionInfo(getCulture.LCID);
                if (!(listCountries.Contains(GetRegionInfo.EnglishName)))
                {
                    listCountries.Add(GetRegionInfo.EnglishName);
                }
            }
            listCountries.Sort();
            return listCountries;
        }
    }
}
