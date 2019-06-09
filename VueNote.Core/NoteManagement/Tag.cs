using System;
using System.Collections.Generic;
using System.Text;
using Dapper.Contrib.Extensions;

namespace VueNote.Core.Note.NoteManagement
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AuthorId { get; set; }
        public int? ParentId { get; set; }
        public string Code { get; set; }

        [Write(false)]
        public Tag Parent { get; set; }
        [Write(false)]
        public List<Tag> Children { get; set; }
    }
}
