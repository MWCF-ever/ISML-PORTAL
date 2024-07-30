$(document).ready(function () {

    $.ajaxSetup({
        async: false
    });

    // 切换到英语
    $("#a_ToEnglish").click(function () {

        $.ajax({
            url: window.preURL + "/api/General_Api/Language_Switch",
            type: "POST",
            ContentType: "application/json",
            dataType: "text",
            data: { "": "1" },
            async: true,
            success: function () {

                window.location.reload();

            }
        })

    });

    // 切换到中文
    $("#a_ToChinese").click(function () {

        $.ajax({
            url: window.preURL + "/api/General_Api/Language_Switch",
            type: "POST",
            ContentType: "application/json",
            dataType: "text",
            data: { "": "0" },
            async: true,
            success: function () {

                window.location.reload();

            }
        })

    });

    // 收缩侧边栏
    $("#btn_sideBar_collapse").click(function () {

        $(".div_sideBar").css("display", "none");

        $(".layui-footer").css("left", "0");
        $(".div_toolContent").css("padding-left", "0px");
        $("#btn_sideBar_collapse").css("display", "none");
        $("#btn_sideBar_expand").css("display", "");

    });

    // 展开侧边栏
    $("#btn_sideBar_expand").click(function () {

        $(".div_sideBar").css("display", "");

        $(".layui-footer").css("left", "230px");
        $(".div_toolContent").css("padding-left", "230px");
        $("#btn_sideBar_collapse").css("display", "");
        $("#btn_sideBar_expand").css("display", "none");

    });

    // 收缩所有分类
    $("#btn_category_collapse").click(function () {

        $(".layui-colla-content").each(function () {
            if ($(this).hasClass("layui-show"))
                $(this).parent().find(".layui-colla-title").click();
        });

        $("#btn_category_expand").css("display", "");
        $("#btn_category_collapse").css("display", "none");

    });

    // 展开所有分类
    $("#btn_category_expand").click(function () {

        $(".layui-colla-content").each(function () {
            if (!$(this).hasClass("layui-show"))
                $(this).parent().find(".layui-colla-title").click();
        });

        $("#btn_category_expand").css("display", "none");
        $("#btn_category_collapse").css("display", "");

    });

    // 每次点击后检查是否已全部折叠，对应切换按钮显示
    $(".layui-colla-title").click(function () {

        var countTotal = 0;
        var countExpanded = 0;
        var countCollapsed = 0;

        $(".layui-colla-content").each(function () {

            countTotal = countTotal + 1;

            if ($(this).hasClass("layui-show"))
                countExpanded = countExpanded + 1;
            else
                countCollapsed = countCollapsed + 1;
        });

        // 如果已全部折叠，则显示全部展开按钮
        if (countTotal == countCollapsed) {
            $("#btn_category_expand").css("display", "");
            $("#btn_category_collapse").css("display", "none");
        }
        // 如果已全部展开，则显示全部折叠按钮
        else if (countTotal == countExpanded) {
            $("#btn_category_expand").css("display", "none");
            $("#btn_category_collapse").css("display", "");
        }

    });

    $("#searchIcon").click(function () {

        var searchstr = $("#searchtxt").val();
        $("#searchInput").val(searchstr);
        window.location.href = window.preURL + "/SSML/search?searchtxt=" + searchstr;

        //layer.msg("Under implementation, Coming soon...");

    });

    // 菜单快捷键
    $(document).keydown(function (event) {

        var oEvent = event || window.event;

        if (oEvent.ctrlKey) {

            // ctrl + left arrow = sidebar collapse
            if (oEvent.keyCode == 37)
                $("#btn_sideBar_collapse").click();
            // ctrl + right arrow = sidebar expand
            if (oEvent.keyCode == 39)
                $("#btn_sideBar_expand").click();
            // ctrl + up arrow = category collapse
            else if (oEvent.keyCode == 38)
                $("#btn_category_collapse").click();
            // ctrl + down arrow = category expand
            else if (oEvent.keyCode == 40)
                $("#btn_category_expand").click();
        }

    });

    layui.util.fixbar({
        top: true,
        css: { right: 30 }
    });

})

// alertType: 1 = success, 2 = failure, 3 = warning, 4 = information, 5 = error
// functionName: closeWindow = close the current window, toHomePage = redirect to home page, toPage = to the page specified in parameter pagePath, refresh = refresh current page
function customAlert(alertType, contentText, functionName, pagePath) {

    var iconNo, titleText;

    if (alertType == 1) {
        iconNo = 1;
        titleText = 'Successed';
    }
    else if (alertType == 2) {
        iconNo = 5;
        titleText = 'Failed';
    }
    else if (alertType == 3) {
        iconNo = 3;
        titleText = 'Warning';
    }
    else if (alertType == 4) {
        iconNo = 0;
        titleText = 'Information';
    }
    else if (alertType == 5) {
        iconNo = 2;
        titleText = 'Error';
    }

    layui.layer.alert(contentText, { icon: iconNo, title: titleText, btn: 'OK', closeBtn: false }, function (index) {

        if (functionName != null) {

            switch (functionName) {
                case "toPage":
                    toPage(pagePath);
                    break;
                case "refresh":
                    refresh();
                    break;
                case "refreshParent":
                    refreshParent();
                    break;
                case "closeFrame":
                    closeFrame();
                    break;
                default:
                    break;
            }
        }

        if (functionName != 'closeWindow') {
            layui.layer.close(index);
        }

    });

}

function closeWindow() {

    if (navigator.userAgent.indexOf("Firefox") != -1 || navigator.userAgent.indexOf("Chrome") != -1) {
        window.location.href = "about:blank";
        window.close();
    } else {
        window.opener = null;
        window.open("", "_self");
        window.close();
    }

}

function refresh() {

    window.location.reload();

}

function focusOnParent() {

    if (window.opener != null && window.opener.closed != true) {
        window.opener.location.reload();
        window.opener.focus();
    }

    window.close();

}

// 打开浮动消息层
function customMsg(iconNo, content) {

    layer.msg(content, {
        icon: iconNo,
        time: 2000 //2秒关闭（如果不配置，默认是3秒）
    });

}

// 打开iframe层
function customIframe(frameType, frameTitle, frameUrl, frameWidth, frameHeight, frameAnim) {

    frameId = layer.open({
        type: frameType,
        title: frameTitle,
        content: frameUrl,
        area: [frameWidth + 'px', frameHeight + 'px'],
        anim: frameAnim,
        shade: [0.9, '#555f69']
    });

}

// 打开加载层
function customLoading() {

    return layer.load(1, { shade: [0.8, '#555f69'] });

}