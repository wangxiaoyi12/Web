

$(function () {
    //是否启用权限
    if ($("#Select_RoleType").find("option:selected").attr("value") == 1) {
        $("#datatable").find("input[type='checkbox']").prop("disabled", true);
    }

    $.ajax({
        url: "/BasicSettings/RoleManage/getRoleNav",
        data: { id: GetQueryString("id") },
        type: "post",
        success: function (data) {
            var data = eval(data);
            for (var i = 0; i < data.length; i++) {
                $(":checkbox[id=\"check_" + data[i].nav_name + "_" + data[i].action_type + "\"]").prop("checked", true);
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
});

$("#Select_RoleType").change(function () {
    if ($(this).find("option:selected").attr("value") == 1) {
        $("#datatable").find("input[type='checkbox']").prop("checked", true);
        $("#datatable").find("input[type='checkbox']").prop("disabled", true);
    } else {
        $("#datatable").find("input[type='checkbox']").prop("disabled", false);
    }
});
//权限全选
$("input[name='checkAll']").click(function () {
    if ($(this).prop("checked") == true) {
        $(this).parent().siblings("td").find("input[type='checkbox']").prop("checked", true);
    } else {
        $(this).parent().siblings("td").find("input[type='checkbox']").prop("checked", false);
    }
});


function GetQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}

function Save() {
    var Role_Nav = "";
    var id = $("#id").val();
    var name = $("#role_name").val();
    var roletype = $("#Select_RoleType").val();
    $("input[type=checkbox]:checked").each(function () {
        if (this.name!="checkAll") {
            Role_Nav += this.name + "@" + this.value + ",";
        }
    });

    $.ajax({
        url: "/BasicSettings/RoleManage/Save",
        data: { id: id, name: name, roletype: roletype, rolenav: Role_Nav },
        type: "post",
        success: function (data) {
            $.alert({
                title: '操作提示',
                content: data.Msg,
                confirmButton: '确定',
                confirmButtonClass: 'btn green',
                icon: 'glyphicon glyphicon-ok',
                animation: 'zoom',
                confirm: function () {
                }
            });
        },
        error: function (error) {
            console.log(error);
        }
    });
    
}