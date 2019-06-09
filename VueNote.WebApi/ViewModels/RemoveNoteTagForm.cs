using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace VueNote.WebApi.ViewModels
{
    public class RemoveNoteTagForm
    {
        [Range(1, int.MaxValue)]
        public int NoteId { get; set; }

        [Range(1, int.MaxValue)]
        public int TagId { get; set; }
    }
}
