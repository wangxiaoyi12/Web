/*****宽窄屏幕自适应****/
$(document).ready(function(){
if(screen.width > 1280 && $(window).width() > 1280)
{
	$("html").addClass("root61");
}
});
$(document).ready(function(){
  
  $('li.hd_menu_tit').mousemove(function(){
  $(this).find('div.hd_menu_list,div.hd_Shopping_list').show();//you can give it a speed
  });
  $('li.hd_menu_tit').mouseleave(function(){
  $(this).find('div.hd_menu_list,div.hd_Shopping_list').hide();
  });
 $(function(){
	$(".fixed_qr_close").click(function(){
		$(".mod_qr").hide();
	})
});
//产品边框样式
 $(".service_list li").hover(function(){
			$(this).addClass("hover");
			//$(this).children(".dorpdown-layer").attr('class','');
		},function(){
			$(this).removeClass("hover");  
			//$(this).children(".dorpdown-layer").attr('class','');
		}
	);
	/*方向*/
	 $("#Brand_Show").hover(function(){
			$(this).addClass("hover");
			//$(this).children(".dorpdown-layer").attr('class','');
		},function(){
			$(this).removeClass("hover");  
			//$(this).children(".dorpdown-layer").attr('class','');
		}
	); 
 //购物车
 $("div.hd_Shopping_list").hover(function(){
			$(this).addClass("hover");
			//$(this).children(".dorpdown-layer").attr('class','');
		},function(){
			$(this).removeClass("hover");  
			//$(this).children(".dorpdown-layer").attr('class','');
		}
	); 
 //支付方式
 $("#payment").hover(function(){
			$(this).addClass("hover");
			//$(this).children(".dorpdown-layer").attr('class','');
		},function(){
			$(this).removeClass("hover");  
			//$(this).children(".dorpdown-layer").attr('class','');
		}
	); 
		 $("li.gl-item").hover(function(){
			$(this).addClass("hover");
			//$(this).children(".dorpdown-layer").attr('class','');
		},function(){
			$(this).removeClass("hover");  
			//$(this).children(".dorpdown-layer").attr('class','');
		}
	); 
	 $(".pro_ad_slide").hover(function(){
			$(this).addClass("hover");
			//$(this).children(".dorpdown-layer").attr('class','');
		},function(){
			$(this).removeClass("hover");  
			//$(this).children(".dorpdown-layer").attr('class','');
		}
	);
	 $(".list_style li").hover(function(){
			$(this).addClass("hover");
			//$(this).children(".dorpdown-layer").attr('class','');
		},function(){
			$(this).removeClass("hover");  
			//$(this).children(".dorpdown-layer").attr('class','');
		}
	);
	 $(".page_recommend").hover(function(){
			$(this).addClass("hover");
			//$(this).children(".dorpdown-layer").attr('class','');
		},function(){
			$(this).removeClass("hover");  
			//$(this).children(".dorpdown-layer").attr('class','');
		}
	);
	/***边框**/
	 $("ul .collect_p").hover(function(){
			$(this).addClass("hover");

		},function(){
			$(this).removeClass("hover");  
		}
	);
	$("#lists li").hover(function(){
		$(this).find(".Detailed").show();
	},function(){
		$(this).find(".Detailed").hide();
});
	$("#lists li").hover(function(){
												
			$(this).addClass("hover_nav");
												
		},function(){
			$(this).removeClass("hover_nav" );  
		}
	); 
});

