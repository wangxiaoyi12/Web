#项目名称： 竹之语

#业务：陈总


#测试地址：
http://zzy.zxfsoft.com/ 账号：admin/11111
http://zzy.zxfsoft.com/admin 账号：admin/admin    zzy888
http://zzytest.ceshi99.com/admin  账号：admin/zzy888

竹迪的备案下来了    浙ICP备17044713号-2
域名  www.tzzdqclm.com  账号：admin/zzy888
zdht
zzy888



#正式地址：
公众号：
帐号：1036736233@qq.com
密码：zdqclm890


公众号：
1801744053@qq.com
hyzzy888


商户：
帐号：1489439702@1489439702
密码：zdqclm809000


AppID:wxc0520775aa8d5d84
appsecret:b9d9f3495baf18eb643c9a069bc65bb6
token:1036736233
key:SLtA4y0sBQgW37rWOZwxm8FNJPTBhV4Dm2xfmdVwaH8

admin:MsJToOZNIDmiShPbB7notjEf25DzOURNhsU9LddmJ4b
kUduA5fBZJQ==


#会员表修改
收益累计：CommissionSum
收益结余：CommissionBan
收益：Commission，电子币：Coins，积分：Scores，
购物币：ShopCoins， 旅游积分：TourScores

有四种结算方式    1、购物币+积分    2、全额购物币   3、现金+积分    4、全额现金

第一次下单金额:FirstActive,
头像：Photo
星级：StarLevel



#会员等级表修改n
级别名称，投资额，日封顶，直推一代，直推二代


#参数整理
1.注册赠送积分倍数：Reg_Score 5 ，购买积分： 100倍数  ，赠送倍数：10
2.互助奖：HuZhuPercent
3.领导奖累计业绩， 领导奖 比例, 一星：10个
4.师长分红：5%
5.旅游奖： 条件，赠送
6.重消奖 ：   会员商品》社区店：10%  间接推荐：5%  直推：10%
			  爆款商品》社区店：10%  
			  促销商品》社区店：5%	间接推荐：2.5% 直推：5%
			
7.重消购物币： 10%
8.提现手续费：5%

#收益整理：
直推一代奖，直推二代奖，互助奖，
领导奖，师长分红，
重消奖


--------------------其他结构整理
#商品表修改
是否包邮：IsPostage


-----收益 结算记录
create table SettleRecord(
ID int identity(1,1),
CreateTime datetime not null,---结算时间
AllCount int not null,---结算人数
AllAmount decimal(18,2) not null,---总结
AllCommission decimal(18,2) not null,---总收益
AllCoins decimal(18,2) not null,--总电子币
Type int not null,---结算类型 1---周结算 2--月结算
primary key(ID)
);
go



----积分充值记录
create table ScoreRecord(
ID int identity(1,1),
CreateTime datetime not null,
Scores decimal(18,2) not null,---充值数量
Amount decimal(18,2) not null,---扣现金数量
State int not null,---充值状态，1--待付款 2--充值成功
TradeNo varchar(100) null,---交易单号
Remark varchar(200) null,
MemberID varchar(50) not null,---充值会员
MemberCode varchar(50) not null,---充值会员编号
MemberName varchar(50) null,---会员姓名
primary key(ID)
);



1/2/3项是秒结；4/9是月结（月结时根据考核条件）；
师长分红和社区店分红，是手工操作；
旅游积分是到了就自动配送，这个秒结月结都可以；



-----调整3：
竹迪商城需要帮调整一下：
1、手机端的首页不要显示订单商品，如果要选购订单商品，点击“订单商品”菜单进入；
2、把领导奖和重消奖由原来的月结都改为秒结（和测试的时候一样）；
3、考虑到月结收益有个次月必须重复消费89元的考核条件，改成秒结就把这个限制去掉。


-----模板消息
1.提现消息：
失败：OPENTM408529050

2.下单消息：


交接后修改问题：
2018-3-21
1.爆款商品不参与满额包邮，现在是参与的
2018-3-23
2.修复服务费重复产生的可能
3.订单发货提示一闪而过问题
2018-3-28
4.业绩统计不对，里面的RPosition之前数据有问题，很多重复的，所有会员又重新设置了一遍
5.增加一个自提的功能，在付款页面增加一个自提和快递两个选项，选择自提的时候，邮费不计入总金额里面
6.优化收益总表查询性能，Fin_info增加辅助字段，相关字段加索引，重新修改查询
