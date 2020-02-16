using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FormBuilder.Helpers
{
    public class ItemListHelper
    {
        public static readonly IDictionary<string, string> StateDictionary = new Dictionary<string, string> {
        
        {"Alabama", "AL"},
        {"Alaska", "AK"},
        {"American Samoa", "AS"},
        {"Arizona", "AZ"},
        {"Arkansas", "AR"},
        {"California", "CA"},
        {"Colorado", "CO"},
        {"Conneticut", "CT"},
        {"Delaware", "DE"},
        {"District Of Columbia", "DC"},
        {"Federated States Of Micronesia", "FM"},
        {"Florida", "FL"},
        {"Georgia", "GA"},
        {"Guam", "GU"},
        {"Hawaii", "HI"},
        {"Idaho", "ID"},
        {"Illinois", "IL"},
        {"Indiana", "IN"},
        {"Iowa", "IA"},
        {"Kansas", "KS"},
        {"Kentucky", "KY"},
        {"Louisiana", "LA"},
        {"Maine", "ME"},
        {"Marshall Islands", "MH"},
        {"Maryland", "MD"},
        {"Massachusetts", "MA"},
        {"Michigan", "MI"},
        {"Minnesota", "MN"},
        {"Missippippi", "MS"},
        {"Missouri", "MO"},
        {"Montana", "MT"},
        {"Nebraska", "NE"},
        {"Nevada", "NV"},
        {"New Hampshire", "NH"},
        {"New Jersey", "NJ"},
        {"New Mexico", "NM"},
        {"New York", "NY"},
        {"North Carolina", "NC"},
        {"North Dakota", "ND"},
        {"Northern Mariana Islands", "MP"},
        {"Ohio", "OH"},
        {"Oklahoma", "OK"},
        {"Oregon", "OR"},
        {"Palau", "PW"},
        {"Pennsylvania", "PA"},
        {"Puerto Rico", "PR"},
        {"Rhode Island", "RI"},
        {"South Carolina", "SC"},
        {"South Dakota", "SD"},
        {"Tennessee", "TN"},
        {"Texas", "TX"},
        {"Utah", "UT"},
        {"Vermont", "VT"},
        {"Virgin Islands", "VI"},
        {"Virginia", "VA"},
        {"Washington", "WA"},
        {"West Virginia", "WV"},
        {"Wisconsin", "WI"},
        {"Wyoming", "WY"}
    };

        public static SelectList StateSelectList
        {
            get { return new SelectList(StateDictionary, "Value", "Key"); }
        }

        public static SelectList ShortStateSelectList
        {
            get { return new SelectList(StateDictionary, "Value", "Value"); }
        }

        public static IEnumerable<SelectListItem> Months()
        {
            return Enumerable.Range(1, 12).Select(x => new SelectListItem { Value = x.ToString(), Text = new DateTime(2009, x, 1).ToString("MMMM") });
        }


        public static IEnumerable<SelectListItem> Days()
        {
            return Enumerable.Range(1, 31).Select(x => (x < 10) ? "0" + x : x.ToString()).Select(v => new SelectListItem { Value = v, Text = v });
        }

        public static IEnumerable<SelectListItem> BirthYears()
        {
            return Enumerable.Range(DateTime.Now.Year - 100, 82).Reverse().Select(v => new SelectListItem { Value = v.ToString(), Text = v.ToString() });
        }

        public static IEnumerable<SelectListItem> BirthYears(int minimumAge, int maximumAge)
        {
            return Enumerable.Range(DateTime.Now.Year - maximumAge, maximumAge - minimumAge).Reverse().Select(v => new SelectListItem { Value = v.ToString(), Text = v.ToString() });
        }

        public static IEnumerable<SelectListItem> NumberRange(int minimum, int maximum)
        {
            return Enumerable.Range(minimum, (maximum - minimum) + 1).Reverse().Select(v => new SelectListItem { Value = v.ToString(), Text = v.ToString() });
        }

        public static List<SelectListItem> Countries
        {
            get
            {
                List<SelectListItem> items = new List<SelectListItem>(new[]
                                                                      {
                                                                          new SelectListItem { Text ="Afghanistan", Value="AF" },
                                                        new SelectListItem { Text ="Albania", Value="AL" },
                                                        new SelectListItem { Text ="Algeria", Value="DZ" },
                                                        new SelectListItem { Text ="American Samoa", Value="AS" },
                                                        new SelectListItem { Text ="Angola", Value="AO" },
                                                        new SelectListItem { Text ="Anguilla", Value="AI" },
                                                        new SelectListItem { Text ="Antigua & Barbuda", Value="AG" },
                                                        new SelectListItem { Text ="Argentina", Value="AR" },
                                                        new SelectListItem { Text ="Armenia", Value="AM" },
                                                        new SelectListItem { Text ="Aruba", Value="AW" },
                                                        new SelectListItem { Text ="Australia", Value="AU" },
                                                        new SelectListItem { Text ="Austria", Value="AT" },
                                                        new SelectListItem { Text ="Azerbaijan", Value="AZ" },
                                                        new SelectListItem { Text ="Bahamas", Value="BS" },
                                                        new SelectListItem { Text ="Bahrain", Value="BH" },
                                                        new SelectListItem { Text ="Bangladesh", Value="BD" },
                                                        new SelectListItem { Text ="Barbados", Value="BB" },
                                                        new SelectListItem { Text ="Belarus", Value="BY" },
                                                        new SelectListItem { Text ="Belgium", Value="BE" },
                                                        new SelectListItem { Text ="Belize", Value="BZ" },
                                                        new SelectListItem { Text ="Benin", Value="BJ" },
                                                        new SelectListItem { Text ="Bhutan", Value="BT" },
                                                        new SelectListItem { Text ="Bolivia", Value="BO" },
                                                        new SelectListItem { Text ="Bonaire", Value="B1" },
                                                        new SelectListItem { Text ="Bosnia & Herzegovina", Value="BA" },
                                                        new SelectListItem { Text ="Botswana", Value="BW" },
                                                        new SelectListItem { Text ="Brazil", Value="BR" },
                                                        new SelectListItem { Text ="British Virgin Islands", Value="VG" },
                                                        new SelectListItem { Text ="Brunei Darussalam", Value="BN" },
                                                        new SelectListItem { Text ="Bulgaria", Value="BG" },
                                                        new SelectListItem { Text ="Burkina Faso", Value="BF" },
                                                        new SelectListItem { Text ="Burundi", Value="BI" },
                                                        new SelectListItem { Text ="Cambodia", Value="KH" },
                                                        new SelectListItem { Text ="Cameroon", Value="CM" },
                                                        new SelectListItem { Text ="Canada", Value="CA" },
                                                        new SelectListItem { Text ="Cape Verde", Value="CV" },
                                                        new SelectListItem { Text ="Cayman Islands", Value="KY" },
                                                        new SelectListItem { Text ="Central African Rep", Value="CF" },
                                                        new SelectListItem { Text ="Chad", Value="TD" },
                                                        new SelectListItem { Text ="Chile", Value="CL" },
                                                        new SelectListItem { Text ="China", Value="CN" },
                                                        new SelectListItem { Text ="Colombia", Value="CO" },
                                                        new SelectListItem { Text ="Comoros", Value="KM" },
                                                        new SelectListItem { Text ="Congo", Value="CG" },
                                                        new SelectListItem { Text ="Cook Islands", Value="CK" },
                                                        new SelectListItem { Text ="Costa Rica", Value="CR" },
                                                        new SelectListItem { Text ="Cote D'Ivoire", Value="CI" },
                                                        new SelectListItem { Text ="Croatia", Value="HR" },
                                                        new SelectListItem { Text ="Curacao", Value="C1" },
                                                        new SelectListItem { Text ="Cyprus", Value="CY" },
                                                        new SelectListItem { Text ="Cyprus (Northern)", Value="C2" },
                                                        new SelectListItem { Text ="Czech Republic", Value="CS" },
                                                        new SelectListItem { Text ="Dem Rep of Congo", Value="CD" },
                                                        new SelectListItem { Text ="Denmark", Value="DK" },
                                                        new SelectListItem { Text ="Djibouti", Value="DJ" },
                                                        new SelectListItem { Text ="Dominica", Value="DM" },
                                                        new SelectListItem { Text ="Dominican Republic", Value="DO" },
                                                        new SelectListItem { Text ="East Timor", Value="TP" },
                                                        new SelectListItem { Text ="Ecuador", Value="EC" },
                                                        new SelectListItem { Text ="Egypt", Value="EG" },
                                                        new SelectListItem { Text ="El Salvador", Value="SV" },
                                                        new SelectListItem { Text ="Equatorial Guinea", Value="GQ" },
                                                        new SelectListItem { Text ="Eritrea", Value="ER" },
                                                        new SelectListItem { Text ="Estonia", Value="EE" },
                                                        new SelectListItem { Text ="Ethiopia", Value="ET" },
                                                        new SelectListItem { Text ="Falkland Islands", Value="FK" },
                                                        new SelectListItem { Text ="Fiji", Value="FJ" },
                                                        new SelectListItem { Text ="Finland", Value="FI" },
                                                        new SelectListItem { Text ="France", Value="FR" },
                                                        new SelectListItem { Text ="French Guiana", Value="GF" },
                                                        new SelectListItem { Text ="French Polynesia", Value="PF" },
                                                        new SelectListItem { Text ="Gabon", Value="GA" },
                                                        new SelectListItem { Text ="Gambia", Value="GM" },
                                                        new SelectListItem { Text ="Georgia", Value="GE" },
                                                        new SelectListItem { Text ="Germany", Value="DE" },
                                                        new SelectListItem { Text ="Ghana", Value="GH" },
                                                        new SelectListItem { Text ="Gibraltar", Value="GI" },
                                                        new SelectListItem { Text ="Greece", Value="GR" },
                                                        new SelectListItem { Text ="Grenada", Value="GD" },
                                                        new SelectListItem { Text ="Guadeloupe", Value="GP" },
                                                        new SelectListItem { Text ="Guam", Value="GU" },
                                                        new SelectListItem { Text ="Guatemala", Value="GT" },
                                                        new SelectListItem { Text ="Guinea", Value="GN" },
                                                        new SelectListItem { Text ="Guinea-Bissau", Value="GW" },
                                                        new SelectListItem { Text ="Guyana", Value="GY" },
                                                        new SelectListItem { Text ="Haiti", Value="HT" },
                                                        new SelectListItem { Text ="Honduras", Value="HN" },
                                                        new SelectListItem { Text ="Hong Kong", Value="HK" },
                                                        new SelectListItem { Text ="Hungary", Value="HU" },
                                                        new SelectListItem { Text ="Iceland", Value="IS" },
                                                        new SelectListItem { Text ="India", Value="IN" },
                                                        new SelectListItem { Text ="Indonesia", Value="ID" },
                                                        new SelectListItem { Text ="Iraq", Value="IQ" },
                                                        new SelectListItem { Text ="Ireland", Value="IE" },
                                                        new SelectListItem { Text ="Israel", Value="IL" },
                                                        new SelectListItem { Text ="Italy", Value="IT" },
                                                        new SelectListItem { Text ="Jamaica", Value="JM" },
                                                        new SelectListItem { Text ="Japan", Value="JP" },
                                                        new SelectListItem { Text ="Jordan", Value="JO" },
                                                        new SelectListItem { Text ="Kazakhstan", Value="KZ" },
                                                        new SelectListItem { Text ="Kenya", Value="KE" },
                                                        new SelectListItem { Text ="Kiribati", Value="KI" },
                                                        new SelectListItem { Text ="Korea, Republic of", Value="KR" },
                                                        new SelectListItem { Text ="Kosovo", Value="K1" },
                                                        new SelectListItem { Text ="Kuwait", Value="KW" },
                                                        new SelectListItem { Text ="Kyrghyz Republic", Value="KG" },
                                                        new SelectListItem { Text ="Laos", Value="LA" },
                                                        new SelectListItem { Text ="Latvia", Value="LV" },
                                                        new SelectListItem { Text ="Lebanon", Value="LB" },
                                                        new SelectListItem { Text ="Liberia", Value="LR" },
                                                        new SelectListItem { Text ="Libya", Value="LY" },
                                                        new SelectListItem { Text ="Liechtenstein", Value="LI" },
                                                        new SelectListItem { Text ="Lithuania", Value="LT" },
                                                        new SelectListItem { Text ="Luxembourg", Value="LU" },
                                                        new SelectListItem { Text ="Macau", Value="MO" },
                                                        new SelectListItem { Text ="Macedonia", Value="MK" },
                                                        new SelectListItem { Text ="Madagascar", Value="MG" },
                                                        new SelectListItem { Text ="Malawi", Value="MW" },
                                                        new SelectListItem { Text ="Malaysia", Value="MY" },
                                                        new SelectListItem { Text ="Maldives", Value="MV" },
                                                        new SelectListItem { Text ="Mali", Value="ML" },
                                                        new SelectListItem { Text ="Malta", Value="MT" },
                                                        new SelectListItem { Text ="Marshall Islands", Value="MH" },
                                                        new SelectListItem { Text ="Martinique", Value="MQ" },
                                                        new SelectListItem { Text ="Mauritania", Value="MR" },
                                                        new SelectListItem { Text ="Mauritius", Value="MU" },
                                                        new SelectListItem { Text ="Mayotte", Value="YT" },
                                                        new SelectListItem { Text ="Mexico", Value="MX" },
                                                        new SelectListItem { Text ="Micronesia", Value="FM" },
                                                        new SelectListItem { Text ="Moldova", Value="MD" },
                                                        new SelectListItem { Text ="Monaco", Value="MC" },
                                                        new SelectListItem { Text ="Mongolia", Value="MN" },
                                                        new SelectListItem { Text ="Montserrat", Value="MS" },
                                                        new SelectListItem { Text ="Morocco", Value="MA" },
                                                        new SelectListItem { Text ="Mozambique", Value="MZ" },
                                                        new SelectListItem { Text ="Nepal", Value="NP" },
                                                        new SelectListItem { Text ="Netherlands", Value="NL" },
                                                        new SelectListItem { Text ="New Caledonia", Value="NC" },
                                                        new SelectListItem { Text ="New Zealand", Value="NZ" },
                                                        new SelectListItem { Text ="Nicaragua", Value="NI" },
                                                        new SelectListItem { Text ="Niger", Value="NE" },
                                                        new SelectListItem { Text ="Nigeria", Value="NG" },
                                                        new SelectListItem { Text ="Niue", Value="NU" },
                                                        new SelectListItem { Text ="Northern Mariana Islands", Value="MP" },
                                                        new SelectListItem { Text ="Norway", Value="NO" },
                                                        new SelectListItem { Text ="Oman", Value="OM" },
                                                        new SelectListItem { Text ="Pakistan", Value="PK" },
                                                        new SelectListItem { Text ="Palau", Value="PW" },
                                                        new SelectListItem { Text ="Palestinian Authority", Value="P1" },
                                                        new SelectListItem { Text ="Panama", Value="PA" },
                                                        new SelectListItem { Text ="Papua New Guinea", Value="PG" },
                                                        new SelectListItem { Text ="Paraguay", Value="PY" },
                                                        new SelectListItem { Text ="Peru", Value="PE" },
                                                        new SelectListItem { Text ="Philippines", Value="PH" },
                                                        new SelectListItem { Text ="Poland", Value="PL" },
                                                        new SelectListItem { Text ="Portugal", Value="PT" },
                                                        new SelectListItem { Text ="Puerto Rico", Value="PR" },
                                                        new SelectListItem { Text ="Qatar", Value="QA" },
                                                        new SelectListItem { Text ="Reunion", Value="RE" },
                                                        new SelectListItem { Text ="Romania", Value="RO" },
                                                        new SelectListItem { Text ="Russia", Value="RU" },
                                                        new SelectListItem { Text ="Rwanda", Value="RW" },
                                                        new SelectListItem { Text ="Samoa", Value="WS" },
                                                        new SelectListItem { Text ="São Tomé and Príncipe", Value="ST" },
                                                        new SelectListItem { Text ="Saudi Arabia", Value="SA" },
                                                        new SelectListItem { Text ="Senegal", Value="SN" },
                                                        new SelectListItem { Text ="Serbia & Montenegro", Value="YU" },
                                                        new SelectListItem { Text ="Sierra Leone", Value="SL" },
                                                        new SelectListItem { Text ="Singapore", Value="SG" },
                                                        new SelectListItem { Text ="Slovakia", Value="SK" },
                                                        new SelectListItem { Text ="Slovenia", Value="SI" },
                                                        new SelectListItem { Text ="Solomon Islands", Value="SB" },
                                                        new SelectListItem { Text ="Spain", Value="ES" },
                                                        new SelectListItem { Text ="Sri Lanka", Value="LK" },
                                                        new SelectListItem { Text ="St. Kitts & Nevis", Value="KN" },
                                                        new SelectListItem { Text ="St. Lucia", Value="LC" },
                                                        new SelectListItem { Text ="St. Maarten", Value="S1" },
                                                        new SelectListItem { Text ="St. Vincent", Value="VC" },
                                                        new SelectListItem { Text ="Sudan", Value="SD" },
                                                        new SelectListItem { Text ="Suriname", Value="SR" },
                                                        new SelectListItem { Text ="Sweden", Value="SE" },
                                                        new SelectListItem { Text ="Switzerland", Value="CH" },
                                                        new SelectListItem { Text ="Syria", Value="SY" },
                                                        new SelectListItem { Text ="Taiwan", Value="TW" },
                                                        new SelectListItem { Text ="Tajikistan", Value="TJ" },
                                                        new SelectListItem { Text ="Tanzania", Value="TZ" },
                                                        new SelectListItem { Text ="Thailand", Value="TH" },
                                                        new SelectListItem { Text ="Togo", Value="TG" },
                                                        new SelectListItem { Text ="Tonga", Value="TO" },
                                                        new SelectListItem { Text ="Trinidad & Tobago", Value="TT" },
                                                        new SelectListItem { Text ="Tunisia", Value="TN" },
                                                        new SelectListItem { Text ="Turkey", Value="TR" },
                                                        new SelectListItem { Text ="Turkmenistan", Value="TM" },
                                                        new SelectListItem { Text ="Turks & Caicos Island", Value="TC" },
                                                        new SelectListItem { Text ="Tuvalu", Value="TV" },
                                                        new SelectListItem { Text ="Uganda", Value="UG" },
                                                        new SelectListItem { Text ="Ukraine", Value="UA" },
                                                        new SelectListItem { Text ="United Arab Emirates", Value="AE" },
                                                        new SelectListItem { Text ="United Kingdom", Value="GB" },
                                                        new SelectListItem { Text ="United States", Value="US" },
                                                        new SelectListItem { Text ="Uruguay", Value="UY" },
                                                        new SelectListItem { Text ="US Virgin Islands", Value="VI" },
                                                        new SelectListItem { Text ="Uzbekistan", Value="UZ" },
                                                        new SelectListItem { Text ="Vanuatu", Value="VU" },
                                                        new SelectListItem { Text ="Venezuela", Value="VE" },
                                                        new SelectListItem { Text ="Vietnam", Value="VN" },
                                                        new SelectListItem { Text ="Yemen", Value="YE" },
                                                        new SelectListItem { Text ="Zambia", Value="ZM" }

                                                                      });

                return items;
            }
        }
    }
}