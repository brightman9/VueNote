<template>
  <div class="login-wrapper">
    <div class="login-container">
      <div class="login-logo">
        <img src="../assets/img/logo.png" alt>
      </div>
      <div class="login-title">VueNote</div>
      <el-form class="login-form-body" ref="form" :model="form" :rules="rules" status-icon>
        <el-form-item prop="username">
          <el-input placeholder="请输入用户名" v-model="form.username"></el-input>
        </el-form-item>
        <el-form-item prop="password">
          <el-input placeholder="请输入密码" v-model="form.password" show-password></el-input>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" style="width:100%" @click="login">登录</el-button>
        </el-form-item>
      </el-form>
    </div>
  </div>
</template>

<script>
import { Message } from "element-ui";

export default {
  name: "Login",
  data() {
    return {
      form: {
        username: null,
        password: null
      },
      rules: {
        username: [{ required: true, message: "请输入用户名", trigger: "blur" }],
        password: [{ required: true, message: "请输入密码", trigger: "blur" }]
      }
    };
  },
  methods: {
    login() {
      this.$refs.form.validate(valid => {
        if (valid) {
          this.$http
            .post("/api/user/login", {
              username: this.form.username,
              password: this.form.password
            })
            .then(data => {
              if (data.isLogin) {
                this.$store.set("token", data.token);
                this.$store.set("currentUser", data.user);
                this.$store.set("permissions", data.permissions);
                Message.success("登录成功");
                this.$router.replace("/");
              } else {
                Message.error("登录失败：" + data.message);
                return false;
              }
            });
        } else {
          return false;
        }
      });
    }
  }
};
</script>

<style scoped>
.login-wrapper {
  display: flex;
  align-items: center;
  justify-content: center;

  width: 100%;
  height: 100%;
}
.login-container {
  /* margin: auto; */
}
.login-logo {
  text-align: center;
}
.login-logo > img {
  width: 100px;
  height: 100px;
}
.login-title {
  line-height: 50px;

  height: 50px;

  text-align: center;
}
.login-form-body {
  width: 300px;
}
</style>
