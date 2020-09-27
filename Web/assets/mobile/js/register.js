$(function () {
    bindInit();
    bindGongYiShang();

    //绑定图片，上传
    $('.uploadDiv .btn').each(function () {
        var thisBtn = $(this);
        thisBtn.empty();
        thisBtn.addClass('uploader');
        (function (thisBtn) {
            var hiddenBox = thisBtn.parents('.thumbnail').find(':hidden');
            var thisImg = thisBtn.parents('.thumbnail').find('img');
            thisBtn.uploader({
                url: uploadUrl,
                text: '选择图片',
                fileExts: 'rar;doc;docx;pdf;png;jpg;gif;gip',
                handleType: '0',
                debug: false,
                max: 1024 * 1204 * 10,
                onSuccess: function (data) {
                    hiddenBox.val(data.newName);
                    thisImg.attr('src', data.relativeName);
                }
            });
        })(thisBtn);
    });

    //验证编号是否存在
    $('#Code').change(function () {
        var code = $(this).val();
        checkCode(code);
    });
});
function checkCode(code, callBack) {
    if (code.length <= 0)
        return;
    $.post('/ajax/IsMemberId', {
        param: code
    }, function (data) {
        if (data.status == 'y') {
            if (callBack)
                callBack();
        } else {
            show(data.info);
            $('#Code').val('').focus();
        }
    });
}

/********其他注册绑定********/
function bindInit() {
    //选择商盟
    $('#MemberLevelID').change(function () {
        var val = $(this).val();
        if (val == 1) {
            //消费商
            $('.area,.shenfen,#divcenter').addClass('hidden');
        }
        else if (val == 2) {
            //供应商
            $('.nextBtnOne').show();
            $('.submitOne').hide();

            $('.shenfen').removeClass('hidden');
            $('.area').removeClass('hidden');
        } else if (val == 3 || val == 4) {
            //服务商、业务商
            $('.shenfen').removeClass('hidden');
            $('.area,#divcenter').addClass('hidden');
        }
        else if (val == 5) {
            //运行中心
            $('.shenfen').removeClass('hidden');
            $('.area,#divcenter').removeClass('hidden');
        }
        $('.nextBtnOne').hide();
        $('.submitOne').show();
    });

    //点击下一步
    $('.page1 button').click(function () {

        //选择商盟
        var MemberLevel = $('#MemberLevelID');
        if (MemberLevel.val().length <= 0) {
            show('请选择商盟身份');
            return;
        }
        ////验证处理
        //var Code = $('#Code');
        //var code = Code.val();
        //if (code.length < 3) {
        //    Code.focus();
        //    show('登录账号不能空,最少3位');
        //    return;
        //} else if (code.length > 11) {
        //    Code.focus();
        //    show('登录账号最多11位');
        //    return;
        //}
        ////真实姓名
        //var NickName = $('input[name="NickName"]');
        //if (NickName.val().length <= 0) {
        //    NickName.focus();
        //    show('请输入姓名');
        //    return;
        //}


        ////密码
        //var LoginPwd = $('input[name="LoginPwd"]');
        //var loginpwd = LoginPwd.val();
        //if (loginpwd.length <= 0) {
        //    LoginPwd.focus();
        //    show('请输入登录密码');
        //    return;
        //}
        ////确认密码
        //var LoginPwdC = $('input[name="LoginPwdC"]');
        //var loginpwdc = LoginPwdC.val();
        //if (loginpwdc.length <= 0) {
        //    LoginPwdC.focus();
        //    show('请再次输入登录密码');
        //    return;
        //} else if (loginpwdc != loginpwd) {
        //    LoginPwdC.focus();
        //    show('两次输入密码不一致');
        //    return;
        //}

        ////手机号
        //var Mobile = $('input[name="Mobile"]');
        //if (Mobile.parents('.list-group-item').hasClass('hidden') == false) {
        //    if (Mobile.val().length > 0) {
        //        if (Regex.PhoneNumber(Mobile.val()) == false) {
        //            Mobile.focus();
        //            show('手机号格式不正确');
        //            return;
        //        }
        //    }
        //}

        //运营中心星级
        var OperationCenterLevel = $('#OperationCenterLevel');
        if (OperationCenterLevel.parents('.list-group-item').hasClass('hidden') == false) {
            if (OperationCenterLevel.val() <= 0) {
                show('请选择星级');
                return;
            }
        }
        //判断是否同意
        if ($('#agreeBox').prop('checked') == false) {
            show('只有同意协议之后才能注册哦');
            return;
        }

            //判断选择的是否是供应商
            if (MemberLevel.val() == 2) {
                show4();
            } else {
                //提交后台处理
                var form = $('#form1');
                var data = form.serialize();
                show('正在提交...');
                $.post('/mobile/mobilelogin/save_one', data, function (data) {
                    if (data.status == 1) {
                        show('注册成功');
                        show3();
                    } else {
                        show(data.msg);
                    }
                });
            }

        ////验证编号
        //checkCode(code, function () {
        //    $('.memberCode').text(code);
        //    //判断选择的是否是供应商
        //    if (MemberLevel.val() == 2) {
        //        show4();
        //    } else {
        //        //提交后台处理
        //        var form = $('#form1');
        //        var data = form.serialize();
        //        show('正在提交...');
        //        $.post('/mobile/mobilelogin/save_one', data, function (data) {
        //            if (data.status == 1) {
        //                show('注册成功');
        //                show3();
        //            } else {
        //                show(data.msg);
        //            }
        //        });
        //    }
        //});
    });
}

