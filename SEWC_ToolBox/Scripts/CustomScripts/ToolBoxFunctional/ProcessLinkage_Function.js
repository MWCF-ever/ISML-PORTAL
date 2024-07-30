$(document).ready(function () {

    $.ajaxSetup({
        async: false
    });

    // 初始化表单监听
    layui.use('form', function () {
        var form = layui.form;
        // 监听创建提交
        form.on('submit(formCreate)', function (data) {


            var loading_index;
            var nodeId = $("#input_nodeID").val();

            $.ajax({
                url: window.preURL + "/api/ToolBox_Api/Add_ProcessLinkage",
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

                    if (resultJSON.OpResult == true) {
                        parent.loadLinkage(nodeId);
                        customAlert(1, resultJSON.OpMsg, 'closeFrame');
                    }
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
            var nodeId = $("#input_nodeID").val();

            $.ajax({
                url: window.preURL + "/api/ToolBox_Api/Update_ProcessLinkage",
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
                    if (resultJSON.OpResult == true) {
                        parent.loadLinkage(nodeId);
                        customAlert(1, resultJSON.OpMsg, "closeFrame");
                    }
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