$(document).ready(function(){
	$("#nav li.no_sub").hover(function(){
			$(this).addClass("hover1");
		},function(){
			$(this).removeClass("hover1");  
		}
	); 
});
$(document).ready(function(){
		$(".clearfix li.list_name, li.phone_c").hover(function(){
			$(this).addClass("hd_menu_hover");
			$(this).children("ul li.list_name_bg").attr('class','');
		},function(){
			$(this).removeClass("hd_menu_hover");  
			$(this).children("ul li.list_name_bg").attr('class','');
		}
	); 				   
$("#allSortOuterbox li").hover(function(){
		$(this).find(".menv_Detail").show();
	},function(){
		$(this).find(".menv_Detail").hide();
});
	$("#allSortOuterbox li.name").hover(function(){
												
			$(this).addClass("hover_nav");
												
		},function(){
			$(this).removeClass("hover_nav" );  
		});
		$("div.display ").hover(function(){
		$(this).addClass("hover");
	},function(){
		$(this).removeClass("hover" );  
});	
});
$(document).ready(function(){
$("#lists li").hover(function(){
		$(this).find(".Detailed").show();
	},function(){
		$(this).find(".Detailed").hide();
});
	$("#lists li").hover(function(){
												
			$(this).addClass("hover_nav");
												
		},function(){
			$(this).removeClass("hover_nav" );  
		}
	); 
});
/********************订单js******************/
$(document).ready(function(){
	 $('#select').find('ul').click(function(){
	$('#select').find('ul').removeClass('active');
	$(this).addClass('active');
  }); 
   $("#select ul").hover(function(){
			$(this).addClass("hover");
			//$(this).children(".dorpdown-layer").attr('class','');
		},function(){
			$(this).removeClass("hover");  
			//$(this).children(".dorpdown-layer").attr('class','');
		}
	); 	
	   $(".dowebok li").hover(function(){
			$(this).addClass("hover");
			//$(this).children(".dorpdown-layer").attr('class','');
		},function(){
			$(this).removeClass("hover");  
			//$(this).children(".dorpdown-layer").attr('class','');
		}
	);					
});
/**********鼠标移动效果************/
$(document).ready(function(){
	$("ul.products li").hover(function() {
		$(this).find(".title").stop()
		.animate({bottom: "0", }, "fast")
		.css("display","block")

	}, function() {
		$(this).find(".title").stop()
		.animate({bottom: "-30",}, "fast")
	});
	//菜单栏效果
			$('.all-sort-list > .item').hover(function(){
			var eq = $('.all-sort-list > .item').index(this),				//获取当前滑过是第几个元素
				h = $('.all-sort-list').offset().top,						//获取当前下拉菜单距离窗口多少像素
				s = $(window).scrollTop(),									//获取游览器滚动了多少高度
				i = $(this).offset().top,									//当前元素滑过距离窗口多少像素
				item = $(this).children('.item-list').height(),				//下拉菜单子类内容容器的高度
				sort = $('.all-sort-list').height();						//父类分类列表容器的高度
			
			if ( item < sort ){												//如果子类的高度小于父类的高度
				if ( eq == 0 ){
					$(this).children('.item-list').css('top', (i-h));
				} else {
					$(this).children('.item-list').css('top', (i-h)+1);
				}
			} else {
				if ( s > h ) {												//判断子类的显示位置，如果滚动的高度大于所有分类列表容器的高度
					if ( i-s > 0 ){											//则 继续判断当前滑过容器的位置 是否有一半超出窗口一半在窗口内显示的Bug,
						$(this).children('.item-list').css('top', (s-h)+2 );
					} else {
						$(this).children('.item-list').css('top', (s-h)-(-(i-s))+2 );
					}
				} else {
					$(this).children('.item-list').css('top', 3 );
				}
			}	

			$(this).addClass('hover');
			$(this).children('.item-list').css('display','block');
		},function(){
			$(this).removeClass('hover');
			$(this).children('.item-list').css('display','none');
		});

		$('.item > .item-list > .close').click(function(){
			$(this).parent().parent().removeClass('hover');
			$(this).parent().hide();
		});
});
$(window).scroll(function() {
		var topToolbar = $("#Sorted");
		var headerH = $("#header_top").outerHeight();
		var headers = $("#Filter_style").outerHeight();
		var scrollTop =$(document.body).scrollTop();	
			if( scrollTop >= headerH + headers ){
				topToolbar.stop(false,true).addClass("fixToTop");
			}else if( scrollTop < headerH + headers  ){
				topToolbar.stop(false,true).removeClass("fixToTop"); 
			}
});
/*********************点击事件*********************/
$( document).ready(function(){
  $('.fixed_bar').find('li').click(function(){
	$('.fixed_bar').find('li').removeClass('active');
	$(this).addClass('active');
  });	
  $('.infonav_hidden').click(function(){
	$('.infonav_more').find('a').removeClass('active');
	$(this).addClass('active');
	$(this).slideToggle();
  });																
})
/*****************************左右伸缩样式****************************/
$(window).scroll(function() {
/*var flag=1;
$('#rightArrow').click(function(){
	if(flag==1){
		$("#floatDivBoxs").animate({left: '-175px'},300);
		$(this).animate({left: '-5px'},300);
		$(this).css('background-position','-50px 0');
		flag=0;
	}else{
		$("#floatDivBoxs").animate({left: '0'},300);
		$(this).animate({left: '170px'},300);
		$(this).css('background-position','0px 0');
		flag=1;
	}
});*/
});

