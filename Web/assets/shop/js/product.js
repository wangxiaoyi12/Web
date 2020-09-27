/*****商品列表绑定******/
$(function () {
    $('.Filter p').click(function () {
        return false;
    });
    $("#scrollsidebar").fix({
        float: 'left',
        //minStatue : true,
        skin: 'green',
        durationTime: 600
    });

    //绑定列表页面
    //1.大分类
    $('.p_f_name a').click(function () {
        $(this).siblings().removeClass('active');
        $(this).addClass('active');
        goProduct();
    });
    $('.Filter_Entire a').click(function () {
        $(this).parents('.Filter_list').find('.active').removeClass('active');
        goProduct();
    });
    //排序
    $('.Sorted_style a').click(function () {
        var thisItem = $(this);
        if (thisItem.hasClass('on')) {
            var orderType = thisItem.find('i');
            if (orderType.hasClass('icon-unfold')) {
                orderType.removeClass('icon-unfold').addClass('icon-fold');
            } else {
                orderType.removeClass('icon-fold').addClass('icon-unfold');
            }
        }
        thisItem.siblings().removeClass('on');
        thisItem.addClass('on');

        goProduct();
    });
    //关键词
    $('.search_btn').click(function () {
        var key = $.trim($('.search_text').val());
        if (key.length > 0) {
            goProduct();
        } else {
            $('.search_text').focus();
        }
    });
});
//获取url
function getProductUrl(pageIndex) {
    var url = location.href;
    var result = [];
    var classid = $('.classContainer .active').attr('data-id');
    if (classid)
        result.push('classid=' + classid);
    else {
        classid = urlHelper.getQueryString('classid');
        if(classid)
        result.push('classid=' + classid);
    }

    var brandid = $('.brandContainer .active').attr('data-id');
    if (brandid)
        result.push('brandid='+brandid);
    var rangeid = $('.rangeContainer .active').attr('data-id');
    if (rangeid)
        result.push('rangeid='+rangeid);
    //关键词
    var key = $.trim($('.search_text').val());
    if (key)
        result.push('key=' + key);
    //排序
    var order = $('.Sorted_style .on').attr('data-id');
    if (order>0)
        result.push('order=' + order);
    var orderType = $('.Sorted .on .icon-unfold').length > 0 ? 0 : 1;
    if (orderType)
        result.push('orderType=' + orderType);
    if (pageIndex&&pageIndex>1)
        result.push('pageIndex=' + pageIndex);
    return result.join('&');
}
//跳转页面
function goProduct(pageIndex) {
    var param = getProductUrl(pageIndex);
    if (param.length > 0) {
        param = "?" + param;
    }
    var url = 'http://' + location.host + '/shop/product';
    if (param.length > 0) {
        url += param;
    }
    console.info(url);
    location.href = url;
}
//点击效果start
function infonav_more_down(index) {
    var inHeight = ($("div[class='p_f_name infonav_hidden']").eq(index).find('p').length) * 30;//先设置了P的高度，然后计算所有P的高度
    if (inHeight > 60) {
        $("div[class='p_f_name infonav_hidden']").eq(index).animate({ height: inHeight });
        $(".infonav_more").eq(index).replaceWith('<p class="infonav_more"><a class="more"  onclick="infonav_more_up(' + index + ');return false;" href="javascript:">收起<em class="pulldown"></em></a></p>');
    } else {
        return false;
    }
}
function infonav_more_up(index) {
    var infonav_height = 85;
    $("div[class='p_f_name infonav_hidden']").eq(index).animate({ height: infonav_height });
    $(".infonav_more").eq(index).replaceWith('<p class="infonav_more"> <a class="more" onclick="infonav_more_down(' + index + ');return false;" href="javascript:">更多<em class="pullup"></em></a></p>');
}
function onclick(event) {
    info_more_down();
    return false;
}
//点击效果end

/***********************商品分类顶部浮动固定层菜单栏**************************/
$(window).scroll(function () {
    var topToolbar = $("#ProductMenu");
    var headerH = $("#Preferential_AD").outerHeight();
    var headers = $("#header_outerHeight").outerHeight();
    var scrollTop = $(document.body).scrollTop();
    if (scrollTop >= headerH + headers) {
        topToolbar.stop(false, true).addClass("fixToTop");
    } else if (scrollTop < headerH + headers) {
        topToolbar.stop(false, true).removeClass("fixToTop");
    }
});
$(window).scroll(function () {
    var topToolbar = $(".fixed_out ");
    var headerH = $("#header_top").outerHeight();
    var headers = $("#goodsInfo").outerHeight();
    var headerd = $("#recommnad").outerHeight();
    var scrollTop = $(document.body).scrollTop();
    if (scrollTop >= headerH + headers + headerd) {
        topToolbar.stop(false, true).addClass("fixToTop");
    } else if (scrollTop < headerH + headers + headerd) {
        topToolbar.stop(false, true).removeClass("fixToTop");
    }
});