/********供应商绑定********/
function bindGongYiShang() {
    //显示4，页面，企业信息
    $('.page4 button').click(function () {
        //验证
        //企业名称
        var Company = $('input[name="Company"]');
        var company = Company.val();
        if (company.length <= 0) {
            Company.focus();
            show('请输入企业名称');
            return;
        }
        //店铺名称
        var ShopName = $('input[name="ShopName"]');
        var shopname = ShopName.val();
        if (shopname.length <= 0) {
            ShopName.focus();
            show('请输入店铺名称');
            return;
        }

        //激励模式
        var EncourageModel = $('#EncourageModel');
        var encouragemodel = EncourageModel.val();
        if (encouragemodel.length <= 0) {
            ShopName.focus();
            show('请选择激励模式');
            return;
        }

        shown(5);

        $('.page5 button').trigger('click');
    });
    ////显示5，页面，负责人
    //$('.page5 button').click(function () {
    //    //纳税人编号
    //    var TaxpayerCode = $('input[name="TaxpayerCode"]');
    //    var taxpayercode = TaxpayerCode.val();
    //    if (taxpayercode.length <= 0) {
    //        TaxpayerCode.focus();
    //        show('请输入纳税人识别号');
    //        return;
    //    }
    //    //开户行
    //    var BankName = $('input[name="BankName"]');
    //    var bankname = BankName.val();
    //    if (bankname.length <= 0) {
    //        BankName.focus();
    //        show('请输入开户行');
    //        return;
    //    }
    //    //账户名
    //    var BankAcount = $('input[name="BankAcount"]');
    //    var bankacount = BankAcount.val();
    //    if (bankacount.length <= 0) {
    //        BankAcount.focus();
    //        show('请输入开户行');
    //        return;
    //    }
    //    //银行账号
    //    var BankCode = $('input[name="BankCode"]');
    //    var bankcode = BankCode.val();
    //    if (bankcode.length <= 0) {
    //        BankCode.focus();
    //        show('请输入开户行');
    //        return;
    //    }

    //    shown(6);
    //});
    ////显示5，页面
    //$('.page5 button').click(function () {
    //    //姓名
    //    var NickName = $('.page6  input[name="NickName"]');
    //    var nickname = NickName.val();
    //    if (nickname.length <= 0) {
    //        NickName.focus();
    //        show('请输入姓名');
    //        return;
    //    }
    //    //支付密码
    //    var Pwd2 = $('.page6 input[name="Pwd2"]');
    //    var pwd2 = Pwd2.val();
    //    if (pwd2.length <= 0) {
    //        Pwd2.focus();
    //        show('请输入支付密码');
    //        return;
    //    }
    //    var Pwd2C = $('.page6  input[name="Pwd2C"]');
    //    var pwd2c = Pwd2C.val();
    //    if (pwd2c.length <= 0) {
    //        Pwd2C.focus();
    //        show('请再次输入支付密码');
    //        return;
    //    } else if (pwd2c != pwd2) {
    //        Pwd2C.focus();
    //        show('两次输入密码不一致');
    //        return;
    //    }

    //    shown(7);
    //    $('.page7 button').trigger('click');
    //});
    //显示5，页面
    $('.page5 button').click(function () {
        //营业执照
        //if ($('input[name="BusinessLicence"]').val().length <= 0) {
        //    show('请上传营业执照');
        //    return;
        //}
        ////法人身份证
        //if ($('input[name="IdentityImg"]').val().length <= 0) {
        //    show('请上法人身份证');
        //    return;
        //}
        ////店面门头照
        //if ($('input[name="Logo"]').val().length <= 0) {
        //    show('请上传店面门头照');
        //    return;
        //}

        //提交后台认证
        var MemberLevel = $('#MemberLevelID');
        if (MemberLevel.val() == 2) {
            $('.page2').remove();
        }
        if (window.confirm('确定信息完善，我要提交')) {
            var form = $('#form1');
            var data = form.serialize();
            show('正在提交...');
            $.post('/mobile/mobilelogin/save_one', data, function (data) {
                if (data.status == 1) {
                    show('注册成功');
                    shown(6);
                } else {
                    show(data.msg);
                }
            });
        }
    });
}


