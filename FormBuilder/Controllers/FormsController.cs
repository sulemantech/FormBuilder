using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using FormBuilder.ViewModels;
using FormBuilder.BusinessObjects;
using FormBuilder.Repositories;
using FormBuilder.Extensions;
using System.Data;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using System.Configuration;
using System.Web.Hosting;
using Amazon.S3.Model;
using FormBuilder.Helpers;
using Amazon.S3;

namespace FormBuilder.Controllers
{
    public class FormsController : Controller
    {
        //
        // GET: /Forms/
        private FormRepository _formRepo { get; set; }

        public FormsController()
            : this(new FormRepository())
        {

        }

        public FormsController(FormRepository formRepo)
        {
            this._formRepo = formRepo;
        }

        public ActionResult Index()
        {
            var formCollectionView = new FormCollectionViewModel();
            formCollectionView.Forms = this._formRepo.GetForms().OrderByDescending(f => f.DateAdded).ToList();
            return View(formCollectionView);
        }


        public ActionResult Edit(int id)
        {

            FormViewModel model = null;
            var form = this._formRepo.GetByPrimaryKey(id);
            model = FormViewModel.CreateFromObject(form);
            return View(model);
        }

        public ActionResult Create()
        {
            var form = this._formRepo.CreateNew();

            return RedirectToAction("edit", new { id = form.ID });
        }



        [HttpPost]
        public ActionResult Update(bool isAutoSave, FormViewModel model, FormCollection collection, IDictionary<string, string> Fields)
        {

            if (!model.Id.HasValue)
            {
                return Json(new { success = false, error = "Unable to save changes. A valid form was not detected.", isautosave = isAutoSave });
            }

            var form = this._formRepo.GetByPrimaryKey(model.Id.Value);


            if (Fields == null)
            {
                return Json(new { success = false, error = "Unable to detect field values.", isautosave = isAutoSave });
            }

            if (!model.NotificationEmail.IsNullOrEmpty() && !model.NotificationEmail.IsValidEmail())
            {
                return Json(new { success = false, error = "Invalid format for Notification Email.", isautosave = isAutoSave });
            }

            try
            {
                // first update the form metadata
                this._formRepo.Update(model, form);

                // then if fields were passed in, update them
                if (Fields.Count() > 0)
                {
                    foreach (var kvp in Fields)
                    {

                        var domId = Convert.ToInt32(kvp.Key);
                        if (domId >= 0)
                        {
                            var fieldType = collection.FormFieldValue(domId, "FieldType");
                            var fieldId = collection.FormFieldValue(domId, "Id");
                            var minAge = collection.FormFieldValue(domId, "MinimumAge").IsInt(18);
                            var maxAge = collection.FormFieldValue(domId, "MaximumAge").IsInt(100);

                            if (minAge >= maxAge)
                            {
                                minAge = 18;
                                maxAge = 100;
                            }

                            var fieldTypeEnum = (Constants.FieldType)Enum.Parse(typeof(Constants.FieldType), fieldType.ToUpper());
                            var fieldView = new FormFieldViewModel
                            {
                                DomId = Convert.ToInt32(domId),
                                FieldType = fieldTypeEnum,
                                MaxCharacters = collection.FormFieldValue(domId, "MaxCharacters").IsInt(),
                                Text = collection.FormFieldValue(domId, "Text"),
                                Label = collection.FormFieldValue(domId, "Label"),
                                IsRequired = collection.FormFieldValue(domId, "IsRequired").IsBool(),
                                Options = collection.FormFieldValue(domId, "Options"),
                                SelectedOption = collection.FormFieldValue(domId, "SelectedOption"),
                                HoverText = collection.FormFieldValue(domId, "HoverText"),
                                Hint = collection.FormFieldValue(domId, "Hint"),
                                MinimumAge = minAge,
                                MaximumAge = maxAge,
                                HelpText = collection.FormFieldValue(domId, "HelpText"),
                                SubLabel = collection.FormFieldValue(domId, "SubLabel"),
                                Size = collection.FormFieldValue(domId, "Size"),
                                Columns = collection.FormFieldValue(domId, "Columns").IsInt(20),
                                Rows = collection.FormFieldValue(domId, "Columns").IsInt(2),
                                Validation = collection.FormFieldValue(domId, "Validation"),
                                Order = collection.FormFieldValue(domId, "Order").IsInt(1),
                                MaxFileSize = collection.FormFieldValue(domId, "MaxFileSize").IsInt(5000),
                                MinFileSize = collection.FormFieldValue(domId, "MinFileSize").IsInt(5),
                                ValidFileExtensions = collection.FormFieldValue(domId, "ValidExtensions"),

                            };

                            if (!fieldId.IsNullOrEmpty() && fieldId.IsInteger())
                            {
                                fieldView.Id = Convert.ToInt32(fieldId);
                            }

                            this._formRepo.UpdateField(form, fieldView);
                        }
                    }
                }


                form.FormFields.Load();
                var fieldOrderById = form.FormFields.Select(ff => new { domid = ff.DomId, id = ff.ID });

                return Json(new { success = true, message = "Your changes were saved.", isautosave = isAutoSave, fieldids = fieldOrderById });


            }
            catch (Exception ex)
            {
                //TODO: log error
                var error = "Unable to save form ".AppendIfDebugMode(ex.ToString());
                return Json(new { success = false, error = error, isautosave = isAutoSave });
            }

        }

