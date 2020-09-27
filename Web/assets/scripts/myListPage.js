/*************
用于列表页的常用js,比如激活会员 页
*************/
function toDate(str) {
    if (str != '') {
        var sd = str.substring(0, 10).split("-");
        return new Date(sd[0], sd[1], sd[2]);
    } else {
        return str;
    }
}
$(document).ready(function () {
    oTable = initTable();
    if ("undefined" == typeof isDetail || ("undefined" != typeof isDetail && isDetail == true)) {
        //双击table行
        $(document).on("dblclick", "#datatable tbody tr", function () {
            var id = $(this).find("input[name='checkList']").val();
            window.location.href = path + "Detail/" + id + "?commentForm=commentForm";
        });
    }
});

/**
* 表格初始化
* returns {*|jQuery}
*/
function initTable() {
    var table = $("#datatable").DataTable({
        bFilter: false,
        dom: '<f<t>lip>',
        ordering: false,
        serverSide: getServerSide(),
        language: {
            url: '/assets/global/plugins/DataTables-1.10.12/zh-CN.json'//汉化
        },
        ajax: {
            url: dataSource,
            type: 'post',
            data: searchData
        },   //异步加载数据
        columnDefs: [
    { "searchable": false, "targets": 0 },//第一列禁用搜索
    { "orderable": false, "targets": 0 }, //第一列禁用排序
        ],
        columns: columns,
        initComplete: function (settings, json) {
            $("a[name='Restore']").css("visibility", $("#IsRestore").val());
        }
    });
    return table;
}
function getServerSide() {
    if ("undefined" == typeof serverSide || ("undefined" != typeof serverSide && serverSide == true)) {
        return true;
    } else {
        return serverSide;
    }
}
