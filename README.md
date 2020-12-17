# WCS_JKR_BZQ
 金凯瑞包装前

# 平板扫描登陆版本更新

## 【更新说明】

1. 菜单模块信息更新
2. 菜单配置添加PDA功能
3. 字典控制PDA启用登陆功能

## 【操作说明】
1. PDA更新是否打开登陆功能需要重新加载基础数据
2. 打开调度字典
3. 修改 'PDA基础字典版本' 字典值 (+1,改变即可)
4. 平板关闭后重新打开

### 1. 功能模块表添加字段
```
ALTER TABLE `wcs_menu_dtl` ADD COLUMN `rf` bit(1) NULL COMMENT '是否是平板的' AFTER `order`;
```

### 2. 字典信息添加
```

-- 主表信息添加
INSERT INTO `diction`(`id`, `type`, `valuetype`, `name`, `isadd`, `isedit`, `isdelete`, `authorizelevel`) VALUES (6, 0, 0, '版本信息', b'0', b'1', b'0', 1);
INSERT INTO `diction`(`id`, `type`, `valuetype`, `name`, `isadd`, `isedit`, `isdelete`, `authorizelevel`) VALUES (7, 0, 1, '配置开关', b'0', b'1', b'0', 100);

-- 字典详细信息添加
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (57, 6, 'PDA_INIT_VERSION', 'PDA基础字典版本', 9, NULL, '', NULL, NULL, NULL, '2020-12-17 14:00:05');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (58, 6, 'PDA_GOOD_VERSION', 'PDA规格字典版本', 5, NULL, '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (60, 7, 'UserLoginFunction', 'PDA登陆功能开关', NULL, b'0', 'PDA登陆功能开关', NULL, NULL, NULL, '2020-12-17 13:59:59');

```

