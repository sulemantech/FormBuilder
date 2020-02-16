using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FormBuilder.ViewModels;
using FormBuilder.BusinessObjects;
using System.Globalization;
using System.Text;
using FormBuilder.Helpers;
using System.Drawing;
using System.Configuration;
using System.IO;
using Amazon.S3;
using Amazon.S3.Model;
using System.Data.Objects.DataClasses;


namespace FormBuilder.Extensions
{
    public static class UtilityExtensions
    {
        public static void LoadOnce(this RelatedEnd entity)
        {
            if (!entity.IsLoaded)
            {
                entity.Load();
            }
        }              

        public static bool IsNullOrEmpty(this string target)
        {
            return string.IsNullOrEmpty(target);
        }

        public static void Each<T>(this IEnumerable<T> ie, Action<T, int> action)
        {
            var i = 0;

            foreach (var e in ie)
            {
                action(e, i++);
            }
        }

        public static string OutputIfFalse(this bool? target, string valueToOutput, string elseOutput = "")
        {
            if (!target.HasValue || target.Value == false)
            {
                return valueToOutput;
            }

            return elseOutput;
        }

        public static string OutputIfTrue(this bool target, string valueToOutput, string elseOutput = "")
        {
            if (target)
            {
                return valueToOutput;
            }

            return elseOutput;
        }

        public static string OutputIfFalse(this bool target, string valueToOutput, string elseOutput = "")
        {
            if (!target)
            {
                return valueToOutput;
            }

            return elseOutput;
        }
        
        
        public static IEnumerable<SelectListItem> InsertAtStart(this IList<SelectListItem> list, string itemName)
        {
            return new SelectList(new string[] { itemName }).Concat(list);
        }

        public static IEnumerable<SelectListItem> InsertAtStart(this IList<SelectListItem> list, string name, string value)
        {
            return new List<SelectListItem>(new[] { new SelectListItem { Value = value, Text = name } }).Concat(list);
        }

        public static IEnumerable<SelectListItem> InsertAtStart(this IEnumerable<SelectListItem> list, string itemName)
        {
            return new SelectList(new string[] { itemName }).Concat(list);
        }

        public static IEnumerable<SelectListItem> InsertAtStart(this IEnumerable<SelectListItem> list, string name, string value)
        {
            return new List<SelectListItem>(new[] { new SelectListItem { Value = value, Text = name } }).Concat(list);
        }

        #region FormExtensions

        public static IHtmlString CreateJson(this Form form)
        {
            return new HtmlString("");
        }

        public static string SubmittedFieldValue(this FormCollection form, int domId, string field)
        {
            return form[SubmittedFieldName(domId,field)];
        }

        public static string SubmittedFieldName(this FormFieldViewModel field)
        {
            return SubmittedFieldName(field.DomId, field.FieldType.ToString().ToTitleCase());
        }

        public static string SubmittedFieldName(int domId, string field)
        {
            return "SubmitFields[{0}].{1}".FormatWith(domId, field);
        }

        public static string FormFieldValue(this FormCollection form, int domId, string field)
        {
            return form["Fields[{0}].{1}".FormatWith(domId, field)];
        }

        public static bool IsEditMode(this FormFieldViewModel field)
        {
            return field.Mode == Constants.FormFieldMode.EDIT;
        }

        public static string ValidationId(this FormFieldViewModel field)
        {
            return "{0}-{1}".FormatWith(field.FieldType.ToString().ToLower(), field.Id.ToString());
        }

        public static void AssignInputValues(this FormViewModel evtForm, FormCollection form)
        {
            if (evtForm.Fields.Any())
            {
                evtForm.Fields.Each((field, index) =>
                {
                    field.InputValue = SubmittedValue(field, form);
                });
            }
        }

