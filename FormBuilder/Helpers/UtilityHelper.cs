using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Amazon.S3;
using Amazon.S3.Model;

namespace FormBuilder.Helpers
{
    public class UtilityHelper
    {
        public static bool IsDebugMode()
        {
            if (HttpContext.Current == null)
            {
                return false;
            }
            return HttpContext.Current.IsDebuggingEnabled;
        }

        public static IEnumerable<SelectListItem> NumberRange(int minimum, int maximum)
        {
            return Enumerable.Range(minimum, (maximum - minimum) + 1).Reverse().Select(v => new SelectListItem { Value = v.ToString(), Text = v.ToString() });
        }

        public static IEnumerable<SelectListItem> ThemeFolderList()
        {
            var directoryList = Directory.GetDirectories(HttpContext.Current.Server.MapPath("/Content/css/form-builder/themes/")).ToList();
            
            directoryList.Insert(0,"");

            if (directoryList != null && directoryList.Any())
            {
                return directoryList.Select(d => new SelectListItem { Value = d.Split('\\').LastOrDefault(), Text = d.Split('\\').LastOrDefault() });
            }

            return new string[] { "[No Themes Available]" }.Select(t => new SelectListItem { Value="", Text=t });
        }

        /// <summary>
        /// Gets the absolute root of the website.
        /// </summary>
        /// <value>A string that ends with a '/'.</value>
        public static Uri AbsoluteWebRoot(bool forceConfig = false)
        {
            if (System.Web.HttpContext.Current == null || forceConfig)
            {
                return new Uri(WebConfig.Get("siteroot"));
            }

            if (string.Compare(WebConfig.Get("Application"), "LOCAL", true) == 0)
            {
                return new Uri(System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority + System.Web.HttpContext.Current.Request.ApplicationPath);
            }
            else
            {
                return new Uri(System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority);
            }
        }

        public static AmazonS3 InitS3Client()
        {
            string accessKeyID = WebConfig.Get("awsaccesskey");
            string secretAccessKeyID = WebConfig.Get("awssecretkey");
            AmazonS3Config config = new AmazonS3Config();
            config.CommunicationProtocol = Protocol.HTTP;
            return Amazon.AWSClientFactory.CreateAmazonS3Client(accessKeyID.Trim(), secretAccessKeyID.Trim(), config);
        }

        public static bool UseCloudStorage()
        {
            return WebConfig.Get<bool>("usecloudstorage", true);
        }

        public static FileStream ReadFile(string filePath)
        {            
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);          
            return fileStream;
        }

        public static void RemoveFileFromBucket(string fileName)
        {
            // remove file from S3
            var client = InitS3Client();
            DeleteObjectRequest request = new DeleteObjectRequest();
            request.WithBucketName(WebConfig.Get("awsbucket"));
            request.Key = fileName;
            S3Response response = client.DeleteObject(request);
            response.Dispose();
        }
    }
}