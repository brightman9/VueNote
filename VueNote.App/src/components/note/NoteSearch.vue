<template>
  <div class="note-search-container">
    <div class="search-box" :style="{ height: uiSettings.searchBoxHeight + 'px' }">
      <div class="search-title-container">
        <div class="search-title">{{searchTitle}}</div>
        <el-button @click="clearTrash" v-show="this.searchTitle === '废纸篓'" :disabled="this.notes.length === 0" class="clear-trash-button" type="danger" plain size="small">清空废纸篓</el-button>
      </div>
      <div class="search-condition-wrapper">
        <div class="search-condition">
          <div class="search-result-count">找到 {{notesTotalCount}} 条笔记</div>
          <el-dropdown class="sort-settings" @command="setSortting" trigger="click">
            <div title="排序选项" class="sort-setting-button">
              <img src="../../assets/img/sort-setting-button.png" alt>
            </div>
            <el-dropdown-menu slot="dropdown">
              <el-dropdown-item v-for="(setting,index) in sortSettings" :key="index" :command="index + ',' + setting.sort + ',' + setting.order">
                <span>{{setting.name}}</span>
                <span v-if="setting.effected" class="el-icon-check"></span>
              </el-dropdown-item>
            </el-dropdown-menu>
          </el-dropdown>
        </div>
        <div class="keyword">
          <el-input v-model="searchCondition.keyword" @keyup.enter.native="searchNotes" size="small" placeholder="搜索笔记..." clearable>
            <el-button slot="append" @click="searchNotes" icon="el-icon-search"></el-button>
          </el-input>
        </div>
      </div>
    </div>
    <div class="search-result-list-wrapper" :style="{ height: uiSettings.searchListHeight + 'px'}">
      <vue-scroll :ops="uiSettings.scrollBarSettings" @handle-scroll="loadMoreNotes">
        <div class="search-result-list">
          <NoteList ref="resultList" :notes="notes" :listItemHeight="uiSettings.listItemHeight" />
        </div>
      </vue-scroll>
    </div>
  </div>
</template>

<script>
import { Message, MessageBox } from "element-ui";
import NoteList from "./NoteList";

