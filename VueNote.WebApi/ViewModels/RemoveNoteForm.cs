using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace VueNote.WebApi.ViewModels
{
    public class RemoveNoteForm
    {
        [Range(1, int.MaxValue)]
        public int NoteId { get; set; }
    }
}
