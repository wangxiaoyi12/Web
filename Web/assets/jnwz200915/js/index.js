// 下拉框
function Select(selectClass){
	$(selectClass).each(function() {
		var s = $(this);
		var z = parseInt(s.css("z-index"));
		var dt = $(this).children("dt");
		var dd = $(this).children("dd");
		var _show = function() {
			dd.slideDown(200);
			dt.addClass("cur");
			s.css("z-index", z + 1);
		}; //展开效果
		var _hide = function() {
			dd.slideUp(200);
			dt.removeClass("cur");
			s.css("z-index", z);
		}; //关闭效果
		dt.click(function() {
			dd.is(":hidden") ? _show() : _hide();
		});
		dd.find("li").click(function() {
			dt.html($(this).children('a').html());
			console.log(dt.html());
			console.log($(this).index());
			_hide();
		}); //选择效果（如需要传值，可自定义参数，在此处返回对应的“value”值 ）
		$("body").click(function(i) {
			!$(i.target).parents(selectClass).first().is(s) ? _hide() : "";
		});
	})
}

// 判断图片加载的函数
function isImgLoad(callback) {
    let t_img; // 定时器
    let isLoad = true; // 控制变量
    // 注意我的图片类名都是cover，因为我只需要处理cover。其它图片可以不管。
    // 查找所有封面图，迭代处理
    $('.coverImg').each(function () {
        // 找到为0就将isLoad设为false，并退出each
        if (this.height === 0) {
            isLoad = false;
            return false;
        }
    });
    // 为true，没有发现为0的。加载完毕
    if (isLoad) {
        clearTimeout(t_img); // 清除定时器
        // 回调函数
        callback();
        // 为false，因为找到了没有加载完成的图，将调用定时器递归
    } else {
        isLoad = true;
        t_img = setTimeout(function () {
            isImgLoad(callback); // 递归扫描
        }, 500); // 我这里设置的是500毫秒就扫描一次，可以自己调整
    }
}

// 禁止数字输入框输入负数
function numInput(reduceNum,addNum,valNum) {
    // 数量 加减
    var min = 1;
    var values = $(valNum);
    // 只能输入正整数
    values.on('keyup', function () {
        if (!/^\d+$/.test(this.value)) {
            this.value = min;
        }
    })
    // 监听input
    $(valNum).keyup(function () {
        var values = $(valNum).val();
        if (values < 0) {
            $(this).val(min);
        }
    })
    $(addNum).click(function () {
        var values =  $(this).siblings(valNum).val();
        values++;
       $(this).siblings(valNum).val(values);
    });
    $(reduceNum).click(function () {
        var values =  $(this).siblings(valNum).val();
        values--;
        if (values < min) {
            $(valNum).val(min);
        } else {
            $(this).siblings(valNum).val(values);
            // console.log(values);
        }
    });

    // input 禁止滚动
    $(valNum)[0].addEventListener('DOMMouseScroll', MouseWheel, false);

    function MouseWheel(event) {
        event = event || window.event;
        event.preventDefault();
    }
}

// tab 切换
function tabs(tabMenu, tabMenuAct, tabCount, tabCountAct) {
    $(tabMenu).click(function () {
        let that = this;
        let i = $(that).index(tabMenu);
        $(tabMenu).eq(i).addClass(tabMenuAct).siblings().removeClass(tabMenuAct);
        $(tabCount).eq(i).addClass(tabCountAct).siblings().removeClass(tabCountAct);
    })
}

function show(msg) {
    layer.msg(msg, {
        time: 2000//20s后自动关闭

    });
}
function showurl(msg,url) {
    layer.msg(msg, {
        time: 2000//20s后自动关闭

    });
    setTimeout(go(url), 2000)
  
} function go(url) {
    location.href = url;
}