/********************密码强度***********************/
 $(document).ready(function(){
        $('#tbPassword').focus(function () {
            $('#pwdLevel_1').attr('class', 'ywz_zhuce_hongxian');
            $('#tbPassword').keyup();
        });
        $('#tbPassword').keyup(function () {
            var __th = $(this);
            if (!__th.val()) {
                $('#pwd_tip').hide();
                $('#pwd_err').show();
                Primary();
                return;
            }
            if (__th.val().length < 6) {
                $('#pwd_tip').hide();
                $('#pwd_err').show();
                Weak();
                return;
            }
            var _r = checkPassword(__th);
            if (_r < 1) {
                $('#pwd_tip').hide();
                $('#pwd_err').show();
                Primary();
                return;
            }
            if (_r > 0 && _r < 2) {
                Weak();
            } else if (_r >= 2 && _r < 4) {
                Medium();
            } else if (_r >= 4) {
                Tough();
            }
            $('#pwd_tip').hide();
            $('#pwd_err').hide();
        });
        function Primary() {
            $('#pwdLevel_1').attr('class', 'ywz_zhuce_huixian');
            $('#pwdLevel_2').attr('class', 'ywz_zhuce_huixian');
            $('#pwdLevel_3').attr('class', 'ywz_zhuce_huixian');
        }
        function Weak() {
            $('#pwdLevel_1').attr('class', 'ywz_zhuce_hongxian');
            $('#pwdLevel_2').attr('class', 'ywz_zhuce_huixian');
            $('#pwdLevel_3').attr('class', 'ywz_zhuce_huixian');
        }
        function Medium() {
            $('#pwdLevel_1').attr('class', 'ywz_zhuce_hongxian');
            $('#pwdLevel_2').attr('class', 'ywz_zhuce_hongxian2');
            $('#pwdLevel_3').attr('class', 'ywz_zhuce_huixian');
        }
        function Tough() {
            $('#pwdLevel_1').attr('class', 'ywz_zhuce_hongxian');
            $('#pwdLevel_2').attr('class', 'ywz_zhuce_hongxian2');
            $('#pwdLevel_3').attr('class', 'ywz_zhuce_hongxian3');
        }
        function checkPassword(pwdinput) {
            var maths, smalls, bigs, corps, cat, num;
            var str = $(pwdinput).val()
            var len = str.length;
            var cat = /.{16}/g
            if (len == 0) return 1;
            if (len > 16) { $(pwdinput).val(str.match(cat)[0]); }
            cat = /.*[\u4e00-\u9fa5]+.*$/
            if (cat.test(str)) {
                return -1;
            }
            cat = /\d/;
            var maths = cat.test(str);
            cat = /[a-z]/;
            var smalls = cat.test(str);
            cat = /[A-Z]/;
            var bigs = cat.test(str);
            var corps = corpses(pwdinput);
            var num = maths + smalls + bigs + corps;
            if (len < 6) { return 1; }
            if (len >= 6 && len <= 8) {
                if (num == 1) return 1;
                if (num == 2 || num == 3) return 2;
                if (num == 4) return 3;
            }
            if (len > 8 && len <= 11) {
                if (num == 1) return 2;
                if (num == 2) return 3;
                if (num == 3) return 4;
                if (num == 4) return 5;
            }
            if (len > 11) {
                if (num == 1) return 3;
                if (num == 2) return 4;
                if (num > 2) return 5;
            }
        }
        function corpses(pwdinput) {
            var cat = /./g
            var str = $(pwdinput).val();
            var sz = str.match(cat)
            for (var i = 0; i < sz.length; i++) {
                cat = /\d/;
                maths_01 = cat.test(sz[i]);
                cat = /[a-z]/;
                smalls_01 = cat.test(sz[i]);
                cat = /[A-Z]/;
                bigs_01 = cat.test(sz[i]);
                if (!maths_01 && !smalls_01 && !bigs_01) { return true; }
            }
            return false;
        } 
 })

