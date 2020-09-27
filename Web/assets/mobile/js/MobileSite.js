/**
* 手机版js封装
*/

(function () {

    /**购物车操作**/
    var catOperate = {
        //判断用户是否登录
        isLogin: function () {
            //只有登录只有返回true
            if (Cookies.get('member'))
                return true;
            //页面跳转

            location.href = '/mobile/login?redirecturl=' + encodeURI(location.href);
            return false;
        },

        isLogincart: function (code) {
            //只有登录只有返回true
            if (Cookies.get('member'))
                return true;
            //页面跳转
            swal({
                title: "",
                text: "您尚未登录,请选择操作",
          
                showCancelButton: true,
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "注册",
                cancelButtonText: "登录",
                closeOnConfirm: true,
                closeOnCancel: true,
                customClass: "custom_swal"
            }, function (isConfirm) {
                if (isConfirm) {
                    location.href = '/mobile/login/register?code='+code+'&redirecturl=' + encodeURI(location.href);
                } else {
                    location.href = '/mobile/login?redirecturl=' + encodeURI(location.href);
                }
            });
            //location.href = '/mobile/login?redirecturl=' + encodeURI(location.href);
            return false;
        },
        clearCookie: function () {
            Cookies.remove('_token_')
        },
        //1.指定商品和数量加入购物车
        insertCat: function (productID, count, guige, code, successCB) {
            var _this = this;
            if (_this.isLogincart(code) == false)
                return false;
            $.post('/shop/usercenter/insertcat', {
                productID: productID, count: count, guige: guige
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
                        alert('删除成功');
                        if (successCB)
                            successCB(data);
                        window.location.reload();
                    } else {
                        alert(data.msg);
                    }
                }
            });
        },
        //修改购物车商品数量
        updateCat: function (productID, count, successCB) {
            var _this = this;
            if (_this.isLogin() == false)
                return false;
            $.post('/shop/usercenter/insertcat', {
                productID: productID, count: count
            }, function (data) {
                if (data.status == 1) {
                    if (successCB)
                        successCB(data);
                } else {
                    alert(data.msg);
                }
            });
        },
        //添加收藏产品
        insertCollect: function (productID, successCB) {
            var _this = this;
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
        bindEvent: function () {
            var _this = this;
            //1.立即购买操作
            $('.buyNowBtn').click(function () {
                var buyNum = $('#buy-num');
                var productid = $(this).attr('data-id');
                var guige = $('#guige').val();
                var code = $('#code').val();
                if (guige == "") {
                    show('请选择尺寸规格');
                    return;
                }
                var number = buyNum.val();
                if (isNaN(number))
                    number = 1;
                if (number > 0) {
                    _this.insertCat(productid, number, guige, code, function (data) {

                        setTimeout(function () {
                            ////跳转购买页面
                            //location.href = '/mobile/mobilecenter/payment/' + productid;
                            //跳转购物车页面
                            location.href = '/mobile/mobilecenter/cart/';
                        }, 800);
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
                var code = $('#code').val();
                if (guige == "") {
                    show('请选择尺寸规格');
                    return;
                }
                var number = buyNum.val();
                if (isNaN(number)) {
                    number = 1;
                }
                
                if (number > 0) {
                    _this.insertCat(productid, number, guige, code, function (data) {
                        alert('加入成功');
                        //文字内容
                        var totalNum = $('#totalNum');
                         var number = parseInt(totalNum.text()) +1;
                        totalNum.text(number);
                        totalNum.addClass('numberAdd');
                        setTimeout(function () {
                            totalNum.removeClass('numberAdd');
                        }, 1000);
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
                    thisBtn.off('click').find('i').addClass('i-fav-active');
                    thisBtn.html(thisBtn.html().replace('收藏', '已收藏'));
                    //判断数量
                    var numCollect = thisBtn.find('.numCollect');
                    if (numCollect.length == 1) {
                        var number = parseInt(numCollect.text()) + 1;
                        numCollect.text(number);
                    }
                });
            });
            //删除收藏
            $(document).on({
                click: function () {
                    var thisBtn = $(this);
                    _this.deleteCollect(thisBtn.attr('data-id'), function (data) {
                        show('删除成功');
                        thisBtn.parents('.productItem').slideUp('fast', function () {
                            thisBtn.parents('.productItem').remove();
                        })
                    });
                }
            }, '.deleteCollectBtn');

            //删除购物车商品
            $(document).on({
                click: function () {
                    var productID = $(this).attr('data-id');
                    var item = $(this).parents('.productItem');
                    _this.deleteCat(productID, function () {
                        item.remove();
                        //重算价格
                        if (calculateTotal)
                            calculateTotal();
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
                    console.info(newNumber);
                    numberBox.val(newNumber);
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

                    changeNumber(numberBox, newNumber, number);
                }
            });

            //计算金额
            function changeNumber(thisBox, newValue, oldValue) {
                var number = parseInt(thisBox.val());
                //重新计算金额
                var thisItem = thisBox.parents('.productItem');
                var PriceShopping = parseInt(thisBox.attr('data-PriceShopping')) || 0;
                var PV = parseInt(thisBox.attr('data-PV')) || 0;
                var PriceScore = parseInt(thisBox.attr('data-PriceScore')) || 0;
                thisItem.find('.total').text((PriceShopping * number).toFixed(2));
                thisItem.find('.totalPV').text((PV * number).toFixed(2));
                thisItem.find('.totalPriceScore').text((PriceScore * number).toFixed(2));
                //重新计算
                if (calculateTotal)
                    calculateTotal();
                //修改购物车
                var productid = thisBox.attr('data-id');
                _this.updateCat(productid, newValue, function (data) {
                });
            }
        }
    }
    window.catOperate;
    function alert(msg) {
        show(msg);
    }
    $(function () {
        catOperate.bindEvent();
        catOperate.bindBuyCount();
    });
})();

