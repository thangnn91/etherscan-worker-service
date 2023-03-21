/*
 Navicat Premium Data Transfer

 Source Server         : localhost
 Source Server Type    : MySQL
 Source Server Version : 50621
 Source Host           : localhost:3306
 Source Schema         : etherscan

 Target Server Type    : MySQL
 Target Server Version : 50621
 File Encoding         : 65001

 Date: 20/03/2023 08:20:58
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for block
-- ----------------------------
DROP TABLE IF EXISTS `block`;
CREATE TABLE `block`  (
  `blockID` int(11) NOT NULL AUTO_INCREMENT,
  `blockNumber` int(11) NULL DEFAULT NULL,
  `hash` varchar(66) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `parentHash` varchar(66) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `miner` varchar(42) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `blockReward` decimal(50, 0) NULL DEFAULT NULL,
  `gasLimit` decimal(50, 0) NULL DEFAULT NULL,
  `gasUsed` decimal(50, 0) NULL DEFAULT NULL,
  PRIMARY KEY (`blockID`) USING BTREE,
  UNIQUE INDEX `unique_blockNumber`(`blockNumber`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Compact;

-- ----------------------------
-- Records of block
-- ----------------------------

-- ----------------------------
-- Table structure for transaction
-- ----------------------------
DROP TABLE IF EXISTS `transaction`;
CREATE TABLE `transaction`  (
  `transactionID` int(11) NOT NULL AUTO_INCREMENT,
  `blockID` int(11) NULL DEFAULT NULL,
  `hash` varchar(66) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `from` varchar(42) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `to` varchar(42) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `value` decimal(50, 0) NULL DEFAULT NULL,
  `gas` decimal(50, 0) NULL DEFAULT NULL,
  `gasPrice` decimal(50, 0) NULL DEFAULT NULL,
  `transactionIndex` int(11) NULL DEFAULT NULL,
  PRIMARY KEY (`transactionID`) USING BTREE,
  INDEX `fk_transaction_block`(`blockID`) USING BTREE,
  UNIQUE INDEX `unique_txhash`(`hash`) USING BTREE,
  CONSTRAINT `fk_transaction_block` FOREIGN KEY (`blockID`) REFERENCES `block` (`blockID`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Compact;

-- ----------------------------
-- Records of transaction
-- ----------------------------

SET FOREIGN_KEY_CHECKS = 1;
