using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eCheck3.Models
{
    public class DevelopmentViewModel
    {
        public string Choice { get; set; }
        public IEnumerable<SelectListItem> ChoiceList { get; set; }
    }
}