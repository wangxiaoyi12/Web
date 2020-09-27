$(document).ready(function () {
    //权限验证控制按钮
    PermissionsLoad(url, roleid);
    if (ismember == 'True') {
        getMyMsg(id);
    }
    $('.sidebar-option').change()
    //表格全选/反选
    $("#checkAll").on("click", function () {
        $("input[name='checkList']").prop("checked", this.checked);
    });
    $(".form-horizontal").Validform({
        tiptype: 2
    });
    document.onkeydown = function (event) {
        var e = event ? event : (window.event ? window.event : null);
        if (e.keyCode == 13) {
            //回车提交
            if (searchList && typeof (searchList) == "function") {
                searchList();
                return false;
            }
        }
    }
});

function refreshcache() {
    $.post("/Ajax/RefreshCache", {}, function () {
        alert("刷新缓存完成");
    })
}
function FinToSettlement() {
    $.post("/Ajax/FinToSettlement", {}, function () {
        alert("收益结算完成");
    })
}

//前台权限设置
function PermissionsLoad(url, roleid) {
    $("#add").css("display", "none");
    $("#delete").css("display", "none");
    $("#Edit").css("visibility", "hidden");
    $("#backup").css("display", "none");
    $("#clear").css("display", "none");
    $("#Restore").css("visibility", "hidden");
    $.post("/Ajax/PermissionsLoad", { url: url, roleid: roleid }, function (d) {
        $("#add").css("display", d.IsAdd);
        $("#delete").css("display", d.IsDelete);
        $("#Edit").css("visibility", d.IsEdit);
        $("#backup").css("display", d.IsBackup);
        $("#clear").css("display", d.IsClear);
        $("#Restore").css("visibility", d.IsRestore);
    })
}
//双击禁用
var commentForm = GetQueryString("commentForm");
if (commentForm == "commentForm") {
    disabled(commentForm);
}
//双击禁用
function disabled(formId) {
    $("form[id='" + formId + "'] :text").attr("disabled", "enable");
    $("form[id='" + formId + "'] textarea").attr("disabled", "enable");
    $("form[id='" + formId + "'] :submit").attr("disabled", "enable");
    $("form[id='" + formId + "'] select").attr("disabled", "enable");
    $("form[id='" + formId + "'] :radio").attr("disabled", "enable");
    $("form[id='" + formId + "'] :checkbox").attr("disabled", "enable");
    $("form[id='" + formId + "'] :password").attr("disabled", "enable");
    $("#SubSave").attr("disabled", "enable");
    $("#ChildSave").attr("disabled", "enable");
}
//获取字符串
function GetQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}

//增删改提交ajax
var SubAjax = {
    Loading: function () {
        $("#SubSave").attr("disabled", true).find("span").html("正在保存中...")
    },
    Success: function (result) {
        SubAjax.Complete();
        if (result.Status == "y") {
            alert("操作提示", result.Msg, function () {
                //如果返回结果中有 跳转url,操作成功则跳转
                if (result.ReUrl != '' && result.ReUrl != null) {
                    location.href = result.ReUrl;
                }
            });
            if (oTable != null) {
                oTable.ajax.reload()
            }
        } else {
            alertError("错误提示", result.Msg);
        }
    },
    SuccessBack: function (result) {
        if (result.Status == "y") {
            alert("提示", result.Msg, function () {
                history.go(-1);
            });
        } else {
            alertError("错误提示", result.Msg);
            SubAjax.Complete();
        }
    },
    Failure: function (e) {
        alertError("错误提示", "网络超时,请稍后再试...");
        SubAjax.Complete();
    },
    Complete: function () {
        $("#SubSave").attr("disabled", false).find("span").html("保 存")
    }
};

/**
* 删除（可复制）
* param id
* private
*/
function _deleteFun(id) {
    var url = window.location.href.split("?")[0].toLowerCase();
    if (url.lastIndexOf("/index") > 0) {
        url = url.substring(0, url.indexOf("/index"));
    }
    url = url + "/Delete";

    confirm("删除确认!", "删除记录后不可恢复，您确定吗？", function () {
        $.ajax({
            url: url,
            data: { idList: id },
            type: "post",
            success: function (backdata) {
                if (backdata.Status == "y") {
                    alert("操作提示", backdata.Msg);
                } else {
                    alertError("错误提示", backdata.Msg);
                }
                $("#checkAll").prop("checked", false);
                oTable.ajax.reload(null, false);
            }, error: function (error) {
                alert("提示", "删除失败，系统错误");
            }
        });
    })
}

/**
* 批量删除（可复制）
* param id list
* private
*/
function _deleteList() {
    var str = "";
    $("input[name='checkList']:checked").each(function (i, o) {
        str += $(this).val();
        str += ",";
    });
    if (str.length > 0) {
        var IDS = str.substr(0, str.length - 1);
        _deleteFun(IDS);
    } else {
        alert("操作提示", "至少选择一条记录操作");
    }
}

function getMyMsg(id) {
    $.post("/Ajax/getMyMsg", { id: id }, function (d) {
        var x = d.indexOf('_');
        var count = d.substring(0, x);
        var text = d.substring(x + 1);
        $("#lab_MyMsg_Count1").html(count);
        $("#lab_MyMsg_Count2").html(count);
        $("#ul_MyMsg").html(text);
    })
}

function getSelect() {
    var str = "";
    $("input[name='checkList']:checked").each(function (i, o) {
        str += $(this).val();
        str += ",";
    });
    if (str.length > 0) {
        var IDS = str.substr(0, str.length - 1);
        return IDS;
    } else {
        alert("操作提示", "至少选择一条记录操作");
    }
    return "";
}
function myAlert(d) {
    if (d.Status == "y") {
        alert("成功提示", d.Msg);
    } else {
        alertError("错误提示", d.Msg);
    }
}