export default {
  name: "NoteSearch",
  components: { NoteList: NoteList },
  data() {
    return {
      sortSettings: [
        { name: "按更新日期: 最新到最旧", sort: "updateTime", order: "desc", effected: true },
        { name: "按更新日期: 最旧到最新", sort: "updateTime", order: "asc", effected: false },
        { name: "按创建日期: 最新到最旧", sort: "createTime", order: "desc", effected: false },
        { name: "按创建日期: 最旧到最新", sort: "createTime", order: "asc", effected: false },
        { name: "按标题: A-Z", sort: "title", order: "asc", effected: false },
        { name: "按标题: Z-A", sort: "title", order: "desc", effected: false }
      ],
      searchCondition: {
        api: null,
        tagId: null,
        keyword: null,
        sort: null,
        order: null,
        pageNum: 1,
        pageSize: 5
      },
      searchTitle: null,
      notes: [],
      notesTotalCount: 0,
      uiSettings: {
        searchBoxHeight: 150,
        searchListHeight: 0,
        listItemHeight: 110,
        scrollBarSettings: {
          bar: {
            background: "#cecece"
          },
          scrollPanel: {
            scrollingX: false
          }
        }
      },
      isLoading: false
    };
  },
  created() {
    this.$bus.on("viewAllNotes", event => {
      this.viewAllNotes()
    });

    this.$bus.on("viewDiscardedNotes", event => {
      this.viewDiscardedNotes()
    });

    this.$bus.on("viewTagNotes", event => {
      this.viewTagNotes(event.tagId)
    });

    this.$bus.on("refreshNotes", event => {
      this.refreshNotes();
    });
  },
  mounted() {
    this.caculateSearchListHeight()
  },
  beforeDestroy() {
    this.$bus.off();
  },
  methods: {
    viewAllNotes() {
      this.searchTitle = "所有笔记";
      this.searchCondition.api = "/api/note/searchNotes";
      this.searchCondition.tagId = null;
      this.searchCondition.keyword = null;
      this.searchCondition.pageNum = 1;
      this.searchNotes();
    },
    viewDiscardedNotes() {
      this.searchTitle = "废纸篓";
      this.searchCondition.api = "/api/note/searchDiscardedNotes";
      this.searchCondition.tagId = null;
      this.searchCondition.keyword = null;
      this.searchCondition.pageNum = 1;
      this.searchNotes();
    },
    viewTagNotes(tagId) {
      this.searchTitle = "所有笔记";
      this.searchCondition.api = "/api/note/searchTagNotes";
      this.searchCondition.tagId = tagId;
      this.searchCondition.keyword = null;
      this.searchCondition.pageNum = 1;
      this.searchNotes();
    },
    refreshNotes() {
      this.searchNotes();
    },
    setSortting(command) {
      const [index, sort, order] = command.split(",");

      this.sortSettings.forEach(setting => {
        setting.effected = false;
      });
      this.sortSettings[index].effected = true;

      this.searchCondition.sort = sort;
      this.searchCondition.order = order;
      this.searchCondition.pageNum = 1;
      this.searchNotes();
    },
    searchNotes() {
      this.$http
        .get(this.searchCondition.api, this.searchCondition)
        .then(data => {
          this.notes = [];
          this.notes.push(...data.notes);
          this.notesTotalCount = data.notesTotalCount;
          if (data.notesTotalCount === 0) {
            this.$bus.emit('notesNotFound')
          }

          this.$nextTick(() => {
            this.$refs.resultList.selectFirst()
          })
        })
        .catch(error => {
          this.isLoading = false;
          Message.error('查询失败');
        });
    },
    loadMoreNotes(vertical, horizontal, nativeEvent) {
      const clientHeight = nativeEvent.target.clientHeight;
      const scrollTop = nativeEvent.target.scrollTop;
      const scrollHeight = nativeEvent.target.scrollHeight;

      const isAtBottom = Math.abs(clientHeight + scrollTop - scrollHeight) < 1;
      const isAllNotesLoaded = this.notes.length === this.notesTotalCount;

      if (isAtBottom && !isAllNotesLoaded && !this.isLoading) {
        this.isLoading = true;
        this.searchCondition.pageNum += 1;
        this.$http
          .get(this.searchCondition.api, this.searchCondition)
          .then(data => {
            this.notes.push(...data.notes);
            this.notesTotalCount = data.notesTotalCount;
            this.isLoading = false;

            if (this.notes.length === this.notesTotalCount) {
              Message.success('已滚动到列表底部');
            }
          })
          .catch(error => {
            this.isLoading = false;
            Message.error('查询失败');
          });
      }
    },
    clearTrash() {
      MessageBox.confirm('废纸篓中的笔记将被永久删除，确定要继续吗？',
        '清空废纸篓', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning',
          beforeClose: (action, instance, done) => {
            if (action === 'confirm') {
              instance.confirmButtonLoading = true;
              this.$http
                .post("/api/note/clearDiscardedNotes")
                .then(data => {
                  if (data.succeeded) {
                    instance.confirmButtonLoading = false;
                    this.$bus.emit('viewDiscardedNotes')
                    Message.success('清除成功')
                  }
                  else {
                    instance.confirmButtonLoading = false;
                    Message.error('清除失败')
                  }
                  done()
                })
                .catch(error => {
                  instance.confirmButtonLoading = false;
                  Message.error('清除失败')
                  done()
                });
            } else {
              instance.confirmButtonLoading = false;
              done();
            }
          }
        })
    },
    caculateSearchListHeight() {
      const searchListVisibleHeight = window.innerHeight - this.uiSettings.searchBoxHeight;
      const listItemsTotalHeight = this.uiSettings.listItemHeight * this.searchCondition.pageSize;
      this.uiSettings.searchListHeight = Math.min(searchListVisibleHeight, listItemsTotalHeight - 1);
    }
  }
};
</script>

<style scoped>
.note-search-container {
  overflow: hidden;
  height: 100%;
  background-color: #f8f8f8;
}

.search-box {
  display: flex;
  flex-flow: column nowrap;
  justify-content: space-between;
  box-sizing: border-box;
  padding-left: 25px;
  border-bottom: 1px solid #e6e6e6;
}
.search-title-container {
  display: flex;
  justify-content: space-between;
}
.search-title {
  font-size: 20px;
  font-weight: 100;
  margin-top: 20px;
}
.clear-trash-button {
  margin-top: 20px;
  margin-right: 20px;
}
.search-condition {
  display: flex;
  justify-content: space-between;
}
.search-result-count {
  font-size: 13px;
  font-weight: 100;
  color: #a6a6a6;
}
.keyword {
  margin-top: 5px;
  margin-bottom: 10px;
  padding-right: 20px;
}

.sort-setting-button {
  margin-right: 20px;
  cursor: pointer;
}
.sort-setting-button > img {
  width: 16px;
}

.search-result-list {
  list-style: none;
}
</style>
