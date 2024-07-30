$(document).ready(function () {

    $.ajaxSetup({
        async: false
    });

});

// 添加用户角色
function addUserRole(deptID) {

    var frameWidth = $(window).width() * 0.9;
    var frameHeight = $(window).height() * 0.9;

    customIframe(2, 'Add User Role', window.preURL + "/ToolBoxFunctional/UserRole_Add?deptID=" + deptID, frameWidth, frameHeight, 3);

}

// 查看指定用户角色
function userRole_view(ur_ID) {

    var frameWidth = $(window).width() * 0.9;
    var frameHeight = $(window).height() * 0.9;

    customIframe(2, 'View User Role', window.preURL + "/ToolBoxFunctional/UserRole_View?userRoleID=" + ur_ID, frameWidth, frameHeight, 3);

}

// 更新指定用户角色
function userRole_update(ur_ID) {

    var frameWidth = $(window).width() * 0.9;
    var frameHeight = $(window).height() * 0.9;

    customIframe(2, 'Edit User Role', window.preURL + "/ToolBoxFunctional/UserRole_Update?userRoleID=" + ur_ID, frameWidth, frameHeight, 3);

}

// 删除指定用户角色
function userRole_delete(ur_ID, userText, roleName) {
    layer.confirm(
        'Remove role ' + roleName + ' from ' + userText + ' ?',
        {
            icon: 3,
            title: 'Attention',
            btn: ['Yes', 'No']
        },
        function (index) {

            var loading_index;

            $.ajax({
                url: window.preURL + "/api/ToolBox_Api/Delete_UserRole",
                type: "POST",
                ContentType: "application/json",
                dataType: "text",
                data: { "": ur_ID },
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
                        customAlert(1, resultJSON.OpMsg, "refresh");
                    else
                        customAlert(2, resultJSON.OpMsg);

                },
                complete: function () {
                    layer.close(loading_index)
                }
            });

            layer.close(index);

        });

}