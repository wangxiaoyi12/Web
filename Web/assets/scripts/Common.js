//空功能提示
$(function() {
    $(".nav-link[href='javascript:;']").unbind().click(function() {
        psp.alert("该功能正在排队上线中！！");
    });
});

//绑定字典内容到指定的Select控件
function BindSelect(ctrlName, url, name) {

    $.fn.select2.defaults.set("theme", "bootstrap");
    var control = ctrlName;
    //设置Select2的处理
    control.select2({
        placeholder: "请输入要选择的项",//文本框的提示信息
        language: "zh-CN",
        allowClear: true,
        formatResult: formatResult,
        formatSelection: formatSelection,
        formatInputTooShort: "请输入",
        formatNoMatches: "没有匹配的项",
        formatSearching: "查询中...",
        escapeMarkup: function (m) {
            return m;
        }
    });

    //绑定Ajax的内容
    $.ajax(url, {
        dataType: "json",
        async: false
    }).done(function (data) {
        control.empty();//清空下拉框
        control.select2("val", "");//清空select2的选中值
        $.each(data, function (i, item) {
            control.append("<option value='" + item.Value + "'>" + item.Text + "</option>");
            if (i == 0) {
                control.select2("val", item.Value);//设置默认选中值
            }
            i++;
        });
    }).fail(function (a, b, c) {
        console.log(a);
        console.log(b);
        console.log(c);
    });

    function formatResult(item) {
        return item.text;
    }

    function formatSelection(item) {
        return item.text;
    }
}

//双击打开层
$(function () {
    //$(document).on("dblclick", "#datatable tr", function () {

    //  // var id= $(this).find("input[name='checkList']").val();
    // //  OpenDialog("ProductLineForm", "/BasicData/ProductLine/Detail?id=" + id + "", "");
    //});


});
//双击行跳转
function OpenDialog(url) {

    window.location.href = url;

    //$("#" + dialogId).modal({

    //    remote: url
    //});

    //$("#" + dialogId).modal("show");

}
//双击行弹窗
function OpenModal(dialogId, url, formId) {
    $("#" + dialogId).data("type", "detail");
    $("#" + dialogId).modal({
        remote: url
    });

    $("#" + dialogId).modal("show");

    $("#" + dialogId).on("loaded.bs.modal", function () {
        var type = $("#" + dialogId).data("type");
        if (type == "detail") {
            disabled(formId);
        }
    });
    $("#" + dialogId).on("hidden.bs.modal", function () {

        // $("#" + dialogId).find("form").remove();
        enable(formId);
        $("#" + dialogId).data("type", "");

    });

}

//双击禁用
function disabled(formId) {

    $("form[id='" + formId + "'] :text").attr("disabled", "enable");
    $("form[id='" + formId + "'] textarea").attr("disabled", "enable");
    $("form[id='" + formId + "'] :submit").attr("disabled", "enable");
    $("form[id='" + formId + "'] select").attr("disabled", "enable");
    $("form[id='" + formId + "'] :radio").attr("disabled", "enable");
    $("form[id='" + formId + "'] :checkbox").attr("disabled", "enable");
    $("#SubSave").attr("disabled", "enable");
    $("#ChildSave").attr("disabled", "enable");

}
//移除
function enable(formId) {

    $("form[id='" + formId + "'] :text").removeAttr("disabled");
    $("form[id='" + formId + "'] textarea").attr("");
    $("form[id='" + formId + "'] :submit").attr("");
    $("form[id='" + formId + "'] select").attr("");
    $("form[id='" + formId + "'] :radio").attr("");
    $("form[id='" + formId + "'] :checkbox").attr("");
    $("#SubSave").attr("");
    $("#ChildSave").attr("");

}

function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]);
    return null;
}

//表示全局唯一标识符 (GUID)。
function Guid(g) {
    var arr = new Array(); //存放32位数值的数组

    if (typeof (g) == "string") { //如果构造函数的参数为字符串
        initByString(arr, g);
    }
    else {
        initByOther(arr);
    }
    //返回一个值，该值指示 Guid 的两个实例是否表示同一个值。
    this.Equals = function (o) {
        if (o && o.IsGuid) {
            return this.ToString() == o.ToString();
        }
        else {
            return false;
        }
    }
    //Guid对象的标记
    this.IsGuid = function () { }
    //返回 Guid 类的此实例值的 String 表示形式。
    this.ToString = function (format) {
        if (typeof (format) == "string") {
            if (format == "N" || format == "D" || format == "B" || format == "P") {
                return toStringWithFormat(arr, format);
            }
            else {
                return toStringWithFormat(arr, "D");
            }
        }
        else {
            return toStringWithFormat(arr, "D");
        }
    }
    //由字符串加载
    function initByString(arr, g) {
        g = g.replace(/\{|\(|\)|\}|-/g, "");
        g = g.toLowerCase();
        if (g.length != 32 || g.search(/[^0-9,a-f]/i) != -1) {
            initByOther(arr);
        }
        else {
            for (var i = 0; i < g.length; i++) {
                arr.push(g[i]);
            }
        }
    }
    //由其他类型加载
    function initByOther(arr) {
        var i = 32;
        while (i--) {
            arr.push("0");
        }
    }
    /*
    根据所提供的格式说明符，返回此 Guid 实例值的 String 表示形式。
    N  32 位： xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
    D  由连字符分隔的 32 位数字 xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
    B  括在大括号中、由连字符分隔的 32 位数字：{xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}
    P  括在圆括号中、由连字符分隔的 32 位数字：(xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx)
    */
    function toStringWithFormat(arr, format) {
        var str;
        switch (format) {
            case "N":
                return arr.toString().replace(/,/g, "");
            case "D":
                str = arr.slice(0, 8) + "-" + arr.slice(8, 12) + "-" + arr.slice(12, 16) + "-" + arr.slice(16, 20) + "-" + arr.slice(20, 32);
                str = str.replace(/,/g, "");
                return str;
            case "B":
                str = toStringWithFormat(arr, "D");
                str = "{" + str + "}";
                return str;
            case "P":
                str = toStringWithFormat(arr, "D");
                str = "(" + str + ")";
                return str;
            default:
                return new Guid();
        }
    }
}
//Guid 类的默认实例，其值保证均为零。
Guid.Empty = new Guid();
//初始化 Guid 类的一个新实例。
Guid.NewGuid = function () {
    var g = "";
    var i = 32;
    while (i--) {
        g += Math.floor(Math.random() * 16.0).toString(16);
    }
    return new Guid(g);
}

Date.prototype.Format = function (fmt) {
    var o = {
        "M+": this.getMonth() + 1,
        //月份
        "d+": this.getDate(),
        //日
        "h+": this.getHours(),
        //小时
        "m+": this.getMinutes(),
        //分
        "s+": this.getSeconds(),
        //秒
        "q+": Math.floor((this.getMonth() + 3) / 3),
        //季度
        "S": this.getMilliseconds() //毫秒
    };
    if (/(y+)/.test(fmt)) {
        fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    }
    for (var k in o) {
        if (o.hasOwnProperty(k)) {
            if (new RegExp("(" + k + ")").test(fmt)) {
                fmt = fmt.replace(RegExp.$1,
                    (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
            }
        }
    }
    return fmt;
}