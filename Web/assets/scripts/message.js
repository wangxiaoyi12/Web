/*
弹出提示框的简单封装（alert,confirm,dialog三种弹框）
作者：小辛
时间：2016-11-3
*/
document.write('<script  src="/assets/global/plugins/sweetalert/jquery-confirm.min.js" charset="gb2312"></script>');

/*************** alert begin***********************
传一个参数，代表：内容
传两个参数，代表：标题，内容
传三个参数，代表：标题，内容，ico 或者 确定后的回调函数（根据传进来的参数类型判断）
传四个参数，代表：内容，ico图标，确定后的回调函数
*/
var alert_color = "green";
var ok = "glyphicon glyphicon-ok";                 //对号
var _error = "glyphicon glyphicon-remove";          //错号
var warning = "glyphicon glyphicon-warning-sign";  //警告号 
var question = "glyphicon glyphicon-question-sign";//问号? 
var search = "glyphicon glyphicon-search";         //搜索
var user = "glyphicon glyphicon-user";             //用户
var home = "glyphicon glyphicon-home";             //主页home
var file = "glyphicon glyphicon-file";             //文件
var time = "glyphicon glyphicon-time";             //时间
var print = "glyphicon glyphicon-print";           //打印
var download = "glyphicon glyphicon-download";     //下载
var upload = "glyphicon glyphicon-upload";         //上传
var refresh = "glyphicon glyphicon-refresh";       //刷新
var zoom_in = "glyphicon glyphicon-zoom-in";       //放大
var zoom_out = "glyphicon glyphicon-zoom-out";     //缩小
var message_defaultico = ok;
function alert1(content) {
    $.alert({
        title: "提示",
        content: content,
        confirmButton: '确定',
        confirmButtonClass: 'btn ' + alert_color,
        icon: message_defaultico,
        animation: 'zoom'
    });
}
function alert2(title, content) {
    $.alert({
        title: title,
        content: content,
        confirmButton: '确定',
        confirmButtonClass: 'btn ' + alert_color,
        icon: message_defaultico,
        animation: 'zoom'

    });
}
function alert3(title, content, confirm) {
    $.alert({
        title: title,
        content: content,
        confirmButton: '确定',
        confirmButtonClass: 'btn ' + alert_color,
        icon: message_defaultico,
        animation: 'zoom',
        confirm: confirm
    });
}
function alert4(title, content, ico, confirm) {
    $.alert({
        title: title,
        content: content,
        confirmButton: '确定',
        confirmButtonClass: 'btn ' + alert_color,
        icon: ico,
        animation: 'zoom',
        confirm: confirm
    });
}
function alert(a1, a2, a3, a4) {
    if (arguments.length == 1) {
        alert1(a1);
    } else if (arguments.length == 2) {
        alert2(a1, a2);
    } else if (arguments.length == 3) {
        if (typeof a3 == 'function') {
            alert3(a1, a2, a3);
        } else {
            alert4(a1, a2, a3, function () {
            });
        }
    } else if (arguments.length == 4) {
        alert4(a1, a2, a3, a4);
    }
}
function alertError(title, content, confirm) {
    var ico = _error;
    alert(title, content, ico, confirm);
}
function alertWarning(title, content, ico, confirm) {
    var ico = warning;
    alert(title, content, ico, confirm);
}
/*************** alert end************************/

/*************** confirm begin***********************
传一个参数，代表：内容
传两个参数，代表：标题，内容
传三个参数，代表：标题，内容，ico图标 或 确定后的回调函数（根据参数类型自动判断）
传四、五个参数，代表：标题 内容，ico,确定后的回调函数，取消后的回调函数  (根据第3个参数类型是否function判断第3个参数是ico或者 确定后的回调函数【当第3个参数是function时】)
*/
function confirm1(content) {
    $.confirm({
        title: "提示",
        content: content,
        confirmButton: '确定',
        cancelButton: '取消',
        autoClose: 'cancel|10000',
        icon: question,
        confirmButtonClass: 'btn green',
        cancelButtonClass: 'btn default'
    });
}
function confirm2(title, content) {
    $.confirm({
        title: title,
        content: content,
        confirmButton: '确定',
        cancelButton: '取消',
        autoClose: 'cancel|10000',
        icon: question,
        confirmButtonClass: 'btn green',
        cancelButtonClass: 'btn default'
    });
}
function confirm3(title, content, confirm) {
    $.confirm({
        title: title,
        content: content,
        confirmButton: '确定',
        cancelButton: '取消',
        autoClose: 'cancel|10000',
        icon: question,
        confirmButtonClass: 'btn green',
        cancelButtonClass: 'btn default',
        confirm: confirm,
        cancel: function () {

        }
    });
}
function confirm_ico(title, content, ico) {
    $.confirm({
        title: title,
        content: content,
        confirmButton: '确定',
        cancelButton: '取消',
        autoClose: 'cancel|10000',
        icon: ico,
        confirmButtonClass: 'btn green',
        cancelButtonClass: 'btn default',
        confirm: function () {
        },
        cancel: function () {
        }
    });
}
function confirm4(title, content, confirm, cancel) {
    $.confirm({
        title: title,
        content: content,
        confirmButton: '确定',
        cancelButton: '取消',
        autoClose: 'cancel|10000',
        icon: 'glyphicon glyphicon-question-sign',
        confirmButtonClass: 'btn green',
        cancelButtonClass: 'btn default',
        confirm: confirm,
        cancel: cancel,
    });
}
function confirm5(title, content, ico, confirm, cancel) {
    $.confirm({
        title: title,
        content: content,
        confirmButton: '确定',
        cancelButton: '取消',
        autoClose: 'cancel|10000',
        icon: ico,
        confirmButtonClass: 'btn green',
        cancelButtonClass: 'btn default',
        confirm: confirm,
        cancel: cancel,
    });
}
function confirm(a1, a2, a3, a4, a5) {
    if (arguments.length == 1) {
        confirm1(a1);
    } else if (arguments.length == 2) {
        confirm2(a1, a2);
    } else if (arguments.length == 3) {
        if (typeof a3 == 'function') {
            confirm3(a1, a2, a3);
        } else {
            confirm_ico(a1, a2, a3);
        }
    } else if (arguments.length == 4 || arguments.length == 5) {
        if (typeof a3 == 'function') {
            confirm4(a1, a2, a3, a4);
        } else if (typeof a3 != 'function') {
            confirm5(a1, a2, a3, a4, a5);
        }
    }
}
/*************** confirm end************************/

/*************** dialog begin***********************
传一个参数，代表：内容
传两个参数，代表：标题，内容
传三个参数，代表：标题，内容，确定后的回调函数
*/
function dialog1(content) {
    $.dialog({
        title: "",
        content: content,
        icon: ok,
        animation: 'zoom'
    });
}
function dialog2(title, content) {
    $.dialog({
        title: title,
        content: content,
        icon: ok,
        animation: 'zoom'

    });
}
function dialog3(title, content, confirm) {
    $.dialog({
        title: title,
        content: content,
        icon: ok,
        animation: 'zoom',
        confirm: confirm
    });
}

function dialog(title, content, confirm) {
    if (arguments.length == 1) {
        dialog1(title);
    } else if (arguments.length == 2) {
        dialog2(title, content);
    } else if (arguments.length == 3) {
        dialog3(title, content, confirm);
    }
}
/*************** dialog end************************/