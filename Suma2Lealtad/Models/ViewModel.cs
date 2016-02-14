using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Suma2Lealtad.Models
{
    public class ViewModel
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public string RouteValues { get; set; }
    }
}