/********page页面的切换********/

//显示3
function show3() {
    var width = $(window).width();
    $('.page1').css({
        left: 0
    }).animate({
        left: -width
    }, 'fast', 'swing');

    $('.page3').addClass('active').css({
        left: width
    }).animate({
        left: 0
    }, 'fast', 'swing', function () {
        $('.page1').toggleClass('active');
    });
}
//显示4
function show4() {
    var width = $(window).width();
    $('.page1').css({
        left: 0
    }).animate({
        left: -width
    }, 'fast', 'swing');

    $('.page4').addClass('active').css({
        left: width
    }).animate({
        left: 0
    }, 'fast', 'swing', function () {
        $('.page1').toggleClass('active');
    });
}
//显示5
function shown(number) {
    var width = $(window).width();
    var abovePage = $('.page' + (number - 1));
    abovePage.css({
        left: 0
    }).animate({
        left: -width
    }, 'fast', 'swing');

    $('.page' + number).addClass('active').css({
        left: width
    }).animate({
        left: 0
    }, 'fast', 'swing', function () {
        abovePage.toggleClass('active');
    });
}
//返回4-1
function back1() {
    var width = $(window).width();
    $('.page4').css({
        left: 0
    }).animate({
        left: width
    }, 'fast', 'swing');

    $('.page1').addClass('active').css({
        left: -width
    }).animate({
        left: 0
    }, 'fast', 'swing', function () {
        $('.page4').toggleClass('active');
    });
}
//返回n
function backn(number) {
    var width = $(window).width();

    number = parseInt(number);

    var abovePage = $('.page' + number);
    abovePage.addClass('active').css({
        left: -width
    }).animate({
        left: 0
    }, 'fast', 'swing');
    var curPage = $('.page' + (number + 1));
    curPage.css({
        left: 0
    }).animate({
        left: width
    }, 'fast', 'swing', function () {
        curPage.toggleClass('active');
    });
}
function changeProv() {
    var id = $("#ProvId").val();

    $('#CityId').empty();
    $.post("/Ajax/GetCity", { id: id }, function (d) {

        for (var i = 0; i < d.length; i++) {
            var item = d[i];
            $("#CityId").append('<option value="' + item.id + '">' + item.text + '</option>');
        }
        changeCity();
    });
}
function changeCity() {
    var id = $("#CityId").val();
    $('#CountyId').empty();
    $.post("/Ajax/GetCounty", { id: id }, function (d) {
        for (var i = 0; i < d.length; i++) {
            var item = d[i];
            $("#CountyId").append('<option value="' + item.id + '">' + item.text + '</option>');
        }
    })
}