        public static bool SubmittedValueIsValid(this FormFieldViewModel field, FormCollection form)
        {
            var fType = field.FieldType.ToString().ToTitleCase();
            string value = "";
            switch (field.FieldType)
            {
                case Constants.FieldType.EMAIL:
                    value = form.SubmittedFieldValue(field.DomId, fType.ToTitleCase());
                    if (value.IsNullOrEmpty()) { return true; }
                    return value.IsValidEmail();
                case Constants.FieldType.ADDRESS:
                    return true;
                case Constants.FieldType.PHONE:
                    var area = form.SubmittedFieldValue(field.DomId, "AreaCode");
                    var number = form.SubmittedFieldValue(field.DomId, "Number");
                    if (area.IsNullOrEmpty() && number.IsNullOrEmpty())
                    {
                        return true;
                    }
                    else if ((area.IsNullOrEmpty() && !number.IsNullOrEmpty()) || (!area.IsNullOrEmpty() && number.IsNullOrEmpty()))
                    {
                        return false;
                    }
                    else
                    {
                        return area.IsNumeric() && number.IsNumeric();
                    }
                case Constants.FieldType.BIRTHDAYPICKER:
                    var day = form.SubmittedFieldValue(field.DomId, "Day");
                    var month = form.SubmittedFieldValue(field.DomId, "Month");
                    var year = form.SubmittedFieldValue(field.DomId, "Year");

                    if (day.IsNullOrEmpty() && month.IsNullOrEmpty() && year.IsNullOrEmpty())
                    {
                        return true;
                    }

                    var dateValue = "{0}-{1}-{2}".FormatWith(month, day, year);
                    var format = new string[] { "M-dd-yyyy" };
                    DateTime date;
                    return DateTime.TryParseExact(dateValue, "M-dd-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault, out date);
                case Constants.FieldType.FILEPICKER:
                    HttpPostedFile file = HttpContext.Current.Request.Files[SubmittedFieldName(field.DomId, fType.ToTitleCase())];
                    var maxSize = field.MaxFileSize * 1024;
                    var minSize = field.MinFileSize * 1024;
                    var validExtensions = field.ValidFileExtensions;

                    if (file != null && file.ContentLength > 0)
                    {
                        var extension = System.IO.Path.GetExtension(file.FileName);
                        // check filesize is within range of Max and Min
                        if (!(file.ContentLength >= minSize && file.ContentLength <= maxSize))
                        {
                            return false;
                        }

                        // check file extension is valid
                        if (!validExtensions.IsNullOrEmpty())
                        {
                            var validExtensionArr = validExtensions.Split(",").Select(ext => ext.Trim()).ToList();
                            bool isValidExt = false;
                            foreach (var ext in validExtensionArr)
                            {
                                var updatedExt = ext;
                                if (!ext.StartsWith("."))
                                {
                                    updatedExt = "." + ext;
                                }

                                if (updatedExt.IsTheSameAs(extension))
                                {
                                    isValidExt = true;
                                }
                            }

                            return isValidExt;
                        }
                    }

                    return true;
            }

            return true;
        }        

        public static string Format(this FormFieldValueViewModel value, bool stripHtml = false)
        {
            switch (value.FieldType)
            {
                case Constants.FieldType.BIRTHDAYPICKER:
                    DateTime dateVal;
                    if (!string.IsNullOrEmpty(value.Value) && DateTime.TryParse(value.Value, out dateVal))
                    {
                        return dateVal.ToString("dddd, MMMM dd, yyyy");
                    }
                    return value.Value.ToString("--");
                case Constants.FieldType.ADDRESS:
                    AddressViewModel address;
                    if (!string.IsNullOrEmpty(value.Value) && value.Value.Length > 0)
                    {
                        address = value.Value.FromJson<AddressViewModel>();
                        return address.Format();
                    }
                    return "--";
                case Constants.FieldType.CHECKBOX:
                    if (!string.IsNullOrEmpty(value.Value) && value.Value.Length > 0)
                    {
                        if (!stripHtml)
                        {
                            var values = value.Value.Split(',');
                            StringBuilder sb = new StringBuilder("<ul class=\"vertical-list selected-checkbox-list\"");
                            values.Each((val, index) =>
                            {
                                sb.AppendFormat("<li>{0}</li>", val);
                            });
                          return sb.ToString();
                        }
                        else
                        {
                            return value.Value;
                        }
                    }
                    return "";
                case Constants.FieldType.FILEPICKER:

                    if (!stripHtml)
                    {
                        if (!string.IsNullOrEmpty(value.Value))
                        {
                            var fileValueObject = value.Value.FromJson<FileValueObject>();

                            if (!string.IsNullOrEmpty(fileValueObject.FileName))
                            {
                            var imagePreviewClass = "";
                            var imagePreviewAttribute = "";
                            var downloadPath = "/forms/file/download/{0}".FormatWith(value.Id);
                            if (fileValueObject.IsImage())
                            {
                                imagePreviewClass = "img-tip";
                                if (fileValueObject.IsSavedInCloud)
                                {
                                    imagePreviewAttribute = "data-image-path='http://{0}.s3.amazonaws.com/{1}'".FormatWith(WebConfig.Get("awsbucket"),fileValueObject.SaveName);
                                }
                                else
                                {                                    
                                    imagePreviewAttribute = "data-image-path='{0}'".FormatWith(fileValueObject.ImageViewPath());
                                }
                            }


                            StringBuilder sb = new StringBuilder();
                            sb.Append("<ul class='horizontal-list'><li class='file-icon-item'>");
                            sb.AppendFormat("<a href='{0}' class='{1}' {2}>", downloadPath, imagePreviewClass, imagePreviewAttribute);
                            sb.AppendFormat("<img src='/content/images/spacer.gif' class='image-bg fm-file-icon fm-file-{0}-icon' alt='file icon' />", fileValueObject.Extension.Replace(".", ""));
                            sb.AppendFormat("</a>");
                            sb.Append("</li><li class='file-name-item'>");
                            sb.AppendFormat("<a href='{0}'>{1}</a>", downloadPath, fileValueObject.FileName.LimitWithElipses(30));
                            sb.Append("</li></ul>");
                            return sb.ToString();
                            }
                        }

                        return "";
                    }
                    else {

                        if (!string.IsNullOrEmpty(value.Value))
                        {
                            var fileValueObject = value.Value.FromJson<FileValueObject>();
                            return fileValueObject.FileName;
                        }
                        else 
                        {
                            return "";
                        }
                    
                    }
                

            }

            return value.Value;
        }

        public static bool IsImage(this FileValueObject obj){

            try
            {
                if(obj.IsSavedInCloud){
                    AmazonS3 client = UtilityHelper.InitS3Client();
                    GetObjectRequest request = new GetObjectRequest();
                    request.BucketName = WebConfig.Get("awsbucket");
                    request.Key = obj.SaveName;
                    GetObjectResponse response = client.GetObject(request);
                    Image.FromStream(response.ResponseStream);
                }else{
                    Image.FromFile(HttpContext.Current.Server.MapPath(obj.FullFilePath()));
                }
                return true;
            }
            catch {
                return false;
            }
        
        }

         public static string FullFilePath(this FileValueObject obj){

             var path = "";
             if (obj != null) {
                 path =  HttpContext.Current.Server.MapPath(obj.SavePath).ConcatWith("\\", obj.SaveName);

                 return path;
             }

             return "";        
        }

         public static string ImageViewPath(this FileValueObject obj)
         {
             if (obj != null)
             {
                 return "/FileUploads/".ConcatWith(obj.SaveName);
             }

             return "";
         }

        public static bool IsEmptyOrWhiteSpace(this string target)
        {
            return string.IsNullOrEmpty(target) || string.IsNullOrWhiteSpace(target);
        }

        public static string SubmittedValue(this FormFieldViewModel field, FormCollection form)
        {
            var fType = field.FieldType.ToString().ToTitleCase();
            string value = "";
            switch (field.FieldType)
            {
                case Constants.FieldType.EMAIL:
                    value = form.SubmittedFieldValue(field.DomId, fType.ToTitleCase());
                    break;
                case Constants.FieldType.ADDRESS:
                    var address = new AddressViewModel
                    {
                        Address1 = form.SubmittedFieldValue(field.DomId, "StreetAddress"),
                        Address2 = form.SubmittedFieldValue(field.DomId, "StreetAddress2"),
                        City = form.SubmittedFieldValue(field.DomId, "City"),
                        State = form.SubmittedFieldValue(field.DomId, "State"),
                        ZipCode = form.SubmittedFieldValue(field.DomId, "ZipCode"),
                        Country = form.SubmittedFieldValue(field.DomId, "Country")

                    };

                    if (address.Address1.IsEmptyOrWhiteSpace() && address.City.IsEmptyOrWhiteSpace() && address.Country.IsEmptyOrWhiteSpace())
                    {
                        value = "";
                    }
                    else
                    {
                        value = address.ToJson();
                    }
                    break;
                case Constants.FieldType.BIRTHDAYPICKER:
                    var day = form.SubmittedFieldValue(field.DomId, "Day");
                    var month = form.SubmittedFieldValue(field.DomId, "Month");
                    var year = form.SubmittedFieldValue(field.DomId, "Year");

                    if (day.IsNullOrEmpty() && month.IsNullOrEmpty() && year.IsNullOrEmpty())
                    {
                        value = "";
                    }
                    else
                    {
                        var dateValue = "{0}-{1}-{2}".FormatWith(month, day, year);
                        var format = new string[] { "M-dd-yyyy" };
                        DateTime date;
                        if (DateTime.TryParseExact(dateValue, "M-dd-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault, out date))
                        {
                            value = date.ToString();
                        }
                        else
                        {
                            value = "";
                        }

                    }
                    break;
                case Constants.FieldType.CHECKBOX:
                    value = form.SubmittedFieldValue(field.DomId, fType.ToTitleCase());
                    break;
                case Constants.FieldType.PHONE:
                    var area = form.SubmittedFieldValue(field.DomId, "AreaCode");
                    var number = form.SubmittedFieldValue(field.DomId, "Number");
                    if (string.IsNullOrEmpty(area) && string.IsNullOrEmpty(number))
                    {
                        value = "";
                    }
                    else
                    {
                        value = "{0}-{1}".FormatWith(area, number);
                    }
                    break;
                case Constants.FieldType.DROPDOWNLIST:
                    value = form.SubmittedFieldValue(field.DomId, fType.ToTitleCase());
                    break;
                case Constants.FieldType.RADIOBUTTON:
                    value = form.SubmittedFieldValue(field.DomId, fType.ToTitleCase());
                    break;
                case Constants.FieldType.FULLNAME:
                    var fName = form.SubmittedFieldValue(field.DomId, "FirstName");
                    var lName = form.SubmittedFieldValue(field.DomId, "LastName");
                    var initials = form.SubmittedFieldValue(field.DomId, "Initials");
                    if (string.IsNullOrEmpty(fName) && string.IsNullOrEmpty(lName))
                    {
                        value = "";
                    }
                    else
                    {
                        value = "{0} {1} {2}".FormatWith(fName, initials, lName);
                    }
                    break;
                case Constants.FieldType.TEXTAREA:
                    value = form.SubmittedFieldValue(field.DomId, fType.ToTitleCase());
                    break;
                case Constants.FieldType.TEXTBOX:
                    value = form.SubmittedFieldValue(field.DomId, fType.ToTitleCase());
                    break;
                case Constants.FieldType.FILEPICKER:
                    HttpPostedFile file = HttpContext.Current.Request.Files[SubmittedFieldName(field.DomId, fType.ToTitleCase())];
                    value = "";
                    if (file != null && file.ContentLength > 0)
                    {
                        var extension = Path.GetExtension(file.FileName);

                        var valueObject = new FileValueObject()
                        {
                            FileName=file.FileName,                            
                            SaveName = Guid.NewGuid().ToString() + extension,
                            SavePath = WebConfig.Get("filesavepath"),
                            IsSavedInCloud = UtilityHelper.UseCloudStorage(),
                            Extension=extension
                        };

                        value = valueObject.ToJson();
                    }
                    break;

            }

            return value;
        }

        public static FileValueObject GetFileValueFromJsonObject(this string value)
        {
            try
            {
                return value.FromJson<FileValueObject>();
            }
            catch{
                return null;
            }
        }

        public static void SetFieldErrors(this FormFieldViewModel field)
        {
            field.Errors = "Invalid entry submitted for {0}".FormatWith(field.Label);
            if (field.FieldType == Constants.FieldType.FILEPICKER)
            {
                field.Errors = field.Errors.ConcatWith(", file must be betweeen {0}kb and {1}kb large ".FormatWith(field.MinFileSize.ToString(), field.MaxFileSize.ToString()));
                if (!string.IsNullOrEmpty(field.ValidFileExtensions))
                {
                    field.Errors = field.Errors.ConcatWith(" and have the extensions {0}.".FormatWith(field.ValidFileExtensions));
                }
                else {
                    field.Errors = field.Errors.ConcatWith(".");
                }
            }
        }

        // for generic interface IEnumerable<T>
        public static string ToString<T>(this IEnumerable<T> source, string separator)
        {
            if (source == null)
                throw new ArgumentException("Parameter source can not be null.");

            if (string.IsNullOrEmpty(separator))
                throw new ArgumentException("Parameter separator can not be null or empty.");

            string[] array = source.Where(n => n != null).Select(n => n.ToString()).ToArray();

            return string.Join(separator, array);
        }

        public static string Format(this AddressViewModel addr)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0},", addr.Address1);

            if (!addr.Address2.IsNullOrEmpty())
            {
                sb.AppendFormat(" {0},", addr.Address2);
            }

            sb.AppendFormat(" {0}, ", addr.City);
            sb.AppendFormat(" {0}", addr.Country);

            return sb.ToString();
        }

        #endregion
        
    }
}