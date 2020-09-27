jQuery.extend({
  getCookie : function(sName) {
    var aCookie = document.cookie.split("; ");
    for (var i=0; i < aCookie.length; i++){
      var aCrumb = aCookie[i].split("=");
      if (sName == aCrumb[0]) return decodeURIComponent(aCrumb[1]);
    }
    return '';
  },
  setCookie : function(sName, sValue, sExpires) {
    var sCookie = sName + "=" + encodeURIComponent(sValue);    
    if (sExpires != null) { 
    	var f = new Date; f.setTime(f.getTime() + 1e3 * sExpires); sExpires = ";expires=" + f.toGMTString();sCookie += "; expires=" + sExpires;    
    }
    document.cookie = sCookie+ ";path=/;";
  },
  removeCookie : function(sName) {
    document.cookie = sName + "=; expires=Fri, 31 Dec 1999 23:59:59 GMT;";
  }
});

/*********页面加载成功*********/

$(document).ready(function () {

    $("#sousou").click(function () {
        $(".order_top_count").show();
    });

    $("#nav-left_ll").click(function () {
        $(".order_top_count").hide();
    });


    //调转首页
    $('.fix_nav').dblclick(function () {
        location.href = '/mobile';
    });

});

window.show = function (msg) {
    floatNotify.simple(msg);
}



