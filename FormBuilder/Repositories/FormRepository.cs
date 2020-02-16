using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FormBuilder.BusinessObjects;
using FormBuilder.ViewModels;
using FormBuilder.Extensions;
using System.IO;
using FormBuilder.Helpers;
using System.Data.Objects;

namespace FormBuilder.Repositories
{
    public class FormRepository : BaseRespository<Form, int>
    {

        FormBuilderEntities db;

        public FormRepository(FormBuilderEntities datacontext)
            : base(datacontext)
        {

        }

        public FormRepository()
            : this(new FormBuilderEntities())
        { }

        public override ObjectSet<Form> EntitySet
        {
            get { return this.DataContext.Forms; }
        }

        protected override Form ConvertToNativeEntity(Form entity)
        {
            return entity;
        }

        protected override int SelectPrimaryKey(Form entity)
        {
            return entity.ID;
        }

        public override Form GetByPrimaryKey(int key)
        {
            return this.GetByPrimaryKey(s => s.ID == key);
        }

        public void UpdateField(Form form, FormFieldViewModel fieldView)
        {
            if (form == null)
            {
                throw new Exception("Cannot update a field when a form is null");
            }

            if (!fieldView.Id.HasValue)
            {
                // create
                var fField = new FormField
                {
                    DomId = fieldView.DomId,
                    Label = fieldView.Label.LimitWithElipses(40),
                    Text = fieldView.Text.Trim(),
                    FieldType = fieldView.FieldType.ToString(),
                    IsRequired = fieldView.IsRequired,
                    MaxChars = fieldView.MaxCharacters,
                    HoverText = fieldView.HoverText.Trim(),
                    Hint = fieldView.Hint.Trim(),
                    SubLabel = fieldView.SubLabel.Trim(),
                    Size = fieldView.Size,
                    Columns = fieldView.Columns,
                    Rows = fieldView.Rows,
                    Options = fieldView.Options,
                    SelectedOption = fieldView.SelectedOption,
                    HelpText = fieldView.HelpText.Trim(),
                    Validation = fieldView.Validation,
                    Order = fieldView.Order,
                    MinimumAge = fieldView.MinimumAge,
                    MaximumAge = fieldView.MaximumAge,
                    MaxFilesizeInKb = fieldView.MaxFileSize,
                    MinFilesizeInKb = fieldView.MinFileSize,
                    ValidFileExtensions = fieldView.ValidFileExtensions,
                    DateAdded = DateTime.UtcNow
                };

                form.FormFields.LoadOnce();
                form.FormFields.Add(fField);
                this.SaveChanges();
            }
            else
            {
                var fField = this.DataContext.FormFields.Where(field => field.ID == fieldView.Id.Value).FirstOrDefault();
                if (fField != null)
                {

                    fField.Label = fieldView.Label.LimitWithElipses(45);
                    fField.Text = fieldView.Text.Trim();
                    fField.FieldType = fieldView.FieldType.ToString();
                    fField.IsRequired = fieldView.IsRequired;
                    fField.MaxChars = fieldView.MaxCharacters;
                    fField.HoverText = fieldView.HoverText.Trim();
                    fField.Hint = fieldView.Hint.Trim();
                    fField.SubLabel = fieldView.SubLabel.Trim();
                    fField.Size = fieldView.Size;
                    fField.Columns = fieldView.Columns;
                    fField.Rows = fieldView.Rows;
                    fField.Options = fieldView.Options;
                    fField.SelectedOption = fieldView.SelectedOption;
                    fField.HelpText = fieldView.HelpText.Trim();
                    fField.Validation = fieldView.Validation;
                    fField.Order = fieldView.Order;
                    fField.MinimumAge = fieldView.MinimumAge;
                    fField.MaximumAge = fieldView.MaximumAge;
                    fField.MaxFilesizeInKb = fieldView.MaxFileSize;
                    fField.MinFilesizeInKb = fieldView.MinFileSize;
                    fField.ValidFileExtensions = fieldView.ValidFileExtensions;
                }

                this.SaveChanges();
            }

        }

        public Form CreateNew()
        {
            string formName = "New Registration Form";
            var form = new Form
            {
                Title = formName,
                Slug = formName.ToSlug(),
                Status = Constants.FormStatus.DRAFT.ToString(),
                DateAdded = DateTime.UtcNow,
                ConfirmationMessage = "Thank you for signing up"
            };

            this.DataContext.Forms.AddObject(form);
            this.SaveChanges();
            return form;
        }

        public void Update(FormViewModel model)
        {
            if (!model.Id.HasValue)
            {
                throw new Exception("Invalid update operation. Form Id required.");
            }
            var form = GetByPrimaryKey(model.Id.Value);
            this.Update(model, form);
        }

        public void Update(FormViewModel model, Form form)
        {
            if (model == null)
            {
                throw new Exception("Invalid update operation. Form view is null.");
            }

            if (form == null)
            {
                throw new Exception("Invalid update operation. Form not found.");
            }

            form.Status = model.Status.ToString();
            form.Title = string.IsNullOrEmpty(model.Title) ? "Registration" : model.Title;
            // form.TabOrder = model.TabOrder; // excluding tab order for first launch
            form.ConfirmationMessage = model.ConfirmationMessage;
            form.Theme = model.Theme;
            form.NotificationEmail = model.NotificationEmail;
            this.SaveChanges();
        }

