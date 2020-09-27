
//登录操作
function userLogin() {
    var flag = true;
    var postData = {};
    $("#theForm").find("input").each(function () {
        var $obj = $(this);
        var message = $obj.attr("message");
        if (isBlank($obj.val()) && $obj.attr("required") == "required") {
            floatNotify.simple(message + '不能为空');
            flag = false;
            return false;
        }
    })

    if (flag) {
        // $("#theForm").submit();
        //后台提交验证
        show('正在提交...');
        $.post('/shop/login/CheckUser', {
            username: $('#username').val(),
            pwd: $('#pwd').val()
        }, function (data) {
            if (data.status) {
                show('登录成功');
                //判断是否有openid
                //if (userHelper.isOpenID() == false) {
                //    //跳转到微信授权登录页面
                //    //var url = urlHelper.getAuth() + '?recirecturl=' + $('#returnUrl').val();
                //    var url = '/mobile';
                //    //if (url.length <= 0) {
                //    //    url = '/mobile/mobilecenter';
                //    //}
                //    location.href = url;
                //} else {
                setTimeout(function () {
                    var url = $('#returnUrl').val();
                    if (url) {
                        location.href = url;
                    } else {
                        //location.href = '/mobile/mindex/index';
                    }
                }, 500);
                //}


            } else {
                alert(data.msg);
            }
        });
    }
}

//方法，判断是否为空
function isBlank(_value) {
    if (_value == null || _value == "" || _value == undefined) {
        return true;
    }
    return false;
}