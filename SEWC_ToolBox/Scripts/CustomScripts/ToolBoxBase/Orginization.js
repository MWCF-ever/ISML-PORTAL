$(document).ready(function () {

    $.ajaxSetup({
        async: false
    });

    var loading_index = customLoading();

    // 初始化高度
    var framHeight = document.documentElement.clientHeight - 210;
    $("#iframepage").attr("height", framHeight);

    // iframe 加载完成后关闭 loading 层
    $("#iframepage").load(function () {

        // 关闭加载层
        layer.close(loading_index);

    });

});