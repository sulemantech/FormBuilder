using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FormBuilder.ViewModels
{
    public class AddressViewModel
    {
            public int? Id { get; set; }

            [Display(Name = "Address")]
            public virtual string Address1 { get; set; }

            [Display(Name = "Address 2")]
            public virtual string Address2 { get; set; }

            public virtual string City { get; set; }

            public virtual string State { get; set; }

            [Display(Name = "Zip Code")]
            public virtual string ZipCode { get; set; }

            public virtual string Country { get; set; }

            public string Longitude { get; set; }

            public string Latitude { get; set; }


            public static AddressViewModel Initialize()
            {
                return new AddressViewModel();
            }

        }
}