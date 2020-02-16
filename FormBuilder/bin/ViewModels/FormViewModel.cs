using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FormBuilder.BusinessObjects;
using FormBuilder.Extensions;
using FormBuilder;

namespace FormBuilder.ViewModels
{
    public class FormViewModel
    {
        #region Properties

        public int? Id { get; set; }
        public string Title { get; set; }
        public string NotificationEmail { get; set; }
        public string Slug { get; set; }
        public DateTime DateAdded { get; set; }
        public List<FormFieldViewModel> Fields { get; set; }
        public Constants.FormStatus Status { get; set; }
        public int TabOrder { get; set; }
        public string ConfirmationMessage { get; set; }
        public IList<FormFieldValueViewModel> Entries { get; set; }
        public IEnumerable<IGrouping<string, FormFieldValueViewModel>> GroupedEntries { get; set; }
        public string Theme { get; set; }
        public bool Embed { get; set; }
        
        public bool HasTheme
        {
            get { 
                return !Theme.IsNullOrEmpty(); 
            }            
        }
        

        #endregion

        #region Public Members

        public static FormViewModel Initialize()
        {   

            var formView = new FormViewModel
            {                
                Title = "Registration",
                Status = Constants.FormStatus.DRAFT,
                TabOrder = 0,
                Theme="",
                NotificationEmail="",
                Fields = Enumerable.Empty<FormFieldViewModel>().ToList()
                
            };

            return formView;
        }

        public static FormViewModel CreateFromObject(Form form)
        {
            return CreateFromObject(form, Constants.FormFieldMode.EDIT);
        }


        public static FormViewModel CreateFromObject(Form form, Constants.FormFieldMode mode)
        {
            if (form != null)
            {

                var formView = CreateBasicFromObject(form);

                if (form.FormFields.Count() > 0)
                {
                    form.FormFields.OrderBy(o => o.Order).Each((field, index) =>
                    {

                        formView.Fields.Add(FormFieldViewModel.CreateFromObject(field, mode));
                    });
                }

                return formView;
            }
            return FormViewModel.Initialize();
        }

        public static FormViewModel CreateBasicFromObject(Form form)
        {

            var formView = new FormViewModel
            {
                Title = form.Title,
                Id = form.ID,
                DateAdded = form.DateAdded.Value,                
                ConfirmationMessage = form.ConfirmationMessage,
                Fields = Enumerable.Empty<FormFieldViewModel>().ToList(),
                Slug = form.Slug,
                Theme=form.Theme,
                NotificationEmail = form.NotificationEmail,
                Status = (Constants.FormStatus)Enum.Parse(typeof(Constants.FormStatus), form.Status)
            };

            return formView;
        }

        public static FormViewModel CreateMock()
        {
            var formView = new FormViewModel
            {
                Title = "Test Form",
                Id = 1,
                DateAdded = DateTime.Now,
                ConfirmationMessage = "Thank you for filling this form",
                Fields = Enumerable.Empty<FormFieldViewModel>().ToList(),
                Slug = "test-form",
                NotificationEmail= ""

            };

            return formView;
        }

        #endregion

        #region Private Members
        #endregion







        
    }
}