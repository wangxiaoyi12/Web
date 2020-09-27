var oTable;
$(document).ready(function () {
    oTable = initTable();
    ///双击table行
    $(document).on("dblclick", "#datatable tbody tr", function () {
        var id = $(this).find("input[name='checkList']").val();
        window.location.href = "/BasicSettings/RoleManage/Detail?id=" + id;
    });
});

/**
* 表格初始化
* returns {*|jQuery}
*/
function initTable() {
    var table = $("#datatable").DataTable({
        dom: '<f<t>lip>',
        language: {
            url: '/assets/global/plugins/DataTables-1.10.12/zh-CN.json'//汉化
        },
        ajax: '/BasicSettings/RoleManage/getDataSource',//异步加载数据
        columnDefs: [
           { "searchable": false, "targets": 0 },//第一列禁用搜索
           { "orderable": false, "targets": 0 }, //第一列禁用排序
           { "searchable": false, "targets": 3 },//第一列禁用搜索
           { "orderable": false, "targets": 3 } //第一列禁用排序
        ],
        columns: [//绑定数据
           {
               data: "id",
               createdCell: function (nTd, sData, oData, iRow, iCol) {
                   $(nTd).html("<input type='checkbox' name='checkList' value='" + sData + "'>");
               }
           },
           { data: 'role_name' },
           {
               data: 'role_type',
               createdCell: function (nTd, sData, oData, iRow, iCol) {
                   switch (sData) {
                       case "2":
                           $(nTd).html("<span>系统用户</span>");
                           break;
                       case "1":
                           $(nTd).html("<span>超级用户</span>");
                           break;
                       default:
                   }
               }
           },
           {
               data: 'id',
               createdCell: function (nTd, sData, oData, iRow, iCol) {
                   $(nTd).html("<a href='/BasicSettings/RoleManage/Detail?id=" + oData.id + "' class='btn green btn-xs redd'><i class='fa fa-edit'></i> 修改</a>&nbsp;")
               }
           }
        ]
    });
    return table;
}

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
    $.confirm({
        title: '删除确认!',
        content: '删除记录后不可恢复，您确定吗？',
        autoClose: 'cancel|10000',
        icon: 'glyphicon glyphicon-question-sign',
        confirmButtonClass: 'btn green',
        cancelButtonClass: 'btn default',
        confirm: function () {
            $.ajax({
                url: url,
                data: { idList: id },
                type: "post",
                success: function (backdata) {
                    $.alert({
                        title: '成功提示',
                        content: backdata.Msg,
                        confirmButton: '确定',
                        confirmButtonClass: 'btn green',
                        icon: 'glyphicon glyphicon-ok',
                        animation: 'zoom',
                        confirm: function () {
                            //window.location.href = result.ReUrl;
                        }
                    });
                    $("#checkAll").prop("checked", false);
                    oTable.ajax.reload()
                }, error: function (error) {
                    console.log(error);
                }
            });
        },
        cancel: function () {

        }
    });
}

function confirm(title)
{
    
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
        oTable.ajax.reload()
    } else {
        $.alert({
            title: '操作提示',
            content: '至少选择一条记录操作',
            confirmButton: '确定',
            confirmButtonClass: 'btn green',
            icon: 'glyphicon glyphicon-ok',
            animation: 'zoom',
            confirm: function () {
                //window.location.href = result.ReUrl;
            }
        });
    }
}