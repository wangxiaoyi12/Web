
function LoadJs(Path, time) {
    if (time && time != "") {
        Path = Path + "?time=" + time;
    }
    document.write("<script type=\"text/javascript\" src=\"" + Path + "\"></script>");
}

function LoadCss(Path, time) {
    if (time != "") {
        Path = Path + "?time=" + time;
    }
    document.write("<link href=\"" + Path + "\" rel=\"stylesheet\" />");
}

var time = Math.floor(Math.random() * 100000000);
var time2 = "20168888";

LoadCss("/Mall/Shop/AllShop/css/qui.css", time2);
LoadCss("/Mall/Shop/AllShop/css/swiper-3.3.1.min.css", time2);
LoadCss("/Mall/Shop/AllShop/css/style.css", time2);
LoadJs("/Mall/Shop/JS/Lib/Ibayton.js", time2);
LoadJs("/Mall/Shop/JS/Lib/Str.js", time2)
LoadJs("/Mall/Shop/JS/Lib/Lib.js", time2);
LoadJs("/Mall/Shop/JS/Lib/URL.js", time2);
LoadJs("/Mall/Shop/JS/LIB/jquery-1.8.3.min.js");
LoadJs("/Mall/Shop/JS/LIB/jquery.lazyload.min.js");
LoadJs("/Mall/Shop/JS/Web/Lib.js", time);
//LoadJs("/Mall/Shop/JS/Html/HtmlJs.aspx", time);//HTML页面装载的JS
//LoadJs("/Mall/Shop/JS/Web/Share.aspx?url=" + encodeURIComponent(location.href) + "&time=" + time, "");

