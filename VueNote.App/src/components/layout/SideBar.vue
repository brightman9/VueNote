<template>
  <div class="sidebar-container">
    <el-dropdown @command="handleDropDownMenu" trigger="click">
      <div class="user-menu-link">
        <span class="el-icon-user-solid"></span>
        <span class="username">{{username}}</span>
        <span class="el-icon-caret-bottom"></span>
      </div>
      <el-dropdown-menu slot="dropdown">
        <el-dropdown-item>
          <el-link type="primary" href="https://vuenote.info" target="blank">关于VueNote</el-link>
        </el-dropdown-item>
        <el-dropdown-item command="logout">退出登录</el-dropdown-item>
      </el-dropdown-menu>
    </el-dropdown>
    <div class="create-note" @click="createNote">
      <img src="../../assets/img/add.png" alt>
      <span class>新建笔记</span>
    </div>
    <div class="menu">
      <div class="menu-item" tabindex="-1" @click="viewAllNotes">
        <img src="../../assets/img/list.png" alt>
        <span>所有笔记</span>
      </div>
      <TagMenu />
      <div class="menu-item" tabindex="-1" @click="viewDiscardedNotes">
        <img src="../../assets/img/trash.png" alt>
        <span>废纸篓</span>
      </div>
    </div>
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
      username: ""
    };
  },
  created() {
    this.username = this.$auth.getCurrentUser().name;
  },
  mounted() {
    this.$bus.emit("viewAllNotes");
  },
  methods: {
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
    createNote() {
      this.$bus.emit("createNote");
    },
    viewAllNotes() {
      this.$bus.emit("viewAllNotes");
    },
    viewDiscardedNotes() {
      this.$bus.emit("viewDiscardedNotes");
    }
  }
};
</script>

<style scoped>
.sidebar-container {
  height: 100%;
  padding-top: 10px;
  padding-left: 10px;
  background-color: black;
}
.user-menu-link {
  font-size: 14px;
  cursor: pointer;
  color: silver;
}
.user-menu-link > .username {
  margin-left: 5px;
}
.create-note {
  line-height: 40px;
  height: 40px;
  margin-top: 20px;
  margin-bottom: 20px;
  cursor: pointer;
}
.create-note:hover > img {
  width: 40px;
}
.create-note:hover > span {
  font-weight: bold;
}
.create-note > img {
  width: 36px;
  vertical-align: middle;
}
.create-note > span {
  font-size: 16px;
  color: silver;
}
.menu {
  padding-left: 10px;
}
.menu-item {
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