/********************/
 //收藏本站 bbs.ecmoban.com
 function AddFavorite(title, url) {
     try {
         window.external.addFavorite(url, title);
     }
     catch (e) {
         try {
             window.sidebar.addPanel(title, url, "");
         }
         catch (e) {
             alert("抱歉，您所使用的浏览器无法完成此操作。\n\n加入收藏失败，请使用Ctrl+D进行添加");
         }
     }
 }
 //退出登录
 function logOff() {
     if (window.confirm('确定要退出吗')) {
         $.post('/shop/login/logoff', {}, function (data) {
             if (data.status == 1) {
                 location.reload();
             } else {
                 alert(data.msg);
             }
         });
     }
 }

 /*
 * 操作消息显示  1.0 样式控制
 * 1.可指定进度条的位置 top center
 * 2.可指定是否是模式 进度条
 * 3.可指定进度条的宽度高度
 */
 (function () {
     var Msg = function (opts) {
         var defaults = {
             width: 150,
             height: 30,
             top: 'top',  //指定显示区 距离顶部的高度 top,ceneter
             model: false,
             content: '数据加载中...',
             hideType: 'fade', //消失的方式 fade 渐变消失 ，upfade 向上渐变消失
             loadImg: '/assets/global/myplugins/Msg2.0/loading.GIF',//指定加载的进度条图片的大小 small---loading.gif | middle-loading2.gif

             onShow: function () { },//开始显示 事件
             onDestory: function () { }, //销毁显示事件
             onHide: function () { }  //隐藏 事件
         }
         this.opts = $.extend({}, defaults, opts);

         this.init();
     }

     //用于显示结果的定时消失
     var timer = null;
     Msg.prototype = {
         //初始化显示
         init: function () {
             var _this = this;
             var _opts = this.opts;

             //创建 任务消息显示 
             var msgPanel = _this.getPanel();
             msgPanel.empty();
             //遮罩层
             if (_opts.model == true) {
                 var msgBack = getDiv('msgBack');
                 msgBack.appendTo(msgPanel);
             }

             //显示区
             var msgInner = getDiv('msgInner');
             msgInner.appendTo(msgPanel);
             msgInner.css({
                 width: _opts.width,
                 height: _opts.height
             });
             _this.setInnerSite();
             if (_opts.top == 'center') {
                 $(window).resize(function () {
                     _this.setInnerSite();
                 });
             }


             //创建内部  进度显示区
             var loadDiv = getDiv('loadDiv');
             loadDiv.appendTo(msgInner)
             //内容控制
             var text = $('<span />');
             text.addClass('msgText');
             text.html(_opts.content);
             loadDiv.html(text);

             var loading = $('<img />')
             loading.addClass('loading').attr('src', _opts.loadImg);
             loadDiv.append(loading);
             loading.css({
                 left: 13,
                 top: (_opts.height - 16) / 2
             });

             // 创建内部 结果显示区
             var msgResult = getDiv('msgResult');
             msgResult.appendTo(msgInner).hide();
         },
         //设置inner 的位置
         setInnerSite: function () {
             var _this = this;
             var _opts = this.opts;
             var inner = _this.getInner();

             //计算内容的y轴位置
             var top = 0;
             if (_opts.top == 'center') {
                 top = ($(window).height() - _opts.height) / 2;

             } else if (_opts.top == 'top') {
                 top = 0;
             } else {
                 top = _opts.top;
             }
             inner.css('top', top);

         },
         ////显示 success信息
         //showSuccess: function () {

         //},
         ////显示 error信息
         //showError: function () {

         //},

         //获取panel
         getPanel: function () {
             var _opts = this.opts;
             var msgPanel = $(document.body).find('.msgPanel');
             if (msgPanel.length <= 0) {
                 //创建容器
                 msgPanel = getDiv('msgPanel');
                 $(document.body).prepend(msgPanel);

                 //判断是否 显示背景
                 if (_opts.model == true) {
                     msgPanel.append(getDiv('msgBack'));
                 }
             }
             return msgPanel;
         },
         //获取内部显示
         getInner: function () {
             return this.getPanel().find('.msgInner');
         },
         //获取显示结果部分
         getResult: function () {
             return this.getInner().find('.msgResult');
         },
         //获取显示 进度部分
         getLoadDiv: function () {
             return this.getInner().find('.loadDiv');
         },

         //显示结果
         showResult: function (content) {
             var _this = this;

             //显示控件
             _this.showPanel();

             var result = _this.getResult();
             result.html(content);

             //显示控制
             result.show().siblings().hide();

             //定时消失
             timer = setTimeout(function () {
                 _this.hide();
             }, 2000);

         },
         //显示 过程中
         show: function (content) {
             var _this = this;
             _this.showPanel();
             var loadDiv = _this.getLoadDiv();
             if (content != undefined && content.length > 0) {
                 var text = loadDiv.find('.msgText');
                 if (/\.{3}$/.test(content) == false) {
                     content += '...';
                 }
                 text.html(content);
             }
             //显示控制
             loadDiv.show().siblings().hide();
             //清楚自动消失
             if (timer != null) {
                 clearTimeout(timer);
             }
         },
         //显示 panel 和inner
         showPanel: function () {
             var _this = this;
             _this.getPanel().show();
             _this.getInner().fadeIn(200);
         },
         //隐藏
         hide: function () {
             var _this = this;
             var _opts = this.opts;

             var inner = _this.getInner();

             if (_opts.hideType == 'fade') {
                 inner.fadeOut('slow', function () {
                     _this.getPanel().hide();
                 });
             }
             else if (_opts.hideType == 'upfade') {
                 var currentTop = inner.offset().top;
                 var currentWidth = inner.width();
                 inner.animate({
                     opacity: 0,
                     top: 0
                 }, 400, function () {
                     _this.getPanel().hide();

                     inner.css({
                         opacity: 1,
                         top: currentTop
                     });
                 });
             }

             //清楚自动消失
             if (timer != null) {
                 clearTimeout(timer);
             }


         },
         //销毁
         destory: function () {
             this.getPanel().remove();
         }
     }
     window.Msg = Msg;
     function getDiv(cla) {
         var div = $('<div />');
         div.addClass(cla);
         return div;
     }
     
     window.alert = function (content) {
         var msg = new Msg();
         msg.showResult(content);
     }
 })();
 /*
 *  window.urlHelper
 *  浏览器 地址栏 操作帮助类
 */
 (function () {
     var urlHelper = {
         //获取地址栏中的ID  http://....../xxx.html
         getUrlID: function () {
             var url = this.getHref();
             var number = url.substr(url.lastIndexOf('/') + 1);
             number = number.substr(0, number.indexOf('.'));
             return number;
         },
         //获取地址栏 url 全路径
         getHref: function () {
             return window.location.href.toLowerCase();
         },
         //获取地址栏 参数部分
         getParams: function () {
             return window.location.search;
         },
         //为地址栏 设置参数-----参数统一小写
         setParams: function (params) {
             params = params.toLowerCase();
             window.location.href =
                 window.location.href.replace(window.location.search, '')
                 + '?' + params;
         },
         //获取 地址栏 参数key对应的值--key不区分大小写
         getQueryString: function (name) {
             var reg = new RegExp("(^|&)" + name.toLowerCase() + "=([^&]*)(&|$)");
             var r = window.location.search.substr(1).match(reg);
             if (r != null)
                 return r[2];
             return "";
         },
         //页面跳转--当前页面,统一小写
         open: function (url) {
             window.location.href = url.toLowerCase();
         },
         //页面跳转---新窗口，统一小写
         openNew: function (url) {
             window.open(url.toLowerCase(), '_blank');
         },
         //判断地址栏中是否有指定的字符串---不区分大小写
         isContain: function (str) {
             str = str.toLowerCase();
             var url = this.getHref();

             if (url.indexOf(str) == -1)
                 return false;
             return true;
         }
         //向地址栏中添加参数
     }
     window.urlHelper = urlHelper;
 })();
 (function ($) {
     $.myPost = function (url, data, successCB, errorCB) {
         $.ajax({
             url: url,
             data: data,
             type: 'post',
             success: function (data) {
                 if (data.status == 1) {
                     if (successCB)
                         successCB(data);
                 } else {
                     if (errorCB)
                         errorCB(data);
                     else {
                         alert(data.msg);
                     }
                 }
             },
             error: function (data) {
                 console.error(data);
             }
         });
     }
 })(jQuery);
 /**购物车操作*/
 (function () {
     var catOperate = {
         //判断用户是否登录
         isLogin: function () {
             //只有登录只有返回true
             if (Cookies.get('member'))
                 return true;
             //页面跳转
             location.href = '/shop/login?redirecturl=' + encodeURI(location.href);
             return false;
         },
         //1.指定商品和数量加入购物车
         insertCat: function (productID, count,guige, successCB) {
             var _this = this;
             if (_this.isLogin() == false)
                 return false;
             $.post('/shop/usercenter/insertcat', {
                 productID: productID, count: count,guige
             }, function (data) {
                 if (data.status == 1) {
                     if (successCB)
                         successCB(data);
                     _this.refresh();
                 } else {
                     alert(data.msg);
                 }
             });
         },
         //移除购物车
         deleteCat: function (productID, successCB) {
             var _this = this;
             if (_this.isLogin() == false)
                 return false;
             $.ajax({
                 url: '/shop/usercenter/deletecat',
                 type: 'post',
                 data: {
                     productID: productID
                 },
                 success: function (data) {
                     if (data.status == 1) {
                         if (successCB)
                             successCB(data);
                         _this.refresh();
                     } else {
                         alert(data);
                     }
                 }
             });
         },
         //添加收藏产品
         insertCollect: function (productID, successCB) {
             var _this=this;
             if (_this.isLogin() == false)
                 return false;
             $.ajax({
                 url: '/shop/usercenter/insertcollect',
                 type: 'post',
                 data: {
                     productID: productID
                 },
                 success: function (data) {
                     if (data.status == 1) {
                         if (successCB)
                             successCB(data);
                       
                     } else {
                         alert(data.msg);
                     }
                 }
             });
         },
         //删除收藏
         deleteCollect: function (id, successCB) {
             var _this = this;
             if (_this.isLogin() == false)
                 return false;
             $.ajax({
                 url: '/shop/usercenter/deletecollect',
                 type: 'post',
                 data: {
                     id: id
                 },
                 success: function (data) {
                     if (data.status == 1) {
                         if (successCB)
                             successCB(data);
                     } else {
                         alert(data);
                     }
                 }
             });
         },
         //去人收货
         firmOrder: function (orderid, successCB) {
             $.ajax({
                 url: '/shop/usercenter/FirmOrder',
                 type: 'post',
                 data: {
                     orderid: orderid
                 },
                 success: function (data) {
                     if (data.status == 1) {
                         if (successCB)
                             successCB(data);
                     } else {
                         alert(data);
                     }
                 }
             });
         },
         //委托
         weituoOrder: function (orderid, successCB) {
             $.ajax({
                 url: '/shop/usercenter/WeiTuoOrder',
                 type: 'post',
                 data: {
                     orderid: orderid
                 },
                 success: function (data) {
                     if (data.status == 1) {
                         if (successCB)
                             successCB(data);
                     } else {
                         alert(data);
                     }
                 }
             });
         },
         //提货
         tihuoOrder: function (orderid, successCB) {
             $.ajax({
                 url: '/shop/usercenter/TiHuoOrder',
                 type: 'post',
                 data: {
                     orderid: orderid
                 },
                 success: function (data) {
                     if (data.status == 1) {
                         if (successCB)
                             successCB(data);
                     } else {
                         alert(data);
                     }
                 }
             });
         },
         //2.刷新购物车内容
         refresh: function () {
             var Shopping_list = $('#Shopping_list');
             if (Shopping_list.length > 0) {
                 //刷新购物车内容
                 $.post('/shop/index/topcart', {}, function (data) {
                     Shopping_list.html(data);
                 });
             }
         },
         //绑定事件
             //绑定事件
             bindEvent: function () {
                 var _this = this;
                 //1.立即购买操作
                 $('.buyNowBtn').click(function () {
                     var buyNum = $('#buy-num');
                     var productid = $(this).attr('data-id');
                     var guige = $('#guige').val();

                     if (guige == "") {
                         show('请选择尺寸规格');
                         return;
                     }
                     var number = buyNum.val();
                     if (isNaN(number))
                         number = 1;
                     if (number > 0) {
                         _this.insertCat(productid, number,guige, function (data) {
                             //跳转购买页面
                             location.href = '/shop/usercenter/payment';
                         });
                     } else {
                         alert('获取购买数量不正确');
                     }
                 });
             //2.加入购物车操作
             $('.cartBtn').click(function () {
                 //加入购物车
                 var thisBtn = $(this);
                 var productid = thisBtn.attr('data-id');
                 var buyNum = $('#buy-num');
                 var guige = $('#guige').val();
                  if (guige == "") {
                     show('请选择尺寸规格');
                     return;
                 }
                 var number = buyNum.val();
                 if (isNaN(number))
                     number = 1;
                 if (number > 0) {
                     _this.insertCat(productid, number,guige, function (data) {
                         alert('加入成功');
                         //文字内容
                     });
                 }
             });

             //收藏商品
             $('.collectBtn').click(function () {
                 var thisBtn = $(this);
                 var productID = $(this).attr('data-id');
                 if (thisBtn.html().indexOf('已收藏') != -1) {
                     return;
                 }
                 _this.insertCollect(productID, function (data) {
                     alert(data.msg);
                     thisBtn.off('click').addClass('red');
                     thisBtn.html(thisBtn.html().replace('收藏', '已收藏'));
                     //判断数量
                     var numCollect = thisBtn.find('.numCollect');
                     if (numCollect.length == 1) {
                         var number = parseInt(numCollect.text()) + 1;
                         numCollect.text(number);
                     }
                 });
             });
             //删除购物车商品
             $(document).on({
                 click: function () {
                     var productID = $(this).attr('data-id');
                     var item = $(this).parents('.productItem');
                     _this.deleteCat(productID, function () {
                         item.remove();
                     });
                 }
             }, '.deleteCatBtn');
    

             //确认收货
             $('.firmOrderBtn').click(function () {
                 var thisBtn = $(this);
                 if (window.confirm('确认已经收到商品了吗')) {
                     var orderid = thisBtn.attr('data-id');
                     _this.firmOrder(orderid, function (data) {
                         alert(data.msg);
                         setTimeout(function () {
                             location.reload();
                         }, 800);
                     });
                 }
             });
                 //委托
             $('.weituoOrderBtn').click(function () {
                 var thisBtn = $(this);
                 if (window.confirm('确认委托吗')) {
                     var orderid = thisBtn.attr('data-id');
                     _this.weituoOrder(orderid, function (data) {
                         alert(data.msg);
                         setTimeout(function () {
                             location.reload();
                         }, 800);
                     });
                 }
             });
                 //提货
             $('.tihuoOrderBtn').click(function () {
                 var thisBtn = $(this);
                 if (window.confirm('确认提货吗')) {
                     var orderid = thisBtn.attr('data-id');
                     _this.tihuoOrder(orderid, function (data) {
                         alert(data.msg);
                         setTimeout(function () {
                             location.reload();
                         }, 800);
                     });
                 }
             });
         },
         //绑定页面购买数量++
         bindBuyCount: function () {
             var _this = this;
             //增加数量
             $('.countAddBtn').click(function () {
                 var thisBtn = $(this);
                 var thisItem = thisBtn.parents('.productItem');
                 var numberBox = thisItem.find('.car_ipt');
                 var number = parseInt(numberBox.val());
                 var max = parseInt(numberBox.attr('data-max'));
                 if (number < max) {
                     var newNumber = number + 1;
                     numberBox.val(newNumber);
                     if (thisItem.hasClass('cartPage'))
                         changeNumber(numberBox, newNumber, number);
                 }
             });
             //减少数量
             $('.countSubBtn').click(function () {
                 var thisBtn = $(this);
                 var thisItem = thisBtn.parents('.productItem');
                 var numberBox = thisItem.find('.car_ipt');
                 var number = parseInt(numberBox.val());
                 if (number > 1) {
                     var newNumber = number - 1;
                     numberBox.val(newNumber);

                     if (thisItem.hasClass('cartPage'))
                         changeNumber(numberBox, newNumber, number);
                 }
             });
             $('.car_ipt').change(function (e) {
                 var val = $(this).val();
                 var thisBox = $(this);
                 var max = parseInt($(this).attr('data-max'));
                 if (isNaN(val)) {
                     thisBox.val('1');
                 } else if (val > max) {
                     thisBox.val(max);
                 }
                 changeNumber(thisBox, val, thisBox.attr('data-old'));
             });
             //计算金额
             function changeNumber(thisBox, newValue, oldValue) {
                 var number = parseInt(thisBox.val());
                 //重新计算金额
                 var thisItem = thisBox.parents('.productItem');
                 var PriceShopping = parseInt(thisBox.attr('data-PriceShopping')) || 0;
                 var PriceScore = parseInt(thisBox.attr('data-PriceScore')) || 0;
                 var PV = parseInt(thisBox.attr('data-PV')) || 0;
                 thisItem.find('.PriceShopping').text((PriceShopping * number).toFixed(2));
                 thisItem.find('.PriceScore').text((PriceScore * number).toFixed(2));
                 thisItem.find('.PV').text((PV * number).toFixed(2));
                 //修改购物车
                 var productid = thisBox.attr('data-id');

                 _this.insertCat(productid, newValue , function (data) {
                     thisBox.attr('data-old', newValue)
                 });
             }
         }
     }
     window.catOperate;
     $(function () {
         catOperate.bindEvent();
         catOperate.bindBuyCount();
     });
 })();