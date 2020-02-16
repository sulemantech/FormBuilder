using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FormBuilder.ViewModels
{
    public class NotificationEmailViewModel
    {
        public string FormName { get; set; }
        public IDictionary<string, FormFieldValueViewModel> Entries { get; set; }
        public string Email { get; set; }

        public NotificationEmailViewModel()
        {
            this.Entries = new Dictionary<string, FormFieldValueViewModel>();
        }
    }
}