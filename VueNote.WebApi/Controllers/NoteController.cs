using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using VueNote.Core.Note.NoteManagement;
using VueNote.WebApi.Common;
using VueNote.WebApi.Extensions;
using VueNote.WebApi.ViewModels;

namespace VueNote.WebApi.Controllers
{
    [Authorize]
    [PermissionFilter("Note")]
    public class NoteController : Controller
    {
        public async Task<IActionResult> SearchNotes(string keyword = null, string sort = "UpdateTime", string order = "desc", int pageNum = 1, int pageSize = 10)
        {
            var currentUser = await this.GetCurrentUser();
            var pageNotes = await NoteService.SearchNotes(keyword: keyword, authorId: currentUser.Id, sort: sort, order: order, pageNum: pageNum, pageSize: pageSize);

            var notesDto = pageNotes.PageRecords.Select(t => new
            {
                id = t.Id,
                title = t.Title,
                @abstract = t.Abstract,
                date = t.UpdateTime.ToString("yyyy-M-d HH:mm")
            });

            return Ok(new
            {
                notes = notesDto,
                notesTotalCount = pageNotes.TotalCount
            });
        }

        public async Task<IActionResult> SearchDiscardedNotes(string keyword = null, string sort = "UpdateTime", string order = "desc", int pageNum = 1, int pageSize = 10)
        {
            var currentUser = await this.GetCurrentUser();
            var pageNotes = await NoteService.SearchNotes(keyword: keyword, authorId: currentUser.Id, isDiscarded: true, sort: sort, order: order, pageNum: pageNum, pageSize: pageSize);

            var notesDto = pageNotes.PageRecords.Select(t => new
            {
                id = t.Id,
                title = t.Title,
                @abstract = t.Abstract,
                date = t.UpdateTime.ToString("yyyy-M-d"),
                tags = t.Tags.Select(tag => tag.Name).ToList()
            });

            return Ok(new
            {
                notes = notesDto,
                notesTotalCount = pageNotes.TotalCount
            });
        }

        public async Task<IActionResult> SearchTagNotes(int tagId, string keyword = null, string sort = "UpdateTime", string order = "desc", int pageNum = 1, int pageSize = 10)
        {
            var currentUser = await this.GetCurrentUser();
            var pageNotes = await NoteService.SearchNotes(keyword: keyword, tagId: tagId, authorId: currentUser.Id, sort: sort, order: order, pageNum: pageNum, pageSize: pageSize);

            var notesDto = pageNotes.PageRecords.Select(t => new
            {
                id = t.Id,
                title = t.Title,
                @abstract = t.Abstract,
                date = t.UpdateTime.ToString("yyyy-M-d"),
                tags = t.Tags.Select(tag => tag.Name).ToList()
            });

            return Ok(new
            {
                notes = notesDto,
                notesTotalCount = pageNotes.TotalCount
            });
        }

        public async Task<IActionResult> GetNoteDetail(int noteId)
        {
            var note = await NoteService.GetNoteDetail(noteId);

            var noteDetailDto = new
            {
                id = note.Id,
                title = note.Title,
                @abstract = note.Abstract,
                content = note.Content,
                createTime = note.CreateTime.ToString("yyyy-M-d"),
                updateTime = note.UpdateTime.ToString("yyyy-M-d"),
                isDiscarded = note.IsDiscarded,
                tags = note.Tags.Select(t => new { id = t.Id, name = t.Name })
            };

            return Ok(new
            {
                noteDetail = noteDetailDto
            });
        }

