using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace VueNote.WebApi.ViewModels
{
    public class UpdateNoteContentForm
    {
        [Range(1, int.MaxValue)]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Abstract { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Content { get; set; }
    }
}
