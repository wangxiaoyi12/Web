$(document).on("click", "li[class^='nav-item'] a", function (obj) {//当点击导航按钮的A标签时
    window.localStorage.currentUrl = $(this).attr("href");
});

if (window.localStorage) {//判断浏览器是否支持localStorage
    if (window.localStorage.currentUrl != "" && window.localStorage.currentUrl != "#" && window.localStorage.currentUrl != "javascript:;" && window.localStorage.currentUrl != undefined) {
        $("li[class^='nav-item']").removeClass().addClass("nav-item");
        $("[class^='arrow']").removeClass().addClass("arrow");
        $("li[class='nav-item']").find("[href='" + localStorage.currentUrl + "']").closest("li").addClass("active open");
        $("li[class='nav-item active open']").find("[href='" + localStorage.currentUrl + "']").closest("ul").closest("li").addClass("active open");
        $("li[class='nav-item active open']").find("[href='" + localStorage.currentUrl + "']").closest("ul").closest("li").find("[class^='arrow']").addClass("open");

        //
        //location.href = localStorage.currentUrl;
    }

} else {//采用cookie方式保存

}