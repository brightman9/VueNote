<template>
  <div class="home-wrapper">
    <div class="sidebar-wrapper">
      <SideBar />
    </div>
    <div class="main-wrapper">
      <keep-alive>
        <router-view />
      </keep-alive>
    </div>
  </div>
</template>

<script>
import SideBar from "../components/layout/SideBar";
export default {
  name: "Home",
  components: {
    SideBar: SideBar
  }
};
</script>

<style scoped>
.sidebar-wrapper {
  width: 100%;
  height: 100%;
  position: absolute;
  left: 0;
  top: 0;
}
.main-wrapper {
  width: 100%;
  height: 100%;
  position: absolute;
  left: 0;
  top: 0;
}
</style>


