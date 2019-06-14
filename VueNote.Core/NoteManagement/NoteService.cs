using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using VueNote.Core.UserManagement;
using VueNote.Core.Util;

namespace VueNote.Core.Note.NoteManagement
{
    public static class NoteService
    {
        public static async Task<Pageable<Note>> SearchNotes(string keyword = null, int? tagId = null, int? authorId = null, bool isDiscarded = false, string sort = "UpdateTime", string order = "desc", int pageNum = 1, int pageSize = 5)
        {
            var param = new DynamicParameters();

            string sql = @"select 
                note.Id,
                note.Title,
                note.Abstract,
                note.CreateTime,
                note.UpdateTime,
                note.AuthorId,
                note.IsDiscarded,
                tag.Id as Tags_Id, 
                tag.Name as Tags_Name 
                from note
                left join note_tag_relation as relation on note.Id = relation.NoteId
                left join tag on tag.Id = relation.TagId 
                where note.IsDiscarded = @IsDiscarded ";

            param.Add("IsDiscarded", isDiscarded);

            if (!string.IsNullOrEmpty(keyword))
            {
                sql += @" and (note.Title like @Keyword or note.Content like @Keyword) ";
                param.Add("Keyword", $"%{keyword}%");
            }

            if (tagId != null)
            {
                var tag = await GetTag(tagId.Value);
                sql += @" and tag.Code like @TagCode ";
                param.Add("TagCode", $"{tag.Code}%");
            }

            if (authorId != null)
            {
                sql += @" and note.AuthorId = @AuthorId ";
                param.Add("AuthorId", authorId);
            }

            sql += $" order by {sort} {order}";
            string rankBy = $"{sort} {order}, Id";

            var notes = await DbHelper.QueryPageable<Note>(sql, param, pageSize, pageNum, rankBy);
            return notes;
        }

        public static async Task<Note> GetNoteDetail(int noteId)
        {
            string sql = @"select 
                note.Id,
                note.Title,
                note.Abstract,
                note.Content,
                note.CreateTime,
                note.UpdateTime,
                note.AuthorId,
                note.IsDiscarded,
                tag.Id as Tags_Id, 
                tag.Name as Tags_Name 
                from note 
                left join note_tag_relation as relation on relation.NoteId = note.Id
                left join tag on tag.Id = relation.TagId
                where note.Id = @NoteId";
            var note = await DbHelper.QueryFirst<Note>(sql, new { NoteId = noteId });
            return note;
        }

        public static async Task<Note> CreateNote(string title, string @abstract, string content, int authorId)
        {
            var now = DateTime.Now.ToUniversalTime();
            Note note = new Note
            {
                Title = title,
                Abstract = @abstract,
                Content = content,
                CreateTime = now,
                UpdateTime = now,
                AuthorId = authorId,
                IsDiscarded = false
            };

            await DbHelper.Insert(note);

            return note;
        }

        public static async Task<DateTime> UpdateNoteTitle(int noteId, string title)
        {
            var note = await DbHelper.Get<Note>(noteId);

            note.Title = title;
            note.UpdateTime = DateTime.Now.ToUniversalTime();
            await DbHelper.Update(note);

            return note.UpdateTime;
        }

        public static async Task<DateTime> UpdateNoteContent(int noteId, string @abstract, string content)
        {
            var note = await DbHelper.Get<Note>(noteId);

            note.Abstract = @abstract;
            note.Content = content;
            note.UpdateTime = DateTime.Now.ToUniversalTime();
            await DbHelper.Update(note);

            return note.UpdateTime;
        }

        public static async Task<bool> DiscardNote(int noteId)
        {
            var note = await DbHelper.Get<Note>(noteId);
            note.IsDiscarded = true;
            bool result = await DbHelper.Update(note);
            return result;
        }

        public static async Task<bool> RestoreNote(int noteId)
        {
            var note = await DbHelper.Get<Note>(noteId);
            note.IsDiscarded = false;
            bool result = await DbHelper.Update(note);
            return result;
        }

