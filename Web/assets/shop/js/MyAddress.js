/// <reference path="MyAddress.js" />
$(function () {
    //收货地址管理
    //添加地址
    var form1 = $('#form1');
    $('#form1 .Add_btn').click(function () {
        //验证
        var Address = form1.find('input[name="Address"]');
        if (Address.val().length <= 0) {
            Address.focus();
            return false;
        }
        var Tel = form1.find('input[name="Tel"]');
        if (Tel.val().length <= 0) {
            Tel.focus();
            return false;
        }
        var Name = form1.find('input[name="Name"]');
        if (Name.val().length <= 0) {
            Name.focus();
            return false;
        }
        var PostCode = form1.find('input[name="PostCode"]');
        if (PostCode.val().length <= 0) {
            PostCode.focus();
            return false;
        }
        //提交
        $(this).prop('disabled',true)
        $.ajax({
            url: '/shop/usercenter/addaddress',
            type: 'post',
            data: form1.serialize(),
            success: function (data) {
                if (data.status == 1) {
                    alert(data.msg);
                    setTimeout(function () {
                        var url = urlHelper.getQueryString('redirectUrl');
                        if (url.length <= 0) {
                            url = '/shop/usercenter/address';;
                        } else {
                            url = window.decodeURIComponent(url);
                        }
                        location.href = url;
                    }, 800);
                } else {
                    alert(data.msg);
                }
            }
        });
        return false;
    });
    //设置默认
    $('.Address_List .Address_info .setDefault').click(function () {
        var id = $(this).attr('data-id');
        $.ajax({
            url: '/shop/usercenter/DefaultAddress',
            type: 'post',
            data: { id: id },
            success: function (data) {
                if (data.status == 1) {
                    alert('设置成功');
                    location.reload();
                } else {
                    alert(data.msg);
                }
            }
        });
    });
});
//删除地址
function removeAddress(id) {
    if (window.confirm('确定要删除吗')) {
        $.ajax({
            url: '/shop/usercenter/deleteaddress',
            type: 'post',
            data: { id: id },
            success: function (data) {
                if (data.status == 1) {
                    location.reload();
                } else {
                    alert(data.msg);
                }
            }
        });
    }
}
//
function changeProv() {
    var id = $("#ProvId").val();
    $('#CityId').empty();
    $.post("/Ajax/GetCity", { id: id }, function (d) {
        for (var i = 0; i < d.length; i++) {
            var item = d[i];
            $("#CityId").append('<option value="' + item.id + '">' + item.text + '</option>');
        }
        changeCity();
    })
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