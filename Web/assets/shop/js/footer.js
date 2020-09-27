jQuery(function(){
  jQuery(".fixedBox ul.fixedBoxList li.fixeBoxLi").hover(
	function (){
	  jQuery(this).find('span.fixeBoxSpan').addClass("hover");
	  jQuery(this).addClass("hover");
	},
	function () {
	 jQuery(this).find('span.fixeBoxSpan').removeClass("hover");
	  jQuery(this).removeClass("hover");
	}
  );
  jQuery('.BackToTop').click(function(){$('html,body').animate({scrollTop: '0px'}, 800);});
  var oDate=new Date();
  var iHour=oDate.getHours();
  if(iHour>8 && iHour<32){
    jQuery(".Service").addClass("startWork");
    jQuery(".Service").removeClass("Commuting");

  }else{
    jQuery(".Service").addClass("Commuting");
    jQuery(".Service").removeClass("startWork");
  };
  
});

jQuery(function(){
  jQuery('.listLeftMenu dl dt').click(function(){
    var but_list=jQuery(this).attr('rel');
    if(but_list=='off'){
      jQuery(this).attr('rel','on');
      jQuery('.listLeftMenu dl').removeClass('off');
      jQuery(this).parent().addClass('on');
  } else {
      jQuery(this).attr('rel', 'off');
      jQuery(this).parent().removeClass('on');
      jQuery(this).parent().addClass('off');
    }
  });
});


$(function () {
    //°ó¶¨ÊäÈë¿ò
    var Search_btn = $('.Search_btn');
    var txtBox = Search_btn.prev()
    Search_btn.click(function () {
        var val = $.trim(txtBox.val());
        if (val.length > 0) {
           
        } else {
            val = Search_btn.parent().next().children().first().text();
            txtBox.val(val);
        }
        location.href = '/shop/product/?key=' + encodeURIComponent(val);
    });
    txtBox.on({
        keydown: function (e) {
            if (e.keyCode == 13) {
                Search_btn.trigger('click');
            }
        }
    });
});