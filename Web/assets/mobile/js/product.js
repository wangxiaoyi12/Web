
$(function () {
    $('.back-icon').dblclick(function () {
        location.href = '/moble/mindex';
    });

    //点赞处理
    $('#containerList').on({
        click: function () {
            return false;
        }
    }, '.praise');;
    $('#containerList').on({
        click: function () {
            var thisBtn = $(this);
            if (thisBtn.hasClass('btn-danger')) {
                show('已经赞过了！');
                return false;
            }
            var id = thisBtn.attr('data-id');
            $.post('/mobile/selfproduct/praise', {
                productid: id
            }, function (data) {
                if (data == 1) {
                    show('点赞成功');
                    thisBtn.addClass('btn-danger');
                    var num = thisBtn.find('i').text();
                    num = parseInt(num);
                    num++;
                    thisBtn.find('i').text(num);
                } else {
                    if (data.indexOf('登录') != -1) {
                        location.href = '/mobile/login';
                        return;
                    }
                    show(data)
                }
            });
        }
    }, '.praise .btn');


    //搜索按钮
    function searchproduct() {
        var keyword = document.getElementById("keyword").value;
        goLink();
        return false;
    }
    $('.pro_sou').click(function () {
        searchproduct();
    });

    //分类按钮
    $('#classList').click(function () {
        $('.sou_count').slideToggle('normal');
    });
    $('.sou_count a').click(function () {
        $(this).siblings().removeClass('active');
        $(this).addClass('active');
        $('.sou_count').attr('data-id', $(this).attr('data-id'))
        goLink();
    });
    if ($('.sou_cou_h1').length == 2) {
        $('.sou_cou_h1').first().removeClass('sou_cou_h1');
    }
    //排序按钮
    $('.mod-filter li:not(:last)').click(function () {
        var thisItem = $(this);
        thisItem.siblings().removeClass('active');
        thisItem.addClass('active');
        var item = $(this).find('i');
        if (item.attr('data-type') == 0) {
            item.attr('data-type', 1).addClass('icon_sort_down');
        } else {
            item.attr('data-type', 0).addClass('icon_sort_up');
        }
        goLink();
    });

    //执行页面跳转
    function goLink() {
        var param = getParam();
        var url = '/Mobile/SelfProduct/?' + param.join('&');
        //console.info(url);
        location.href = url;
    }
    //获取参数信息
    function getParam() {
        var param = [];
        //关键词
        var keywords = $('#keyword').val();
        if (keywords.length > 0)
            param.push('keywords=' + keywords);
        //品牌
        var brandid = $('#keyword').attr('data-brand');
        if (brandid > 0)
            param.push('brandid=' + brandid);
        //分类
        var classid = $('.sou_count').attr('data-id');
        if (classid > 0) {
            param.push('classid=' + classid);
        }
        //商家ID
        var ShopID = $('#hidShopID').val();
        if (ShopID > 0) {
            param.push('ShopID=' + ShopID);
        }
        //点赞
        var praise = $('#praise').val();
        if (praise > 0) {
            param.push('praise=' + praise);
        }
        //排序
        var orderItem = $('.mod-filter .active');
        var order = orderItem.attr('data-id');
        if (order)
            param.push('order=' + order);
        var orderType = orderItem.find('i').attr('data-type') || 0;
        param.push('orderType=' + orderType);
        return param;
    }

    //监听 滚动条
    $(window).scroll(function () {
        var scrollHeight = $(document).scrollTop();
        var winHeight = $(window).height();
        var docHeight = $(document).height();
        if ((scrollHeight + winHeight) >= docHeight) {
            $('#ajax_loading').show().trigger('click');
        }
    });

    function appendList(skip) {
        var param = getParam();
        var url = '/Mobile/SelfProduct/List?' + param.join('&');
        $.post(url, {
            skipCount: skip || 0
        }, function (data) {
            $('#containerList').append(data);
            $('#ajax_loading').remove();
        });
    }
    appendList(0);
    $(document).on({
        click: function () {
            var thisItem = $(this);
            //ajax获取内容
            appendList(thisItem.attr('data-skip'));
        }
    }, '#ajax_loading');
});