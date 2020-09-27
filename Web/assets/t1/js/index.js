$(function () {
    //初始化轮播图
    var swiper = new Swiper('.swiper-container', {
        pagination: '.swiper-pagination',
        nextButton: '.swiper-button-next',
        prevButton: '.swiper-button-prev',
        paginationClickable: true,
        spaceBetween: 0,
        centeredSlides: true,
        autoplay: 3500,
        autoplayDisableOnInteraction: false,
        loop: true
    });
    // 获取限时抢购商品
    GetTimePro();
    // 获取消息条数
    GetMessCount();

    //加载默认美容院
    //$.get('/ajax/index.ashx', {type: 'CurrShop'}, function(){}, 'json')
    //.done(function (response) {
    //    if (response.Status == "1") {
    //        //没有登录状态下
    //        $(".unloggedin").show();
    //    }
    //    else {
    //        var strs = response.Msg.split(","); //字符分割
    //        var html = " <p class=\"lxd-integral-num\"><a href=\"/MyCenter/mylxb.aspx\"><span class=\"iconfont icon-jinbi\"></span>";
    //        html += "   积分：<span class=\"integral-num\">" + strs[0] + "</span></a></p>";
    //        html += "<p class=\"lxd-integral-vendor\"><a href=\"/MyCenter/Companyaddress.aspx\"><span class=\"iconfont icon-shangdian\"></span>" + strs[1] + "</a></p>";
    //        $(".lxd-integral-t").html(html);
    //    }
    //})
    //.fail(function(err){
    //    console.log(err);
    //})

    //计算行宽度
    $('.floor').each(function () {
        var rowLength = $(this).find('.goods-list .item').outerWidth(true) * $(this).find('.goods-list .item').length;
        $(this).find('.goods-list ul').width(rowLength);
    })

    // 滚动消息
    var lineHeight = parseInt($('.msg-list .item').css('lineHeight'));
    var reallHeight = $('.msg-list .item').height();
    if (reallHeight <= reallHeight + 1 && reallHeight >= lineHeight - 1) {
        $('.msg-list .item').css({ marginTop: -lineHeight + 'px' })
    }
    
    $('.msg-list .item').on('webkitTransitionEnd', function () {
        // 清空动画
        $('.msg-list .item').css('transition', '')
        if (parseInt($('.msg-list .item').css('marginTop')) < 0) {
            $('.msg-list .item').css({ marginTop: '0px' })
        } else {
            $('.msg-list .item').css({ marginTop: -lineHeight + 'px' })

        }
    })

    // 统计按钮点击
    $('.menu .item').on('click', function () {
        var image = document.createElement('img');
        image.src = '/ajax/BrowseData.ashx?type=Query&BrowseUrl=' + $(this).find('a').attr('href');
    });

});

var countDown = function () {
    $(".clock").each(function () {
        var h = $(".h", $(this)).text();
        var m = $(".m", $(this)).text();
        var s = $(".s", $(this)).text();
        if (h + m + s == 0) {
            return true;
        }
        if (s == 0) {
            s = 59;
            if (m == 0) {
                m = 59;
                h--;
            }
            else {
                m--;
            }
        }
        else {
            s--;
        }
        $(".h", $(this)).text(h);
        $(".m", $(this)).text(m);
        $(".s", $(this)).text(s);
    });
}
setInterval(countDown, 1000);

//添加cookie
function addCookie(objName, objValue, objHours) {//添加cookie 
    var str = objName + "=" + escape(objValue);
    if (objHours > 0) {//为0时不设定过期时间，浏览器关闭时cookie自动消失 
        var date = new Date();
        var ms = objHours * 3600 * 1000;
        date.setTime(date.getTime() + ms);
        str += "; expires=" + date.toGMTString();
    }
    document.cookie = str;
}
//读取Cookie
function getCookie(name) {
    if (document.cookie.length > 0) {
        cookieStart = document.cookie.indexOf(name + "=")
        if (cookieStart != -1) {
            cookieStart = cookieStart + name.length + 1
            cookieEnd = document.cookie.indexOf(";", cookieStart)
            if (cookieEnd == -1) cookieEnd = document.cookie.length
            //return unescape(document.cookie.substring(cookieStart, cookieEnd))
            return decodeURIComponent(document.cookie.substring(cookieStart, cookieEnd))
        }
    }
    return ""
}

//限时抢购
 function GetTimePro() {
     //$.get('/ajax/index.ashx?type=time&r=' + Math.random(), {}, function (data) {
     //    $(".time-sale").html(data);
     //});
 }
//站内消息
function GetMessCount() {
    //$.get('/ajax/index.ashx?type=MessCount&r=' + Math.random(), {}, function (data) {
    //    if (data != "0")
    //    {
    //        $(".lxd-remind a").append('<span class=\"red-icon\"></span>');
    //    }
    //});
}