        public void DeleteField(int id)
        {
            var field = this.DataContext.FormFields.Where(f => f.ID == id).FirstOrDefault();
            if (field != null)
            {
                this.DataContext.FormFields.DeleteObject(field);
                this.SaveChanges();
            }
        }

        public IEnumerable<FormFieldValueViewModel> GetRegistrantsByForm(FormViewModel model)
        {
            var fieldValues = this.GetRegistrantsByForm(model.Id.Value);
            var values = fieldValues
                         .Select((fv) =>
                         {
                             return FormFieldValueViewModel.CreateFromObject(fv);
                         })
                         .OrderBy(f => f.FieldOrder)
                         .ThenByDescending(f => f.DateAdded);
            
            return values;
        }

        public List<FormFieldValue> GetRegistrantsByForm(int formId)
        {
            var fieldValues = this.DataContext.FormFieldValues;
            return this.DataContext
                             .FormFields
                             .Include("Forms")
                             .Where(field => field.Forms.Any(f => f.ID == formId))
                             .Join(fieldValues, fields => fields.ID, fieldValue => fieldValue.FieldId, (FormField, FormFieldValue) => FormFieldValue)
                             .ToList();
        }

        public void InsertFieldValue(FormFieldViewModel field, string value, Guid entryId, string userId = "")
        {
            if (field.FieldType != Constants.FieldType.HEADER)
            {
                var fieldVal = new FormFieldValue
                {
                    FieldId = field.Id.Value,
                    Value = value,
                    EntryId = entryId,
                    DateAdded = DateTime.UtcNow
                };

                this.DataContext.FormFieldValues.AddObject(fieldVal);
                this.SaveChanges();
            }
        }

        public void DeleteEntries(IEnumerable<string> selectedEntries)
        {
            var selectedGuids = selectedEntries.Select(se => new Guid(se));
            var entries = this.DataContext.FormFieldValues.Where(fv => selectedGuids.Any(se => fv.EntryId == se));

            foreach (var entry in entries)
            {
                this.DeleteFileEntry(entry);
                this.DataContext.FormFieldValues.DeleteObject(entry);
            }

            this.SaveChanges();
        }

        public void DeleteFileEntry(FormFieldValue entry)
        {
            entry.FormFieldReference.LoadOnce();
            if (entry.FormField.FieldType.ToUpper().IsTheSameAs(Constants.FieldType.FILEPICKER.ToString()))
            {
                var fileObj = entry.Value.FromJson<FileValueObject>();

                if (fileObj.IsSavedInCloud)
                {
                    UtilityHelper.RemoveFileFromBucket(fileObj.SaveName);
                }
                else
                {
                    if (fileObj != null && !fileObj.FileName.IsNullOrEmpty())
                    {
                        if (File.Exists(fileObj.FullFilePath().Replace(@"\\", @"\")))
                        {
                            File.Delete(fileObj.FullFilePath().Replace(@"\\", @"\"));
                        }
                    }
                }
            }
        }


        public List<FormViewModel> GetForms()
        {
            var formViews = new List<FormViewModel>();
            var formSet = this.DataContext.Forms.ToList();
            foreach (var form in formSet)
            {
                formViews.Add(FormViewModel.CreateBasicFromObject(form));
            }

            return formViews;
        }

        public void DeleteForm(int formId)
        {
            var form = this.GetByPrimaryKey(formId);
            this.DeleteForm(form);
        }

        public void DeleteForm(Form form)
        {
            form.FormFields.LoadOnce();
            this.DataContext.Forms.DeleteObject(form);
            var fields = form.FormFields.ToList();

            foreach (var f in fields)
            {
                this.DataContext.FormFields.DeleteObject(f);
            }

            this.SaveChanges();
        }

        public FileValueObject GetFileFieldValue(int valueId)
        {
            var valueObject = this.DataContext.FormFieldValues.Where(v => v.ID == valueId).FirstOrDefault();
            if (valueObject != null) {
                var value = valueObject.Value.FromJson<FileValueObject>();
                return value;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="olderThanInDays"></param>
        /// <returns></returns>
        public int DeleteForms(int olderThanInDays)
        {
            int counter = 0;
            this.DeleteSubmissions(olderThanInDays);
            var deleteDate = DateTime.Now.AddDays(-olderThanInDays);
            var forms = this.DataContext.Forms.Where(f => f.DateAdded < deleteDate).ToList();

            foreach (var f in forms)
            {
                this.DeleteForm(f);
                counter++; ;
            }

            return counter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="olderThanInDays"></param>
        /// <returns></returns>
        public int DeleteSubmissions(int olderThanInDays)
        {
            int counter = 0;
            var deleteDate = DateTime.Now.AddDays(-olderThanInDays);
            var entries = this.DataContext.FormFieldValues.Where(fv => fv.DateAdded < deleteDate).ToList();

            if (entries.Any())
            {
                foreach (var entry in entries)
                {
                    this.DataContext.FormFieldValues.DeleteObject(entry);
                    counter++;
                }

                this.SaveChanges();
            }

            return counter;
        }
    }
}