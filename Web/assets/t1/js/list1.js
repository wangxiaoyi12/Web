
$(function () {
    // 初始化
    var page = 1;
    var par = getPar();
    var sort = $(".curr").attr("type");
    //初始化懒加载
    $("img.lazy").lazyload({
        effect: "fadeIn", //渐现，show(直接显示),fadeIn(淡入),slideDown(下拉)
        threshold: 200, //预加载，在图片距离屏幕180px时提前载入
        // placeholder : "images/grey.gif"
    });
    //console.log(par);
    //getGoodslist(page, sort);

    //// 获得数据方法
    //function getGoodslist(page, sort) {

    //    var data = {
    //        page: page,
    //        KeyWord: par.KeyWord,
    //        parab: par.parab,
    //        btype: par.btype,
    //        sort: sort,
    //        Coupon:par.Coupon
    //    };
    //    //console.log(data);

    //    var html = "";
    //    var URL = "ajax/List.ashx?type=GetApplyPro&t=" + new Date().getTime();

    //    $.post(URL, data, function (data, status) {
    //        if (status == "success") {
    //            console.log(data);
    //            if (data.rows.length == 0) {
    //                $(".loader_more>p").text("这已经是我能给你的全部了~");
    //                $(".loader_more").show();
    //                return;
    //            }

    //            if (data.total > 10) {
    //                $(".loader_more>p").text("下拉加载更多~");
    //                $(".loader_more").show();
    //            }

    //            // console.log(data);
    //            html = template("goodslist", data);

    //        } else {
    //            html = "<p class='empty-ol'>网络不佳！请求发送失败</p>"
    //        }
    //        $(".goods-list>ol").append(html);

    //        //初始化懒加载
    //        $("img.lazy").lazyload({
    //            effect: "fadeIn", //渐现，show(直接显示),fadeIn(淡入),slideDown(下拉)
    //            threshold: 200, //预加载，在图片距离屏幕180px时提前载入
    //            // placeholder : "images/grey.gif"
    //        });

    //    }, "json")
    //}

    //初始化数据
    function pageInit() {
        page = 1;
        $(".goods-list>ol").empty();
        $(".loader_more").hide();
    }

    // 获得url中的参数
    function getPar() {

        var str = window.location.search.slice(1);

        if (!str) {
            return;
        }

        var arr = str.split("&");
        var obj = {};

        $.each(arr, function (i, e) {
            var item = e.split("=");
            obj[item[0]] = item[1];
        })

        //console.log(str, obj, arr);
        return obj;
    }

    // 滚动加载
    function scrollData() {
        var scrollTop = $(window).scrollTop();//页面屏幕外高度
        var scrollHeight = $(document).height();//页面高度
        var windowHeight = $(window).height();//窗口高度

        if (scrollTop + windowHeight == scrollHeight) {
            page++;
            //console.log(page);
            getGoodslist(page, sort);
        }
    }

    //滚动页面
    $(window).scroll(function () {
        scrollData();
    });

    // 搜索框清除按钮
    $(".clear-search").click(function () {
        $(".search>input").val("");
        $(".clear-search").hide();
    })
    $(".search>input").blur(function () {
        $(".clear-search").show();
    })

    // 点击搜索
    $('#search').click(function () {
        location.href = '/mobile/mindex/search'
    })

    // 点击返回
    $('.left-menu').click(function () {
        history.back();
    })

    // 点击切换样式
    $(".right-menu").click(function () {
        $(".goods-list>ol").toggleClass("line-layout").toggleClass("row-layout");
        $(".menu-icon").toggleClass("menu-icon-more");
    })

    // 点击综合
    $(".item-a").click(function () {
        $(".sub-nav").slideToggle();
        $(this).addClass("curr").siblings().removeClass("curr");
        $(".item-c").removeClass("sub-curr");
    });

    // 点击已售、莲香币、现金
    $(".item-b").click(function () {
        $(".sub-nav").slideUp();
        $(this).addClass("curr").siblings().removeClass("curr");
        $(".item-c").removeClass("sub-curr");
        sort = $(this).attr("type");
        pageInit();
        getGoodslist(page, sort);
    });

    // 点击goods-evaluation口碑、goods-news新品
    $(".item-c").click(function () {
        $(this).addClass("sub-curr").siblings().removeClass("sub-curr");
        $(".item-a").removeClass("curr");
        sort = $(this).attr("type");
        pageInit();
        getGoodslist(page, sort);
    });


    //====================筛选框===================================================
    // 弹出界面
    $(".filter-menu").click(function () {
        initScreening();// 初始化
        $(".screening-box").animate({ width: 'toggle' });
        $(".screening-close").fadeIn();

    });
    //关闭界面
    $(".screening-close").click(function () {
        $(".screening-box").animate({ width: 'toggle' });
        $(".screening-close").fadeOut();

    })

    // 验证最大值和最小值
    $("#max-price").blur(function () {
        // 数字验证
        testReg($("#max-price"));
        // 验证最大值和最小值
        testBig(this);
    })

    $("#min-price").blur(function () {
        // 数字验证
        testReg($("#min-price"));
        // 验证最大值和最小值
        testBig(this);
    })

    // 二级下拉窗
    $(".select-brand h5").click(function () {
        $(".select-brand ul").slideToggle();
    })

    $(".select-efficacy h5").click(function () {
        $(".select-efficacy ul").slideToggle();
    })

    $(".select-capacity h5").click(function () {
        $(".select-capacity ul").slideToggle();
    })

    $(".select-brand").on("click", "li", function (e) {
        $(e.target).addClass("selected").siblings().removeClass("selected");
    })

    $(".select-efficacy").on("click", "li", function (e) {
        $(e.target).addClass("selected").siblings().removeClass("selected");
    })

    $(".select-capacity").on("click", "li", function (e) {
        $(e.target).addClass("selected").siblings().removeClass("selected");
    })

    // 重置
    $("._button .reset-btn").click(function () {
        initScreening();
    })
    // 发送信息
    $(".determine-btn").click(function () {
        var max = $("#max-price").val(),
            min = $("#min-price").val(),
            brand = $(".select-brand").find(".selected").attr("value");
        efficacy = $(".select-efficacy").find(".selected").attr("value");
        capacity = $(".select-capacity").find(".selected").attr("value");
        var _url = window.location.href + "?max=" + max + "&min=" + min + "&brand=" + brand + "&efficacy=" + efficacy + "&capacity=" + capacity;
        var k = window.location.href + "?max=&min=&brand=undefined&efficacy=undefined&capacity=undefined";
        if (_url == k) {
            alert("请输入查询信息");
            return false;
        }
        //console.log(_url);
        // $.get();
    })

    // 排序请求
    function sortAjax(i) {
        var _url = window.location.href;
        _url += "?sort=" + i;
        $.get(_url, function (data) {
            console.log("来了");
            console.log(data);
        })
    }

    // 初始化筛选框
    function initScreening() {
        $(".screening-item").removeAttr("selected");
        $(".screening-item").removeClass("selected");
        $("#min-price").val("");
        $("#max-price").val("");
    }
    // 验证输入的数字
    function testReg(e) {
        var str = $.trim(e.val());
        if (str.length != 0) {
            reg = /^[1-9]\d{0,7}$/;
            if (!reg.test(str)) {
                alert("请输入整数");//请将“整数类型”要换成你要验证的那个属性名称！ 
                e.val("");
            }
        }
    }
    function testBig(e) {
        var min = $.trim($("#min-price").val());
        var max = $.trim($("#max-price").val());
        if (max.length != 0 && min.length != 0 && max < min) {
            alert("请正确书写最大值最小值");
            $(e).val("");
        }
    }
});