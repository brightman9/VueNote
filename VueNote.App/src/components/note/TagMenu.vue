<template>
    <div>
      <div class="menu-item-tag" tabindex="-1" @click="switchTagTree">
        <span class="title">
          <img src="../../assets/img/tag.png" alt>
          <span>标签</span>
        </span>
        <span class="operation">
          <el-button title="添加标签" type="text" size="mini" @click.stop="addRootTag">
            <img src="../../assets/img/add-item.png" alt>
          </el-button>
        </span>
      </div>
      <el-tree :data="tags" ref="tagTree" lazy v-show="showTagTree" class="tags-tree" :load="loadTagTree" node-key="id" :props="defaultProps" @current-change="getTagNotes">
        <span class="tag-tree-node" slot-scope="{ node }">
          <span>{{ node.label }}</span>
          <span>
            <el-button title="添加标签" type="text" size="mini" @click.stop="addSubTag(node)">
              <img src="../../assets/img/add-item.png" alt>
            </el-button>
            <el-button title="删除标签" type="text" size="mini" @click.stop="removeTag(node)">
              <img src="../../assets/img/remove-item.png" alt>
            </el-button>
          </span>
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
    addRootTag() {
      this.addTag(this.$refs.tagTree.root, true)
    },
    addSubTag(node) {
      this.addTag(node, false)
    },
    addTag(parentNode, isRootTag) {
      const parentData = parentNode.data
      MessageBox.prompt('', {
        title: isRootTag ? '添加标签' : '给“' + parentData.name + '”添加子标签',
        inputPlaceholder: '请输入标签名称',
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        inputPattern: /\S+/,
        inputErrorMessage: '标签名称不能为空',
        beforeClose: (action, instance, done) => {
          if (action === 'confirm') {
            instance.confirmButtonLoading = true;
            const tagName = instance.inputValue
            this.$http
              .post("/api/note/createTag", { tagName: tagName, parentId: isRootTag ? null : parentData.id })
              .then(data => {
                if (data.succeeded) {
                  const newTag = { id: data.tagId, name: tagName, children: [] };
                  this.$refs.tagTree.append(newTag, parentNode)
                  instance.confirmButtonLoading = false;
                  Message.success('添加成功')
                  done()
                }
                else {
                  instance.editorErrorMessage = data.message
                  instance.confirmButtonLoading = false;
                  Message.success('添加失败')
                }
              })
          } else {
            instance.confirmButtonLoading = false;
            done();
          }
        }
      })
    },
    removeTag(node) {
      const nodeData = node.data
      MessageBox.confirm('此操作将删除“' + nodeData.name + '”及其所有子标签，确定要继续吗？',
        '删除标签：' + nodeData.name, {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning',
          beforeClose: (action, instance, done) => {
            if (action === 'confirm') {
              instance.confirmButtonLoading = true;
              this.$http
                .post("/api/note/removeTag", { tagId: nodeData.id })
                .then(data => {
                  if (data.succeeded) {
                    this.$refs.tagTree.remove(node)
                    instance.confirmButtonLoading = false;

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
    getTagNotes(node) {
      this.$bus.emit("viewTagNotes", { tagId: node.id });
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

