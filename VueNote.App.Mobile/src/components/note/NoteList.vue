<template>
  <div>
    <div v-for="note in notes" :key="note.id" class="note-item-container" @click="select(note.id)" :class="{ selected:note.id === selectedItemId }" :style="{ height: listItemHeight + 'px'}">
      <div class="title">{{note.title}}</div>
      <div class="abstract">{{note.abstract}}</div>
      <div class="date">{{toLocalTime(note.date)}}</div>
    </div>
  </div>
</template>

<script>
import time from '../../util/time'
export default {
  name: "NoteList",
  data() {
    return {
      selectedItemId: null
    }
  },
  props: {
    notes: Array,
    listItemHeight: Number
  },
  methods: {
    select(noteId) {
      this.selectedItemId = noteId
      this.$router.push({ path: 'note', query: { noteId } })
    },
    toLocalTime(utcTime) {
      return time.toLocalTime(utcTime)
    }
  }
}
</script>

<style scoped>
.note-item-container {
  overflow: hidden;
  padding-right: 20px;
  padding-left: 20px;
  border-bottom: 1px solid #e6e6e6;
}
.note-item-container:hover,
.selected {
  outline: none;
  background-color: #ebeaec;
}
.title {
  font-size: 15px;
  font-weight: 300;
  overflow: hidden;
  margin-top: 16px;
  white-space: nowrap;
  text-overflow: ellipsis;
  color: #333;
}
.abstract {
  font-size: 13px;
  font-weight: 300;
  line-height: 1.5;
  overflow: hidden;
  height: 40px;
  margin-top: 5px;
  word-wrap: break-word;
  color: gray;
}
.date {
  font-size: 12px;
  font-weight: 100;
  margin-top: 6px;
  margin-bottom: 8px;
  color: #a6a6a6;
}
</style>
