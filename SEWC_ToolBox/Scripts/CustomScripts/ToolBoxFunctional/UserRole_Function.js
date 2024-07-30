﻿$(document).ready(function () {

    $.ajaxSetup({
        async: false
    });

    // 初始化表单监听
    layui.use('form', function () {
        var form = layui.form;
        //var prurlstr = $("#PreUrltxt").val();
        // 监听radio选项变更
        form.on('select(select_Role)', function (data) {

            showDescription(data.value);

        });

        // 监听创建提交
        form.on('submit(formCreate)', function (data) {

            var loading_index;

            $.ajax({
                url: window.preURL + "/api/ToolBox_Api/Add_UserRole",
                type: "POST",
                ContentType: "application/json",
                dataType: "text",
                data: data.field,
                async: true,
                beforeSend: function () {
                    loading_index = customLoading();
                },
                success: function (result) {
                    var resultJSON = result;
                    if (typeof result == 'string') {
                        resultJSON = JSON.parse(result);
                    }

                    if (resultJSON.OpResult == true)
                        customAlert(1, resultJSON.OpMsg, "refreshParent");
                    else
                        customAlert(2, resultJSON.OpMsg);

                },
                complete: function () {
                    layer.close(loading_index);
                }
            });

            return false;

        });

        // 监听修改提交
        form.on('submit(formUpdate)', function (data) {

            var loading_index;

            $.ajax({
                url: window.preURL + "/api/ToolBox_Api/Update_UserRole",
                type: "POST",
                ContentType: "application/json",
                dataType: "text",
                data: data.field,
                async: true,
                beforeSend: function () {
                    loading_index = customLoading();
                },
                success: function (result) {
                    var resultJSON = result;
                    if (typeof result == 'string') {
                        resultJSON = JSON.parse(result);
                    }

                    if (resultJSON.OpResult == true)
                        customAlert(1, resultJSON.OpMsg, "refreshParent");
                    else
                        customAlert(2, resultJSON.OpMsg);

                },
                complete: function () {
                    layer.close(loading_index);
                }
            });

            return false;

        });

    });

    // 关闭窗口按钮
    $("#btn_close").click(function () {

        closeFrame();

    });

    // 初始化表单内控件样式
    $("#btn_reset").click();

});

function showDescription(a_id) {

    $(".layui-table").each(function () {

        var id = $(this).attr("id");

        if (id.indexOf("table_access_") != -1) {

            var cur_a_id = id.replace("table_access_", "");

            if (cur_a_id == a_id)
                $(this).css("display", "");
            else
                $(this).css("display", "none");

        }

    });

}