<template>
  <div class="note-detail-container" v-if="hasData" v-loading="isLoading">
    <div class="header">
      <div class="title-wrapper">
        <input class="title" v-model="editedNote.title" type="text" @input="saveNoteTitle" :readonly="!editable">
        <el-dropdown @command="handleDropDownMenu" trigger="click">
          <div class="more-button">
            <img src="../../assets/img/more.png" alt="">
          </div>
          <el-dropdown-menu slot="dropdown">
            <el-dropdown-item v-show="editable" command="discardNote">删除笔记</el-dropdown-item>
            <el-dropdown-item v-show="!editable" command="restoreNote">还原笔记</el-dropdown-item>
          </el-dropdown-menu>
        </el-dropdown>
      </div>
      <div class="tags">
        <el-tag class="tag" v-for="tag in editedNote.tags" @close="removeTag(tag.id)" :key="tag.id" :closable="editable" :type="!editable ? 'info' : ''">{{tag.name}}</el-tag>
        <el-autocomplete class="addTag" v-show="editable" @select="addSelectedTag" v-model="searchTagName" :fetch-suggestions="searchTags" value-key="name" placeholder="添加标签..." size="small"></el-autocomplete>
      </div>
    </div>
    <el-divider></el-divider>
    <div class="content">
      <quill-editor class="editor" v-model="editedNote.content" @input="saveNoteContent" ref="editor" :options="editorOption" :disabled="!editable">
      </quill-editor>
    </div>
  </div>
</template>

