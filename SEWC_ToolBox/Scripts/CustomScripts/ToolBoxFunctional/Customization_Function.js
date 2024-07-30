﻿$(document).ready(function () {

    $.ajaxSetup({
        async: false
    });

    // 初始化表单监听
    layui.use('form', function () {
        var form = layui.form;
        var prurlstr = $("#PreUrltxt").val();
        // 监听radio选项变更
        form.on('radio(input_Category_IsSelect)', function (data) {
            if (data.value == 'true') {
                $("[name='select_Category']").attr("lay-verify", "required");
                $("[name='input_Category']").removeAttr("lay-verify");
                $("#div_categorySelection").removeClass("hidden");
                $("#div_categoryInput").addClass("hidden");
            }
            else if (data.value == 'false') {
                $("[name='select_Category']").removeAttr("lay-verify");
                $("[name='input_Category']").attr("lay-verify", "required");
                $("#div_categorySelection").addClass("hidden");
                $("#div_categoryInput").removeClass("hidden");
            }

        });

        // 监听创建提交
        form.on('submit(formCreate)', function (data) {
            var loading_index;

            $.ajax({
                url: window.preURL + "/api/ToolBox_Api/Add_Customization",
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
                url: window.preURL + "/api/ToolBox_Api/Update_Customization",
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

    // 初始上传模块
    layui.use('upload', function () {

        var upload = layui.upload;
        var customizationGUID = $("#input_customizationGUID").val();
        var prurlstr = $("#PreUrltxt").val();
        //执行实例
        debugger
        var uploadInst = upload.render({
            elem: '#btn_upload' //绑定元素
            , url: window.preURL + "/api/ToolBox_Api/Upload_ItemImage"
            , method: "POST"
            , data: { "picType": "CustomizationImg", "itemGUID": customizationGUID }
            , done: function (resultJSON) {

                //上传完毕回调
                if (resultJSON.OpResult == true) {

                    customMsg(1, resultJSON.OpMsg);

                    $("#input_picPath").val(resultJSON.EXT02);
                    $("#img_customizationPic").attr("src", resultJSON.EXT03 + "?t=" + Math.random());

                    $("#img_customizationPic").css("display", "block");

                }
                else {

                    customAlert(2, resultJSON.OpMsg);

                    $("#input_picPath").val("");
                    $("#img_customizationPic").attr("src", "");

                    $("#img_customizationPic").css("display", "none");

                }

            }
        });

    });

    // 关闭窗口按钮
    $("#btn_close").click(function () {

        closeFrame();

    });

    // 初始化表单内控件样式
    $("#btn_reset").click();

});