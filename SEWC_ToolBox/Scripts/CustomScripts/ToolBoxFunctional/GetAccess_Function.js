$(document).ready(function () {

    $.ajaxSetup({
        async: false
    });

    // 点击预览图进行选中或取消时，执行隐藏的checkbox点击
    $(".div_report_ImgView").click(function () {

        $(this).parent().find("[name='checkBox_selectReport']").click();

    });

    // checkbox 选中和取消选中时，更新已选List
    $("[name='checkBox_selectReport']").change(function () {

        var isChecked = $(this).prop("checked");
        var report_ID = $(this).attr("report_ID");
        var selectedFrame = $(this).parent().find(".div_report_ImgView").find(".div_selectedFrame");

        reportSelectionChanged(report_ID, isChecked, selectedFrame);

    });

    // 切换菜单时，保存已选的值
    $("[name='a_menuSwitch']").click(function () {

        var href = $(this).attr("href");

        var idList = $("[name='input_reportID_list']").val();

        if (idList != "") {

            $(this).attr("href", href + "&ReportID_List=" + idList);

        }

    });

    // 提交按钮
    $("#btn_Submit").click(function () {
        var loading_index;
        var idList = $("[name='input_reportID_list']").val();
        if (idList != "") {

            idList = idList.substring(1, idList.length - 1);

            // 阻止元素发生默认的行为
            event.preventDefault();

            layer.open({
                type: 1,
                title: "Please input reason for access aplication :",
                fix: false,
                shadeClose: false,
                maxmin: true,
                area: ['380px', '215px'],
                content: '<textarea rows="6" cols="55" style="margin:10px;border:1px solid #eee" id="remarkId"></textarea>',
                btn: ['Submit', 'Cancle'],
                yes: function (index, layro) {

                    var remark = $("#remarkId").val();
                    if (remark != "") {
                        var pageInfo = idList + "*" + remark;
                        $.ajax({
                            url: window.preURL + "/api/ToolBox_Api/Get_ReportAccess_Multiple",
                            type: "POST",
                            ContentType: "application/json",
                            dataType: "json",
                            data: { "": pageInfo },
                            async: true,
                            beforeSend: function () {
                                loading_index = customLoading();
                            },
                            success: function (result) {
                                if (result.OpResult == true)
                                    customAlert(1, result.OpMsg);
                                else
                                    customAlert(2, result.OpMsg);
                            },
                            complete: function () {
                                layer.close(loading_index);
                                layer.close(index);
                            }
                        });
                    } else {
                        customAlert(2, "Please input the reason!");
                    }
                }
            });

            //$.ajax({
            //    url: "/api/ToolBox_Api/Get_ReportAccess_Multiple",
            //    type: "POST",
            //    ContentType: "application/json",
            //    dataType: "text",
            //    data: { "": idList },
            //    async: true,
            //    beforeSend: function () {
            //        loading_index = customLoading();
            //    },
            //    success: function (result) {

        
            //        if (resultJSON.OpResult == true)
            //            customAlert(1, resultJSON.OpMsg, "refreshParent");
            //        else
            //            customAlert(2, resultJSON.OpMsg);

            //    },
            //    complete: function () {
            //        layer.close(loading_index);
            //    }
            //});

        }
        else
            customAlert(2, 'No report selected');

    });

    // 关闭窗口按钮
    $("#btn_close").click(function () {

        closeFrame();

    });

});

// 选中时更新已选列表
function reportSelectionChanged(report_ID, isChecked, selectedFrame) {

    var idList = $("[name='input_reportID_list']").val();
    var curID = ',' + report_ID + ',';

    if (isChecked == true) {

        if (idList == "")
            idList = curID;
        else
            idList += report_ID + ',';

        selectedFrame.css("display", "block");

    }
    else {

        if (idList != "")
            idList = idList.replace(curID, ",");

        if (idList == ",")
            idList = "";

        selectedFrame.css("display", "none");

    }

    $("[name='input_reportID_list']").val(idList);

}