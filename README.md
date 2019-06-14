# VueNote简介
基于Vue.js + .NetCore的在线笔记应用，采用前后端分离架构  
在功能、UI设计上参考了印象笔记的Web版  

# 开发目的
练习前后端技术的实验项目

# 技术栈
1. 前端：Vue.js 2.5、Vue-Router、Axios、ElementUI
2. 服务端：ASP.NET Core 2.2（MVC)、Dapper
3. 数据库：MySql 8.0
4. 操作系统  
    * 研发环境：Windows 10
    * 生产环境：Ubuntu 18.10（Vultr VPS）  
5. 开发工具：Visual Studio 2017、VSCode  

# 安装
1. 在本机host文件中增加一行：127.0.0.1 dev.vuenote.info
2. 打开MySql8.0，新建数据库vuenote，然后执行deploy目录下的db-init.sql
3. 在前端项目VueNote.App目录下，执行命令：npm install
4. 启动webpack dev-server：npm run dev
5. 双击VueNote.sln，将VueNote.WebApi设为启动项目，F5运行
6. 在浏览器中打开地址：http://dev.vuenote.info:8080 如果一切正常则可以看到登录界面

# 在线演示  
演示地址：[https://vuenote.info](https://vuenote.info)  
登录账号：bob，密码：123  

注：为达到最佳效果，建议使用chrome浏览站点  


<img src="https://i.loli.net/2019/06/09/5cfcfeb161e7b90837.png">  

<img src="https://i.loli.net/2019/06/09/5cfcfeb17e90048643.png">

