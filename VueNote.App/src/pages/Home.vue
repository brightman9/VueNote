<template>
  <div class="home-wrapper">
    <div class="sidebar-wrapper">
      <SideBar/>
    </div>
    <div class="note-list-wrapper">
      <NoteSearch/>
    </div>
    <div class="note-detail-wrapper">
      <NoteDetail/>
    </div>
  </div>
</template>

<script>
import SideBar from "../components/layout/SideBar";
import NoteSearch from "../components/note/NoteSearch";
import NoteDetail from "../components/note/NoteDetail";
export default {
  name: "Home",
  components: {
    SideBar: SideBar,
    NoteSearch: NoteSearch,
    NoteDetail: NoteDetail
  }
};
</script>

<style scoped>
.home-wrapper {
  height: 100%;
  display: flex;
}
.sidebar-wrapper {
  flex: 0 0 auto;
  width: 20%;
}
.note-list-wrapper {
  flex: 0 0 auto;
  width: 25%;
}
.note-detail-wrapper {
  flex: 1 0 content;
   width: 55%;
}
/* 当设备屏幕较小时，只显示笔记列表栏 */
/* @media only screen and (max-width: 600px) {
  .sidebar-wrapper {
    display: none;
  }
  .note-list-wrapper {
    flex: 1 0 auto;
    width: 100%;
  }
  .note-detail-wrapper {
    display: none;
  }
} */
</style>


