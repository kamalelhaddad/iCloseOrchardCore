using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrchardCore;
using OrchardCore.ContentManagement;

namespace iClose.Pages {
    public class EditorModel : PageModel {
        //private readonly IOrchardHelper _orchardHelper;

        //public EditorModel(IOrchardHelper orchardHelper) {
        //    _orchardHelper = orchardHelper;
        //}
        
        public void OnGet() {

        }


        [HttpPost]
        public string OnPost(string id) {
            return (id);
        }
    }
}
