$(document).ready(function () {

    $.ajaxSetup({
        async: false
    });

});

// 获取当前iframe的layui id
var frameId = "";
if (parent != undefined && parent.layer != undefined) {
    frameId = parent.layer.getFrameIndex(window.name);
}

// 关闭当前iframe
function closeFrame() {

    if (frameId != undefined)
        (parent || window).layer.close(frameId);

}

// 关闭当前iframe并刷新父页面
function refreshParent() {

    if (parent != undefined)
        parent.window.location.reload();

}