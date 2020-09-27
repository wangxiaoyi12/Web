$(function () {
    //鍗曠嫭閫夋嫨鏌愪竴涓�
    //    $("input[name='check_item']").live('click', function () {
    //        var index = $("input[name='check_item']").index(this);
    //        $("input[name='check_item']").eq(index).toggleClass("checked"); //浼閫�
    //    });
    //鍏ㄩ€�
    //    $("#check_all,#box_all").live('click', function () {
    //        $("input[name='check_item']").attr("checked", $(this).attr("checked"));
    //        $("input[name='check_item'],#check_all,#box_all").toggleClass("checked");
    //    });
    $(".menu").on("click", function () {//闅愯棌鏄剧ず鑿滃崟
        $(".tab-menu").toggle();
    });
    $(".detail_img img").each(function () {
        var url = $(this).attr("src");
        $(this).attr("src", url);
    });
});
function pageBack() {
    var a = window.location.href;
    if (/#top/.test(a)) {
        window.history.go(-2);
        window.location.load(window.location.href)
    } else {
        window.history.back();
        window.location.load(window.location.href)
    }
}
/**
* 鏃堕棿瀵硅薄鐨勬牸寮忓寲;
*/
function renderTime(date) {
    var da = new Date(parseInt(date.replace("/Date(", "").replace(")/", "").split("+")[0]));
    return da.getFullYear() + "-" + (da.getMonth() + 1) + "-" + da.getDate() + " " + da.getHours() + ":" + da.getSeconds();
}