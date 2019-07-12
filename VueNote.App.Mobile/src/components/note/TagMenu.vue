<template>
  <div>
    <div class="menu-item-tag" tabindex="-1" @click="switchTagTree">
      <span class="title">
        <img src="../../assets/img/tag.png" alt>
        <span>标签</span>
      </span>
    </div>
    <el-tree :data="tags" ref="tagTree" lazy v-show="showTagTree" class="tags-tree" :load="loadTagTree" node-key="id" :props="defaultProps" @current-change="getTagNotes">
      <span class="tag-tree-node" slot-scope="{ node }">
        <span>{{ node.label }}</span>
      </span>
    </el-tree>
  </div>
</template>

<script>
import { Message, MessageBox } from 'element-ui'
export default {
  data() {
    return {
      tags: [],
      showTagTree: true,
      defaultProps: {
        label: "name"
      }
    };
  },
  methods: {
    switchTagTree() {
      this.showTagTree = !this.showTagTree;
    },
    loadTagTree(node, resolve) {
      if (node.level === 0) {
        this.$http
          .get("/api/note/getRootTags")
          .then(data => {
            return resolve(data.tags);
          })
          .catch(error => {
            return resolve([]);
          });
      } else {
        this.$http
          .get("/api/note/getSubTags", { parentId: node.data.id })
          .then(data => {
            return resolve(data.tags);
          })
          .catch(error => {
            return resolve([]);
          });
      }
    },
    getTagNotes(node) {
      this.$bus.emit('hideSideBar')
      this.$router.push({ path: "notes", query: { type: 'tag', tag: { id: node.id, name: node.name } } });
    }
  }
};
</script>


<style scoped>
.menu-item-tag {
  line-height: 30px;
  display: flex;
  justify-content: space-between;
  min-height: 30px;
  cursor: pointer;
  outline: none;
}
.menu-item-tag > span {
  font-size: 14px;
  color: silver;
}
.menu-item-tag img {
  width: 20px;
  vertical-align: middle;
}
.menu-item-tag > .title {
  font-size: 14px;
  color: silver;
}
.menu-item-tag > .operation {
  padding-right: 8px;
}
.tags-tree {
  font-size: 12px;
  color: silver;
}
.tag-tree-node {
  font-size: 14px;
  display: flex;
  align-items: center;
  flex: 1;
  justify-content: space-between;
  padding-right: 8px;
}
.tag-tree-node img {
  width: 20px;
  vertical-align: middle;
}
</style>
<style>
.el-tree-node__content {
  background-color: black;
}
.el-tree-node__content:hover {
  background-color: #333;
}
.el-tree-node:focus > .el-tree-node__content {
  background-color: #404040;
}
.el-tree__empty-block {
  background-color: black;
}
</style>

