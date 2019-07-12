<template>
  <div class="sidebar-container" :class="{active:isActive}">
    <div class="main" :class="{active:isActive}">
      <el-dropdown @command="handleDropDownMenu" trigger="click">
        <div class="user-menu-link">
          <span class="el-icon-user-solid"></span>
          <span class="username">{{username}}</span>
          <span class="el-icon-caret-bottom"></span>
        </div>
        <el-dropdown-menu slot="dropdown">
          <el-dropdown-item>
            <el-link type="primary" href="https://github.com/brightman9/VueNote" target="blank">关于VueNote</el-link>
          </el-dropdown-item>
          <el-dropdown-item command="logout">退出登录</el-dropdown-item>
        </el-dropdown-menu>
      </el-dropdown>
      <div class="menu">
        <div class="menu-item" tabindex="-1" @click="viewAllNotes">
          <img src="../../assets/img/list.png" alt>
          <span>所有笔记</span>
        </div>
        <div class="menu-item" tabindex="-1">
          <TagMenu />
        </div>
        <div class="menu-item" tabindex="-1" @click="viewDiscardedNotes">
          <img src="../../assets/img/trash.png" alt>
          <span>废纸篓</span>
        </div>
      </div>
    </div>
    <div class="mask" :class="{active:isActive}" @click="hideSideBar"></div>
  </div>
</template>

<script>
import { Message, MessageBox } from 'element-ui'
import TagMenu from '../note/TagMenu'
export default {
  components: {
    TagMenu
  },
  data() {
    return {
      username: "",
      isActive: false
    };
  },
  created() {
    this.username = this.$auth.getCurrentUser().name;
    this.$bus.on("showSideBar", event => {
      this.isActive = true
    });
    this.$bus.on("hideSideBar", event => {
      this.isActive = false
    });
  },
  mounted() {
    this.$bus.emit("viewAllNotes");
  },
  methods: {
    hideSideBar() {
      this.isActive = false
    },
    handleDropDownMenu(command) {
      if (command === 'logout') {
        this.logout()
      }
    },
    logout() {
      MessageBox.confirm('确定要退出吗？').then(() => {
        this.$auth.deleteToken();
        this.$router.push("/login");
      })
    },
    viewAllNotes() {
      this.isActive = false
      this.$router.push({ path: "/notes", query: { type: 'all' } });
    },
    viewDiscardedNotes() {
      this.isActive = false
      this.$router.push({ path: "/notes", query: { type: 'discarded' } });
    }
  }
};
</script>

<style scoped>
.sidebar-container {
  height: 100%;
  visibility: hidden;
  transition: visibility 1s;
}
.sidebar-container.active {
  visibility: visible;
}

.main {
  z-index: 1000;
  position: absolute;
  left: -100%;
  transition: left 1s;
  width: 80%;
  height: 100%;
  padding-top: 10px;
  padding-left: 10px;
  background-color: black;
}
.main.active {
  left: 0;
}
.mask {
  z-index: 999;
  position: absolute;
  width: 100%;
  height: 100%;
  opacity: 0;
  transition: opacity 1s;
  background-color: black;
}
.mask.active {
  opacity: 0.2;
}

.user-menu-link {
  font-size: 14px;
  cursor: pointer;
  color: silver;
}
.user-menu-link > .username {
  margin-left: 5px;
}
.menu {
  padding-left: 10px;
}
.menu-item {
  margin-top: 20px;
  line-height: 30px;
  min-height: 30px;
  cursor: pointer;
  outline: none;
}
.menu-item:hover {
  font-weight: bold;
  background-color: #333;
}
.menu-item:focus {
  font-weight: bold;
  background-color: #404040;
}
.menu-item > img {
  width: 20px;
  vertical-align: middle;
}
.menu-item > span {
  font-size: 14px;
  color: silver;
}
</style>

