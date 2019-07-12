<template>
  <div class="note-detail-container" v-if="hasData" v-loading="isLoading">
    <div class="header-wrapper" ref="headerWrapper">
      <div class="header" @back="goBack" @click="goBack">
        <i class="el-icon-back" style="font-size:20px"></i>
      </div>
      <div class="title">{{note.title}}</div>
      <div class="tags">
        <el-tag class="tag" v-for="tag in note.tags" :key="tag.id">{{tag.name}}</el-tag>
      </div>
      <div class="hr-wrapper">
        <el-divider></el-divider>
      </div>
    </div>

    <div class="content-wrapper" :style="{ height: contentHeight + 'px'}">
      <vue-scroll :ops="scrollBarSettings">
        <div class="content" v-html="note.content">
        </div>
      </vue-scroll>
    </div>
  </div>
</template>

<script>
import { Message, MessageBox } from "element-ui";
export default {
  data() {
    return {
      note: {},
      isLoading: false,
      searchTagName: '',
      contentHeight: 0,
      scrollBarSettings: {
        bar: {
          background: "#cecece"
        },
        scrollPanel: {
          scrollingX: false
        }
      }
    }
  },
  beforeRouteEnter(to, from, next) {
    next(vm => {
      vm.viewNoteDetail(to.query.noteId)
    })
  },
  updated() {
    this.caculateContentHeight()
  },
  methods: {
    viewNoteDetail(noteId) {
      this.isLoading = true
      this.$http
        .get('/api/note/getNoteDetail', { noteId })
        .then(data => {
          this.note = data.noteDetail
          this.isLoading = false
        })
        .catch(error => {
          this.isLoading = false;
          Message.error('加载失败');
        });
    },
    goBack() {
      this.$router.go(-1)
    },
    caculateContentHeight() {
      this.contentHeight = window.innerHeight - this.$refs.headerWrapper.scrollHeight - 30
    }
  },
  computed: {
    hasData() {
      return this.note !== {}
        && this.note.id > 0
    },
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
.header-wrapper {
  padding-top: 20px;
}
.header {
  cursor: pointer;
}
.title {
  margin-top: 10px;
  min-height: 40px;
  line-height: 40px;
  font-size: 20px;
  font-weight: 100;
}
.tags {
  margin-top: 16px;
}
.tag {
  margin-right: 10px;
}
</style>