        public ActionResult Delete(int formId)
        {
            var form = this._formRepo.GetByPrimaryKey(formId);
            var formView = FormViewModel.CreateFromObject(form);

            if (form != null)
            {
                formView.Entries = this._formRepo.GetRegistrantsByForm(formView).ToList();
                if (!formView.Entries.Any())
                {
                    try
                    {
                        this._formRepo.DeleteForm(formId);
                        TempData["success"] = "Form Deleted";
                        return RedirectToRoute("form-home");
                    }
                    catch (Exception ex)
                    {
                        TempData["error"] = "Unable to delete form - forms must have no entries to be able to be deleted";
                    }
                }
            }

            TempData["error"] = "Unable to delete form - forms must have no entries to be able to be deleted";
            return RedirectToRoute("form-home");
        }

        [HttpPost]
        public ActionResult DeleteField(int? fieldid)
        {
            if (fieldid.HasValue)
            {
                this._formRepo.DeleteField(fieldid.Value);
                return Json(new { success = true, message = "Field was deleted." });
            }

            return Json(new { success = false, message = "Unable to delete field." });
        }

        public ActionResult TogglePublish(bool toOn, int id)
        {
            var form = this._formRepo.GetByPrimaryKey(id);

            if (form.FormFields.Count() > 0)
            {
                form.Status = toOn ? Constants.FormStatus.PUBLISHED.ToString() : Constants.FormStatus.DRAFT.ToString();
                this._formRepo.SaveChanges();
                if (toOn)
                {
                    TempData["success"] = "This electronic form has been published and is now live";
                }
                else
                {
                    TempData["success"] = "This electronic form is now offline";
                }
            }
            else
            {
                TempData["error"] = "Cannot publish form until fields have been added.";
            }


            return RedirectToAction("edit", new { id = form.ID });
        }


        public void InsertValuesIntoTempData(IDictionary<string, string> submittedValues, FormCollection form)
        {
            foreach (var key in form.AllKeys)
            {
                ViewData[key.ToLower()] = form[key];
            }

        }

