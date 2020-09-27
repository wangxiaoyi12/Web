
$(document).ready(function () {
    //计算总价
    calculateTotal();

    //返回顶部
    $(window).scroll(function () {
        if ($(this).scrollTop() > 300) {
            $(".fanhui_cou").fadeIn(1500);

        } else {
            $(".fanhui_cou").fadeOut(1500);
        }
    });
    $(".fanhui_cou").click(function () {
        $("body,html").animate({ scrollTop: 0 }, 200);
        return false;
    });


    //勾选
    $(".checkLabel").click(function () {
        alert(1);
        var checkBox = $(this).prev();
        var flag = checkBox.prop('checked');
        if (flag) {
            checkBox.prop("checked", false);
            $("#buy-sele-all").prop("checked", false); //将全选勾除
            $("#buy-sele-all").val(0);
        } else {
            checkBox.prop("checked", true);
            //如果全部都选中了，将全选勾选
            var groupUL = $(".container").find(".list-group").get();
            var checkUL = $(".container").find(".ids").get();
            if (groupUL.length == checkUL.length) {
                $("#buy-sele-all").prop("checked", true);
                $("#buy-sele-all").val(1);
            }
        }

        //计算总价
        calculateTotal();
    });

    // 全选，全不选
    $("#buy-sele-all").click(function () {
        var flag = $(this).val();
        if (flag == 1) {
            $(this).val(0);
            $(".ids").prop("checked", false);
        } else {
            $(this).val(1);
            $(".ids").prop("checked", true);
        }
        //计算总价
        calculateTotal();
    });


});

//相加
function increase(obj) {
    var _this = $(obj);
    var _count_obj = _this.prev();
    var count = Number($(_count_obj).val());
    var basket_id = $(_count_obj).attr("itemkey");
    var prod_id = $(_count_obj).attr("prodId");
    var sku_id = $(_count_obj).attr("skuId");

    var _num = parseInt(count) + 1;
    var re = /^[1-9]+[0-9]*]*$/;
    if (isNaN(_num) || !re.test(_num)) {
        return;
    } else if (_num == 9999) {
        return;
    }

    var result = changeShopCartNumber(basket_id, _num, prod_id, sku_id, 1);
    if (result) {
        $(_count_obj).val(count * 1 + 1);
        var cash = $(_this).parent().parent().prev().find("em[class='price']").html().substring(1);//单价
        var total_cash = $(_this).parent().prev().find("em[class='red productTotalPrice']").html().substring(1);//商品小计
        var e_cash = Math.round((Number(total_cash) + Number(cash)) * 100) / 100;
        var pos_decimal = e_cash.toString().indexOf('.');
        if (pos_decimal < 0) {
            e_cash += '.00';
        }
        $(_this).parent().prev().find("em").html("¥" + e_cash);
        //计算总价
        calculateTotal();
    }

}

//减
function disDe(obj) {
    var _this = $(obj);
    var _count_obj = _this.next();
    var count = Number($(_count_obj).val());
    var basket_id = $(_count_obj).attr("itemkey");
    var prod_id = $(_count_obj).attr("prodId");
    var sku_id = $(_count_obj).attr("skuId");
    var _num = parseInt(count) - 1;

    var re = /^[1-9]+[0-9]*]*$/;
    if (isNaN(_num) || !re.test(_num)) {
        return;
    } else if (_num == 0) {
        return;
    }
    var result = changeShopCartNumber(basket_id, _num, prod_id, sku_id, 0);
    if (result) {
        $(_count_obj).val(count * 1 - 1);
        var cash = $(_this).parent().parent().prev().find("em[class='price']").html().substring(1);//单价
        var total_cash = $(_this).parent().prev().find("em[class='red productTotalPrice']").html().substring(1);//商品小计		
        var e_cash = Math.round((Number(total_cash) - Number(cash)) * 100) / 100;
        var pos_decimal = e_cash.toString().indexOf('.');
        if (pos_decimal < 0) {
            e_cash += '.00';
        }
        $(_this).parent().prev().find("em").html("¥" + e_cash);
        //计算总价
        calculateTotal();
    }
}
//更新购物车商品数量
function changeShopCartNumber(_basketId, _num, _prodId, _skuId, type) {
    var config = false;
    $.ajax({
        url: contextPath + "/changeShopCartNum",
        data: { "num": _num, "basketId": _basketId, "prodId": _prodId, "skuId": _skuId, "type": type },
        type: 'post',
        async: false, //默认为true 异步   
        dataType: 'json',
        error: function (data) {
        },
        success: function (result) {
            if (result.status == "OK") {
                config = true;
            } else if (result.fail == "RESTRICTION") {
                floatNotify.simple("超出购买限制");
            } else if (result.fail == "ERR") {
                floatNotify.simple("更新失败");
            } else if (result.fail == "PROD_RESTRICTION") {
                floatNotify.simple("商品超出购买限制");
            } else if (result.fail == "PARAM_ERR") {
                floatNotify.simple("参数有误");
            }
        }
    });
    return config;
}

//计算总价
function calculateTotal() {
    var allCash = 0;
    var list = $(".container").find(".list-group");
    list.each(function () {
        var thisItem = $(this);
        if (thisItem.find('.ids').prop('checked')) {
            var total = thisItem.find('.total').text();
            total = parseFloat(total);
            allCash += total;
        }
    });
    $("#totalPrice").html(allCash);
}



function submitShopCart() {
    var array = $(".ids:checked").get();
    if (array.length == 0) {
        floatNotify.simple("请选择要结算的商品");
        return;
    }
    var shopList = new Object();
    $(".ids:checked").each(function () {
        var thisItem = $(this).parents('.productItem').find('.car_ipt');
        shopList[thisItem.attr('data-id')] = thisItem.val();
    });

    Cookies.set('cart', JSON.stringify(shopList));

    var allQuan = 0;
    var allScores = 0;
    var list = $(".container").find(".list-group");
    list.each(function () {
        var thisItem = $(this);
        if (thisItem.find('.ids').prop('checked')) {
            var total = thisItem.find('.total').text();
            total = parseFloat(total);
            if (thisItem.find('.p-info .text-primary').length > 0) {
                allScores += total;
            } else {
                allQuan += total;
            }
        }
    });

    //消费券
    Cookies.set('total', allQuan);
    //积分
    Cookies.set('total2', allScores);

    //调用方法  
    location.href = '/mobile/mobilecenter/payment?q=t';
}

