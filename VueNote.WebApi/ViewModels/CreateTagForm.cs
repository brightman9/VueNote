using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace VueNote.WebApi.ViewModels
{
    public class CreateTagForm
    {
        [Required]
        public string TagName { get; set; }

        [Range(1, int.MaxValue)]
        public int? ParentId { get; set; }
    }
}
