<template>
  <div class="container">
    <h1 class="title">您访问的页面不存在</h1>
    <img class="img" src="../assets/img/404.svg" alt="">
    <el-button class="go-home-button" @click="goHome" type="primary" size="medium">返回首页</el-button>
  </div>
</template>

<script>
export default {
  methods: {
    goHome() {
      this.$router.push('/')
    }
  }
}
</script>


<style scoped>
.container {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  width: 100%;
  height: 100%;
  background-color: #f1f1f1;
}
.title {
  margin-top: 50px;
}
.img {
  margin-top: 20px;
}
.go-home-button {
  margin-top: 50px;
  cursor: pointer;
}
</style>