        public async Task<IActionResult> CreateNote([FromBody]CreateNoteForm form)
        {
            var currentUser = await this.GetCurrentUser();
            var note = await NoteService.CreateNote(form.Title, form.Abstract, form.Content, currentUser.Id);

            var noteDetailDto = new
            {
                id = note.Id,
                title = note.Title,
                @abstract = note.Abstract,
                content = note.Content,
                createTime = note.CreateTime.ToString("yyyy-M-d HH:mm"),
                updateTime = note.UpdateTime.ToString("yyyy-M-d HH:mm"),
                isDiscarded = note.IsDiscarded,
                tags = note.Tags.Select(t => new { id = t.Id, name = t.Name })
            };

            return Ok(new
            {
                noteDetail = noteDetailDto
            });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateNoteTitle([FromBody]UpdateNoteTitleForm form)
        {
            var updateTime = await NoteService.UpdateNoteTitle(form.Id, form.Title);

            return Ok(new
            {
                updateTime = updateTime.ToString("yyyy-MM-dd HH:mm")
            });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateNoteContent([FromBody]UpdateNoteContentForm form)
        {
            var updateTime = await NoteService.UpdateNoteContent(form.Id, form.Abstract, form.Content);

            return Ok(new
            {
                updateTime = updateTime.ToString("yyyy-MM-dd HH:mm")
            });
        }

        [HttpPost]
        public async Task<IActionResult> DiscardNote([FromBody]RemoveNoteForm form)
        {
            bool succeeded = await NoteService.DiscardNote(form.NoteId);

            return Ok(new { succeeded });
        }

        [HttpPost]
        public async Task<IActionResult> RestoreNote([FromBody]RemoveNoteForm form)
        {
            bool succeeded = await NoteService.RestoreNote(form.NoteId);

            return Ok(new { succeeded });
        }

        [HttpPost]
        public async Task<IActionResult> ClearDiscardedNotes()
        {
            var currentUser = await this.GetCurrentUser();
            int result = await NoteService.ClearDiscardedNotes(currentUser.Id);

            return Ok(new { succeeded = result > 0 });
        }


        public async Task<IActionResult> GetRootTags()
        {
            var currentUser = await this.GetCurrentUser();
            var tags = await NoteService.GetRootTags(currentUser.Id);
            var tagsDto = tags.Select(t => new
            {
                id = t.Id,
                name = t.Name
            });

            return Ok(new { tags = tagsDto });
        }

        public async Task<IActionResult> GetSubTags(int parentId)
        {
            var tags = await NoteService.GetSubTags(parentId);
            var tagsDto = tags.Select(t => new
            {
                id = t.Id,
                name = t.Name
            });

            return Ok(new { tags = tagsDto });
        }

        public async Task<IActionResult> SearchCandidateNoteTags(int noteId, string tagName)
        {
            var currentUser = await this.GetCurrentUser();
            var tags = await NoteService.SearchCandidateNoteTags(noteId, tagName, currentUser.Id);

            var tagsDto = tags.Select(t => new
            {
                id = t.Id,
                name = t.Name
            });

            return Ok(new { tags = tagsDto });
        }

        [HttpPost]
        public async Task<IActionResult> CreateTag([FromBody]CreateTagForm form)
        {
            var currentUser = await this.GetCurrentUser();
            bool isTagExisted = await NoteService.IsTagExisted(form.TagName, currentUser.Id);
            if (isTagExisted)
            {
                return Ok(new { succeeded = false, message = "该标签已存在" });
            }

            var newTag = await NoteService.CreateTag(form.TagName, form.ParentId, currentUser.Id);

            return Ok(new { succeeded = true, tagId = newTag.Id });
        }

        [HttpPost]
        public async Task<IActionResult> AddNoteTag([FromBody]AddNoteTagForm form)
        {
            var currentUser = await this.GetCurrentUser();
            var tag = await NoteService.GetTag(form.TagName, currentUser.Id);
            bool isNewTag = tag == null;
            if (isNewTag)
            {
                tag = await NoteService.CreateTag(form.TagName, null, currentUser.Id);
            }
            await NoteService.AddNoteTag(form.NoteId, tag.Id);

            var tagDto = new
            {
                id = tag.Id,
                name = tag.Name
            };

            return Ok(new { isNewTag, tag = tagDto });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveTag([FromBody]RemoveTagForm form)
        {
            int result = await NoteService.RemoveTag(form.TagId);

            return Ok(new { succeeded = result > 0, message = "标签不存在" });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveNoteTag([FromBody]RemoveNoteTagForm form)
        {
            int result = await NoteService.RemoveNoteTag(form.NoteId, form.TagId);

            return Ok(new { succeeded = result > 0 });
        }
    }
}