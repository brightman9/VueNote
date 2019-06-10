/*
Navicat MySQL Data Transfer

Source Server         : vuenote_prod
Source Server Version : 80013
Source Host           : 149.28.91.33:3306
Source Database       : vuenote

Target Server Type    : MYSQL
Target Server Version : 80013
File Encoding         : 65001

Date: 2019-06-10 09:35:39
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for note
-- ----------------------------
DROP TABLE IF EXISTS `note`;
CREATE TABLE `note` (
`Id`  int(11) NOT NULL AUTO_INCREMENT ,
`Title`  varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL ,
`Abstract`  varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL ,
`Content`  longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL ,
`CreateTime`  datetime NULL DEFAULT NULL ,
`UpdateTime`  datetime NULL DEFAULT NULL ,
`AuthorId`  int(11) NULL DEFAULT NULL ,
`IsDiscarded`  tinyint(1) NULL DEFAULT NULL ,
PRIMARY KEY (`Id`)
)
ENGINE=InnoDB
DEFAULT CHARACTER SET=utf8mb4 COLLATE=utf8mb4_general_ci
AUTO_INCREMENT=1

;

-- ----------------------------
-- Records of note
-- ----------------------------
BEGIN;
INSERT INTO `note` VALUES ('1', '欢迎使用VueNote', 'VueNote是一款在线笔记应用\n', '<p>VueNote是一款在线笔记应用</p>', '2019-06-06 16:28:06', '2019-06-08 07:38:48', '1', '0');
COMMIT;

-- ----------------------------
-- Table structure for note_tag_relation
-- ----------------------------
DROP TABLE IF EXISTS `note_tag_relation`;
CREATE TABLE `note_tag_relation` (
`NoteId`  int(11) NOT NULL ,
`TagId`  int(11) NOT NULL ,
PRIMARY KEY (`NoteId`, `TagId`)
)
ENGINE=InnoDB
DEFAULT CHARACTER SET=utf8mb4 COLLATE=utf8mb4_general_ci

;

-- ----------------------------
-- Records of note_tag_relation
-- ----------------------------
BEGIN;
INSERT INTO `note_tag_relation` VALUES ('1', '1');
COMMIT;

-- ----------------------------
-- Table structure for permission
-- ----------------------------
DROP TABLE IF EXISTS `permission`;
CREATE TABLE `permission` (
`Id`  int(11) NOT NULL AUTO_INCREMENT ,
`Name`  varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL ,
PRIMARY KEY (`Id`)
)
ENGINE=InnoDB
DEFAULT CHARACTER SET=utf8mb4 COLLATE=utf8mb4_general_ci

;

-- ----------------------------
-- Records of permission
-- ----------------------------
BEGIN;
INSERT INTO `permission` VALUES ('1', 'Note');
COMMIT;

-- ----------------------------
-- Table structure for role
-- ----------------------------
DROP TABLE IF EXISTS `role`;
CREATE TABLE `role` (
`Id`  int(11) NOT NULL AUTO_INCREMENT ,
`Name`  varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL ,
`DisplayName`  varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL ,
PRIMARY KEY (`Id`)
)
ENGINE=InnoDB
DEFAULT CHARACTER SET=utf8mb4 COLLATE=utf8mb4_general_ci
AUTO_INCREMENT=1

;

-- ----------------------------
-- Records of role
-- ----------------------------
BEGIN;
INSERT INTO `role` VALUES ('1', 'user', '普通用户');
COMMIT;

-- ----------------------------
-- Table structure for role_permission_relation
-- ----------------------------
DROP TABLE IF EXISTS `role_permission_relation`;
CREATE TABLE `role_permission_relation` (
`RoleId`  int(11) NOT NULL ,
`PermissionId`  int(11) NOT NULL ,
PRIMARY KEY (`RoleId`, `PermissionId`)
)
ENGINE=InnoDB
DEFAULT CHARACTER SET=utf8mb4 COLLATE=utf8mb4_general_ci

;

-- ----------------------------
-- Records of role_permission_relation
-- ----------------------------
BEGIN;
INSERT INTO `role_permission_relation` VALUES ('1', '1');
COMMIT;

-- ----------------------------
-- Table structure for tag
-- ----------------------------
DROP TABLE IF EXISTS `tag`;
CREATE TABLE `tag` (
`Id`  int(11) NOT NULL AUTO_INCREMENT ,
`Name`  varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL ,
`AuthorId`  int(11) NULL DEFAULT NULL ,
`ParentId`  int(11) NULL DEFAULT NULL ,
`Code`  varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL ,
PRIMARY KEY (`Id`)
)
ENGINE=InnoDB
DEFAULT CHARACTER SET=utf8mb4 COLLATE=utf8mb4_general_ci
AUTO_INCREMENT=1

;

-- ----------------------------
-- Records of tag
-- ----------------------------
BEGIN;
INSERT INTO `tag` VALUES ('1', 'VueNote', '1', null, '[1]');
COMMIT;

-- ----------------------------
-- Table structure for user
-- ----------------------------
DROP TABLE IF EXISTS `user`;
CREATE TABLE `user` (
`Id`  int(11) NOT NULL AUTO_INCREMENT ,
`Name`  varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL ,
`DisplayName`  varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL ,
`HashedPassword`  varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL ,
`RoleId`  int(11) NULL DEFAULT NULL ,
PRIMARY KEY (`Id`)
)
ENGINE=InnoDB
DEFAULT CHARACTER SET=utf8mb4 COLLATE=utf8mb4_general_ci
AUTO_INCREMENT=1

;

-- ----------------------------
-- Records of user
-- ----------------------------
BEGIN;
INSERT INTO `user` VALUES ('1', 'bob', 'bob', 'AQAAAAEAACcQAAAAECiaVH9+r84uTOYlTatt0P3NDf8loxn/NOrBlv4GmF67FGPDQ/GdFL/xtEoB5M48xg==', '1');
COMMIT;

-- ----------------------------
-- Auto increment value for note
-- ----------------------------
ALTER TABLE `note` AUTO_INCREMENT=1;

-- ----------------------------
-- Auto increment value for role
-- ----------------------------
ALTER TABLE `role` AUTO_INCREMENT=1;

-- ----------------------------
-- Auto increment value for tag
-- ----------------------------
ALTER TABLE `tag` AUTO_INCREMENT=1;

-- ----------------------------
-- Auto increment value for user
-- ----------------------------
ALTER TABLE `user` AUTO_INCREMENT=1;
