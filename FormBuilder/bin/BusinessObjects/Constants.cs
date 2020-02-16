using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FormBuilder.BusinessObjects
{
    public class Constants
    {
        public static string FILESAVEPATHKEY = "filesavepath";
        public static string FILESAVENAMEKEY = "filesavename";
        public static string DEAULTFILEEXTENSIONS = ".jpg,.png,.gif,.pdf,.bmp,.zip";
        public static int DEAULTMAXFILESIZEINKB = 5000;
        public static int DEAULTMINFILESIZEINKB = 10;

        public enum FormStatus
        {
            DRAFT,
            PUBLISHED
        }

        public enum FormFieldMode
        {
            EDIT,
            INPUT
        }

        public enum FieldType
        {
            TEXTBOX,
            TEXTAREA,
            RADIOBUTTON,
            PASSWORD,
            CHECKBOX,
            H1,
            HEADER,
            DROPDOWNLIST,
            FULLNAME,
            EMAIL,
            ADDRESS,
            PHONE,
            BIRTHDAYPICKER,
            FILEPICKER
        }

    }
}