        //[SSl]
        public ActionResult Register(int id, bool embed=false)
        {
            FormViewModel model = null;
            var form = this._formRepo.GetByPrimaryKey(id);

            if (form != null)
            {
                model = FormViewModel.CreateFromObject(form, Constants.FormFieldMode.INPUT);
                model.Embed = embed;
            }
            else
            {
                return RedirectToAction("edit", new { id = form.ID });
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Register(IDictionary<string, string> SubmitFields, FormViewModel model, FormCollection form)
        {
            IList<string> errors = Enumerable.Empty<string>().ToList();
            var formObj = this._formRepo.GetByPrimaryKey(model.Id.Value);
            var formView = FormViewModel.CreateFromObject(formObj, Constants.FormFieldMode.INPUT);
            formView.AssignInputValues(form);
            this.InsertValuesIntoTempData(SubmitFields, form);


            if (formView.Fields.Any())
            {
                // first validate fields
                foreach (var field in formView.Fields)
                {
                    var valId = field.ValidationId();
                    if (!field.SubmittedValueIsValid(form))
                    {
                        field.SetFieldErrors();
                        errors.Add(field.Errors);
                        goto Error;
                    }

                    var value = field.SubmittedValue(form);
                    if (field.IsRequired && value.IsNullOrEmpty())
                    {
                        field.Errors = "{0} is a required field".FormatWith(field.Label);
                        errors.Add(field.Errors);
                        goto Error;
                    }
                };

                //then insert values
                var entryId = Guid.NewGuid();
                var notificationView = new NotificationEmailViewModel();
                notificationView.FormName = formView.Title;
                IDictionary<string, FormFieldValueViewModel> notificationEntries = new Dictionary<string, FormFieldValueViewModel>();
                foreach (var field in formView.Fields)
                {
                    var value = field.SubmittedValue(form);

                    //if it's a file, save it to hard drive
                    if (field.FieldType == Constants.FieldType.FILEPICKER && !string.IsNullOrEmpty(value))
                    {
                        var file = Request.Files[field.SubmittedFieldName()];
                        var fileValueObject = value.GetFileValueFromJsonObject();

                        if (fileValueObject != null)
                        {
                            if (UtilityHelper.UseCloudStorage())
                            {
                                this.SaveImageToCloud(file, fileValueObject.SaveName);
                            }
                            else
                            {
                                file.SaveAs(Path.Combine(HostingEnvironment.MapPath(fileValueObject.SavePath), fileValueObject.SaveName));
                            }
                        }                       
                    }

                    this.AddValueToDictionary(ref notificationEntries, field.Label, new FormFieldValueViewModel(field.FieldType, value));
                    notificationView.Entries = notificationEntries;
                    this._formRepo.InsertFieldValue(field, value, entryId);
                }

                //send notification
                if (!formView.NotificationEmail.IsNullOrEmpty() && WebConfig.Get<bool>("enablenotifications",true))
                {
                    notificationView.Email = formView.NotificationEmail;
                    this.NotifyViaEmail(notificationView);
                }

                TempData["success"] = formView.ConfirmationMessage;
                return RedirectToRoute("form-confirmation", new
                {
                    id = formObj.ID,
                    embed = model.Embed
                });

            }

        Error:
            TempData["error"] = errors.ToUnorderedList();
            return View("Register", formView);
        }

        public ActionResult FormConfirmation(int id, bool? embed)
        {

            var form = this._formRepo.GetByPrimaryKey(id);
            if (form != null)
            {
                var formView = FormViewModel.CreateFromObject(form);
                formView.Embed = embed ?? embed.Value;
                TempData["success"] = formView.ConfirmationMessage;
                return View(formView);
            }

            return RedirectToAction("Index", "Error");
        }

        public ActionResult ViewEntries(int formId)
        {
            var form = this._formRepo.GetByPrimaryKey(formId);
            var formView = FormViewModel.CreateFromObject(form);

            formView.Entries = this._formRepo.GetRegistrantsByForm(formView).ToList();
            formView.GroupedEntries = formView.Entries.GroupBy(g => g.EntryId);

            return View(formView);
        }

        private void NotifyViaEmail(NotificationEmailViewModel model)
        {
            EmailSender emailSender = new EmailSender("MailTemplates", false);
            var submimssionDetail = this.RenderPartialViewToString("_SubmissionEmailPartial", model);
            emailSender.SendSubmissionNotificationEmail(model.Email, "New Submission for form \"{0}\"".FormatWith(model.FormName), submimssionDetail);            
        }


        private void AddValueToDictionary(ref IDictionary<string, FormFieldValueViewModel> collection, string key, FormFieldValueViewModel value)
        {
            if (collection.ContainsKey(key))
            {
                var newKey = "";
                int counter = 2;
                do
                {
                    newKey = "{1} {0}".FormatWith(key, counter);
                    counter++;

                } while (collection.ContainsKey(newKey));

                collection.Add(newKey, value);
            }
            else {
                collection.Add(key, value);
            }
        }

        private void SaveImageToCloud(HttpPostedFileBase file, string fileName)
        {
            AmazonS3 client = UtilityHelper.InitS3Client();
            PutObjectRequest request = new PutObjectRequest();
            request.WithBucketName(WebConfig.Get("awsbucket"));
            request.CannedACL = S3CannedACL.PublicReadWrite;
            request.ContentType = file.ContentType;
            request.Key = fileName;
            request.InputStream = file.InputStream;
            S3Response response = client.PutObject(request);
            response.Dispose();
        }

        public FileStreamResult GetFileFromDisk(int valueId)
        {
            FileValueObject obj = _formRepo.GetFileFieldValue(valueId);
            if (obj != null)
            {
                if (obj.IsSavedInCloud)
                {
                    AmazonS3 client = UtilityHelper.InitS3Client();
                    GetObjectRequest request = new GetObjectRequest();
                    request.BucketName = WebConfig.Get("awsbucket");
                    request.Key = obj.SaveName;
                    GetObjectResponse response = client.GetObject(request);
                    return File(response.ResponseStream, System.Net.Mime.MediaTypeNames.Application.Octet, obj.FileName);

                }
                else
                {
                    var filePath = Server.MapPath(obj.SavePath.ConcatWith("/", obj.SaveName));
                    var stream = UtilityHelper.ReadFile(filePath);
                    return File(stream, System.Net.Mime.MediaTypeNames.Application.Octet, obj.FileName);
                }
            }

            throw new Exception("File Not Found");
        }

        public ActionResult ExportToExcel(int formId)
        {
            var form = this._formRepo.GetByPrimaryKey(formId);
            var formView = FormViewModel.CreateFromObject(form);

            formView.Entries = this._formRepo.GetRegistrantsByForm(formView).ToList();
            formView.GroupedEntries = formView.Entries.GroupBy(g => g.EntryId);

            var gridView = new GridView();
            gridView.DataSource = this.CreateFormEntriesDataTable(formView);
            gridView.DataBind();

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename={0}.xls".FormatWith(form.Title.ToSlug()));
            Response.ContentType = "application/vnd.ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gridView.RenderControl(htw);
            Response.Write(sw.ToString());
            Response.End();

            return RedirectToRoute("form-entries", new { formid = formId });

        }

        private DataTable CreateFormEntriesDataTable(FormViewModel form)
        {
            var dt = new DataTable(form.Title);
            List<string> columnNames = new List<string>();
            int columnCount = 0;
            foreach (var field in form.GroupedEntries.FirstOrDefault())
            {
                if (field.FieldType != Constants.FieldType.HEADER)
                {
                    var colName = field.FieldLabel;
                    if (columnNames.Any(cn => cn.IsTheSameAs(colName)))
                    {
                        int colNumber = 1;
                        do
                        {
                            colName = string.Format("{0} ({1})", colName, colNumber);
                            colNumber++;

                        } while (columnNames.Any(cn => cn.IsTheSameAs(colName)));
                    }
                    columnNames.Add(colName);
                    dt.Columns.Add(new DataColumn(colName));
                }
                columnCount++;
            }

            dt.Columns.Add(new DataColumn("Submitted On"));

            foreach (var group in form.GroupedEntries)
            {
                DataRow row = dt.NewRow();
                var fieldAddedOn = group.FirstOrDefault().DateAdded;
                int columnIndex = 0;
                for (columnIndex = 0; columnIndex < columnCount; columnIndex++)
                {
                    if (columnIndex < group.Count())
                    {
                        var field = group.ElementAt(columnIndex);
                        row[columnIndex] = field.Format(true);
                    }
                    else
                    {
                        row[columnIndex] = "";
                    }
                }
                row[columnIndex] = fieldAddedOn.ToString("ddd, MMMM dd, yyyy");
                dt.Rows.Add(row);
            }

            return dt;
        }

        [HttpPost]
        public ActionResult DeleteEntries(IEnumerable<string> selectedEntries, FormViewModel model)
        {

            var form = this._formRepo.GetByPrimaryKey(model.Id.Value);
            var formView = FormViewModel.CreateFromObject(form);

            try
            {
                if (selectedEntries != null && selectedEntries.Any())
                {
                    this._formRepo.DeleteEntries(selectedEntries);
                    TempData["success"] = "The selected entries were deleted";
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = "An error occured while deleting entries. Try again later.";
            }

            return RedirectToRoute("form-entries", new { formid = model.Id.Value });

        }

        public ActionResult Preview(int id)
        {
            FormViewModel model = null;
            var form = this._formRepo.GetByPrimaryKey(id);

            if (form != null)
            {
                model = FormViewModel.CreateFromObject(form, Constants.FormFieldMode.INPUT);
                
            }
            else
            {
                return RedirectToAction("edit", new { id = form.ID });
            }

            return View(model);            
        }

        protected string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        public ActionResult Error()
        {
            return View();
        }

    }
}