<script>
import { Message, MessageBox } from "element-ui";
export default {
  data() {
    return {
      editedNote: {},
      uneditedNote: {},
      updateDalay: 2000,
      isLoading: false,
      searchTagName: '',
      editorOption: {
        scrollingContainer: '.editor',
        theme: 'bubble',
        placeholder: '添加文本...',
        bounds: '.note-detail-container',
        modules: {
          toolbar: [
            [{ 'font': [] }],         //字体
            [{ 'size': ['small', false, 'large', 'huge'] }],  // 字体大小
            [{ 'color': [] }, { 'background': [] }],          // 字体颜色，字体背景颜色

            ['bold', 'italic', 'underline', 'strike'],        //加粗，斜体，下划线，删除线

            [{ 'list': 'ordered' }, { 'list': 'bullet' }],          //列表
            [{ 'indent': '-1' }, { 'indent': '+1' }],          // 缩进
            [{ 'align': [] }],        //对齐方式

            ['link', 'image', 'video'],        //上传图片、上传视频

            ['clean'],        //清除字体样式
          ]
        },
      },
    }
  },
  created() {
    this.$bus.on("viewNoteDetail", event => {
      this.viewNoteDetail(event.noteId)
    });

    this.$bus.on("notesNotFound", event => {
      this.clearNote()
    });

    this.$bus.on("createNote", () => {
      this.createNote()
    });
  },
  destroyed() {
    this.$bus.off()
  },
  methods: {
    viewNoteDetail(noteId) {
      this.isLoading = true
      this.$http
        .get('/api/note/getNoteDetail', { noteId })
        .then(data => {
          this.uneditedNote = JSON.parse(JSON.stringify(data.noteDetail))
          this.editedNote = JSON.parse(JSON.stringify(data.noteDetail))
          this.isLoading = false
        })
        .catch(error => {
          this.isLoading = false;
          Message.error('加载失败');
        });
    },
    clearNote() {
      this.editedNote = {}
    },
    createNote() {
      this.isLoading = true
      this.$http
        .post('/api/note/createNote', { title: '无标题笔记', abstract: '', content: '' })
        .then(data => {
          this.uneditedNote = JSON.parse(JSON.stringify(data.noteDetail))
          this.editedNote = JSON.parse(JSON.stringify(data.noteDetail))
          this.isLoading = false
        })
        .catch(error => {
          this.isLoading = false;
          Message.error('新建笔记失败');
        });
    },
    saveNoteTitle() {
      setTimeout(() => {
        if (this.editedNote.title === this.uneditedNote.title)
          return

        this.$http.post('/api/note/updateNoteTitle', { id: this.editedNote.id, title: this.editedNote.title })
          .then(data => {
            this.editedNote.updateTime = data.updateTime

            this.uneditedNote.title = this.editedNote.title
            this.uneditedNote.updateTime = this.editedNote.updateTime
            // Message.success('已保存笔记');
            this.$bus.emit('refreshNotes', { noteId: this.editedNote.id })
          })
          .catch(error => {
            Message.error('保存失败');
          })
      }, this.updateDalay);
    },
    saveNoteContent() {
      setTimeout(() => {
        if (this.editedNote.content === this.uneditedNote.content)
          return

        const plainContent = this.$refs.editor.quill.getText()
        if (plainContent && plainContent.length > 100) {
          this.editedNote.abstract = plainContent.substr(0, 100)
        }
        else {
          this.editedNote.abstract = plainContent
        }

        this.$http.post('/api/note/updateNoteContent', this.editedNote)
          .then(data => {
            this.editedNote.updateTime = data.updateTime

            this.uneditedNote.abstract = this.editedNote.abstract
            this.uneditedNote.content = this.editedNote.content
            this.uneditedNote.updateTime = this.editedNote.updateTime
            // Message.success('已保存笔记');
            this.$bus.emit('refreshNotes', { noteId: this.editedNote.id })
          })
          .catch(error => {
            Message.error('保存失败');
          })
      }, this.updateDalay);
    },
    discardNote() {
      MessageBox.confirm('“' + this.editedNote.title + '”将被移至废纸篓，确定要继续吗？',
        '删除笔记：' + this.editedNote.title, {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning',
          beforeClose: (action, instance, done) => {
            if (action === 'confirm') {
              instance.confirmButtonLoading = true;
              this.$http
                .post("/api/note/discardNote", { noteId: this.editedNote.id })
                .then(data => {
                  if (data.succeeded) {
                    instance.confirmButtonLoading = false;
                    this.$bus.emit('viewAllNotes')
                    Message.success('删除成功')
                  }
                  else {
                    instance.confirmButtonLoading = false;
                    Message.error('删除失败：' + data.message)
                  }
                  done()
                })
                .catch(error => {
                  instance.confirmButtonLoading = false;
                  Message.error('删除失败')
                  done()
                });
            } else {
              instance.confirmButtonLoading = false;
              done();
            }
          }
        })
    },
    restoreNote() {
      this.$http
        .post("/api/note/restoreNote", { noteId: this.editedNote.id })
        .then(data => {
          if (data.succeeded) {
            this.$bus.emit('viewDiscardedNotes')
            Message.success('还原成功')
          }
          else {
            Message.error('还原失败：' + data.message)
          }
          done()
        })
    },
    searchTags(queryString, callback) {
      this.$http.get('/api/note/searchCandidateNoteTags', { noteId: this.editedNote.id, tagName: queryString })
        .then(data => {
          if (data.tags.length > 0) {
            callback(data.tags)
          }
        })
    },
    addSelectedTag(tag) {
      this.addTag(tag.name)
    },
    addTag(tagName) {
      this.$http.post('/api/note/addNoteTag', { noteId: this.editedNote.id, tagName })
        .then(data => {
          this.editedNote.tags.push(data.tag)
          this.searchTagName = ''
        })
    },
    removeTag(tagId) {
      this.$http.post('/api/note/removeNoteTag', { noteId: this.editedNote.id, tagId })
        .then(data => {
          let tags = this.editedNote.tags;
          tags.splice(tags.findIndex(t => t.id === tagId), 1)
        })
        .catch(error => {
          Message.error('删除标签失败');
        })
    },
    handleDropDownMenu(command) {
      if (command === 'discardNote') {
        this.discardNote()
      }
      else if (command === 'restoreNote') {
        this.restoreNote()
      }
    }
  },
  computed: {
    hasData() {
      return this.editedNote !== {}
        && this.editedNote.id > 0
    },
    editable() {
      return this.hasData
        && this.editedNote.isDiscarded === false
    }
  }
}
</script>

<style scoped>
.note-detail-container {
  overflow: hidden;
  height: 100%;
  background-color: white;
  padding: 0 25px;
}
.header {
  margin-top: 20px;
}
.title-wrapper {
  display: flex;
  justify-content: space-between;
}
.tags {
  margin-top: 16px;
}
.tag {
  margin-right: 10px;
}
.addTag {
  width: 100px;
}
.title {
  border: none;
  flex: 1;
  height: 40px;
  line-height: 40px;
  font-size: 20px;
  font-weight: 100;
  outline: none;
}
.more-button {
  cursor: pointer;
  outline: none;
  display: flex;
  align-items: center;
  height: 100%;
}
.more-button > img {
  width: 20px;
  height: 20px;
}

.content {
  height: 100%;
}
.editor {
  height: 70%;
  margin-top: 15px;
}
</style>
<style>
.ql-editor {
  padding: 0;
}
.ql-editor.ql-blank::before {
  font-style: italic;
  right: 15px;
  left: 15px;
  width: 100%;
  content: attr(data-placeholder);
  pointer-events: none;
  color: rgba(0, 0, 0, 0.6);
}
.ql-editor.ql-blank:focus::before {
  content: "";
}
</style>

