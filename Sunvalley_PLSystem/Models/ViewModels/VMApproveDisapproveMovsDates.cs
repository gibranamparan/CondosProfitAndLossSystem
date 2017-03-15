using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sunvalley_PLSystem.Models.ViewModels
{
    public class VMApproveDisapproveMovsDates
    {
        [Display(Name = "Filter Date")]
        DateTime filterDate { get; set; }
        [Display(Name = "Month to Approve")]
        DateTime approveDate { get; set; }
        [Display(Name = "Month to Disapprove")]
        DateTime disapproveDate { get; set; }
    }
}