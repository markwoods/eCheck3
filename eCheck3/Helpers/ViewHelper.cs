using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eCheck3.Helpers
{
    public class ViewHelper
    {
        public static List<SelectListItem> PageSizeList(string pageListSize)
        {

            //
            // Set up drop down list for gridview row size
            //

            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "10 Rows", Value = "10" });
            items.Add(new SelectListItem { Text = "50 Rows", Value = "50" });
            items.Add(new SelectListItem { Text = "All Rows", Value = "All" });

            if (String.IsNullOrEmpty(pageListSize))
            {
                pageListSize = "10";
            }

            // Show selected (or default) page size
            items.Find(Itm => Itm.Value == pageListSize).Selected = true;

            return items;
        }
    }
}