using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace FormBuilder.Helpers
{
    /// <summary>
    /// Summary description for WebConfig.
    /// </summary>
    public class WebConfig
    {


        #region Properties
        #endregion

        #region Constructors
        public WebConfig()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        #endregion

        #region Methods
        public static string Get(string settingName)
        {
            try
            {
                return ConfigurationManager.AppSettings.Get(settingName);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Missing webconfig setting::" + settingName, ex);
            }

        }

        public static V Get<V>(string settingName, V valueIfNull)
        {
            try
            {
                object val = ConfigurationManager.AppSettings.Get(settingName);
                return (V)Convert.ChangeType(val, typeof(V));

            }
            catch
            {
                return valueIfNull;
            }
        }
        #endregion

    }
}