        public static async Task<int> ClearDiscardedNotes(int authorId)
        {
            string sqlGetDiscardedNoteIds = "select Id from note where IsDiscarded = @IsDiscarded and AuthorId = @AuthorId";
            var discardedNoteIds = await DbHelper.Query<int>(sqlGetDiscardedNoteIds, new { IsDiscarded = true, AuthorId = authorId });

            string sql = @"delete from Note_Tag_Relation where NoteId in @NoteId;
                           delete from Note where Id in @NoteId;";
            int result = await DbHelper.Execute(sql, new { NoteId = discardedNoteIds }, true);
            return result;
        }


        public static async Task<IEnumerable<Tag>> SearchCandidateNoteTags(int noteId, string tagName, int authorId)
        {
            string sqlGetNoteTags = "select TagId from note_tag_relation where NoteId = @NoteId";
            var noteTagIds = await DbHelper.Query<int>(sqlGetNoteTags, new { NoteId = noteId });

            string sql = @"select *
                from tag 
                where AuthorId = @AuthorId
                and Name like @Name
                and Id not in @NoteTagIds";

            var tags = await DbHelper.Query<Tag>(sql, new
            {
                Name = $"%{tagName}%",
                NoteTagIds = noteTagIds,
                AuthorId = authorId
            });

            return tags;
        }

        public static async Task<Tag> GetTag(int tagId)
        {
            var tag = await DbHelper.Get<Tag>(tagId);
            return tag;
        }

        public static async Task<Tag> GetTag(string tagName, int authorId)
        {
            string sql = @"select *
                from tag 
                where AuthorId = @AuthorId
                and Name = @Name";

            var tag = await DbHelper.QueryFirst<Tag>(sql, new
            {
                Name = tagName,
                AuthorId = authorId
            });

            return tag;
        }

        public static async Task<IEnumerable<Tag>> GetRootTags(int authorId)
        {
            string sql = @"select *
                from tag 
                where AuthorId = @AuthorId
                and ParentId is null
                order by Name";

            var tags = await DbHelper.Query<Tag>(sql, new { AuthorId = authorId });
            return tags;
        }

        public static async Task<IEnumerable<Tag>> GetSubTags(int parentId)
        {
            string sql = @"select *
                from tag 
                where ParentId = @ParentId
                order by Name";

            var tags = await DbHelper.Query<Tag>(sql, new { ParentId = parentId });
            return tags;
        }

        public static async Task<bool> IsTagExisted(string tagName, int authorId)
        {
            string sql = @"select count(*)
                from tag 
                where AuthorId = @AuthorId
                and Name = @Name";

            var tagsCount = await DbHelper.ExecuteScalar<int>(sql, new
            {
                Name = tagName,
                AuthorId = authorId
            });

            bool isTagExisted = tagsCount > 0;
            return isTagExisted;
        }

        public static async Task<Tag> CreateTag(string tagName, int? parentId, int authorId)
        {
            var newTag = new Tag { Name = tagName, ParentId = parentId, AuthorId = authorId };
            await DbHelper.Insert(newTag);

            if (parentId != null)
            {
                var parentTag = await DbHelper.Get<Tag>(parentId.Value);
                newTag.Code = $"{parentTag.Code}-[{newTag.Id}]";
            }
            else
            {
                newTag.Code = $"[{newTag.Id.ToString()}]";
            }
            await DbHelper.Update(newTag);

            return newTag;
        }

        public static async Task<int> AddNoteTag(int noteId, int tagId)
        {
            string sql = @"insert into Note_Tag_Relation (NoteId, TagId)
                           values(@NoteId, @TagId);";
            int result = await DbHelper.Execute(sql, new { NoteId = noteId, TagId = tagId });
            return result;
        }

        public static async Task<int> RemoveTag(int tagId)
        {
            var tag = await DbHelper.Get<Tag>(tagId);
            string sql = @"delete from Note_Tag_Relation where TagId = @TagId;
                           delete from Tag where Code like @Code;";
            int result = await DbHelper.Execute(sql, new { TagId = tagId, Code = $"{tag.Code}%" }, true);
            return result;
        }

        public static async Task<int> RemoveNoteTag(int noteId, int tagId)
        {
            string sql = @"delete from Note_Tag_Relation 
                            where NoteId = @NoteId
                            and TagId = @TagId;";
            int result = await DbHelper.Execute(sql, new { NoteId = noteId, TagId = tagId });
            return result;
        }
    }
}