```
-- 功能模块配置信息更新
-- 清空表数据后更新
INSERT INTO `wcs_module`(`id`, `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES (1, '主页', 0, 'Home', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'HomeCtl', '启动调度自动打开');
INSERT INTO `wcs_module`(`id`, `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES (2, '区域', 0, 'Area', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'AreaCtl', '配置砖机/摆渡车轨道');
INSERT INTO `wcs_module`(`id`, `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES (3, '开关', 0, 'AreaSwitch', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'AreaSwitchCtl', '任务开关控制');
INSERT INTO `wcs_module`(`id`, `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES (4, '摆渡车', 0, 'Ferry', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'FerryCtl', '摆渡车详细状态信息');
INSERT INTO `wcs_module`(`id`, `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES (5, '运输车', 0, 'Carrier', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'CarrierCtl', '运输车详细状态信息');
INSERT INTO `wcs_module`(`id`, `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES (6, '上下砖机', 0, 'TileLifter', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'TileLifterCtl', '上下砖机详细状态信息');
INSERT INTO `wcs_module`(`id`, `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES (7, '轨道信息', 0, 'Track', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'TrackCtl', '轨道状态信息');
INSERT INTO `wcs_module`(`id`, `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES (8, '规格', 0, 'Goods', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'GoodsCtl', '规格详细信息');
INSERT INTO `wcs_module`(`id`, `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES (9, '测可入砖', 0, 'TestGood', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'TestGoodCtl', '测试轨道是否可以放指定的砖');
INSERT INTO `wcs_module`(`id`, `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES (10, '轨道库存', 0, 'Stock', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'StockCtl', '单一轨道库存的详细信息');
INSERT INTO `wcs_module`(`id`, `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES (11, '任务', 0, 'Trans', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'TransCtl', '调度设备的任务信息,状态');
INSERT INTO `wcs_module`(`id`, `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES (12, '添加任务', 0, 'AddManualTrans', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'AddManualTransCtl', '手动添加任务');
INSERT INTO `wcs_module`(`id`, `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES (13, '摆渡对位', 0, 'FerryPos', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'FerryPosCtl', '摆渡车对位，复位');
INSERT INTO `wcs_module`(`id`, `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES (14, '轨道分配', 0, 'TrackAllocate', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'TrackAllocateCtl', '查看砖机选定规格的轨道分配');
INSERT INTO `wcs_module`(`id`, `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES (15, '库存', 0, 'StockSum', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'StockSumCtl', '所有轨道的库存列表信息');
INSERT INTO `wcs_module`(`id`, `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES (16, '警告', 0, 'WarnLog', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'WarnLogCtl', '警告详细信息记录');
INSERT INTO `wcs_module`(`id`, `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES (17, '空满轨道', 0, 'TrackLog', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'TrackLogCtl', '轨道空满轨道信息记录');
INSERT INTO `wcs_module`(`id`, `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES (18, '按轨出库', 0, 'TileTrack', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'TileTrackCtl', '上砖机按轨道上砖使用');
INSERT INTO `wcs_module`(`id`, `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES (19, '品种设置', 1, 'RFGOODTYPESETTING', 'com.keda.wcsfixplatformapp.screen.rfgood.RfGoodMainScreen', '', 'goodstype.png', '', '平板-品种查看/添加');
INSERT INTO `wcs_module`(`id`, `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES (20, '轨道设置', 1, 'RFTRACK', 'com.keda.wcsfixplatformapp.screen.rftrack.RfTrackScreen', '', 'assignment.png', '', '平板-轨道查看/修改状态');
INSERT INTO `wcs_module`(`id`, `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES (21, '砖机规格', 1, 'RFTILEGOOD', 'com.keda.wcsfixplatformapp.screen.rftilegood.RfTileGoodScreen', '', 'updowndev.png', '', '平板-砖机规格查看/修改');
INSERT INTO `wcs_module`(`id`, `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES (22, '任务开关', 1, 'RFTASKSWITCH', 'com.keda.wcsfixplatformapp.screen.rfswitch.RfSwitchScreen', '', 'othersetting.png', '', '平板-区域任务开关控制');
INSERT INTO `wcs_module`(`id`, `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES (23, '摆渡车状态', 1, 'RFDEVFERRY', 'com.keda.wcsfixplatformapp.screen.rfferry.RfFerryScreen', '', 'othersetting.png', '', '平板-摆渡车详细状态');
INSERT INTO `wcs_module`(`id`, `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES (24, '运输车状态', 1, 'RFDEVCARRIER', 'com.keda.wcsfixplatformapp.screen.rfdevcarrier.RfDevCarrierScreen', '', 'shiftcar.png', '', '平板-运输车详细状态');
INSERT INTO `wcs_module`(`id`, `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES (25, '砖机状态', 1, 'RFDEVTILELIFTER', 'com.keda.wcsfixplatformapp.screen.rftilelifter.RfTileLifterScreen', '', 'assignment.png', '', '平板-砖机详细状态');
INSERT INTO `wcs_module`(`id`, `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES (26, '轨道库存', 1, 'RFTRACKSTOCK', 'com.keda.wcsfixplatformapp.screen.rftrackstock.RfTrackStockScreen', '', 'assignment.png', '', '平板-单一轨道库存查看/添加/删除');
INSERT INTO `wcs_module`(`id`, `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES (27, '任务信息', 1, 'RFSTOCKTRANS', 'com.keda.wcsfixplatformapp.screen.rftrans.RfTransScreen', '', 'updowndev.png', '', '平板-任务查看/操作');
INSERT INTO `wcs_module`(`id`, `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES (28, '按轨上砖', 1, 'RFTILETRACK', 'com.keda.wcsfixplatformapp.screen.rftiletrack.RfTileTrackScreen', '', 'updowndev.png', '', '平板-上砖机按轨道上砖');
INSERT INTO `wcs_module`(`id`, `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES (29, '字典', 0, 'Diction', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'DictionCtl', '字典值查看');
INSERT INTO `wcs_module`(`id`, `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES (30, '菜单', 0, 'Menu', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'MenuCtl', 'PC调度菜单配置修改');
INSERT INTO `wcs_module`(`id`, `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES (31, '用户', 0, 'User', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'UserCtl', 'PC调度用户配置');
INSERT INTO `wcs_module`(`id`, `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES (32, '摆渡车对位', 1, 'RFARFTRACK', 'com.keda.wcsfixplatformapp.screen.rfferrypose.RfFerryPosScreen', '', 'arttoposition.png', NULL, '平板-摆渡车对位');

```