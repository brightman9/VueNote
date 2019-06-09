using System;
using System.Collections.Generic;
using System.Text;
using Dapper.Contrib.Extensions;
using VueNote.Core.UserManagement;

namespace VueNote.Core.Note.NoteManagement
{
    public class Note
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Abstract { get; set; }
        public string Content { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public int AuthorId { get; set; }
        public bool IsDiscarded { get; set; }
        [Write(false)]
        public User Author { get; set; }
        [Write(false)]
        public List<Tag> Tags { get; set; }

        public Note()
        {
            if (this.Tags == null)
                this.Tags = new List<Tag>();
        }
    }
}
