$(document).ready(function () {

    $.ajaxSetup({
        async: false
    });

    layui.carousel.render({
        elem: '#div_RollingPlay',
        width: '100%',
        height: '600px',
        arrow: 'always',
        anim: 'fade',
        autoplay: true
    });

});